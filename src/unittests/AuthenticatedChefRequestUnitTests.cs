namespace mattberther.chef.unittests
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class AuthenticatedChefRequestUnitTests
    {
        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void SignSetsOpsSignHeader()
        {
            // Arrange
            var request = new AuthenticatedChefRequest("test", new Uri("/", UriKind.Relative));

            // Act
            request.Sign(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "test.pem")));

            // Assert
            Assert.AreEqual("algorithm=sha1;version=1.0",
                request.Parameters.Single(p => p.Name.Equals("X-Ops-Sign")).Value);
        }

        [Test]
        public void SignSetsOpsTimestampHeader()
        {
            // Arrange
            var request = new AuthenticatedChefRequest("test", new Uri("/", UriKind.Relative));

            // Act
            request.Sign(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "test.pem")));

            // Assert
            Assert.IsNotNull(request.Parameters.Single(p => p.Name.Equals("X-Ops-Timestamp")).Value);
        }

        [Test]
        public void SignSetsOpsContentHashHeader()
        {
            // Arrange
            var request = new AuthenticatedChefRequest("test", new Uri("/", UriKind.Relative));

            // Act
            request.Sign(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "test.pem")));

            // Assert
            Assert.IsNotNull(request.Parameters.Single(p => p.Name.Equals("X-Ops-Content-Hash")).Value);
        }
    }
}