using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Threading;
using Controlpanel.Menus;
using Controlpanel.Model;

namespace Controlpanel
{
    public abstract class Program
    {
        static Account _account;

        private static void Main()
        {
            // Change the title
            Console.Title = "Soerensen's Controlpanel";
            Console.CursorVisible = false;
            while (_account == null)
            {
                _account = new LoginMenu().printMenu();
            }

            while (true)
            {
                new MainMenu(_account).printMenu();
            }
        }
    }
}