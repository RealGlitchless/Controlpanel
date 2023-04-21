namespace Controlpanel.Model
{
    public class Preset
    {
        private string _name;
        private string _URL;
        
        public Preset(string name, string URL)
        {
            _name = name;
            _URL = URL;
        }
        
        public string Name { get; set; }
        
        public string URL { get; set; }
    }
}