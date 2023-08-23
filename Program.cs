using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Configuration;
using System.Net;

internal class Program
{
    static RestClient client;

    private static void Main(string[] args)
    {
        var apiKey = ConfigurationManager.AppSettings["OpenaiAPIKey"];

        var options = new RestClientOptions("https://api.openai.com/v1/images/generations")
        {
            Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(
                apiKey, "Bearer"),

        };
        client = new RestClient(options);
        Console.WriteLine("Enter a description for the image:");
        var description = Console.ReadLine();

        var response = GenerateImage(description);
        var imageUrl = GetGeneratedImageURL(response);

        Console.WriteLine($"Generated Image URL: {imageUrl}");

        // Display the image or integrate it into your application
        DisplayGeneratedImage(imageUrl);
    }

    static string GenerateImage(string description)
    {
        var request = new RestRequest()
        {
            Method = Method.Post,
            RequestFormat = DataFormat.Json
        };
        var param = new { 
            prompt = description,
            n = 1,
            size = "256x256"
        };

        request.AddJsonBody(param);

        return client.Execute(request).Content;
    }

    static string GetGeneratedImageURL(string response)
    {
        dynamic jsonResponse = JObject.Parse(response);
        return jsonResponse["data"][0]["url"];
    }

    static void DisplayGeneratedImage(string imageUrl)
    {
        using (var client = new WebClient())
        {
            var imageName = "generated_image.jpg"; // Change this to your preferred name
            client.DownloadFile(imageUrl, imageName);
            Console.WriteLine($"Image saved as {imageName}");
        }
    }
}