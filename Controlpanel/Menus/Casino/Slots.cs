using System;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus.Casino;

public class Slots
{
    private Account _user;
    private readonly CasinoController _casinoController = new();
    private readonly AccountController _accountController = new();

    public Slots(Account user)
    {
        _user = user;
    }
        
    public void PrintMenu()
    {
        _user = _accountController.GetAccount(_user.Username);
        Console.Clear();
        Console.WriteLine("Welcome to Slots");
        Console.WriteLine("How much do you want to bet per spin?");

        long bet = CasinoController.PlaceBetMenu();
            
        Console.WriteLine(
            "CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
        Thread.Sleep(5000);
        PrintGame(bet);
    }

    private void PrintGame(long bet)
    {
        ConsoleKeyInfo chara = Console.ReadKey(true);
        while (chara.Key != ConsoleKey.Escape)
        {
            Console.Clear();
            Console.WriteLine("Press ESC to go back, Press Backspace to change bet");
            Console.WriteLine(
                "***************   X-X = 3x                                                   ");
            Console.WriteLine(
                "*             *   X-X-Y-Y = 5x                                               ");
            Console.WriteLine(
                "*             *   X-X-X = 10x                                                ");
            Console.WriteLine(
                "*             *   X-X-X-Y-Y = 50x                                            ");
            Console.WriteLine(
                "*             *   X-X-X-X = 100x                                             ");
            Console.WriteLine(
                "*             *   X-X-X-X-X = 1000x                                          ");
            Console.WriteLine(
                "*             *                                                              ");
            Console.WriteLine(
                "***************   For each win you will get a jackpot spin and 100x your win!");

            Console.SetCursorPosition(0, 9);

            // if the bet is above what user has, send user back
            if (!_casinoController.PlaceBet(_user, bet))
            {
                break;
            }
            _user = _accountController.GetAccount(_user.Username);
            Console.WriteLine("Current balance: " + _user.Balance);

            Console.SetCursorPosition(0, 4);
            Console.Write("*  ");
            Thread.Sleep(200);

            char firstIcon = GetSymbol(2);
            char secondIcon = GetSymbol(3);
            char thirdIcon = GetSymbol(4);
            char fourthIcon = GetSymbol(5);
            char fifthIcon = GetSymbol(6);

            //X-X-X-X-X
            if (firstIcon == secondIcon && secondIcon == thirdIcon && thirdIcon == fourthIcon &&
                fourthIcon == fifthIcon)
            {
                Win(bet, 1000);
            }

            //X-X-X-X-Y
            else if (firstIcon == secondIcon && secondIcon == thirdIcon && thirdIcon == fourthIcon)
            {
                Win(bet, 100);
            }

            //X-X-X-Y-Y
            else if (firstIcon == secondIcon && secondIcon == thirdIcon && fourthIcon == fifthIcon)
            {
                Win(bet, 50);
            }

            else if (firstIcon == secondIcon && thirdIcon == fourthIcon && fourthIcon == fifthIcon)
            { 
                Win(bet, 10);
            }

            //X-X-Y-Y-Z
            else if (firstIcon == secondIcon)
            {
                if (thirdIcon == fourthIcon)
                {
                    Win(bet, 5);
                }

                Win(bet, 2);
            }
        }
    }

    private void Win(long bet, int multiplier)
    {
        Console.SetCursorPosition(0, 10);

        if (IsJackpot())
        {
            Console.WriteLine("Congratulations! You won the jackpot!");
            multiplier *= 100;
        }

        _casinoController.Win(_user , bet, multiplier);
    }
        
    private static char GetSymbol(int x)
    {
        string symbols = Symbols();

        Random rndNumber = new();
        char symbol = ' '; 
        for (int i = 0; i < 5; i++)
        {
            Console.SetCursorPosition(3, 4);
            symbol = symbols[rndNumber.Next(0, symbols.Length)];
            Console.Write(symbol);
            Console.SetCursorPosition(x, 4);
            Thread.Sleep(100);
        }

        return symbol;
    }

    private static bool IsJackpot()
    {
        Random rnd = new();

        int userNum = 0;
        int jackpotNum = 0;
        for (int i = 0; i < 24; i++)
        {
            userNum = rnd.Next(0, 99) + 1;
        }
        Console.WriteLine("Your number is: " + userNum);

        for (int i = 0; i < 24; i++)
        {
            jackpotNum = rnd.Next(0, 99) + 1;
        }
        Console.WriteLine("Jackpot number is: " + jackpotNum);

        return userNum == jackpotNum;
    }

    private static string Symbols()
    {
        return "€&7¤#%§½@=+";
    }
}