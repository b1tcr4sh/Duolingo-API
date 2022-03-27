using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Playwright;
using Microsoft.Playwright.Core;
using Microsoft.Playwright.Transport;

namespace DuolingoAPI.Login
{
    public class LoginManager
    {
        public CredentialPair Credentials { get; private set; }
        private Func<Task> HandleIncorrectLogin;

        public LoginManager(Func<Task> HandleIncorrectLoginMethod, CredentialPair credentials) {
            Credentials = credentials;
            HandleIncorrectLogin = HandleIncorrectLoginMethod;
        }
        public LoginManager(Func<Task> HandleIncorrectLoginMethod) {
            HandleIncorrectLogin = HandleIncorrectLoginMethod;
        }


        public static CredentialPair CollectCredentials() {
            LoginCredentials duolingo = CollectServiceCredentials(Services.Duolingo);
            LoginCredentials google = CollectServiceCredentials(Services.Google);

            if (google == null) google = duolingo;

            return new CredentialPair {
                DuolingoCredentials = duolingo,
                GoogleCredentials = google
            };
        }
        private static LoginCredentials CollectServiceCredentials(Services serviceName)
        {
            if (serviceName == Services.Google) {
                Console.Write("Is your Google login the same as Duolingo?  If not, you'll have to re-enter crendentials. (y/N) > ");
                ConsoleKeyInfo response = Console.ReadKey();
                Console.WriteLine();

                if (response.Key == ConsoleKey.Y) {
                    return null;
                } 
            }

            LoginCredentials credentials = new LoginCredentials();  

            Console.Write($"{serviceName.ToString()} Username > ");
            credentials.Username = Console.ReadLine();

            Console.Write($"{serviceName.ToString()} Password > ");
            // Collect Password without showing text
            StringBuilder input = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) {
                    input.Remove(input.Length - 1, 1);
                    Console.Write($"\r{serviceName} Password > ");
                    for (int i = 0; i < input.Length; i++) Console.Write("*");
                }
                else if (key.Key != ConsoleKey.Backspace) {
                    input.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            credentials.Password = input.ToString(); // Console.ReadLine();
            credentials.Service = serviceName;

            return credentials;
        }

        internal async Task LoginAsync(IPage page) {
            page.Popup += new EventHandler<IPage>(FallBackWithGoogleLogin);


            Console.WriteLine("Attempting to Log In...");
            await page.WaitForSelectorAsync("input._3MNft.fs-exclude");


            await page.TypeAsync("[data-test=\"email-input\"]", Credentials.DuolingoCredentials.Username);
            await page.TypeAsync("[data-test=\"password-input\"]", Credentials.DuolingoCredentials.Password);

            await page.ClickAsync("button._1rl91._3HhhB._2NolF._275sd._1ZefG._2oW4v");

            Thread.Sleep(TimeSpan.FromSeconds(2));

            IElementHandle googleButton = await page.QuerySelectorAsync("_3HhhB._2NolF._275sd._1ZefG._2Dar-");
            if (googleButton != null) {
                await googleButton.ClickAsync();
            }

            await page.RunAndWaitForNavigationAsync(() => {
                return Task.CompletedTask;
            }, new PageRunAndWaitForNavigationOptions { Timeout = 0 });

            // Attempt at checking for incorrect passwords
            if (await page.QuerySelectorAsync("[data-test=\"invalid-form-field\"]") != null) {
                await HandleIncorrectLogin();
            }            
        }
        private async void FallBackWithGoogleLogin(object sender, IPage googlePopup)
        {
            Console.WriteLine("\"Continue With Google\" Popup appeared.  It appears the account was created with Google.");


            await googlePopup.WaitForSelectorAsync("input.whsOnd.zHQkBf");

            await googlePopup.TypeAsync("[type=\"email\"]", Credentials.GoogleCredentials.Username);
            await googlePopup.ClickAsync("button.VfPpkd-LgbsSe.VfPpkd-LgbsSe-OWXEXe-k8QpJ.VfPpkd-LgbsSe-OWXEXe-dgl2Hf.nCP5yc.AjY5Oe.DuMIQc.qIypjc.TrZEUc.lw1w4b");

            // TODO: Handle incorrect email/password

            Thread.Sleep(4000);
            await googlePopup.WaitForSelectorAsync("[type=\"password\"]");
            await googlePopup.TypeAsync("[type=\"password\"]", Credentials.GoogleCredentials.Password);
            await googlePopup.ClickAsync("button.VfPpkd-LgbsSe.VfPpkd-LgbsSe-OWXEXe-k8QpJ.VfPpkd-LgbsSe-OWXEXe-dgl2Hf.nCP5yc.AjY5Oe.DuMIQc.qIypjc.TrZEUc.lw1w4b");
        }
    }
}
