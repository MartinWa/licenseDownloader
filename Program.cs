using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var licenseUris = licenses.Select(ConvertStringToAbsoluteUri).ToList();
                Console.WriteLine($"From {licenses.Length} lines {licenseUris.Count} urls were valid");
                using var webClient = new System.Net.WebClient();
                var count = 0;
                var result = new List<string>();
                foreach (var uri in licenseUris)
                {
                    count += 1;
                    if (uri == null)
                    {
                        result.Add(string.Empty);
                        continue;
                    }
                    var licenseText = webClient.DownloadString(uri);
                    var licenseFile = $"License_{count}.txt";
                    var licenseFilePath = $"{resultsDirectory}\\{licenseFile}";
                    await File.WriteAllTextAsync(licenseFilePath, licenseText);
                    result.Add($"=HYPERTEXT(\"{licenseFilePath}\",\"{licenseFile}\"\n");
                }
                await File.WriteAllLinesAsync("result.txt", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");
                Console.WriteLine($"{ex.StackTrace}");
            }
        }

        private static Uri ConvertStringToAbsoluteUri(string stringToConvert)
        {
            return Uri.TryCreate(stringToConvert, UriKind.Absolute, out var uri) ? uri : null;
        }
    }
}