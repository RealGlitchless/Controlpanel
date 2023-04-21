using System;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus.Casino;

public class Slots
{
    private Account _user;
    private readonly CasinoController _casinoController = new CasinoController();
    private readonly AccountController _accountController = new AccountController();

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

        long bet = _casinoController.PlaceBetMenu();
            
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
                int multiplier = 1000;
                Win(bet, multiplier);
            }

            //X-X-X-X-Y
            else if (firstIcon == secondIcon && secondIcon == thirdIcon && thirdIcon == fourthIcon)
            {
                int multiplier = 100;
                Win(bet, multiplier);
            }

            //X-X-X-Y-Y
            else if (firstIcon == secondIcon && secondIcon == thirdIcon && fourthIcon == fifthIcon)
            {
                int multiplier = 50;
                Win(bet, multiplier);
            }

            else if (firstIcon == secondIcon && thirdIcon == fourthIcon && fourthIcon == fifthIcon)
            { 
                int multiplier = 10;
                Win(bet, multiplier);
            }

            //X-X-Y-Y-Z
            else if (firstIcon == secondIcon)
            {
                int multiplier;
                if (thirdIcon == fourthIcon)
                {
                    multiplier = 5;
                    Win(bet, multiplier);
                }

                multiplier = 2;
                Win(bet, multiplier);
            }
        }
    }

    private void Win(long bet, int multiplier)
    {
        Console.SetCursorPosition(0, 10);

        if (isJackpot())
        {
            Console.WriteLine("Congratulations! You won the jackpot!");
            multiplier *= 100;
        }

        _casinoController.Win(_user , bet, multiplier);
    }
        
    private char GetSymbol(int x)
    {
        string symbols = Symbols();

        Random rndNumber = new Random();
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

    private bool isJackpot()
    {
        Random rnd = new Random();

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

    private string Symbols()
    {
        return "€&7¤#%§½@=+";
    }
}