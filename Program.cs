using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace licenseDownloader
{
    internal static class Program
    {
        private static async Task Main()
        {
            try
            {
                const string path = "licenseUrls.txt";
                const string resultsDirectory = "licenses";
                Directory.CreateDirectory(resultsDirectory);
                var licenses = await File.ReadAllLinesAsync(path);
                using var webClient = new WebClient();
                var count = 0;
                var result = new List<string>();
                foreach (var urlString in licenses)
                {
                    count += 1;
                    Console.WriteLine($"Working on nr {count}: {urlString}");
                    try
                    {
                        var downloaded = await webClient.DownloadStringTaskAsync(urlString);
                        string extension = "txt";
                        if (downloaded.ToLower().TrimStart().StartsWith("<html") || downloaded.ToLower().TrimStart().StartsWith("<!doctype html"))
                        {
                            extension = "html";
                        }
                        var licenseFile = $"License_{count}.{extension}";
                        var licenseFilePath = $"{resultsDirectory}\\{licenseFile}";
                        await File.WriteAllTextAsync(licenseFilePath, downloaded);
                        result.Add($"=HYPERLINK(\"{licenseFilePath}\",\"{licenseFile}\"");
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