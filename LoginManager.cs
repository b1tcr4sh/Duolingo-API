using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PuppeteerSharp;

namespace DuolingoAPI.Login
{
    public class LoginManager
    {
        public LoginCredentials Credentials { get; private set; }
        private Func<Page, Task> HandleIncorrectLogin;

        public LoginManager(Func<Page, Task> HandleIncorrectLoginMethod, string username, string password) {
            Credentials = new LoginCredentials {
                Username = username,
                Password = password
            };
            HandleIncorrectLogin = HandleIncorrectLoginMethod;
        }
        public LoginManager(Func<Page, Task> HandleIncorrectLoginMethod) {
            HandleIncorrectLogin = HandleIncorrectLoginMethod;
        }


        public static LoginCredentials CollectCredentials(Services serviceName)
        {
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
        public async Task LoginAsync(Page page) {
            page.Popup += new EventHandler<PopupEventArgs>(FallBackWithGoogleLogin); 


            // LoginCredentials credentials = CollectCredentials("Duolingo");

            Console.WriteLine("Attempting to Log In...");
            await page.WaitForSelectorAsync("input._3MNft.fs-exclude");


            await page.TypeAsync("[data-test=\"email-input\"]", Credentials.Username);
            await page.TypeAsync("[data-test=\"password-input\"]", Credentials.Password);

            await page.ClickAsync("button._1rl91._3HhhB._2NolF._275sd._1ZefG._2oW4v");

            Thread.Sleep(TimeSpan.FromSeconds(5));
            // Attempt at checking for incorrect passwords
            if (await page.QuerySelectorAsync("[data-test=\"invalid-form-field\"]") != null) {
                await HandleIncorrectLogin(page);
            }            
        }
        private async void FallBackWithGoogleLogin(object sender, PopupEventArgs e)
        {
            // TODO: Make collecting Google credentials independent from terminal.  It should get them elsewhere to ensure this doesn't have to be used in terminal.



            Console.WriteLine("\"Continue With Google\" Popup appeared.  It appears the account was created with Google.");
            // LoginManager passwordManager = new LoginManager();

            Console.Write("Is your Google login the same as Duolingo?  If not, you'll have to re-enter crendentials. (y/N) > ");
            ConsoleKeyInfo response = Console.ReadKey();
            Console.WriteLine();

            if (response.Key == ConsoleKey.N) {
                Credentials = CollectCredentials(Services.Google);
            } else {
                Console.WriteLine("Continuing with previously entered credentials...");
            }


            Page googlePopup = e.PopupPage;
            await googlePopup.WaitForSelectorAsync("input.whsOnd.zHQkBf");

            // LoginCredentials googleCredentials = passwordManager.CollectCredentials("Google");

            await googlePopup.TypeAsync("[type=\"email\"]", Credentials.Username);
            await googlePopup.ClickAsync("button.VfPpkd-LgbsSe.VfPpkd-LgbsSe-OWXEXe-k8QpJ.VfPpkd-LgbsSe-OWXEXe-dgl2Hf.nCP5yc.AjY5Oe.DuMIQc.qIypjc.TrZEUc.lw1w4b");

            // TODO: Handle incorrect email/password

            Thread.Sleep(4000);
            await googlePopup.WaitForSelectorAsync("[type=\"password\"]");
            await googlePopup.TypeAsync("[type=\"password\"]", Credentials.Password);
            await googlePopup.ClickAsync("button.VfPpkd-LgbsSe.VfPpkd-LgbsSe-OWXEXe-k8QpJ.VfPpkd-LgbsSe-OWXEXe-dgl2Hf.nCP5yc.AjY5Oe.DuMIQc.qIypjc.TrZEUc.lw1w4b");
        }
    }
}
