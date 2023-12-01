using System.Net;
using WebScraping.Models;

namespace WebScraping.Services;

public static class FileDowloader
{
    public static bool DowloadFileFromWeb(string address, string path, string filename)
    {
        try
        {
            using (WebClient webClien = new())
            {
                webClien.Headers.Add(ConstantValues.USER_AGENT_HEADER, ConstantValues.STANDART_USER_AGENT);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Console.WriteLine("Directory created successfully.");
                }
                string filePath = Path.Combine(path, filename);
                webClien.DownloadFile(address, filePath);

                Console.WriteLine($"File {filename}, successfully created in {path}");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.GetType().Name} was thfowed while downloadding file.\nException message: {ex.Message}");
            return false;
        }
    }
}
