# .NET Chef API [![Build Status](https://travis-ci.org/gmreburn/dotnet-chef-api.svg?branch=tests)](https://travis-ci.org/gmreburn/dotnet-chef-api)
This is a simple C# class library used to interact with a Chef Server's REST API. You can use this with a open source and Enterprise versions of the Chef Server.

## Usage
As of yet, this is not a full wrapper for the Chef Server API. You will still need to refer to the API documentation at <http://docs.opscode.com/api_chef_server.html> to determine which methods to call.

Using the class library is relatively straightforward. First, you create an AuthenticatedChefRequest and sign it with your private key. Then, pass that off to an instance of the ChefServer class and send the request. 

Some example code is:

	var baseUri = new Uri("https://api.opscode.com"); 
	var requestUri = new Uri(baseUri, "/organizations/organization_name/roles");
	var authenticatedRequest = new AuthenticatedChefRequest("client_name", requestUri);

	authenticatedRequest.Sign(PrivateKey);

	var client = new RestSharp.Client(baseUri);
	string resultContent = client.Execute(authenticatedRequest);
	Console.WriteLine(resultContent);

