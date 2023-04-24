using System;
using Controlpanel.Model;

namespace Controlpanel.Controller;

public class CasinoController
{
    private readonly AccountController _accountController;
        
    public CasinoController()
    {
        _accountController = new AccountController();
    }

    public static long PlaceBetMenu()
    {
        string num = "";

        // Allow user only to use numbers
        ConsoleKeyInfo chr;
        do
        {
            chr = Console.ReadKey(true);
            if (chr.Key != ConsoleKey.Backspace)
            {
                bool control = double.TryParse(chr.KeyChar.ToString(), out _);
                if (!control) continue;
                num += chr.KeyChar;
                Console.Write(chr.KeyChar);
            }
            else if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
            {
                num = num[^1..];
                Console.Write("\b \b");
            }
        } while (chr.Key != ConsoleKey.Enter);

        return long.Parse(num);
    }
        
    public bool PlaceBet(Account user, long bet)
    {
        if (user.Balance < bet)
        {
            return false;
        }

        user.Balance -= bet;
        _accountController.UpdateAccount(user);
        return true;
    }

    public void Win(Account user, long bet, float multiplier)
    {
        user.Balance += (long) (bet * multiplier);
        _accountController.UpdateAccount(user);
    }

    public void ReturnBet(Account user, long bet)
    {
        user.Balance += bet;
        _accountController.UpdateAccount(user);
    }
}