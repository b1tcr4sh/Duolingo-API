using System;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using Microsoft.Playwright;

namespace DuolingoAPI.Chromium
{
    public static class ChromiumDownloadController 
    {
        // private static BrowserFetcher fetcher;
        // public static async Task CheckDownload() {
        //     fetcher = new BrowserFetcher();

        //     IEnumerable<string> localRevisions = fetcher.LocalRevisions();

        //     bool downloadNeeded = true;
        //     foreach (string revision in localRevisions) {
        //         if (revision == BrowserFetcher.DefaultChromiumRevision) {
        //             Console.WriteLine($"Found Version Chromium/{revision}");
        //             downloadNeeded = false;
        //         }
        //     }

        //     if (downloadNeeded) {
        //         await DownloadDefaultAsync();
        //     } else {
        //         Console.WriteLine("Default Chromium version is already installed...");
        //     }
        // }
            
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