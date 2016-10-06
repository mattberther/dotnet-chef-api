# .NET Chef API [![Build Status](https://travis-ci.org/gmreburn/dotnet-chef-api.svg?branch=tests)](https://travis-ci.org/gmreburn/dotnet-chef-api)
This is a simple C# class library used to interact with a Chef Server's REST API. You can use this with a open source and Enterprise versions of the Chef Server.

## Usage
As of yet, this is not a full wrapper for the Chef Server API. You will still need to refer to the API documentation at <http://docs.opscode.com/api_chef_server.html> to determine which methods to call.

Using the class library is relatively straightforward. First, you create an AuthenticatedChefRequest and sign it with your private key. Then, pass that off to an instance of the ChefServer class and send the request. 

Some example code is:

	System.Uri baseUri = new System.Uri("https://api.opscode.com"); 
	System.Uri requestUri = new System.Uri(baseUri, "/organizations/organization_name/roles");
	mattberther.chef.AuthenticatedChefRequest authenticatedRequest = new AuthenticatedChefRequest("client_name", requestUri);

	authenticatedRequest.Sign(PrivateKey);

	RestSharp.IRestResponse client = new RestSharp.Client(baseUri);
	string resultContent = client.Execute(authenticatedRequest);
	Console.WriteLine(resultContent);

