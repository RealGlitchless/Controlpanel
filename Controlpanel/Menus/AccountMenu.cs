using System;
using Controlpanel.Model;

namespace Controlpanel.Menus;

public class AccountMenu
{
    private readonly Account _user;
        
    public AccountMenu(Account user)
    {
        _user = user;
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
            for(int i = 0; i < _user.Presets.Count; i++)
            {
                Console.Write(i + 1);
                Console.Write(": ");
                Console.WriteLine(_user.Presets[i].Name);
            }
            Console.WriteLine("0: Go back");
            Console.WriteLine("Press a number between 1-10 to get more info");
            Console.WriteLine("\n***********************************\n");
            char option = Console.ReadKey().KeyChar;
            if (option == '0')
                return;
            new EditPresetMenu(_user).PrintMenu();
        }
    }
        
    private void ChangePassword()
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