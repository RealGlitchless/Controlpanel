using System;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus.Casino;

public class CasinoMenu
{
    private Account _user;
    private readonly AccountController _accountController = new();
    public CasinoMenu(Account user)
    {
        _user = user;
    }
        
    public void PrintMenu()
    {
        while (true)
        {
            Console.Clear();
            _user = _accountController.GetAccount(_user.Username);
            Console.WriteLine("Welcome to Soerensen's casino");
            Console.WriteLine("Logged in as: " + _user.Username);
            Console.WriteLine($"Balance: " + _user.Balance);

            Console.WriteLine("What would you like to play?");
            Console.Write("1: ");
            Console.WriteLine("Black Jack // Not working");
            Console.Write("2: ");
            Console.WriteLine("Roulette");
            Console.Write("3: ");
            Console.WriteLine("Dice");
            Console.Write("4: ");
            Console.WriteLine("Baccarat");
            Console.Write("5: ");
            Console.WriteLine("Slots");
            Console.Write("6: ");
            Console.WriteLine("Reset money");
            Console.Write("0: ");
            Console.WriteLine("Go back");
            Console.WriteLine("***********************************");
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
            default:
            {
                Thread.Sleep(500);
                Console.WriteLine("");
                Console.WriteLine("Please enter a valid number");
                Thread.Sleep(1000);
                break;
            }

            case '1':
            {
                Console.WriteLine("Under construction");
                break;
            }
                
            case '2':
            {
                Console.WriteLine("Under construction");
                break;
            }

            case '3':
            {
                new Dice(_user).PrintMenu();
                break;
            }

            case '4':
            {
                Console.WriteLine("Under construction");
                break;
            }

            case '5':
            {
                new Slots(_user).PrintMenu();
                break;
            }

            case '6':
            {
                _user.Balance = 1000;
                _accountController.UpdateAccount(_user);
                break;
            }
        }
    }
}