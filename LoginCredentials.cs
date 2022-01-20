using PuppeteerSharp;

namespace DuolingoAPI.Login {
    public struct LoginCredentials : Credentials {
        public Services Service { get; set; }
    }

    public enum Services {
        Google, Duolingo
    }
}