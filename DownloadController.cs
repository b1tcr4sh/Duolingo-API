using System;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using Microsoft.Playwright;

namespace DuolingoAPI.Chromium
{
    public static class ChromiumDownloadController 
    {
        public static void Download() {
            Console.WriteLine("Installing firefox build...");
            int exitCode = Microsoft.Playwright.Program.Main(new string[] { "install", "firefox" });
            if (exitCode != 0) throw new Exception($"Browser install failed with code {exitCode}");
        }
            
        // private static async Task DownloadDefaultAsync() {
        //     Console.WriteLine($"Downloading Chromium/{BrowserFetcher.DefaultChromiumRevision}:");


        //     fetcher.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DisplayDownloadProgress);

        //     await fetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        // }
        // private static void DisplayDownloadProgress(object sender, DownloadProgressChangedEventArgs e) {
        //     // Console.WriteLine(e);

        //     Console.Write("\rDownloaded {0}MB(s) of {1}MBs. {2}% complete...",
        //     e.BytesReceived / 1000000,
        //     e.TotalBytesToReceive / 1000000,
        //     e.ProgressPercentage);

        //     if (e.BytesReceived == e.TotalBytesToReceive) {
        //         Console.WriteLine("Download Complete!");
        //     }
        // }
    }
}