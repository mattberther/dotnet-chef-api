using System;
using System.Net.Http;

namespace mattberther.chef
{
    public class ChefServer
    {
        private readonly Uri server;

        public ChefServer(string server)
            : this(new Uri(server))
        {
        }

        public ChefServer(Uri server)
        {
            this.server = server;
        }

        public string SendRequest(AuthenticatedRequest request)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = server;
                
                var message = request.Create();
                var result = client.SendAsync(message).Result;
                
                result.EnsureSuccessStatusCode();

                return result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
