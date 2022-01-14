using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PuppeteerSharp;

namespace DuolingoAPI.Login
{
    class LoginManager
    {
        public LoginCredentials Credentials { get; private set; }

        public LoginManager(string username, string password) {
            Credentials = new LoginCredentials {
                Username = username,
                Password = password
            };
        }
        public LoginManager() {}


        public LoginCredentials CollectCredentials(string serviceName)
        {
            LoginCredentials credentials = new LoginCredentials();  

            Console.Write($"{serviceName} Username > ");
            credentials.Username = Console.ReadLine();

            Console.Write($"{serviceName} Password > ");
            credentials.Password = Console.ReadLine();


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
            // Attempt at checking for incorrect passwords
            if (await page.QuerySelectorAsync("[data-test=\"invalid-form-field\"]") != null) {
                await HandleIncorrectLogin();
            }            
        }
        private async void FallBackWithGoogleLogin(object sender, PopupEventArgs e)
        {
            Console.WriteLine("\"Continue With Google\" Popup appeared.  It appears the account was created with Google.");
            // LoginManager passwordManager = new LoginManager();

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

        public virtual async Task HandleIncorrectLogin() {
            Console.WriteLine("Incorrect Login Information Entered");
        }
    }
}
