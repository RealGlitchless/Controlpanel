using System;
using System.Threading;
using Controlpanel.Controller;
using Controlpanel.Model;

namespace Controlpanel.Menus.Casino
{
    public class Dice
    {
        private readonly Account _user;
        private readonly CasinoController _casinoController = new CasinoController();
        
        public Dice(Account user)
        {
            _user = user;
        }
        
        public void PrintMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Dice");
                Console.WriteLine("How much do you want to bet? Type 0 to go back");
                

                long bet = _casinoController.PlaceBetMenu();
                if(bet == 0)
                    return;

                bool placeBet = _casinoController.PlaceBet(_user, bet);
                if (placeBet)
                {
                    PrintGame(bet);
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Bet is exceeds your current balance");
                    Console.WriteLine("Please reset your money at the menu");
                    Thread.Sleep(2000);
                }
            }
        }

        private void PrintGame(long bet)
        {
            Console.WriteLine("");
            Console.WriteLine($"Your balance is now {_user.Balance}");
            Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
            Thread.Sleep(5000);

            Console.Clear();
            Console.WriteLine("Which number do you want to bet on?");
            int number = Int32.Parse(Console.ReadKey().KeyChar.ToString());
            

            if (number > 6 || number < 1)
            {
                Console.WriteLine("");
                Console.WriteLine("Your number is too high, try again. Your number has to be 6 or less");
                Thread.Sleep(700);
            }

            Thread.Sleep(200);

            int rndNumber = new Random().Next(1, 7);

            Console.Clear();

            Rolling();

            Console.Write($"The number is {rndNumber}");
            Thread.Sleep(500);

            if (rndNumber == number)
            {
                Console.WriteLine("");
                Console.WriteLine("You win");
                _casinoController.Win(_user, bet, 6);
            }

            else
            {
                Console.WriteLine("You lose");
            }
        }

        private void Rolling()
        {
            for(int i = 0; i < 3; i++)
            {
                Console.Write("Rolling");
                Thread.Sleep(100);
                Console.Write(".");
                Thread.Sleep(100);
                Console.Write(".");
                Thread.Sleep(100);
                Console.Write(".");
                Thread.Sleep(100);
                Console.Clear();
            }
        }
    }
}