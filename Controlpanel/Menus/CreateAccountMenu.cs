using System;
using System.Text;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus;

public class CreateAccountMenu
{
    private readonly AccountController _accountController;

    public CreateAccountMenu()
    {
        _accountController = new AccountController();
    }

    public void PrintCreateAccount()
    {
        Console.Clear();
        //Get username and pw
        Console.WriteLine("Creating account");
        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = RedactPassword();

        Console.WriteLine("");

        Account account = new(username, password);

        bool result = _accountController.CreateAccount(account);
        Console.WriteLine(result ? "Account has been made, please sign in" : "Account already exists");
        Thread.Sleep(500);
    }

    private static string RedactPassword()
    {
        var password = new StringBuilder();
        ConsoleKeyInfo input;

        do
        {
            input = Console.ReadKey(true);

            if (input.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (char.IsLetterOrDigit(input.KeyChar) || char.IsPunctuation(input.KeyChar))
            {
                password.Append(input.KeyChar);
                Console.Write("*");
            }
        } while (input.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password.ToString();
    }
}