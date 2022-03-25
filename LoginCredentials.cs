namespace DuolingoAPI.Login {
    public class LoginCredentials {
        public Services Service { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public enum Services {
        Google, Duolingo
    }
}