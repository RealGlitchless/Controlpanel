using System;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus
{
    public class EditPresetMenu
    {
        private readonly Account _user;
        private readonly AccountController _accountController = new AccountController();
        public EditPresetMenu(Account user)
        {
            _user = user;
        }
        
        public void PrintMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("What do you want to change?");
                Console.Write("1: ");
                Console.WriteLine("Change name");
                Console.Write("2: ");
                Console.WriteLine("Change URL");
                Console.Write("0: ");
                Console.WriteLine("Go back");
            
                char option = Console.ReadKey().KeyChar;
                if(option == '0')
                    return;
                ChooseOption(option);
            }
        }
        
        private void ChooseOption(char option)
        {
            Console.Clear();
            switch (option)
            {
                case '1':
                    ChangeName();
                    break;
                case '2':
                    ChangeURL();
                    break;
                default:
                    Console.WriteLine("Please press a valid key");
                    break;
            }
        }
        
        private void ChangeName()
        {
            Console.Clear();
            Console.WriteLine("What is the new name?");
            string name = Console.ReadLine();
            _user.Presets[0].Name = name;
            _accountController.UpdateAccount(_user);
        }
        
        private void ChangeURL()
        {
            Console.Clear();
            Console.WriteLine("What is the new URL?");
            string URL = Console.ReadLine();
            _user.Presets[0].URL = URL;
            _accountController.UpdateAccount(_user);
        }
    }
}