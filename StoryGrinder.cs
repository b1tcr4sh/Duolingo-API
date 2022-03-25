using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Playwright;
using DuolingoAPI;

namespace DuolingoAPI {
    class StoryGrinder {
        public StoryGrinder(IBrowser browser) {

        }


        async Task StoryGrind(IPage storiesPage, IPage pageToClose, IEnumerable<String> storyList)
        {
            // await pageToClose.CloseAsync();
            // Navigate to stories page and begin story grinding


            foreach (string url in storyList) {

                string processedURL = ProcessURL(url);

                await storiesPage.GotoAsync(processedURL, new PageGotoOptions {Timeout = 0});

                IElementHandle title = await storiesPage.WaitForSelectorAsync("div.saQLX", new PageWaitForSelectorOptions { Timeout = 0 });
                IJSHandle titleText = await title.GetPropertyAsync("textContent");   
                Console.WriteLine("\nBeginning grinding on \"{0}\"", await titleText.JsonValueAsync<IJSHandle>());

                IElementHandle startButton = await storiesPage.WaitForSelectorAsync("[data-test=\"story-start\"]");
                await startButton.ClickAsync();


                // Story has been entered/started
                await CompleteStory(storiesPage);
                await ExitStory(storiesPage);
            }
            Console.ReadKey();
        }
        async Task CompleteStory(IPage storiesPage) {
            IElementHandle button = await storiesPage.WaitForSelectorAsync("[data-test=\"stories-player-continue\"]");
            int attempts = 0;

            while (storiesPage.QuerySelectorAsync("[data-test=\"stories-player-continue\"]") != null) {
                IElementHandle continueButton = await storiesPage.WaitForSelectorAsync("[data-test=\"stories-player-continue\"]");
                await continueButton.ClickAsync();

                IReadOnlyList<IElementHandle> choices = new List<IElementHandle>();

                if (await storiesPage.QuerySelectorAsync("[data-test=\"stories-choice\"]") != null) {
                    choices = await storiesPage.QuerySelectorAllAsync("[data-test=\"stories-choice\"]");

                    foreach (IElementHandle element in choices) {
                        await element.ClickAsync();
                    } 
                } else if (await storiesPage.QuerySelectorAsync("[data-test=\"challenge-tap-token\"]") != null) {
                    choices = await storiesPage.QuerySelectorAllAsync("[data-test=\"challenge-tap-token\"]");

                    foreach (IElementHandle element in choices) {
                        await element.ClickAsync();
                    } 
                } else if (await storiesPage.QuerySelectorAsync("[data-test=\"stories-token\"]") != null) {
                    IReadOnlyList<IElementHandle> tokens = await storiesPage.QuerySelectorAllAsync("[data-test=\"stories-token\"]");
                    Random rng = new Random();

                    IReadOnlyList<IElementHandle> disabledTokens = await storiesPage.QuerySelectorAllAsync("[disabled=\"\"");
                    while (await storiesPage.QuerySelectorAsync("span._3Y29z._176_d._2jNpf") == null || await storiesPage.QuerySelectorAsync("h2._1qFda") != null) {
                        
                        rng.Shuffle<IElementHandle>(tokens.ToList<IElementHandle>());
                        foreach (IElementHandle element in tokens) {
                            disabledTokens = await storiesPage.QuerySelectorAllAsync("[disabled=\"\"");
                            foreach (IElementHandle disabledToken in disabledTokens) {
                                int index = Array.IndexOf(tokens.ToArray<IElementHandle>(), disabledToken);
                                tokens.Where(val => val != disabledToken).ToArray();
                            }
                            await element.ClickAsync();
                            Console.Write("\rTook {0} attempt(s) to complete matching tokens.", attempts);   
                            attempts++;                            
                        }
                    }
                    return;
                }
            }            
        }
        async Task ExitStory(IPage page) {
            while (await page.QuerySelectorAsync("[data-test=\"stories-player-done\"]") == null) {
                await page.ClickAsync("[data-test=\"stories-player-continue\"]");
            }


            IElementHandle completeButton = await page.WaitForSelectorAsync("[data-test=\"stories-player-done\"]");
            await page.ClickAsync("[data-test=\"stories-player-done\"]");

            await page.WaitForSelectorAsync("div._3wEt9");
        }
        async Task<IEnumerable<String>> GetStoryList(IBrowser browser) {
            IEnumerable<String> storyUrls = new string[] {};
            IPage page = await browser.NewPageAsync();
            await page.GotoAsync("https://www.duolingo.com/stories", new PageGotoOptions {Timeout = 0});
            Thread.Sleep(TimeSpan.FromSeconds(2));

            IReadOnlyList<IElementHandle> storyIcons = await page.QuerySelectorAllAsync("div.X4jDx");


            foreach (IElementHandle element in storyIcons) {
                await element.ClickAsync();
                IElementHandle startButton = await page.QuerySelectorAsync("[data-test=\"story-start-button\"]");
                if (startButton != null) {
                        IJSHandle buttonHref = await startButton.GetPropertyAsync("href");
                    object hrefJSON = await buttonHref.JsonValueAsync<IJSHandle>();

                    storyUrls = storyUrls.Append<String>(hrefJSON.ToString());
                    Console.WriteLine("Appended {0} to list of stories.", await buttonHref.JsonValueAsync<IJSHandle>());
                } else {
                    Console.WriteLine("Skipped a story because it was locked.");
                }
            }

            // await page.CloseAsync();
            return storyUrls;
        }
        String ProcessURL(String url) {
            if (url.Contains("listen") == true) {
                // Remove "listen" from the end of the string, and replace with "read"
                string trimmedUrl = url.Remove(url.Length - 6);
                string joinedUrl = trimmedUrl + "read";

                return joinedUrl;
            }
            return url;
        }
    }

    static class ArrayShuffler {
        public static void Shuffle<T> (this Random rng, List<T> array) {
            int n = array.Count;
            while (n > 1) 
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}