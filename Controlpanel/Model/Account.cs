using System.Collections.Generic;

namespace Controlpanel.Model
{
    public class Account
    {
        private string _username;
        private string _password;
        private long _balance;
        private List<Preset> _presets;

        public Account(string username, string password)
        {
            _username = username;
            _password = password;
            _balance = 1000;
            _presets = new List<Preset>();
        }
        
        public Account(string username, string password, long balance, List<Preset> presets)
        {
            _username = username;
            _password = password;
            _balance = balance;
            _presets = presets;
        }

        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public long Balance { get; set; }
        
        public List<Preset> Presets { get; set; }
    }
}