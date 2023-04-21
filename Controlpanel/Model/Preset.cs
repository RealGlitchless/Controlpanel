namespace Controlpanel.Model;

public class Preset
{
    public Preset(string name, string url)
    {
        Name = name;
        URL = url;
    }
        
    public string Name { get; set; }
        
    public string URL { get; set; }
}