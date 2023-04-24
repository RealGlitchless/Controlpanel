using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus;

public class AccountMenu
{
    private static Account _user;
    private readonly PresetController _presetController;
    public AccountMenu(Account user)
    {
        _user = user;
        _presetController = new PresetController(_user);
    }

    public void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("What do you want to change?");
        Console.Write("1: ");
        Console.WriteLine("Change presets");
        Console.Write("2: ");
        Console.WriteLine("Change password");
        Console.Write("0: ");
        Console.WriteLine("Go back");

        char option = Console.ReadKey().KeyChar;
        ChooseOption(option);
    }

    private void ChooseOption(char option)
    {
        Console.Clear();
        switch (option)
        {
            case '1':
                ChangePreset();
                break;
            case '2':
                ChangePassword();
                break;
            case '0':
                return;
            default:
                Console.WriteLine("Please press a valid key");
                break;
        }
    }

    private void ChangePreset()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("What preset do you want to change?");
            List<Preset> presets = _presetController.GetAll();
            for(int i = 0; i < presets.Count; i++)
            {
                Console.Write(i + 1);
                Console.Write(": ");
                Console.WriteLine(presets[i].Name);
            }
            Console.WriteLine("0: Go back");
            Console.WriteLine("\n***********************************\n");
            char option = Console.ReadKey().KeyChar;
            if (option == '0')
                return;
            int index = int.Parse(option.ToString()) - 1;

            if (index < 0 || presets.Count < index)
            {
                Console.WriteLine("Invalid choice");
                Thread.Sleep(500);
                return;
            }

            EditPresetMenu editPresetMenu = new EditPresetMenu(_user, presets[index]);
            editPresetMenu.PrintMenu();
        }
    }
        
    private static void ChangePassword()
    {
        Console.Clear();
        Console.WriteLine("What is your current password?");
        string password = Console.ReadLine();
        if (password != _user.Password)
        {
            Console.WriteLine("Your password is incorrect");
            return;
        }
        Console.WriteLine("What is your new password?");
        string newPassword = Console.ReadLine();
        _user.Password = newPassword;
        Console.WriteLine("Your password has been changed");
    }
}