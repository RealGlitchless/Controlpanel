using System.Collections.Generic;

namespace Controlpanel.Model;

public class Account
{
    public Account(string username, string password)
    {
        Username = username;
        Password = password;
        Balance = 1000;
        Presets = new List<Preset>();
    }
        
    public Account()
    {
    }

    public string Username { get; set; }
        
    public string Password { get; set; }
        
    public long Balance { get; set; }
        
    public List<Preset> Presets { get; set; }
}