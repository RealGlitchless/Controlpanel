using System;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus
{
    public class CreatePresetMenu
    {
        private Account _user;
        
        public CreatePresetMenu(Account user)
        {
            _user = user;
        }

        public void PrintMenu()
        {
            Console.Clear();
            Console.WriteLine("What program do you want as shortcut?");
            string name = Console.ReadLine();
            Console.WriteLine("What is the URL of the program?");
            string URL = Console.ReadLine();
            bool result = CreatePreset(name, URL);
            Console.WriteLine(result ? "Preset created" : "Error creating preset");
            Thread.Sleep(500);
        }
        
        private bool CreatePreset(string name, string URL)
        {
            Preset preset = new Preset(name, URL);
            PresetController presetController = new PresetController(_user);
            return presetController.Create(preset);
        }
    }
}