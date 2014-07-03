# ChefUpdater.NET
This is a simple C# class library used to interact with a Chef Server's REST API. You can use this with a open source and Enterprise versions of the Chef Server.

## Usage
As of yet, this is not a full wrapper for the Chef Server API. You will still need to refer to the API documentation at XXX to determine which methods to call.

Using the class library is relatively straightforward. First, you create an AuthenticatedRequest and sign it with your private key. Then, pass that off to an instance of the ChefServer class and send the request. 

Some example code is:

	var baseUri = new Uri("https://api.opscode.com:443"); 
	var requestUri = new Uri(BaseUri, "/organizations/organization_name/roles");
	var authenticatedRequest = new AuthenticatedRequest("client_name", uri);

	authenticatedRequest.Sign(PrivateKey);

	var server = new ChefServer(baseUri);
	string resultContent = server.SendRequest(authenticatedRequest);
	Console.WriteLine(resultContent);

