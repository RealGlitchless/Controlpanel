using System;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus;

public class EditPresetMenu
{
    private readonly Preset _preset;
    private readonly PresetController _presetController;
    public EditPresetMenu(Account user, Preset preset)
    {
        _preset = preset;
        _presetController = new PresetController(user);
    }
        
    public void PrintMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("What do you want to change?");
            Console.Write("1: ");
            Console.WriteLine("Change name");
            Console.Write("2: ");
            Console.WriteLine("Change URL");
            Console.Write("3: ");
            Console.WriteLine("Delete preset");
            Console.Write("0: ");
            Console.WriteLine("Go back");
            Console.WriteLine("*******************************");
            
            char option = Console.ReadKey().KeyChar;
            if(option == '0')
                return;
            ChooseOption(option);
        }
    }
        
    private void ChooseOption(char option)
    {
        switch (option)
        {
            case '1':
                ChangeName();
                break;
            case '2':
                ChangeURL();
                break;
            case '3':
                DeletePreset();
                break;
            default:
                Console.WriteLine("Please press a valid key");
                break;
        }
    }
        
    private void ChangeName()
    {
        Console.Clear();
        Console.WriteLine("What is the new name?");
        string name = Console.ReadLine();
        _preset.Name = name;
        _presetController.Update(_preset);
    }
        
    private void ChangeURL()
    {
        Console.Clear();
        Console.WriteLine("What is the new URL?");
        string URL = Console.ReadLine();
        _preset.URL = URL;
        _presetController.Update(_preset);
    }

    private void DeletePreset()
    {
        Console.Clear();
        Console.WriteLine("Are you sure you want to delete this preset?");
        Console.WriteLine("1: Yes");
        Console.WriteLine("2: No");
        char option = Console.ReadKey().KeyChar;
        if(option == '1')
            _presetController.Delete(_preset);
    }
}