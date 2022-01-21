using PuppeteerSharp;

namespace DuolingoAPI.Login {
    public class LoginCredentials : Credentials {
        public Services Service { get; set; }
    }

    public enum Services {
        Google, Duolingo
    }
}