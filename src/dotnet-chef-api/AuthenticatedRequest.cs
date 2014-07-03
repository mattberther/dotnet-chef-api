using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;

namespace mattberther.chef
{
    public class AuthenticatedRequest
    {
        private readonly string client;
        private readonly Uri requestUri;
        private readonly HttpMethod method;
        private readonly string body;

        private readonly string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        private string signature = String.Empty;

        public AuthenticatedRequest(string client, Uri requestUri)
            : this(client, requestUri, HttpMethod.Get, String.Empty)
        {
        }

        public AuthenticatedRequest(string client, Uri requestUri, HttpMethod method, string body)
        {
            this.client = client;
            this.requestUri = requestUri;
            this.method = method;
            this.body = body;
        }

        public void Sign(string privateKey)
        {
            string canonicalHeader =
                String.Format(
                    "Method:{0}\nHashed Path:{1}\nX-Ops-Content-Hash:{4}\nX-Ops-Timestamp:{3}\nX-Ops-UserId:{2}",
                    method,
                    requestUri.AbsolutePath.ToBase64EncodedSha1String(),
                    client,
                    timestamp,
                    body.ToBase64EncodedSha1String());

            byte[] input = Encoding.UTF8.GetBytes(canonicalHeader);

            var pemReader = new PemReader(new StringReader(privateKey));
            AsymmetricKeyParameter key = ((AsymmetricCipherKeyPair) pemReader.ReadObject()).Private;

            ISigner signer = new RsaDigestSigner(new NullDigest());
            signer.Init(true, key);
            signer.BlockUpdate(input, 0, input.Length);

            signature = Convert.ToBase64String(signer.GenerateSignature());
        }

        public HttpRequestMessage Create()
        {
            var requestMessage = new HttpRequestMessage(method, requestUri);

            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("X-Ops-Sign", "algorithm=sha1;version=1.0");
            requestMessage.Headers.Add("X-Ops-UserId", client);
            requestMessage.Headers.Add("X-Ops-Timestamp", timestamp);
            requestMessage.Headers.Add("X-Ops-Content-Hash", body.ToBase64EncodedSha1String());
            requestMessage.Headers.Add("Host", String.Format("{0}:{1}", requestUri.Host, requestUri.Port));
            requestMessage.Headers.Add("X-Chef-Version", "11.4.0");

            if (method != HttpMethod.Get)
            {
                requestMessage.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(body));
                requestMessage.Content.Headers.Add("Content-Type", "application/json");
            }

            var i = 1;
            foreach (var line in signature.Split(60))
            {
                requestMessage.Headers.Add(String.Format("X-Ops-Authorization-{0}", i++), line);
            }

            return requestMessage;
        }
    }
}