using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using PuppeteerSharp;
using DuolingoAPI;

namespace DuolingoAPI {
    class StoryGrinder {
        public StoryGrinder(Browser browser) {

        }


        async Task StoryGrind(Page storiesPage, Page pageToClose, IEnumerable<String> storyList)
        {
            // await pageToClose.CloseAsync();
            // Navigate to stories page and begin story grinding


            foreach (string url in storyList) {

                string processedURL = ProcessURL(url);

                await storiesPage.GoToAsync(processedURL, new NavigationOptions {Timeout = 0});

                ElementHandle title = await storiesPage.WaitForSelectorAsync("div.saQLX", new WaitForSelectorOptions {Timeout = 0});
                JSHandle titleText = await title.GetPropertyAsync("textContent");   
                Console.WriteLine("\nBeginning grinding on \"{0}\"", await titleText.JsonValueAsync());

                ElementHandle startButton = await storiesPage.WaitForSelectorAsync("[data-test=\"story-start\"]");
                await startButton.ClickAsync();


                // Story has been entered/started
                await CompleteStory(storiesPage);
                await ExitStory(storiesPage);
            }
            Console.ReadKey();
        }
        async Task CompleteStory(Page storiesPage) {
            ElementHandle button = await storiesPage.WaitForSelectorAsync("[data-test=\"stories-player-continue\"]");
            int attempts = 0;

            while (storiesPage.QuerySelectorAsync("[data-test=\"stories-player-continue\"]") != null) {
                ElementHandle continueButton = await storiesPage.WaitForSelectorAsync("[data-test=\"stories-player-continue\"]");
                await continueButton.ClickAsync();

                if (await storiesPage.QuerySelectorAsync("[data-test=\"stories-choice\"]") != null) {
                    ElementHandle[] choices = await storiesPage.QuerySelectorAllAsync("[data-test=\"stories-choice\"]");

                    foreach (ElementHandle element in choices) {
                        await element.ClickAsync();
                    } 
                } else if (await storiesPage.QuerySelectorAsync("[data-test=\"challenge-tap-token\"]") != null) {
                    ElementHandle[] choices = await storiesPage.QuerySelectorAllAsync("[data-test=\"challenge-tap-token\"]");

                    foreach (ElementHandle element in choices) {
                        await element.ClickAsync();
                    } 
                } else if (await storiesPage.QuerySelectorAsync("[data-test=\"stories-token\"]") != null) {
                    ElementHandle[] tokens = await storiesPage.QuerySelectorAllAsync("[data-test=\"stories-token\"]");
                    Random rng = new Random();

                    ElementHandle[] disabledTokens = await storiesPage.QuerySelectorAllAsync("[disabled=\"\"");
                    while (await storiesPage.QuerySelectorAsync("span._3Y29z._176_d._2jNpf") == null || await storiesPage.QuerySelectorAsync("h2._1qFda") != null) {
                        
                        rng.Shuffle<ElementHandle>(tokens);
                        foreach (ElementHandle element in tokens) {
                            disabledTokens = await storiesPage.QuerySelectorAllAsync("[disabled=\"\"");
                            foreach (ElementHandle disabledToken in disabledTokens) {
                                int index = Array.IndexOf(tokens, disabledToken);
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
        async Task ExitStory(Page page) {
            while (await page.QuerySelectorAsync("[data-test=\"stories-player-done\"]") == null) {
                await page.ClickAsync("[data-test=\"stories-player-continue\"]");
            }


            ElementHandle completeButton = await page.WaitForSelectorAsync("[data-test=\"stories-player-done\"]");
            await page.ClickAsync("[data-test=\"stories-player-done\"]");

            await page.WaitForSelectorAsync("div._3wEt9");
        }
        async Task<IEnumerable<String>> GetStoryList(Browser browser) {
            IEnumerable<String> storyUrls = new string[] {};
            Page page = await browser.NewPageAsync();
            await page.GoToAsync("https://www.duolingo.com/stories", new NavigationOptions {Timeout = 0});
            Thread.Sleep(TimeSpan.FromSeconds(2));

            ElementHandle[] storyIcons = await page.QuerySelectorAllAsync("div.X4jDx");


            foreach (ElementHandle element in storyIcons) {
                await element.ClickAsync();
                ElementHandle startButton = await page.QuerySelectorAsync("[data-test=\"story-start-button\"]");
                if (startButton != null) {
                        JSHandle buttonHref = await startButton.GetPropertyAsync("href");
                    object hrefJSON = await buttonHref.JsonValueAsync();

                    storyUrls = storyUrls.Append<String>(hrefJSON.ToString());
                    Console.WriteLine("Appended {0} to list of stories.", await buttonHref.JsonValueAsync());
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
        public static void Shuffle<T> (this Random rng, T[] array) {
            int n = array.Length;
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