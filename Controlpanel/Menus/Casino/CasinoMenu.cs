using System;
using System.IO;
using System.Threading;

namespace Controlpanel.Menus.Casino
{
    public class CasinoMenu
    {
        static void Casino(string username)
        {
            Console.Clear();
            string root = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logins";

            string balancetxt = $"{root}\\{username}\\Casino\\Balance";

            // Get balance from balance file
            int balance = GetBalance(balancetxt);

            Console.WriteLine("Welcome to Soerensen's casino");
            Console.WriteLine("Logged in as: " + username);
            Console.WriteLine($"Balance: " + balance);

            Console.WriteLine("What would you like to play?");
            Console.Write("1: ");
            // When hitting 2 times and then stand dealer wins when dealer is lower
            Console.WriteLine("Black Jack // Not working");
            Console.Write("2: ");
            Console.WriteLine("Roulette // Should work");
            Console.Write("3: ");
            Console.WriteLine("Dice // Should work");
            Console.Write("4: ");
            Console.WriteLine("Baccarat // Should work");
            Console.Write("5: ");
            Console.WriteLine("Slots // Working");
            Console.Write("6: ");
            Console.WriteLine("Reset money // Working");
            Console.Write("0: ");
            Console.WriteLine("Go back");

            char gamemodechoice = Console.ReadKey().KeyChar;

            while (true)
            {
                switch (gamemodechoice)
                {
                    default:
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("");
                        Console.WriteLine("Please enter a valid number");
                        Thread.Sleep(1000);
                    }
                        continue;

                    case '1':
                    {
                        Console.WriteLine("Under construction");
                    }
                        continue;

                    case '2':
                    {
                        Console.WriteLine("Under construction");
                    }
                        continue;

                    case '3':
                    {
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Welcome to Dice");
                            Console.WriteLine("How much do you want to bet?");
                            string num = "";

                            // Allow user only to use numbers
                            ConsoleKeyInfo chr;
                            do
                            {
                                chr = Console.ReadKey(true);
                                if (chr.Key != ConsoleKey.Backspace)
                                {
                                    bool control = double.TryParse(chr.KeyChar.ToString(), out _);
                                    if (control)
                                    {
                                        num += chr.KeyChar;
                                        Console.Write(chr.KeyChar);
                                    }
                                }
                                else if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                {
                                    num = num[^1..];
                                    Console.Write("\b \b");
                                }
                            } while (chr.Key != ConsoleKey.Enter);

                            int bet = Int32.Parse(num);

                            // if the bet is above what user has, send user back
                            if (PlaceBet(root, bet))
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Bet is exceeds your current balance");
                                Console.WriteLine("Please reset your money at the menu");
                                Thread.Sleep(2000);
                                continue;
                            }

                            // if bet isnt above total amount then subtract bet from total and put in file
                            Console.WriteLine("");
                            Console.WriteLine($"Your balance is now {GetBalance(balancetxt)}");
                            Console.WriteLine(
                                "CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                            Thread.Sleep(5000);

                            Console.Clear();
                            Console.WriteLine("Which number do you want to bet on?");
                            char number = Console.ReadKey().KeyChar;
                            int newnumber = int.Parse(number.ToString());

                            if (newnumber > 6 || number <= 0)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Your number is too high, try again. Your number has to be 6 or less");
                                Thread.Sleep(700);
                                continue;
                            }

                            File.WriteAllText(balancetxt, Encoder(balance.ToString()));

                            Thread.Sleep(200);

                            Random rnd = new Random();
                            int rndnumber = rnd.Next(1, 6);

                            Console.Clear();

                            Rolling();

                            Console.Write($"The number is {rndnumber}");
                            Thread.Sleep(500);

                            if (rndnumber == newnumber)
                            {
                                Win(username, bet, 6);
                            }

                            else
                            {
                                Console.WriteLine("You lose");
                            }

                            Console.WriteLine("Would you like to play again? y/n");
                            char playagain = Console.ReadKey().KeyChar;

                            if (playagain == 'y')
                            {
                                continue;
                            }

                            break;
                        }
                    }
                        continue;

                    case '4':
                    {
                        Console.WriteLine("Under construction");
                    }
                        continue;

                    case '5':
                    {
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Welcome to Slots");
                            Console.WriteLine("How much do you want to bet?");

                            if (!Int32.TryParse(Console.ReadLine(), out int bet))
                            {
                                Console.WriteLine("Invalid value");
                                Thread.Sleep(500);
                                continue;
                            }

                            // Get balance from balance file
                            PlaceBet(balancetxt, bet);
                            Console.WriteLine("");
                            Console.WriteLine($"Your balance is now {GetBalance(balancetxt)}");
                            Console.WriteLine(
                                "CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                            Thread.Sleep(5000);

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
                                if (!PlaceBet(balancetxt, bet))
                                {
                                    break;
                                }

                                Console.WriteLine("Current balance: " + GetBalance(balancetxt));

                                Console.SetCursorPosition(0, 4);
                                Console.Write("*  ");
                                Thread.Sleep(200);

                                static string get_symbol(int x)
                                {
                                    string[] icons = new string[11];
                                    icons[0] = "€";
                                    icons[1] = "&";
                                    icons[2] = "7";
                                    icons[3] = "¤";
                                    icons[4] = "#";
                                    icons[5] = "%";
                                    icons[6] = "§";
                                    icons[7] = "½";
                                    icons[8] = "@";
                                    icons[9] = "=";
                                    icons[10] = "+";

                                    Random rndicons = new Random();
                                    string icon = "";
                                    for (int i = 0; i < 5; i++)
                                    {
                                        Console.SetCursorPosition(3, 4);
                                        icon = icons[rndicons.Next(0, 10)];
                                        Console.Write(icon);
                                        Console.SetCursorPosition(x, 4);
                                        Thread.Sleep(100);
                                    }

                                    return icon;
                                }

                                static (int user, int jackpot) jackpot()
                                {
                                    Random rnd = new Random();

                                    int userNum = 0;
                                    int jackpotNum = 0;
                                    for (int i = 0; i < 24; i++)
                                    {
                                        userNum = rnd.Next(0, 99) + 1;
                                    }

                                    for (int i = 0; i < 24; i++)
                                    {
                                        jackpotNum = rnd.Next(0, 99) + 1;
                                    }

                                    return (userNum, jackpotNum);
                                }

                                var nums = jackpot();
                                int userNum = nums.user;
                                int jackpotNum = nums.jackpot;

                                string firstIcon = get_symbol(2);
                                string secondIcon = get_symbol(3);
                                string thirdIcon = get_symbol(4);
                                string fourthIcon = get_symbol(5);
                                string fifthIcon = get_symbol(6);

                                //X-X-X-X-X
                                if (firstIcon == secondIcon && secondIcon == thirdIcon && thirdIcon == fourthIcon &&
                                    fourthIcon == fifthIcon)
                                {

                                    Console.SetCursorPosition(0, 10);
                                    Console.Write("Your number is: " + userNum);

                                    Console.WriteLine("");

                                    int multiplier = 1000;
                                    if (userNum == jackpotNum)
                                    {
                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                        multiplier *= 100;
                                    }

                                    Win(username, bet, multiplier);
                                    continue;
                                }

                                //X-X-X-X-Y
                                else if (firstIcon == secondIcon && secondIcon == thirdIcon && thirdIcon == fourthIcon)
                                {
                                    Console.SetCursorPosition(0, 10);
                                    Console.Write("Your number is: " + userNum);

                                    Console.WriteLine("");

                                    int multiplier = 100;
                                    if (userNum == jackpotNum)
                                    {
                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                        multiplier *= 100;
                                    }

                                    Win(username, bet, multiplier);
                                    continue;
                                }

                                //X-X-X-Y-Y
                                else if (firstIcon == secondIcon && secondIcon == thirdIcon && fourthIcon == fifthIcon)
                                {
                                    Console.SetCursorPosition(0, 10);
                                    Console.Write("Your number is: " + userNum);

                                    Console.WriteLine("");

                                    int multiplier = 10;
                                    if (userNum == jackpotNum)
                                    {
                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                        multiplier *= 100;
                                    }

                                    Win(username, bet, multiplier);
                                    continue;
                                }

                                else if (firstIcon == secondIcon && thirdIcon == fourthIcon && fourthIcon == fifthIcon)
                                {
                                    Console.SetCursorPosition(0, 10);
                                    Console.Write("Your number is: " + userNum);

                                    Console.WriteLine("");

                                    int multiplier = 10;
                                    if (userNum == jackpotNum)
                                    {
                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                        multiplier *= 100;
                                    }

                                    Win(username, bet, multiplier);
                                    continue;
                                }

                                //X-X-Y-Y-Z
                                else if (firstIcon == secondIcon)
                                {
                                    int multiplier;
                                    if (thirdIcon == fourthIcon)
                                    {
                                        Console.SetCursorPosition(0, 10);
                                        Console.Write("Your number is: " + userNum);
                                        Console.WriteLine("");

                                        multiplier = 5;
                                        if (userNum == jackpotNum)
                                        {
                                            Console.WriteLine("Congratulations! You won the jackpot!");
                                            multiplier *= 100;
                                        }

                                        Win(username, bet, multiplier);
                                        continue;
                                    }

                                    Console.SetCursorPosition(0, 10);
                                    Console.Write("Your number is: " + userNum);
                                    Console.WriteLine("");

                                    multiplier = 2;
                                    if (userNum == jackpotNum)
                                    {
                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                        multiplier *= 100;
                                    }

                                    Win(username, bet, multiplier);
                                    continue;
                                }
                            }

                            break;
                        }
                    }
                        continue;

                    case '6':
                    {
                        File.Delete(balancetxt);
                        string new_balance = "1000";
                        File.WriteAllText(balancetxt, Encoder(new_balance));
                    }
                        continue;

                    case '0':
                    {
                        break;
                    }
                }
            }
        }

        static void Win(string username, int bet, int multiplier)
        {
            string root = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logins";
            string balancetxt = $"{root}\\{username}\\Casino\\Balance";
            int balance = GetBalance(balancetxt);

            balance += (bet * multiplier);
            File.WriteAllText(balancetxt, Encoder(balance.ToString()));
            Console.WriteLine($"You won {bet * multiplier}!\nYour balance is now {balance}");
            Thread.Sleep(1000);
        }

        static int GetBalance(string root)
        {
            string encoded_balance = File.ReadAllText(root);
            return Int32.Parse(Decoder(encoded_balance));
        }

        static bool PlaceBet(string root, int bet)
        {
            int balance = GetBalance(root);
            if (balance < bet)
            {
                Console.WriteLine("");
                Console.WriteLine("Bet is exceeds your current balance");
                Console.WriteLine("Please reset your money at the menu");
                Thread.Sleep(2000);
                return false;
            }

            balance -= bet;
            File.WriteAllText(root, Encoder(balance.ToString()));
            return true;
        }
        
        private static void Rolling()
        {
            Console.Write("Rolling");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.Write(".");
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("                              ");
            Console.SetCursorPosition(0, 0);
            Console.Write("Rolling");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.Write(".");
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("                              ");
            Console.SetCursorPosition(0, 0);
            Console.Write("Rolling");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.WriteLine(".");
        }
    }
}