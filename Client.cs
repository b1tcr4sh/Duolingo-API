using System;
using System.Threading.Tasks;
using PuppeteerSharp;
using DuolingoAPI.Login;

namespace DuolingoAPI {
    public class Client {
        public ClientOptions Options;

        private Browser browser;

        public Client(ClientOptions options) {
           Options = options; 
        }
        public Client() {}

        public async Task LoginToDuolingo(LoginCredentials loginCredentials) {
            LoginManager loginManager = new LoginManager(HandleIncorrectLogin, loginCredentials.Username, loginCredentials.Password);

            browser = await Puppeteer.LaunchAsync(new LaunchOptions {
                Headless = Options.BrowserHeadless,
                Timeout = Options.BrowserTimeout               
            });
            
            Page page = await browser.NewPageAsync();
            await page.GoToAsync("https://www.duolingo.com/?isLoggingIn=true", new NavigationOptions {Timeout = 0});

            await page.WaitForSelectorAsync("[data-test=have-account]");
            await page.ClickAsync("div._3uMJF");

            await loginManager.LoginAsync(page);

            while (await page.QuerySelectorAsync("button._3HhhB._2NolF._275sd._1ZefG._2Dar-._2zhZF") != null) {
                if (await page.QuerySelectorAsync("button._3HhhB._2NolF._275sd._1ZefG._2Dar-._2zhZF") != null)
                    await page.ClickAsync("button._3HhhB._2NolF._275sd._1ZefG._2Dar-._2zhZF");
            }
        }
        private async Task HandleIncorrectLogin(Page page) {
            Console.WriteLine("Incorrect Login Credentials Detected, please re-enter your credentials:");

            LoginCredentials newCredentials = LoginManager.CollectCredentials(Services.Duolingo);
            await LoginToDuolingo(newCredentials);
        }  
    }
}