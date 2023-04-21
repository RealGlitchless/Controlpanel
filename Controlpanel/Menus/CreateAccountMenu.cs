using System;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus
{
    public class CreateAccountMenu
    {
        private readonly AccountController _accountController;

        public CreateAccountMenu()
        {
            _accountController = new AccountController();
        }

        public void printCreateAccount()
        {
            Console.Clear();
            //Get username and pw
            Console.WriteLine("Creating account");
            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = RedactPassword();

            Console.WriteLine("");

            Account account = new Account(username, password);

            bool result = _accountController.CreateAccount(account);
            Console.WriteLine(result ? "Account has been made, please sign in" : "Account already exists");
            Thread.Sleep(500);
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