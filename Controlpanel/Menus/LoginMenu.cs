using System;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;
using Controlpanel.Utilities;

namespace Controlpanel.Menus
{
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
                        new CreateAccountMenu().printCreateAccount();
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
        
        private string RedactPassword()
        {
            string pw = "";
            //Replaces input with '*'
            ConsoleKeyInfo input = Console.ReadKey(true);
            while (input.Key != ConsoleKey.Enter)
            {
                if (input.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    pw += input.KeyChar;
                }
                else if (input.Key == ConsoleKey.Backspace && !string.IsNullOrEmpty(pw))
                {
                    // remove one character from the list of password characters
                    pw = pw[0..^1];
                    // get the location of the cursor
                    int pos = Console.CursorLeft;
                    // move the cursor to the left by one character
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    // replace it with space
                    Console.Write(" ");
                    // move the cursor to the left by one character again
                    Console.SetCursorPosition(pos - 1, Console.CursorTop);
                }

                input = Console.ReadKey(true);
            }

            return pw;
        }
    }
}