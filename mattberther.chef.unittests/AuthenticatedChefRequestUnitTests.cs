namespace mattberther.chef.unittests
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestSharp;

    [TestClass]
    public class AuthenticatedChefRequestUnitTests
    {
        [TestMethod]
        public void ConstructorSetsAcceptHeader()
        {
            // Arrange
            var client = "gmreburn";
            var resource = new Uri("/", UriKind.Relative);
            
            // Act
            var request = new AuthenticatedChefRequest(client, resource);
            
            // Assert
            Assert.AreEqual("application/json", request.Parameters.Single(p => p.Name.Equals("Accept")).Value);
        }

        [TestMethod]
        public void ConstructorSetsChefVersionHeader()
        {
            // Arrange
            var client = "gmreburn";
            var resource = new Uri("/", UriKind.Relative);

            // Act
            var request = new AuthenticatedChefRequest(client, resource);

            // Assert
            Assert.AreEqual("11.4.0", request.Parameters.Single(p => p.Name.Equals("X-Chef-Version")).Value);
        }

        [TestMethod]
        public void ConstructorSetsOpsUserIdHeader()
        {
            // Arrange
            var client = "gmreburn";
            var resource = new Uri("/", UriKind.Relative);

            // Act
            var request = new AuthenticatedChefRequest(client, resource);

            // Assert
            Assert.AreEqual(client, request.Parameters.Single(p => p.Name.Equals("X-Ops-UserId")).Value);
        }

        [TestMethod]
        public void SignSetsOpsSignHeader()
        {
            // Arrange
            var request = new AuthenticatedChefRequest("gmreburn", new Uri("/", UriKind.Relative));

            // Act
            request.Sign(File.ReadAllText("gmreburn.pem"));
            
            // Assert
            Assert.AreEqual("algorithm=sha1;version=1.0", request.Parameters.Single(p => p.Name.Equals("X-Ops-Sign")).Value);
        }

        [TestMethod]
        public void SignSetsOpsTimestampHeader()
        {
            // Arrange
            var request = new AuthenticatedChefRequest("gmreburn", new Uri("/", UriKind.Relative));

            // Act
            request.Sign(File.ReadAllText("gmreburn.pem"));

            // Assert
            Assert.IsNotNull(request.Parameters.Single(p => p.Name.Equals("X-Ops-Timestamp")).Value);
        }

        [TestMethod]
        public void SignSetsOpsContentHashHeader()
        {
            // Arrange
            var request = new AuthenticatedChefRequest("gmreburn", new Uri("/", UriKind.Relative));

            // Act
            request.Sign(File.ReadAllText("gmreburn.pem"));

            // Assert
            Assert.IsNotNull(request.Parameters.Single(p => p.Name.Equals("X-Ops-Content-Hash")).Value);
        }
    }
}