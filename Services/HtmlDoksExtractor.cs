using HtmlAgilityPack;
using WebScraping.Models;

namespace WebScraping.Services;
public static class HtmlDoksExtractor
{
    public static async Task<bool> CheckResponseSuccess(string address, string method = "GET")
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add(ConstantValues.USER_AGENT_HEADER, ConstantValues.STANDART_USER_AGENT);

                HttpResponseMessage response = new HttpResponseMessage();
                if (method.ToUpper() is "GET")
                {
                    Console.WriteLine($"Check the request to the address: {address}...");
                    response = httpClient.GetAsync(address).Result;
                }
                else if (method.ToUpper() is "POST")
                {
                    response = httpClient.PostAsync(address, null).Result;
                }
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response succeed!");
                }
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.GetType().Name} was thrown from \'WebScraping.Services.HtmlDoksExtractor)\'\n\nException message: {ex.Message}");
            return false;
        }
    }

    public static HtmlDocument GetHtmlDocument(string address, string method = "GET")
    {
        var res = new HtmlDocument();
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add(ConstantValues.USER_AGENT_HEADER, ConstantValues.STANDART_USER_AGENT);
                HttpResponseMessage response = new HttpResponseMessage();
                if (method.ToUpper() is "GET")
                {
                    Console.WriteLine($"Check the request to the address: {address}...");
                    response = httpClient.GetAsync(address).Result;
                }
                else if (method.ToUpper() is "POST")
                {
                    Console.WriteLine($"Check the request to the address: {address}...");
                    response = httpClient.PostAsync(address, null).Result; 
                }
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response succeed!");
                }
                var pageContent = response.Content.ReadAsStringAsync().Result;

                res.LoadHtml(pageContent);

                return res;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.GetType().Name} was thrown from {nameof(WebScraping.Services.HtmlDoksExtractor)}\n\nException message: {ex.Message}");
        }
        return res;
    }

    public static string? FindApi(HtmlDocument htmlDocument, string targetBlock)
    {
        if (htmlDocument is null || string.IsNullOrWhiteSpace(targetBlock)) return null;
        var targetNode = htmlDocument.DocumentNode.SelectSingleNode(targetBlock);
        if (targetNode is not null)
        {
            string api = targetNode.GetAttributeValue("href", "");
            Console.WriteLine($"Target api was found {api}");
            return api;
        }
        return null;
    }
}
