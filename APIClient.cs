using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using DuolingoAPI.Login;

// TODO:
// Get Duolingo Assignments, Get all-time XP, Get currency (Lingots, Crowns), Get streak, Get lesson information by name.

namespace DuolingoAPI {
    public class APIClient {
        public ClientOptions Options;

        private IBrowser browser;

        public APIClient(ClientOptions options) {
           Options = options; 
        }
        public APIClient() {}

        public async Task LoginToDuolingo(LoginCredentials loginCredentials) {
            LoginManager loginManager = new LoginManager(HandleIncorrectLogin, loginCredentials.Username, loginCredentials.Password);

            IPlaywright playwright = await Playwright.CreateAsync();
            browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions {
                Headless = Options.BrowserHeadless,
                Timeout = Options.BrowserTimeout
            });


            // browser = await Puppeteer.LaunchAsync(new LaunchOptions {
            //     Headless = Options.BrowserHeadless,
            //     Timeout = Options.BrowserTimeout               
            // });
            
            IPage page = await browser.NewPageAsync();
            await page.GotoAsync("https://www.duolingo.com/?isLoggingIn=true");

            await page.WaitForSelectorAsync("[data-test=have-account]");
            await page.ClickAsync("div._3uMJF");

            await loginManager.LoginAsync(page);

            while (await page.QuerySelectorAsync("button._3HhhB._2NolF._275sd._1ZefG._2Dar-._2zhZF") != null) {
                if (await page.QuerySelectorAsync("button._3HhhB._2NolF._275sd._1ZefG._2Dar-._2zhZF") != null)
                    await page.ClickAsync("button._3HhhB._2NolF._275sd._1ZefG._2Dar-._2zhZF");
            }
        }
        public virtual async Task HandleIncorrectLogin(IPage page) {
            Console.WriteLine("Incorrect Login Credentials Detected, please re-enter your credentials:");

            LoginCredentials newCredentials = LoginManager.CollectCredentials(Services.Duolingo);
            await LoginToDuolingo(newCredentials);
        }  
    }
}