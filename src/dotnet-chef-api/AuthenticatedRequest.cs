namespace mattberther.chef
{
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.Signers;
    using Org.BouncyCastle.OpenSsl;
    using RestSharp;
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class AuthenticatedChefRequest : RestRequest
    {
        private readonly string client;
        private string signature = string.Empty;

        public AuthenticatedChefRequest(string client, Uri requestUri) : base(requestUri)
        {
            this.client = client;
            this.AddHeader("Accept", "application/json");
            this.AddHeader("X-Chef-Version", "11.4.0");
            this.AddHeader("X-Ops-UserId", client);
        }

        public void Sign(string privateKey)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            string canonicalHeader =
                string.Format(
                    "Method:{0}\nHashed Path:{1}\nX-Ops-Content-Hash:{4}\nX-Ops-Timestamp:{3}\nX-Ops-UserId:{2}",
                    Method,
                    ToBase64EncodedSha1String(this.Resource),
                    client,
                    timestamp,
                    ToBase64EncodedSha1String(GetBody()
                        ));

            byte[] input = Encoding.UTF8.GetBytes(canonicalHeader);

            var pemReader = new PemReader(new StringReader(privateKey));
            AsymmetricKeyParameter key = ((AsymmetricCipherKeyPair) pemReader.ReadObject()).Private;

            ISigner signer = new RsaDigestSigner(new NullDigest());
            signer.Init(true, key);
            signer.BlockUpdate(input, 0, input.Length);

            signature = Convert.ToBase64String(signer.GenerateSignature());

            this.AddHeader("X-Ops-Sign", "algorithm=sha1;version=1.0");
            this.AddHeader("X-Ops-Timestamp", timestamp);
            this.AddHeader("X-Ops-Content-Hash", ToBase64EncodedSha1String(GetBody()));
            //this.AddHeader("Host", string.Format("{0}:{1}", this.Resource, requestUri.Port)); // TODO this might be taken care of by restsharp

            if (Method != Method.GET)
            {
                //todo ???.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(Parameters.Single(p => p.Type == ParameterType.RequestBody).Value.ToString()));
                this.AddHeader("Content-Type", "application/json");
            }

            var i = 1;
            foreach (var line in signature.Split(60))
            {
                this.AddHeader(string.Format("X-Ops-Authorization-{0}", i++), line);
            }
        }

        private string ToBase64EncodedSha1String(string input)
        {
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        private string GetBody()
        {
            return Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value.ToString() ?? string.Empty;
        }
    }
}