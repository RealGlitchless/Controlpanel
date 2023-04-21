using System;
using System.Text;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus;

public class LoginMenu
{
    private readonly AccountController _accountController;

    public LoginMenu()
    {
        _accountController = new AccountController();
    }
        
    public Account PrintMenu()
    {
        Account account = null;
        while (account == null)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Controlpanel");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create account");
            Console.WriteLine("3. Exit");
            Console.Write("Choice: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    account = PrintLogin();
                    break;
                case "2":
                    new CreateAccountMenu().PrintCreateAccount();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    Thread.Sleep(500);
                    break;
            }
        }

        return account;
    }
        
    private Account PrintLogin()
    {
        Console.Clear();
        Account account = null;
        Console.WriteLine("Login");
        Console.Write("Username: ");
        string username = Console.ReadLine();
        Console.Write("Password: ");
        string password = RedactPassword();
        Console.WriteLine("");
        bool result = CheckCredentials(username, password);
        if (result)
        {
            Console.WriteLine("Login successful");
            account = _accountController.GetAccount(username);
        }
        else
        {
            Console.WriteLine("Login failed");
        }
        Thread.Sleep(500);
        return account;
    }

    private bool CheckCredentials(string username, string password)
    {
        Account account = _accountController.GetAccount(username);
        if (account == null)
        {
            return false;
        }
        return account.Password == password;
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