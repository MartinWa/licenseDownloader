using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace licenseDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                const string path = "licenseUrls.txt";
                const string resultsDirectory = "licenses";
                Directory.CreateDirectory(resultsDirectory);
                var licenses = await File.ReadAllLinesAsync(path);
                using var webClient = new System.Net.WebClient();
                var count = 0;
                var result = new List<string>();
                foreach (var urlString in licenses)
                {
                    count += 1;
                    {
                        var licenseFile = $"License_{count}.txt";
                        var licenseFilePath = $"{resultsDirectory}\\{licenseFile}";
                        try
                        {
                            var uri = new Uri(urlString, UriKind.Absolute);
                            await webClient.DownloadFileTaskAsync(uri, licenseFilePath);
                            result.Add($"=HYPERTEXT(\"{licenseFilePath}\",\"{licenseFile}\"\n");
                        }
                        catch (UriFormatException ex)
                        {
                            result.Add(ex.Message);
                        }
                        catch (WebException ex)
                        {
                            result.Add(ex.Message);
                        }
                    }
                }
                await File.WriteAllLinesAsync("result.txt", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
                Console.WriteLine($"{ex.StackTrace}");
            }
        }
    }
}