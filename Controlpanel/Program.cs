using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;

namespace Controlpanel
{
    partial class Program
    {
        // Importing shell32.dll
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]

        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        //import kernal.dll
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        class Accounts
        {
            public string Username
            { get; set; }
            public string Password
            { get; set; }
            public string Balance
            { get; set; }
        }
        class Presets
        {
            public string Preset1
            { get; set; }
            public string Preset2
            { get; set; }
            public string Preset3
            { get; set; }
        }

        private static void Main()
        {
            // Change the title
            Console.Title = "qBait's Controlpanel";

            Console.CursorVisible = false;

            // Useless login feature that nobody will use. Also very unsecure, dont use personal pw here
            string root = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logins";
            string decodedUsername = "";
            string presetdir = "";

            // If directory does not exist, create it. 
            if (!Directory.Exists(root))
            {
                //Creates root folder
                Directory.CreateDirectory(root);
            }
            else
            {
                //Doesnt create root folder
            }

            Accounts accounts = new Accounts();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Do you want to 'login' or 'create login'?");
                string decision = Console.ReadLine();
                decision.ToLower();
                Console.Clear();

                if (decision == "login")
                {
                    Login:
                    Console.WriteLine("Logging in");
                    //Gets username
                    Console.Write("Username: ");
                    string username = Console.ReadLine();
                    accounts.Username = username;
                    //Gets password
                    Console.Write("Password: ");
                    string password = "";

                    //Replaces input with '*'
                    ConsoleKeyInfo info = Console.ReadKey(true);
                    while (info.Key != ConsoleKey.Enter)
                    {
                        if (info.Key != ConsoleKey.Backspace)
                        {
                            Console.Write("*");
                            password += info.KeyChar;
                        }
                        else if (info.Key == ConsoleKey.Backspace)
                        {
                            if (!string.IsNullOrEmpty(password))
                            {
                                // remove one character from the list of password characters
                                password = password.Substring(0, password.Length - 1);
                                // get the location of the cursor
                                int pos = Console.CursorLeft;
                                // move the cursor to the left by one character
                                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                                // replace it with space
                                Console.Write(" ");
                                // move the cursor to the left by one character again
                                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                            }
                        }
                        info = Console.ReadKey(true);
                    }
                    accounts.Password = password;

                    //Encrypts username
                    string un = accounts.Username;
                    try
                    {
                        byte[] un_byte = Encoding.UTF8.GetBytes(un);
                        accounts.Username = HttpUtility.UrlEncode(Convert.ToBase64String(un_byte));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Couldn't encrypt password, please try again");
                        Console.WriteLine("Error message: " + e);
                        Thread.Sleep(500);
                        goto Login;
                    }

                    //Encrypts password
                    string pw = accounts.Password;
                    try
                    {
                        byte[] pw_byte = Encoding.UTF8.GetBytes(pw);
                        accounts.Password = HttpUtility.UrlEncode(Convert.ToBase64String(pw_byte));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Couldn't encrypt password");
                        Console.WriteLine("Error message: " + e);
                        Thread.Sleep(500);
                        goto Login;
                    }


                    Console.WriteLine("");
                    string userdir = $"{root}\\{accounts.Username}";
                    string logins = $"{userdir}\\login";
                    string casinodir = $"{userdir}\\Casino";
                    string balancetxt = $"{casinodir}\\Balance";

                    //Check if username exist
                    //If username doesnt exist
                    if (!Directory.Exists(userdir))
                    {
                        Console.WriteLine("Username or password doesnt exist");
                        Thread.Sleep(500);
                        goto Login;
                    }

                    //If username exist
                    if (Directory.Exists(userdir))
                    {
                        //Gets pw info
                        string infos = File.ReadAllText(logins);
                        //Checks if pw is correct
                        if (infos.Contains($"PW: {accounts.Password}"))
                        {
                            //Checks if balance file exist
                            //If balance file doesnt exist
                            if (!File.Exists(balancetxt))
                            {
                                //Make a new balance file
                                Directory.CreateDirectory(balancetxt);
                                string balance = "1000";
                                accounts.Balance = balance;

                                //Encrypts balance so user cant change it
                                string ba = accounts.Balance;
                                try
                                {
                                    byte[] ba_byte = Encoding.UTF8.GetBytes(ba);
                                    accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Encrypting went wrong");
                                    Console.WriteLine("Error message: " + e);
                                    Thread.Sleep(500);
                                }

                                //Write encrypted balance to file
                                File.WriteAllText(balancetxt, accounts.Balance);

                                string EncodedUsername = accounts.Username;
                                try
                                {
                                    byte[] username_byte = Convert.FromBase64String(HttpUtility.UrlDecode(EncodedUsername));
                                    decodedUsername = Encoding.UTF8.GetString(username_byte);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Couldn't decrypt password");
                                    Console.WriteLine("Error message: " + e);
                                    Thread.Sleep(500);
                                }
                                Console.WriteLine("You have logged in succesfully as: " + decodedUsername);
                                Thread.Sleep(1000);
                                break;
                            }

                            //If balance file exists
                            if (File.Exists(balancetxt))
                            {
                                string EncodedUsername = accounts.Username;
                                try
                                {
                                    byte[] username_byte = Convert.FromBase64String(HttpUtility.UrlDecode(EncodedUsername));
                                    decodedUsername = Encoding.UTF8.GetString(username_byte);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Couldn't decrypt password");
                                    Console.WriteLine("Error message: " + e);
                                    Thread.Sleep(500);
                                }
                                Console.WriteLine("You have logged in succesfully as: " + decodedUsername);
                                Thread.Sleep(1000);
                                break;
                            }
                        }

                        //If pw isnt correct
                        else
                        {
                            Console.WriteLine("Username or password doesnt exist");
                            Thread.Sleep(500);
                            goto Login;
                        }

                    }

                    //If username isnt correct
                    else
                    {
                        Console.WriteLine("Username or password doesnt exist");
                        Thread.Sleep(500);
                        goto Login;
                    }
                }

                else if (decision == "create" || decision == "create login")
                {
                    MakeAccount:
                    //Get username and pw
                    Console.WriteLine("Creating account");
                    Console.Write("Username: ");
                    string username = Console.ReadLine();
                    accounts.Username = username;
                    Console.Write("Password: ");
                    string password = "";

                    //Change input to '*'
                    ConsoleKeyInfo info = Console.ReadKey(true);
                    while (info.Key != ConsoleKey.Enter)
                    {
                        if (info.Key != ConsoleKey.Backspace)
                        {
                            Console.Write("*");
                            password += info.KeyChar;
                        }
                        else if (info.Key == ConsoleKey.Backspace)
                        {
                            if (!string.IsNullOrEmpty(password))
                            {
                                // remove one character from the list of password characters
                                password = password.Substring(0, password.Length - 1);
                                // get the location of the cursor
                                int pos = Console.CursorLeft;
                                // move the cursor to the left by one character
                                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                                // replace it with space
                                Console.Write(" ");
                                // move the cursor to the left by one character again
                                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                            }
                        }
                        info = Console.ReadKey(true);
                    }
                    accounts.Password = password;

                    Console.WriteLine("");

                    //Encrypt username
                    string un = accounts.Username;
                    try
                    {
                        byte[] un_byte = Encoding.UTF8.GetBytes(un);
                        accounts.Username = HttpUtility.UrlEncode(Convert.ToBase64String(un_byte));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Encrypting went wrong");
                        Console.WriteLine("Error message: " + e);
                        Thread.Sleep(500);
                        goto MakeAccount;
                    }

                    if (!Directory.Exists($"{root}\\{accounts.Username}"))
                    {
                        //Create sub folder
                        Directory.CreateDirectory($"{root}\\{accounts.Username}");

                        string pw = accounts.Password;
                        try
                        {
                            byte[] pw_byte = Encoding.UTF8.GetBytes(pw);
                            accounts.Password = HttpUtility.UrlEncode(Convert.ToBase64String(pw_byte));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Encrypting went wrong");
                            Console.WriteLine("Error message: " + e);
                            Thread.Sleep(500);
                            goto MakeAccount;
                        }

                        string account = $"{root}\\{accounts.Username}";
                        string logins = $"{account}\\login";
                        presetdir = $"{account}\\Presets";
                        string presetfolder1 = $"{presetdir}\\Preset 1";
                        string presetfolder2 = $"{presetdir}\\Preset 2";
                        string presetfolder3 = $"{presetdir}\\Preset 3";
                        string casinodir = $"{account}\\Casino";
                        string balancetxt = $"{casinodir}\\Balance";

                        if (!Directory.Exists(presetdir))
                        {
                            //Create sub folder
                            Directory.CreateDirectory(presetdir);
                        }
                        else
                        {
                            //Doesnt create sub folder
                        }
                        if (!Directory.Exists(presetfolder1))
                        {
                            //Create sub folder
                            Directory.CreateDirectory(presetfolder1);
                        }
                        else
                        {
                            //Doesnt create sub folder
                        }
                        if (!Directory.Exists(presetfolder2))
                        {
                            //Create sub folder
                            Directory.CreateDirectory(presetfolder2);
                        }
                        else
                        {
                            //Doesnt create sub folder
                        }
                        if (!Directory.Exists(presetfolder3))
                        {
                            //Create sub folder
                            Directory.CreateDirectory(presetfolder3);
                        }
                        else
                        {
                            //Doesnt create sub folder
                        }
                        if (!Directory.Exists(casinodir))
                        {
                            //Create sub folder
                            Directory.CreateDirectory(casinodir);
                        }
                        else
                        {
                            //Doesnt create sub folder
                        }

                        string balance = "1000";
                        accounts.Balance = balance;

                        //Encrypt balance
                        string ba = accounts.Balance;
                        try
                        {
                            byte[] ba_byte = Encoding.UTF8.GetBytes(ba);
                            accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("Encrypting went wrong");
                            Console.WriteLine("Error message: " + e);
                            Thread.Sleep(500);
                            goto MakeAccount;
                        }

                        //Write encrypted username, pw and balance to file
                        File.AppendAllText(logins, "UN: " + accounts.Username + " PW: " + accounts.Password);
                        File.AppendAllText(balancetxt, accounts.Balance);

                        Console.WriteLine("Account has been made, please sign in");
                        Thread.Sleep(500);
                    }
                    else if (Directory.Exists($"{root}\\{accounts.Username}"))
                    {
                        Console.WriteLine("Username is taken");
                        Thread.Sleep(500);
                        goto MakeAccount;
                    }
                }

                else
                {
                    Console.WriteLine("Please type 'login' or 'create login'");
                    Thread.Sleep(500);
                }
            }

            Start:

            Presets preset = new Presets();

            presetdir = $"{root}\\{accounts.Username}\\Presets";
            string presetfolder_1 = $"{presetdir}\\Preset 1";
            string presetname_1 = $"{presetfolder_1}\\ProgramName";
            string presetaddress_1 = $"{presetfolder_1}\\address";
            string presetfolder_2 = $"{presetdir}\\Preset 2";
            string presetname_2 = $"{presetfolder_2}\\ProgramName";
            string presetaddress_2 = $"{presetfolder_2}\\address";
            string presetfolder_3 = $"{presetdir}\\Preset 3";
            string presetname_3 = $"{presetfolder_3}\\ProgramName";
            string presetaddress_3 = $"{presetfolder_3}\\address";

            Console.Clear();

            // Menu
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Welcome to qBait's Controlpanel");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("***********************************");
            Console.WriteLine("Logged in as: " + decodedUsername);
            Console.WriteLine("What do you want to do?");
            Console.Write("1: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (File.Exists(presetaddress_1))
            {
                Console.Write("Open ");
                string preset1Content = File.ReadAllText(presetname_1);
                preset.Preset1 = preset1Content;
                Console.WriteLine(preset.Preset1);
            }
            if (!File.Exists(presetaddress_1))
            {
                Console.WriteLine("Set shortcut 1");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("2: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (File.Exists(presetaddress_2))
            {
                Console.Write("Open ");
                string preset2Content = File.ReadAllText(presetname_2);
                preset.Preset2 = preset2Content;
                Console.WriteLine(preset.Preset2);
            }
            if (!File.Exists(presetaddress_2))
            {
                Console.WriteLine("Set shortcut 2");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("3: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (File.Exists(presetaddress_3))
            {
                Console.Write("Open ");
                string preset3Content = File.ReadAllText(presetname_3);
                preset.Preset3 = preset3Content;
                Console.WriteLine(preset.Preset3);
            }
            if (!File.Exists(presetaddress_3))
            {
                Console.WriteLine("Set shortcut 3");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("4: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Clean PC");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("5: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Enter Casino");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("6: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("See Hardware Information //Not done");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("7: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Settings");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("8: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Exit Controlpanel");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("7: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Restart PC");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("0: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Shutdown PC");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("***********************************");

            // Update time, CPU and Ram while waiting for input
            while (!Console.KeyAvailable)
            {
                // Writes after the menu
                Console.SetCursorPosition(0, 15);

                // Gets the date and write
                string day = DateTime.Today.ToString("D");
                Console.WriteLine("The date is {0}", day);

                // Gets the time and write
                string time = DateTime.Now.ToString("T");
                Console.WriteLine("The current time is {0}", time);

                // Getting Cpu info
                PerformanceCounter cpuCounter = new PerformanceCounter();
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";

                // Getting ram info
                PerformanceCounter ramCounter = new PerformanceCounter();
                ramCounter.CategoryName = "Memory";
                ramCounter.CounterName = "Available MBytes";
                ramCounter.InstanceName = String.Empty;
                dynamic RamValue = ramCounter.NextValue();

                // Getting total of ram
                long memKb;
                GetPhysicallyInstalledSystemMemory(out memKb);
                int memKbInt = Convert.ToInt32(memKb);
                // Converting to mb
                int TotalRaminMb = memKbInt / 1024;

                // Convert to procent
                dynamic RamComp = RamValue / TotalRaminMb;
                dynamic NewRamComp = RamComp * 100;
                dynamic RamProcent = 100 - NewRamComp;

                // Get first value from CPU which is always 0
                dynamic firstValue = cpuCounter.NextValue();
                Thread.Sleep(500);
                // Call again to get an actual value
                dynamic secondValue = cpuCounter.NextValue();

                // Get a round number
                dynamic ShortRamProcent = Math.Round(RamProcent);
                dynamic ShortsecondValue = Math.Round(secondValue);

                // Prints values
                Console.WriteLine($"CPU Load: " + ShortsecondValue + "%     ");
                Console.WriteLine($"RAM Load: " + ShortRamProcent + "%      ");
                Thread.Sleep(500);
            }

            char keyInfo = Console.ReadKey().KeyChar;

            // Choose what to do
            switch (keyInfo)
            {
                // If an invalid key is pressed execute
                default:
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("");
                        Console.WriteLine("Please enter a valid number");
                        Thread.Sleep(1500);
                        goto Start;
                    }

                case '1':
                    {
                        Console.Clear();
                        if (!File.Exists(presetaddress_1))
                        {
                            Console.WriteLine("What program do you want as shortcut?");
                            string program = Console.ReadLine();
                            string exe = "";
                            if (!program.EndsWith(".exe"))
                            {
                                exe = program;
                            }
                            if (program.EndsWith(".exe"))
                            {
                                program.Substring(program.Length - 4);
                                exe = program;
                            }
                            File.WriteAllText(presetname_1, exe);

                            Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
                            string address = Console.ReadLine();
                            File.WriteAllText(presetaddress_1, address);
                            Console.WriteLine("Preset has been set");
                            Thread.Sleep(1000);
                            goto Start;
                        }

                        if (File.Exists(presetaddress_1))
                        {
                            try
                            {
                                string address = File.ReadAllText(presetaddress_1);
                                Process.Start(address);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("File doesn't exist, change preset under 'Settings'");
                                Thread.Sleep(2000);
                            }
                            goto Start;
                        }
                    }
                    break;

                case '2':
                    {
                        Console.Clear();
                        if (!File.Exists(presetaddress_2))
                        {
                            Console.WriteLine("What program do you want as shortcut?");
                            string program = Console.ReadLine();
                            string exe = "";
                            if (!program.EndsWith(".exe"))
                            {
                                exe = program;
                            }
                            if (program.EndsWith(".exe"))
                            {
                                program.Substring(program.Length - 4);
                                exe = program;
                            }
                            File.WriteAllText(presetname_2, exe);

                            Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
                            string address = Console.ReadLine();
                            File.WriteAllText(presetaddress_2, address);
                            Console.WriteLine("Preset has been set");
                            Thread.Sleep(1000);
                            goto Start;
                        }

                        if (File.Exists(presetaddress_2))
                        {
                            try
                            {
                                string address = File.ReadAllText(presetaddress_2);
                                Process.Start(address);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("File doesn't exist, change preset under 'Settings'");
                                Thread.Sleep(2000);
                            }
                            goto Start;
                        }
                    }
                    break;

                case '3':
                    {
                        Console.Clear();
                        if (!File.Exists(presetaddress_3))
                        {
                            Console.WriteLine("What program do you want as shortcut?");
                            string program = Console.ReadLine();
                            string exe = "";
                            if (!program.EndsWith(".exe"))
                            {
                                exe = program;
                            }
                            if (program.EndsWith(".exe"))
                            {
                                program.Substring(program.Length - 4);
                                exe = program;
                            }
                            File.WriteAllText(presetname_3, exe);

                            Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
                            string address = Console.ReadLine();
                            File.WriteAllText(presetaddress_3, address);
                            Console.WriteLine("Preset has been set");
                            Thread.Sleep(1000);
                            goto Start;
                        }

                        if (File.Exists(presetaddress_3))
                        {
                            try
                            {
                                string address = File.ReadAllText(presetaddress_3);
                                Process.Start(address);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("File doesn't exist, change preset under 'Settings'");
                                Thread.Sleep(2000);
                            }
                            goto Start;
                        }
                    }
                    break;

                case '4':
                    {
                        var s = new ConsoleSpinner();

                        // Find path to temp folder
                        Console.Clear();
                        Console.WriteLine("Finding Temp folder");
                        DirectoryInfo temp = new DirectoryInfo(Path.GetTempPath());
                        Console.WriteLine("Temp folder found");

                        // Measuring size of temp folder
                        float size = 0;
                        foreach (FileInfo fi in temp.GetFiles("*", SearchOption.AllDirectories))
                        {
                            size += fi.Length;
                        }

                        // Convert kb to mb
                        float tempsize = size / 1000000;

                        // If temp is above 1gb, execute
                        if (tempsize > 1024)
                        {
                            // Convert the value from mb to gb
                            decimal gbtempsize = (decimal)(tempsize / 1000);
                            // Convert value to only 2 decimals
                            decimal newgbtempsize = Math.Round(Convert.ToDecimal(gbtempsize), 2);
                            Console.WriteLine($"The Temp folder is {newgbtempsize}GB");
                        }

                        // If value is less than 1gb dont convert
                        else
                        {
                            // Converting the number to a number with 0 decimals
                            decimal mbtempsize = Math.Round(Convert.ToDecimal(tempsize), 0);
                            Console.WriteLine($"The Temp folder is {mbtempsize}MB");
                        }
                        Thread.Sleep(2000);

                        // Delete files
                        Console.WriteLine("Deleting temp files and folders");

                        foreach (FileInfo file in temp.EnumerateFiles())
                        {
                            s.UpdateProgress();
                            // Will try to delete the file
                            try
                            {
                                // Deletes the file
                                file.Delete();
                            }
                            catch (Exception)
                            {
                                // Ignore the failure and continue
                            }

                            // Delete folders
                            foreach (DirectoryInfo dir in temp.EnumerateDirectories())
                            {
                                // Will try and delete folder
                                try
                                {
                                    // Deletes folder
                                    dir.Delete(true);
                                }
                                catch (Exception)
                                {
                                    // Ignore the failure and continue
                                }
                            }
                        }

                        // Getting size of temp again
                        float delsize = 0;
                        foreach (FileInfo fi in temp.GetFiles("*", SearchOption.AllDirectories))
                        {
                            delsize += fi.Length;
                        }

                        // Convert kb to mb
                        float delmbsize = delsize / 1000000;

                        // Comparing the old and new size
                        float ttldelsize = tempsize - delmbsize;

                        // If new size is above 1gb show total value in gb with 2 decimals
                        if (ttldelsize > 1024)
                        {
                            decimal gbttldelsize = (decimal)(ttldelsize / 1000);
                            // Converting number to a number with 2 decimals
                            decimal newgbttldelsize = Math.Round(Convert.ToDecimal(gbttldelsize), 2);
                            Console.WriteLine($"There has been deleted {newgbttldelsize}GB from the Temp folder");
                        }

                        // If value is less than 1gb show total value in mb
                        else
                        {
                            // Converting the number to a number with 0 decimals
                            decimal mbttldelsize = Math.Round(Convert.ToDecimal(ttldelsize), 0);
                            Console.WriteLine($"There has been deleted {mbttldelsize}MB from the Temp folder");
                        }
                        Thread.Sleep(1000);

                        // Emptying the recycle bin
                        Thread.Sleep(500);
                        Console.WriteLine("Emptying the recycle bin");
                        Thread.Sleep(500);

                        uint result = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOCONFIRMATION);
                        if (result == 0)
                        {
                            Console.WriteLine("Recycle bin is already empty, nothing has been done");
                            Thread.Sleep(2000);
                        }

                        // if there is nothing to empty
                        else
                        {
                            Thread.Sleep(500);
                            Console.WriteLine("Files in the recycle bin has been deleted");
                            Thread.Sleep(1500);
                        }

                        // Done
                        Console.Clear();
                        Console.WriteLine("Everything is done and your PC has been cleaned!");
                        Console.WriteLine("Press any key to return to the menu");
                        Console.ReadKey();
                        goto Start;
                    }

                case '5':
                    {
                    Startover:
                        Console.Clear();

                        string balancetxt = $"{root}\\{accounts.Username}\\Casino\\Balance";
                        string tmpbalancetxt = $"{root}\\{accounts.Username}\\Casino\\Balancetmp";
                        string tmpbalance = "";

                        // Get balance from balance file
                        string balance = File.ReadAllText(balancetxt);
                        string data = balance;
                        try
                        {
                            byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                            tmpbalance = Encoding.UTF8.GetString(balance_byte);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Error while decoding balance");
                            Thread.Sleep(500);
                            goto Startover;
                        }
                        
                        File.WriteAllText(tmpbalancetxt, tmpbalance);

                        Console.WriteLine("Welcome to qBait's casino");
                        Console.WriteLine("Logged in as: " + decodedUsername);
                        Console.WriteLine($"Balance: " + tmpbalance);

                        Console.WriteLine("What would you like to play? // All need Thread.Sleep commands");
                        Console.Write("1: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        // When hitting 2 times and then stand dealer wins when dealer is lower
                        Console.WriteLine("Black Jack // Some output is wrong");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("2: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Roulette // Should work");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("3: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Dice // Should work");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("4: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Baccarat // Should work");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("5: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Slots // Working");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("6: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Reset money // Working");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("0: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Go back");
                        Console.ForegroundColor = ConsoleColor.White;

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
                                        goto Startover;
                                    }

                                case '1':
                                    {
                                    BJStart:
                                        Console.Clear();
                                        Console.WriteLine("Welcome to Black Jack");

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
                                            else

                                            {
                                                if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                {
                                                    num = num.Substring(num.Length - 1);
                                                    Console.Write("\b \b");
                                                }
                                            }
                                        }

                                        while (chr.Key != ConsoleKey.Enter);

                                        // Get balance from balance file
                                        try
                                        {
                                            byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                            tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("Error while decoding balance");
                                            Thread.Sleep(500);
                                            goto Startover;
                                        }

                                        int betInt = Int32.Parse(num);
                                        int moneyInt = Int32.Parse(tmpbalance);
                                        int totalAfterBet = moneyInt - betInt;

                                        // if the bet is above what user has, send user back
                                        if (totalAfterBet < 0)
                                        {
                                            Console.WriteLine("");
                                            Console.WriteLine("Bet is exceeds your current balance");
                                            Console.WriteLine("Please reset your money at the menu");
                                            Thread.Sleep(2000);
                                            goto BJStart;
                                        }

                                        // if bet isnt above total amount then subtract bet from total and put in file
                                        else
                                        {
                                            string newTotal = totalAfterBet.ToString();
                                            File.WriteAllText(tmpbalancetxt, newTotal);
                                            //Encrypt balance
                                            try
                                            {
                                                byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                            }

                                            catch (Exception e)
                                            {
                                                Console.WriteLine("Encrypting went wrong");
                                                Console.WriteLine("Error message: " + e);
                                                Thread.Sleep(500);
                                                goto Startover;
                                            }
                                            using StreamReader sr = File.OpenText(tmpbalancetxt);
                                            string moneyTotal = "";
                                            while ((moneyTotal = sr.ReadLine()) != null)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine($"Your balance is now {moneyTotal}");
                                                Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                Thread.Sleep(5000);
                                                break;
                                            }
                                        }

                                        Console.Clear();

                                        int[] numbers = new int[52];
                                        numbers[0] = 1;
                                        numbers[1] = 1;
                                        numbers[2] = 1;
                                        numbers[3] = 1;
                                        numbers[4] = 2;
                                        numbers[5] = 2;
                                        numbers[6] = 2;
                                        numbers[7] = 2;
                                        numbers[8] = 3;
                                        numbers[9] = 3;
                                        numbers[10] = 3;
                                        numbers[11] = 3;
                                        numbers[12] = 4;
                                        numbers[13] = 4;
                                        numbers[14] = 4;
                                        numbers[15] = 4;
                                        numbers[16] = 5;
                                        numbers[17] = 5;
                                        numbers[18] = 5;
                                        numbers[19] = 5;
                                        numbers[20] = 6;
                                        numbers[21] = 6;
                                        numbers[22] = 6;
                                        numbers[23] = 6;
                                        numbers[24] = 7;
                                        numbers[25] = 7;
                                        numbers[26] = 7;
                                        numbers[27] = 7;
                                        numbers[28] = 8;
                                        numbers[29] = 8;
                                        numbers[30] = 8;
                                        numbers[31] = 8;
                                        numbers[32] = 9;
                                        numbers[33] = 9;
                                        numbers[34] = 9;
                                        numbers[35] = 9;
                                        numbers[36] = 10;
                                        numbers[37] = 10;
                                        numbers[38] = 10;
                                        numbers[39] = 10;
                                        numbers[40] = 10;
                                        numbers[41] = 10;
                                        numbers[42] = 10;
                                        numbers[43] = 10;
                                        numbers[44] = 10;
                                        numbers[45] = 10;
                                        numbers[46] = 10;
                                        numbers[47] = 10;
                                        numbers[48] = 10;
                                        numbers[49] = 10;
                                        numbers[50] = 10;
                                        numbers[51] = 10;

                                        Random rnd = new Random();

                                        int firstCard = numbers[rnd.Next(0, 51)];
                                        int secoundCard = numbers[rnd.Next(0, 51)];
                                        int dealerfirstCard = numbers[rnd.Next(0, 51)];

                                        Console.WriteLine($"Your first card is {firstCard}");
                                        Thread.Sleep(200);
                                        Console.WriteLine($"Your secound card is {secoundCard}");
                                        Thread.Sleep(200);
                                        Console.WriteLine($"Dealers card is {dealerfirstCard}");
                                        Thread.Sleep(200);
                                        Console.WriteLine("");
                                        int total1 = firstCard + secoundCard;
                                        Console.WriteLine($"Your total is now {total1}");
                                        Thread.Sleep(200);
                                        int dealertotal = dealerfirstCard;
                                        Console.WriteLine($"Dealers total is now {dealertotal}");
                                        Thread.Sleep(200);

                                    hitorstand:

                                        if (total1 == 21)
                                        {
                                            int moneyWin = betInt * 2;
                                            Console.WriteLine($"You win {moneyWin}");
                                            string newString = File.ReadAllText(tmpbalancetxt);
                                            int moneyTotalInt = Int32.Parse(newString);
                                            int newTotal = moneyTotalInt + moneyWin;
                                            string newStringTotal = newTotal.ToString();
                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                            try
                                            {
                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine("Couldn't encrypt winning");
                                                Console.WriteLine("Error message: " + e);
                                                Thread.Sleep(500);
                                            }
                                            File.WriteAllText(balancetxt, balance);
                                            Thread.Sleep(1000);

                                            while (true)
                                            {
                                                Console.WriteLine("Would you like to play again? y/n");
                                                char playagain = Console.ReadKey().KeyChar;

                                                if (playagain == 'y')
                                                {
                                                    goto BJStart;
                                                }

                                                if (playagain == 'n')
                                                {
                                                    goto Start;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Press a valid key");
                                                }
                                            }
                                        }

                                        if (total1 < 21)
                                        {
                                            Console.WriteLine("Do you hit or stand?");
                                            string cho = Console.ReadLine();
                                            string choice = cho.ToLower();

                                            if (choice == "hit")
                                            {
                                                Thread.Sleep(200);
                                                int thirdCard = numbers[rnd.Next(0, 51)];
                                                Console.WriteLine($"Your next card is {thirdCard}");
                                                int total2 = total1 + thirdCard;
                                                Thread.Sleep(200);
                                                Console.WriteLine($"Your total is now {total2}");
                                                Thread.Sleep(1000);
                                                if (total2 > 21)
                                                {
                                                    Console.WriteLine("Busted!");
                                                    while (true)
                                                    {
                                                        Console.WriteLine("Would you like to play again? y/n");
                                                        char playagain = Console.ReadKey().KeyChar;

                                                        if (playagain == 'y')
                                                        {
                                                            goto BJStart;
                                                        }

                                                        if (playagain == 'n')
                                                        {
                                                            goto Start;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Press a valid key");
                                                        }
                                                    }
                                                }

                                                if (total2 == 21)
                                                {
                                                    int moneyWin = betInt * 2;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);

                                                    while (true)
                                                    {
                                                        Console.WriteLine("Would you like to play again? y/n");
                                                        char playagain = Console.ReadKey().KeyChar;

                                                        if (playagain == 'y')
                                                        {
                                                            goto BJStart;
                                                        }

                                                        if (playagain == 'n')
                                                        {
                                                            goto Start;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Press a valid key");
                                                        }
                                                    }
                                                }

                                                if (total2 < 21)
                                                {
                                                    Console.WriteLine("Do you hit or stand?");
                                                    string cho1 = Console.ReadLine();
                                                    string choice1 = cho1.ToLower();

                                                    if (choice1 == "hit")
                                                    {
                                                        Thread.Sleep(200);
                                                        int fourthCard = numbers[rnd.Next(0, 51)];
                                                        Console.WriteLine($"Your next card is {fourthCard}");
                                                        int total3 = total2 + fourthCard;
                                                        Thread.Sleep(200);
                                                        Console.WriteLine($"Your total is now {total3}");
                                                        Thread.Sleep(200);

                                                        if (total3 > 21)
                                                        {
                                                            Console.WriteLine("Busted!");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (total3 < 21)
                                                        {
                                                            Console.WriteLine("Do you hit or stand");
                                                            string cho2 = Console.ReadLine();
                                                            string choice2 = cho2.ToLower();

                                                            if (choice2 == "hit")
                                                            {
                                                                Thread.Sleep(200);
                                                                int fifthCard = numbers[rnd.Next(0, 51)];
                                                                Console.WriteLine($"Your next card is {fifthCard}");
                                                                int total4 = total3 + fifthCard;
                                                                Thread.Sleep(200);
                                                                Console.WriteLine($"Your total is now {total4}");
                                                                Thread.Sleep(200);

                                                                if (total4 > 21)
                                                                {
                                                                    Console.WriteLine("Busted!");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (total4 == 21)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (total4 < 21)
                                                                {
                                                                    Console.WriteLine("Do you hit or stand?");
                                                                    string cho3 = Console.ReadLine();
                                                                    string choice3 = cho3.ToLower();

                                                                    if (choice3 == "hit")
                                                                    {
                                                                        Thread.Sleep(200);
                                                                        int sixthCard = numbers[rnd.Next(0, 51)];
                                                                        Console.WriteLine($"Your next card is {sixthCard}");
                                                                        Thread.Sleep(200);
                                                                        int total5 = total4 + sixthCard;
                                                                        Console.WriteLine($"Your total is now {total5}");
                                                                        Thread.Sleep(200);

                                                                        if (total5 > 21)
                                                                        {
                                                                            Console.WriteLine("Busted!");
                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }

                                                                        if (total5 == 21)
                                                                        {
                                                                            int moneyWin = betInt * 2;
                                                                            Console.WriteLine($"You win {moneyWin}");
                                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                                            int moneyTotalInt = Int32.Parse(newString);
                                                                            int newTotal = moneyTotalInt + moneyWin;
                                                                            string newStringTotal = newTotal.ToString();
                                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                            try
                                                                            {
                                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                            }
                                                                            catch (Exception e)
                                                                            {
                                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                                Console.WriteLine("Error message: " + e);
                                                                                Thread.Sleep(500);
                                                                            }
                                                                            File.WriteAllText(balancetxt, balance);

                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("How did you get to here??");
                                                                            Thread.Sleep(200);
                                                                            Console.WriteLine("You get 10x back your bet!");

                                                                            int moneyWin = betInt * 10;
                                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                                            int moneyTotalInt = Int32.Parse(newString);
                                                                            int newTotal = moneyTotalInt + moneyWin;
                                                                            string newStringTotal = newTotal.ToString();
                                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                            try
                                                                            {
                                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                            }
                                                                            catch (Exception e)
                                                                            {
                                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                                Console.WriteLine("Error message: " + e);
                                                                                Thread.Sleep(500);
                                                                            }
                                                                            File.WriteAllText(balancetxt, balance);

                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    if (choice3 == "stand")
                                                                    {
                                                                        Thread.Sleep(200);
                                                                        int dealersecoundCard = numbers[rnd.Next(0, 51)];
                                                                        Console.WriteLine($"Dealers secound card is {dealersecoundCard}");
                                                                        Thread.Sleep(200);
                                                                        int dealertotal1 = dealertotal + dealersecoundCard;
                                                                        Console.WriteLine($"Dealer has a total of {dealertotal1}");
                                                                        Thread.Sleep(200);

                                                                        if (dealertotal1 == 21)
                                                                        {
                                                                            Console.WriteLine("Dealer wins");
                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }

                                                                        if (dealertotal1 == total4)
                                                                        {
                                                                            Console.WriteLine("Tie");
                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }

                                                                        if (dealertotal1 > total4)
                                                                        {
                                                                            if (dealertotal1 <= 21)
                                                                            {
                                                                                Console.WriteLine("Dealer wins");
                                                                                while (true)
                                                                                {
                                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                                    if (playagain == 'y')
                                                                                    {
                                                                                        goto BJStart;
                                                                                    }

                                                                                    if (playagain == 'n')
                                                                                    {
                                                                                        goto Start;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Press a valid key");
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                int moneyWin = betInt * 2;
                                                                                Console.WriteLine($"You win {moneyWin}");
                                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                                string newStringTotal = newTotal.ToString();
                                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                                try
                                                                                {
                                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                                }
                                                                                catch (Exception e)
                                                                                {
                                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                                    Console.WriteLine("Error message: " + e);
                                                                                    Thread.Sleep(500);
                                                                                }
                                                                                File.WriteAllText(balancetxt, balance);

                                                                                while (true)
                                                                                {
                                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                                    if (playagain == 'y')
                                                                                    {
                                                                                        goto BJStart;
                                                                                    }

                                                                                    if (playagain == 'n')
                                                                                    {
                                                                                        goto Start;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Press a valid key");
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        if (dealertotal1 > 21)
                                                                        {
                                                                            int moneyWin = betInt * 2;
                                                                            Console.WriteLine($"You win {moneyWin}");
                                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                                            int moneyTotalInt = Int32.Parse(newString);
                                                                            int newTotal = moneyTotalInt + moneyWin;
                                                                            string newStringTotal = newTotal.ToString();
                                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                            try
                                                                            {
                                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                            }
                                                                            catch (Exception e)
                                                                            {
                                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                                Console.WriteLine("Error message: " + e);
                                                                                Thread.Sleep(500);
                                                                            }
                                                                            File.WriteAllText(balancetxt, balance);

                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }

                                                                        if (dealertotal1 < total4)
                                                                        {
                                                                            Thread.Sleep(200);
                                                                            int dealerthirdCard = numbers[rnd.Next(0, 51)];
                                                                            Console.WriteLine($"Dealers third card is {dealerthirdCard}");
                                                                            Thread.Sleep(200);
                                                                            int dealertotal2 = dealerthirdCard + dealertotal1;
                                                                            Console.WriteLine($"Dealer now has a total of {dealertotal2}");
                                                                            Thread.Sleep(200);

                                                                            if (dealertotal2 == 21)
                                                                            {
                                                                                Console.WriteLine("Dealer wins");
                                                                                while (true)
                                                                                {
                                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                                    if (playagain == 'y')
                                                                                    {
                                                                                        goto BJStart;
                                                                                    }

                                                                                    if (playagain == 'n')
                                                                                    {
                                                                                        goto Start;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Press a valid key");
                                                                                    }
                                                                                }
                                                                            }

                                                                            if (dealertotal2 == total4)
                                                                            {
                                                                                Console.WriteLine("Tie");
                                                                                while (true)
                                                                                {
                                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                                    if (playagain == 'y')
                                                                                    {
                                                                                        goto BJStart;
                                                                                    }

                                                                                    if (playagain == 'n')
                                                                                    {
                                                                                        goto Start;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Press a valid key");
                                                                                    }
                                                                                }
                                                                            }

                                                                            if (dealertotal2 > total4)
                                                                            {
                                                                                if (dealertotal2 <= 21)
                                                                                {
                                                                                    Console.WriteLine("Dealer wins");
                                                                                    while (true)
                                                                                    {
                                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                                        if (playagain == 'y')
                                                                                        {
                                                                                            goto BJStart;
                                                                                        }

                                                                                        if (playagain == 'n')
                                                                                        {
                                                                                            goto Start;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Press a valid key");
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    int moneyWin = betInt * 2;
                                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                                    string newStringTotal = newTotal.ToString();
                                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                                    try
                                                                                    {
                                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                                    }
                                                                                    catch (Exception e)
                                                                                    {
                                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                                        Console.WriteLine("Error message: " + e);
                                                                                        Thread.Sleep(500);
                                                                                    }
                                                                                    File.WriteAllText(balancetxt, balance);

                                                                                    while (true)
                                                                                    {
                                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                                        if (playagain == 'y')
                                                                                        {
                                                                                            goto BJStart;
                                                                                        }

                                                                                        if (playagain == 'n')
                                                                                        {
                                                                                            goto Start;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Press a valid key");
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }

                                                                            if (dealertotal2 > 21)
                                                                            {
                                                                                int moneyWin = betInt * 2;
                                                                                Console.WriteLine($"You win {moneyWin}");
                                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                                string newStringTotal = newTotal.ToString();
                                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                                try
                                                                                {
                                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                                }
                                                                                catch (Exception e)
                                                                                {
                                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                                    Console.WriteLine("Error message: " + e);
                                                                                    Thread.Sleep(500);
                                                                                }
                                                                                File.WriteAllText(balancetxt, balance);

                                                                                while (true)
                                                                                {
                                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                                    if (playagain == 'y')
                                                                                    {
                                                                                        goto BJStart;
                                                                                    }

                                                                                    if (playagain == 'n')
                                                                                    {
                                                                                        goto Start;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Press a valid key");
                                                                                    }
                                                                                }
                                                                            }

                                                                            if (dealertotal2 < total4)
                                                                            {
                                                                                Thread.Sleep(200);
                                                                                int dealerfourthCard = numbers[rnd.Next(0, 51)];
                                                                                Console.WriteLine($"Dealers third card is {dealerfourthCard}");
                                                                                Thread.Sleep(200);
                                                                                int dealertotal3 = dealerfourthCard + dealertotal2;
                                                                                Console.WriteLine($"Dealer now has a total of {dealertotal3}");
                                                                                Thread.Sleep(200);

                                                                                if (dealertotal3 == 21)
                                                                                {
                                                                                    Console.WriteLine("Dealer wins");
                                                                                    while (true)
                                                                                    {
                                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                                        if (playagain == 'y')
                                                                                        {
                                                                                            goto BJStart;
                                                                                        }

                                                                                        if (playagain == 'n')
                                                                                        {
                                                                                            goto Start;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Press a valid key");
                                                                                        }
                                                                                    }
                                                                                }

                                                                                if (dealertotal3 == total4)
                                                                                {
                                                                                    Console.WriteLine("Tie");
                                                                                    while (true)
                                                                                    {
                                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                                        if (playagain == 'y')
                                                                                        {
                                                                                            goto BJStart;
                                                                                        }

                                                                                        if (playagain == 'n')
                                                                                        {
                                                                                            goto Start;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Press a valid key");
                                                                                        }
                                                                                    }
                                                                                }

                                                                                if (dealertotal3 > total4)
                                                                                {
                                                                                    if (dealertotal3 <= 21)
                                                                                    {
                                                                                        Console.WriteLine("Dealer wins");
                                                                                        while (true)
                                                                                        {
                                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                                            if (playagain == 'y')
                                                                                            {
                                                                                                goto BJStart;
                                                                                            }

                                                                                            if (playagain == 'n')
                                                                                            {
                                                                                                goto Start;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Console.WriteLine("Press a valid key");
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        int moneyWin = betInt * 2;
                                                                                        Console.WriteLine($"You win {moneyWin}");
                                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                                        string newStringTotal = newTotal.ToString();
                                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                                        try
                                                                                        {
                                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                                        }
                                                                                        catch (Exception e)
                                                                                        {
                                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                                            Console.WriteLine("Error message: " + e);
                                                                                            Thread.Sleep(500);
                                                                                        }
                                                                                        File.WriteAllText(balancetxt, balance);

                                                                                        while (true)
                                                                                        {
                                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                                            if (playagain == 'y')
                                                                                            {
                                                                                                goto BJStart;
                                                                                            }

                                                                                            if (playagain == 'n')
                                                                                            {
                                                                                                goto Start;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Console.WriteLine("Press a valid key");
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }

                                                                                if (dealertotal3 > 21)
                                                                                {
                                                                                    int moneyWin = betInt * 2;
                                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                                    string newStringTotal = newTotal.ToString();
                                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                                    try
                                                                                    {
                                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                                    }
                                                                                    catch (Exception e)
                                                                                    {
                                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                                        Console.WriteLine("Error message: " + e);
                                                                                        Thread.Sleep(500);
                                                                                    }
                                                                                    File.WriteAllText(balancetxt, balance);

                                                                                    while (true)
                                                                                    {
                                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                                        if (playagain == 'y')
                                                                                        {
                                                                                            goto BJStart;
                                                                                        }

                                                                                        if (playagain == 'n')
                                                                                        {
                                                                                            goto Start;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Press a valid key");
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (choice2 == "stand")
                                                            {
                                                                Thread.Sleep(200);
                                                                int dealersecoundCard = numbers[rnd.Next(0, 51)];
                                                                Console.WriteLine($"Dealers secound card is {dealersecoundCard}");
                                                                Thread.Sleep(200);
                                                                int dealertotal1 = dealertotal + dealersecoundCard;
                                                                Console.WriteLine($"Dealer has a total of {dealertotal1}");
                                                                Thread.Sleep(200);

                                                                if (dealertotal1 == 21)
                                                                {
                                                                    Console.WriteLine("Dealer wins");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (dealertotal1 == total3)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (dealertotal1 > total3)
                                                                {
                                                                    if (dealertotal1 <= 21)
                                                                    {
                                                                        Console.WriteLine("Dealer wins");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto BJStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine($"You win {moneyWin}");
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto BJStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                if (dealertotal1 > 21)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (dealertotal1 < total3)
                                                                {
                                                                    Thread.Sleep(200);
                                                                    int dealerthirdCard = numbers[rnd.Next(0, 51)];
                                                                    Console.WriteLine($"Dealers third card is {dealerthirdCard}");
                                                                    int dealertotal2 = dealerthirdCard + dealertotal1;
                                                                    Thread.Sleep(200);
                                                                    Console.WriteLine($"Dealer now has a total of {dealertotal2}");
                                                                    Thread.Sleep(200);

                                                                    if (dealertotal2 == 21)
                                                                    {
                                                                        Console.WriteLine("Dealer wins");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto BJStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (dealertotal2 == total3)
                                                                    {
                                                                        Console.WriteLine("Tie");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto BJStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (dealertotal2 > total3)
                                                                    {
                                                                        if (dealertotal2 <= 21)
                                                                        {
                                                                            Console.WriteLine("Dealer wins");
                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            int moneyWin = betInt * 2;
                                                                            Console.WriteLine($"You win {moneyWin}");
                                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                                            int moneyTotalInt = Int32.Parse(newString);
                                                                            int newTotal = moneyTotalInt + moneyWin;
                                                                            string newStringTotal = newTotal.ToString();
                                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                            try
                                                                            {
                                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                            }
                                                                            catch (Exception e)
                                                                            {
                                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                                Console.WriteLine("Error message: " + e);
                                                                                Thread.Sleep(500);
                                                                            }
                                                                            File.WriteAllText(balancetxt, balance);

                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    if (dealertotal2 > 21)
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine($"You win {moneyWin}");
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto BJStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (dealertotal2 < total3)
                                                                    {
                                                                        Thread.Sleep(200);
                                                                        int dealerfourthCard = numbers[rnd.Next(0, 51)];
                                                                        Console.WriteLine($"Dealers third card is {dealerfourthCard}");
                                                                        Thread.Sleep(200);
                                                                        int dealertotal3 = dealerfourthCard + dealertotal2;
                                                                        Console.WriteLine($"Dealer now has a total of {dealertotal3}");
                                                                        Thread.Sleep(200);

                                                                        if (dealertotal3 == 21)
                                                                        {
                                                                            Console.WriteLine("Dealer wins");
                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }

                                                                        if (dealertotal3 == total3)
                                                                        {
                                                                            Console.WriteLine("Tie");
                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }

                                                                        if (dealertotal3 > total1)
                                                                        {
                                                                            if (dealertotal3 <= 21)
                                                                            {
                                                                                Console.WriteLine("Dealer wins");
                                                                                while (true)
                                                                                {
                                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                                    if (playagain == 'y')
                                                                                    {
                                                                                        goto BJStart;
                                                                                    }

                                                                                    if (playagain == 'n')
                                                                                    {
                                                                                        goto Start;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Press a valid key");
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                int moneyWin = betInt * 2;
                                                                                Console.WriteLine($"You win {moneyWin}");
                                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                                string newStringTotal = newTotal.ToString();
                                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                                try
                                                                                {
                                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                                }
                                                                                catch (Exception e)
                                                                                {
                                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                                    Console.WriteLine("Error message: " + e);
                                                                                    Thread.Sleep(500);
                                                                                }
                                                                                File.WriteAllText(balancetxt, balance);

                                                                                while (true)
                                                                                {
                                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                                    if (playagain == 'y')
                                                                                    {
                                                                                        goto BJStart;
                                                                                    }

                                                                                    if (playagain == 'n')
                                                                                    {
                                                                                        goto Start;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Press a valid key");
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        if (dealertotal3 > 21)
                                                                        {
                                                                            int moneyWin = betInt * 2;
                                                                            Console.WriteLine($"You win {moneyWin}");
                                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                                            int moneyTotalInt = Int32.Parse(newString);
                                                                            int newTotal = moneyTotalInt + moneyWin;
                                                                            string newStringTotal = newTotal.ToString();
                                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                            try
                                                                            {
                                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                            }
                                                                            catch (Exception e)
                                                                            {
                                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                                Console.WriteLine("Error message: " + e);
                                                                                Thread.Sleep(500);
                                                                            }
                                                                            File.WriteAllText(balancetxt, balance);

                                                                            while (true)
                                                                            {
                                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                                char playagain = Console.ReadKey().KeyChar;

                                                                                if (playagain == 'y')
                                                                                {
                                                                                    goto BJStart;
                                                                                }

                                                                                if (playagain == 'n')
                                                                                {
                                                                                    goto Start;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Press a valid key");
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (choice1 == "stand")
                                                    {
                                                        Thread.Sleep(200);
                                                        int dealersecoundCard = numbers[rnd.Next(0, 51)];
                                                        Console.WriteLine($"Dealers secound card is {dealersecoundCard}");
                                                        Thread.Sleep(200);
                                                        int dealertotal1 = dealertotal + dealersecoundCard;
                                                        Console.WriteLine($"Dealer has a total of {dealertotal1}");
                                                        Thread.Sleep(200);

                                                        if (dealertotal1 == 21)
                                                        {
                                                            Console.WriteLine("Dealer wins");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (dealertotal1 == total2)
                                                        {
                                                            Console.WriteLine("Tie");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (dealertotal1 > total2)
                                                        {
                                                            if (dealertotal1 <= 21)
                                                            {
                                                                Console.WriteLine("Dealer wins");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto BJStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine($"You win {moneyWin}");
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto BJStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (dealertotal1 > 21)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine($"You win {moneyWin}");
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (dealertotal1 < total2)
                                                        {
                                                            Thread.Sleep(200);
                                                            int dealerthirdCard = numbers[rnd.Next(0, 51)];
                                                            Console.WriteLine($"Dealers third card is {dealerthirdCard}");
                                                            Thread.Sleep(200);
                                                            int dealertotal2 = dealerthirdCard + dealertotal1;
                                                            Console.WriteLine($"Dealer now has a total of {dealertotal2}");
                                                            Thread.Sleep(200);

                                                            if (dealertotal2 == 21)
                                                            {
                                                                Console.WriteLine("Dealer wins");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto BJStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (dealertotal2 == total1)
                                                            {
                                                                Console.WriteLine("Tie");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto BJStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (dealertotal2 > total1)
                                                            {
                                                                if (dealertotal2 <= 21)
                                                                {
                                                                    Console.WriteLine("Dealer wins");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (dealertotal2 > 21)
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine($"You win {moneyWin}");
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto BJStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (dealertotal1 < total1)
                                                            {
                                                                Thread.Sleep(200);
                                                                int dealerfourthCard = numbers[rnd.Next(0, 51)];
                                                                Console.WriteLine($"Dealers third card is {dealerfourthCard}");
                                                                Thread.Sleep(200);
                                                                int dealertotal3 = dealerfourthCard + dealertotal2;
                                                                Console.WriteLine($"Dealer now has a total of {dealertotal3}");
                                                                Thread.Sleep(200);

                                                                if (dealertotal3 == 21)
                                                                {
                                                                    Console.WriteLine("Dealer wins");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (dealertotal3 == total1)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (dealertotal3 > total1)
                                                                {
                                                                    if (dealertotal3 <= 21)
                                                                    {
                                                                        Console.WriteLine("Dealer wins");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto BJStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine($"You win {moneyWin}");
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto BJStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                if (dealertotal3 > 21)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto BJStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (choice == "stand")
                                            {
                                                Thread.Sleep(200);
                                                int dealersecoundCard = numbers[rnd.Next(0, 51)];
                                                Console.WriteLine($"Dealers secound card is {dealersecoundCard}");
                                                Thread.Sleep(200);
                                                int dealertotal1 = dealertotal + dealersecoundCard;
                                                Console.WriteLine($"Dealer has a total of {dealertotal1}");
                                                Thread.Sleep(200);

                                                if (dealertotal1 == 21)
                                                {
                                                    Console.WriteLine("Dealer wins");
                                                    while (true)
                                                    {
                                                        Console.WriteLine("Would you like to play again? y/n");
                                                        char playagain = Console.ReadKey().KeyChar;

                                                        if (playagain == 'y')
                                                        {
                                                            goto BJStart;
                                                        }

                                                        if (playagain == 'n')
                                                        {
                                                            goto Start;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Press a valid key");
                                                        }
                                                    }
                                                }

                                                if (dealertotal1 == total1)
                                                {
                                                    Console.WriteLine("Tie");
                                                    while (true)
                                                    {
                                                        Console.WriteLine("Would you like to play again? y/n");
                                                        char playagain = Console.ReadKey().KeyChar;

                                                        if (playagain == 'y')
                                                        {
                                                            goto BJStart;
                                                        }

                                                        if (playagain == 'n')
                                                        {
                                                            goto Start;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Press a valid key");
                                                        }
                                                    }
                                                }

                                                if (dealertotal1 > total1)
                                                {
                                                    if (dealertotal1 <= 21)
                                                    {
                                                        Console.WriteLine("Dealer wins");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto BJStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto BJStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (dealertotal1 > 21)
                                                {
                                                    int moneyWin = betInt * 2;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);

                                                    while (true)
                                                    {
                                                        Console.WriteLine("Would you like to play again? y/n");
                                                        char playagain = Console.ReadKey().KeyChar;

                                                        if (playagain == 'y')
                                                        {
                                                            goto BJStart;
                                                        }

                                                        if (playagain == 'n')
                                                        {
                                                            goto Start;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Press a valid key");
                                                        }
                                                    }
                                                }

                                                if (dealertotal1 < total1)
                                                {
                                                    Thread.Sleep(200);
                                                    int dealerthirdCard = numbers[rnd.Next(0, 51)];
                                                    Console.WriteLine($"Dealers third card is {dealerthirdCard}");
                                                    Thread.Sleep(200);
                                                    int dealertotal2 = dealerthirdCard + dealertotal1;
                                                    Console.WriteLine($"Dealer now has a total of {dealertotal2}");
                                                    Thread.Sleep(200);

                                                    if (dealertotal2 == 21)
                                                    {
                                                        Console.WriteLine("Dealer wins");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto BJStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (dealertotal2 == total1)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto BJStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (dealertotal2 > total1)
                                                    {
                                                        if (dealertotal2 <= 21)
                                                        {
                                                            Console.WriteLine("Dealer wins");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine($"You win {moneyWin}");
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (dealertotal2 > 21)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto BJStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (dealertotal2 < total1)
                                                    {
                                                        Thread.Sleep(200);
                                                        int dealerfourthCard = numbers[rnd.Next(0, 51)];
                                                        Console.WriteLine($"Dealers third card is {dealerfourthCard}");
                                                        Thread.Sleep(200);
                                                        int dealertotal3 = dealerfourthCard + dealertotal2;
                                                        Console.WriteLine($"Dealer now has a total of {dealertotal3}");
                                                        Thread.Sleep(200);

                                                        if (dealertotal3 == 21)
                                                        {
                                                            Console.WriteLine("Dealer wins");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (dealertotal3 == total1)
                                                        {
                                                            Console.WriteLine("Tie");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (dealertotal3 > total1)
                                                        {
                                                            if (dealertotal3 <= 21)
                                                            {
                                                                Console.WriteLine("Dealer wins");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto BJStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine($"You win {moneyWin}");
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto BJStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (dealertotal3 > 21)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine($"You win {moneyWin}");
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto BJStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            else
                                            {
                                                Console.WriteLine("");
                                                goto hitorstand;
                                            }
                                        }

                                    }
                                    break;

                                case '2':
                                    {
                                    RouletteStart:
                                        Console.Clear();
                                        Console.WriteLine("Welcome to Roulette");
                                        Console.WriteLine("How do you want to bet?");
                                        Thread.Sleep(200);
                                        Console.Write("1: ");
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("Straight-on");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Thread.Sleep(200);
                                        Console.Write("2: ");
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("Red, Black or Green");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Thread.Sleep(200);
                                        Console.Write("3: ");
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("Even or Odd");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Thread.Sleep(200);
                                        Console.Write("4: ");
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("Low or High");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.Write("5: ");
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine("Colums");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        char bet = Console.ReadKey().KeyChar;
                                        Console.WriteLine("");
                                        Thread.Sleep(500);
                                        // Choose what to do
                                        while (true)
                                        {
                                            switch (bet)
                                            {
                                                default:
                                                    {
                                                        Console.WriteLine("Please enter a valid number");
                                                        goto RouletteStart;
                                                    }

                                                case '1':
                                                    {
                                                        while (true)
                                                        {
                                                            Console.Clear();
                                                            Console.WriteLine("Which number do you want to bet on?");
                                                            string number = "";
                                                            ConsoleKeyInfo ch;
                                                            do
                                                            {
                                                                ch = Console.ReadKey(true);
                                                                if (ch.Key != ConsoleKey.Backspace)
                                                                {
                                                                    bool control = double.TryParse(ch.KeyChar.ToString(), out _);
                                                                    if (control)
                                                                    {
                                                                        number += ch.KeyChar;
                                                                        Console.Write(ch.KeyChar);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (ch.Key == ConsoleKey.Backspace && number.Length > 0)
                                                                    {
                                                                        number = number.Substring(number.Length - 1);
                                                                        Console.Write("\b \b");
                                                                    }
                                                                }
                                                            }

                                                            while (ch.Key != ConsoleKey.Enter);
                                                            int numberInt = Int32.Parse(number);

                                                            Console.WriteLine("");

                                                            if (number == null)
                                                            {
                                                                Console.WriteLine("Write a number before you press enter");
                                                                Thread.Sleep(1000);
                                                                goto RouletteStart;
                                                            }

                                                            if (numberInt > 36)
                                                            {
                                                                Console.WriteLine("Your number is too high, try again. Your number has to be 36 or less");
                                                                Thread.Sleep(2000);
                                                                goto RouletteStart;
                                                            }

                                                            else
                                                            {
                                                            }

                                                            Thread.Sleep(200);
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
                                                                else

                                                                {
                                                                    if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                    {
                                                                        num = num.Substring(num.Length - 1);
                                                                        Console.Write("\b \b");
                                                                    }
                                                                }
                                                            }

                                                            while (chr.Key != ConsoleKey.Enter);

                                                            // Get balance from balance file
                                                            try
                                                            {
                                                                byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                            }
                                                            catch (Exception)
                                                            {
                                                                Console.WriteLine("Error while decoding balance");
                                                                Thread.Sleep(500);
                                                                goto Startover;
                                                            }
                                                            int betInt = Int32.Parse(num);
                                                            int moneyInt = Int32.Parse(tmpbalance);
                                                            int totalAfterBet = moneyInt - betInt;

                                                            // if the bet is above what user has, send user back
                                                            if (totalAfterBet < 0)
                                                            {
                                                                Console.WriteLine("");
                                                                Console.WriteLine("Bet is exceeds your current balance");
                                                                Console.WriteLine("Please reset your money at the menu");
                                                                Thread.Sleep(2000);
                                                                goto RouletteStart;
                                                            }

                                                            // if bet isnt above total amount then subtract bet from total and put in file
                                                            else
                                                            {
                                                                string newTotal = totalAfterBet.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newTotal);
                                                                //Encrypt balance
                                                                try
                                                                {
                                                                    byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                    accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                }

                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Encrypting went wrong");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }
                                                                using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                string moneyTotal = "";
                                                                while ((moneyTotal = sr.ReadLine()) != null)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                    Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                    Thread.Sleep(5000);
                                                                    break;
                                                                }
                                                            }

                                                            Console.Clear();

                                                            Random rnd = new Random();
                                                            int rndnumber = rnd.Next(0, 36);

                                                            Rolling();

                                                            Console.Write("The number is ");
                                                            Thread.Sleep(500);
                                                            Console.WriteLine($"{rndnumber}");

                                                            if (rndnumber == numberInt)
                                                            {
                                                                int moneyWin = betInt * 35;
                                                                Console.WriteLine($"You win {moneyWin}");
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto RouletteStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("You lose");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto RouletteStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                case '2':
                                                    {
                                                        while (true)
                                                        {
                                                            Console.Clear();
                                                            Console.WriteLine("Which color do you want to bet on?");
                                                            string col = Console.ReadLine();
                                                            string color = col.ToLower();
                                                            Thread.Sleep(1000);

                                                            if (color == "red")
                                                            {
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                Console.Clear();

                                                                var red = new List<int>
                                                                    {
                                                                    1,
                                                                    3,
                                                                    5,
                                                                    7,
                                                                    9,
                                                                    12,
                                                                    14,
                                                                    16,
                                                                    18,
                                                                    19,
                                                                    21,
                                                                    23,
                                                                    25,
                                                                    27,
                                                                    30,
                                                                    32,
                                                                    34,
                                                                    36
                                                                    };

                                                                Random rand = new Random();
                                                                int randcolor = rand.Next(0, 36);

                                                                Rolling();

                                                                if (red.Contains(randcolor))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randcolor} which is {color}");
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randcolor} which is not {color}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (color == "black")
                                                            {
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var black = new List<int>
                                                                    {
                                                                    2,
                                                                    4,
                                                                    6,
                                                                    8,
                                                                    10,
                                                                    11,
                                                                    13,
                                                                    15,
                                                                    17,
                                                                    20,
                                                                    22,
                                                                    24,
                                                                    26,
                                                                    28,
                                                                    29,
                                                                    31,
                                                                    33,
                                                                    35
                                                                    };

                                                                Random rand = new Random();
                                                                int randcolor = rand.Next(0, 36);

                                                                Rolling();

                                                                if (black.Contains(randcolor))
                                                                {

                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randcolor} which is {color}");
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randcolor} which is not {color}");
                                                                    Thread.Sleep(2000);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (color == "green")
                                                            {
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var green = new List<int>
                                                                    {
                                                                        0
                                                                    };

                                                                Random rand = new Random();
                                                                int randcolor = rand.Next(0, 36);

                                                                Rolling();

                                                                if (green.Contains(randcolor))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randcolor} which is {color}");
                                                                    int moneyWin = betInt * 35;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randcolor} which is not {color}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Please choose one of the options");
                                                                Thread.Sleep(1500);
                                                            }
                                                        }
                                                    }

                                                case '3':
                                                    {
                                                        while (true)
                                                        {
                                                            Console.Clear();
                                                            Console.WriteLine("Even or Odd?");
                                                            string cho = Console.ReadLine();
                                                            string choice = cho.ToLower();

                                                            if (choice == "even")
                                                            {
                                                                Thread.Sleep(200);
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var even = new List<int>
                                                                {
                                                                2,
                                                                4,
                                                                6,
                                                                8,
                                                                10,
                                                                12,
                                                                14,
                                                                16,
                                                                18,
                                                                20,
                                                                22,
                                                                24,
                                                                26,
                                                                28,
                                                                30,
                                                                32,
                                                                34,
                                                                36
                                                                };

                                                                Random rand = new Random();
                                                                int randnum = rand.Next(0, 36);

                                                                Rolling();

                                                                if (even.Contains(randnum))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randnum} which is {choice}");
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randnum} which is not {choice}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (choice == "odd")
                                                            {
                                                                Thread.Sleep(200);
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var odd = new List<int>
                                                                {
                                                                1,
                                                                3,
                                                                5,
                                                                7,
                                                                9,
                                                                11,
                                                                13,
                                                                15,
                                                                17,
                                                                19,
                                                                21,
                                                                23,
                                                                25,
                                                                27,
                                                                29,
                                                                31,
                                                                33,
                                                                35
                                                                };

                                                                Random rand = new Random();
                                                                int randnum = rand.Next(0, 36);

                                                                Rolling();

                                                                if (odd.Contains(randnum))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randnum} which is {choice}");
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randnum} which is not {choice}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Please choose one of the options");
                                                                Thread.Sleep(1500);
                                                            }
                                                        }
                                                    }

                                                case '4':
                                                    {
                                                        while (true)
                                                        {
                                                            Console.Clear();
                                                            Console.WriteLine("High or Low?");
                                                            string cho = Console.ReadLine();
                                                            string choice = cho.ToLower();

                                                            if (choice == "high")
                                                            {
                                                                Thread.Sleep(200);
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var high = new List<int>
                                                                    {
                                                                        19,
                                                                        20,
                                                                        21,
                                                                        22,
                                                                        23,
                                                                        24,
                                                                        25,
                                                                        26,
                                                                        27,
                                                                        28,
                                                                        29,
                                                                        30,
                                                                        31,
                                                                        32,
                                                                        33,
                                                                        34,
                                                                        35,
                                                                        36,
                                                                    };

                                                                Random rand = new Random();
                                                                int randnum = rand.Next(0, 36);

                                                                Rolling();

                                                                if (high.Contains(randnum))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randnum} which is {choice}");
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randnum} which is not {choice}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (choice == "low")
                                                            {
                                                                Thread.Sleep(200);
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var low = new List<int>
                                                                    {
                                                                        1,
                                                                        2,
                                                                        3,
                                                                        4,
                                                                        5,
                                                                        6,
                                                                        7,
                                                                        8,
                                                                        9,
                                                                        10,
                                                                        11,
                                                                        12,
                                                                        13,
                                                                        14,
                                                                        15,
                                                                        16,
                                                                        17,
                                                                        18,
                                                                    };

                                                                Random rand = new Random();
                                                                int randnum = rand.Next(0, 36);

                                                                Rolling();

                                                                if (low.Contains(randnum))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randnum} which is {choice}");
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randnum} which is not {choice}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Please choose one of the options");
                                                                Thread.Sleep(1500);
                                                            }
                                                        }
                                                    }

                                                case '5':
                                                    {
                                                        while (true)
                                                        {
                                                            Console.Clear();
                                                            Thread.Sleep(200);
                                                            Console.WriteLine("Which colum do you want to bet on? 1, 2 or 3?");
                                                            string cho = Console.ReadLine();
                                                            string choice = cho.ToLower();

                                                            if (choice == "1")
                                                            {
                                                                Thread.Sleep(200);
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }
                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var low = new List<int>
                                                                    {
                                                                        1,
                                                                        2,
                                                                        3,
                                                                        4,
                                                                        5,
                                                                        6,
                                                                        7,
                                                                        8,
                                                                        9,
                                                                        10,
                                                                        11,
                                                                        12
                                                                    };

                                                                Random rand = new Random();
                                                                int randnum = rand.Next(0, 36);

                                                                Rolling();

                                                                if (low.Contains(randnum))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randnum} which is {choice}");
                                                                    int moneyWin = betInt * 3;
                                                                    Thread.Sleep(200);
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randnum} which is not {choice}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (choice == "2")
                                                            {
                                                                Thread.Sleep(200);
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var mid = new List<int>
                                                                    {
                                                                        13,
                                                                        14,
                                                                        15,
                                                                        16,
                                                                        17,
                                                                        18,
                                                                        19,
                                                                        20,
                                                                        21,
                                                                        22,
                                                                        23,
                                                                        24
                                                                    };

                                                                Random rand = new Random();
                                                                int randnum = rand.Next(0, 36);

                                                                Rolling();

                                                                if (mid.Contains(randnum))
                                                                {
                                                                    Console.Write("The number is ");
                                                                    Thread.Sleep(500);
                                                                    Console.WriteLine($"{randnum} which is {choice}");
                                                                    int moneyWin = betInt * 3;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randnum} which is not {choice}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (choice == "3")
                                                            {
                                                                Thread.Sleep(200);
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
                                                                    else

                                                                    {
                                                                        if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                                        {
                                                                            num = num.Substring(num.Length - 1);
                                                                            Console.Write("\b \b");
                                                                        }
                                                                    }
                                                                }

                                                                while (chr.Key != ConsoleKey.Enter);

                                                                // Get balance from balance file
                                                                try
                                                                {
                                                                    byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                                    tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    Console.WriteLine("Error while decoding balance");
                                                                    Thread.Sleep(500);
                                                                    goto Startover;
                                                                }

                                                                int betInt = Int32.Parse(num);
                                                                int moneyInt = Int32.Parse(tmpbalance);
                                                                int totalAfterBet = moneyInt - betInt;

                                                                // if the bet is above what user has, send user back
                                                                if (totalAfterBet < 0)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine("Bet is exceeds your current balance");
                                                                    Console.WriteLine("Please reset your money at the menu");
                                                                    Thread.Sleep(2000);
                                                                    goto RouletteStart;
                                                                }

                                                                // if bet isnt above total amount then subtract bet from total and put in file
                                                                else
                                                                {
                                                                    string newTotal = totalAfterBet.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newTotal);
                                                                    //Encrypt balance
                                                                    try
                                                                    {
                                                                        byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                                        accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                                    }

                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Encrypting went wrong");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                        goto Startover;
                                                                    }
                                                                    using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                                    string moneyTotal = "";
                                                                    while ((moneyTotal = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal}");
                                                                        Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                                        Thread.Sleep(5000);
                                                                        break;
                                                                    }
                                                                }

                                                                var odd = new List<int>
                                                                    {
                                                                        25,
                                                                        26,
                                                                        27,
                                                                        28,
                                                                        29,
                                                                        30,
                                                                        31,
                                                                        32,
                                                                        33,
                                                                        34,
                                                                        35,
                                                                        36
                                                                    };

                                                                Random rand = new Random();
                                                                int randnum = rand.Next(0, 36);

                                                                Rolling();

                                                                if (odd.Contains(randnum))
                                                                {
                                                                    Console.WriteLine($"The number is {randnum} which is {choice}");
                                                                    int moneyWin = betInt * 3;
                                                                    Console.WriteLine($"You win {moneyWin}");
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("You lose");
                                                                    Console.WriteLine($"The number is {randnum} which is not {choice}");

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto RouletteStart; ;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            else
                                                            {
                                                                Console.WriteLine("Please choose one of the options");
                                                                Thread.Sleep(1500);
                                                            }
                                                        }
                                                    }
                                            }
                                        }
                                    }

                                case '3':
                                    {
                                    DiceStart:
                                        Console.Clear();
                                        Console.WriteLine("Welcome to Dice");
                                        while (true)
                                        {
                                        Bet:
                                            Console.Clear();
                                            Console.WriteLine("Which number do you want to bet on?");
                                            char number = Console.ReadKey().KeyChar;
                                            int newnumber = int.Parse(number.ToString());

                                            Thread.Sleep(200);

                                            if (newnumber > 6 || number == 0)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine("Your number is too high, try again. Your number has to be 6 or less");
                                                Thread.Sleep(700);
                                                goto Bet;
                                            }

                                            else
                                            {
                                            }

                                            Console.WriteLine("");
                                            Thread.Sleep(200);
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
                                                else

                                                {
                                                    if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                    {
                                                        num = num.Substring(num.Length - 1);
                                                        Console.Write("\b \b");
                                                    }
                                                }
                                            }

                                            while (chr.Key != ConsoleKey.Enter);

                                            // Get balance from balance file
                                            try
                                            {
                                                byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                                tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine("Error while decoding balance");
                                                Thread.Sleep(500);
                                                goto Startover;
                                            }

                                            int betInt = Int32.Parse(num);
                                            int moneyInt = Int32.Parse(tmpbalance);
                                            int totalAfterBet = moneyInt - betInt;

                                            // if the bet is above what user has, send user back
                                            if (totalAfterBet < 0)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine("Bet is exceeds your current balance");
                                                Console.WriteLine("Please reset your money at the menu");
                                                Thread.Sleep(2000);
                                                goto DiceStart;
                                            }

                                            // if bet isnt above total amount then subtract bet from total and put in file
                                            else
                                            {
                                                string newTotal = totalAfterBet.ToString();
                                                File.WriteAllText(tmpbalancetxt, newTotal);
                                                //Encrypt balance
                                                try
                                                {
                                                    byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                    accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                }

                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Encrypting went wrong");
                                                    Console.WriteLine("Error message: " + e);
                                                    Thread.Sleep(500);
                                                    goto Startover;
                                                }
                                                using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                string moneyTotal = "";
                                                while ((moneyTotal = sr.ReadLine()) != null)
                                                {
                                                    Console.WriteLine("");
                                                    Console.WriteLine($"Your balance is now {moneyTotal}");
                                                    Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                    Thread.Sleep(5000);
                                                    break;
                                                }
                                            }

                                            Random rnd = new Random();
                                            int rndnumber = rnd.Next(0, 5) + 1;

                                            Console.Clear();

                                            Rolling();

                                            Console.Write("The number is ");
                                            Thread.Sleep(500);
                                            Console.WriteLine($"{rndnumber}");

                                            if (rndnumber == newnumber)
                                            {
                                                int moneyWin = betInt * 6;
                                                Console.WriteLine($"You win {moneyWin}");
                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                int moneyTotalInt = Int32.Parse(newString);
                                                int newTotal = moneyTotalInt + moneyWin;
                                                string newStringTotal = newTotal.ToString();
                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                try
                                                {
                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Couldn't encrypt winning");
                                                    Console.WriteLine("Error message: " + e);
                                                    Thread.Sleep(500);
                                                }
                                                File.WriteAllText(balancetxt, balance);

                                                while (true)
                                                {
                                                    Console.WriteLine("Would you like to play again? y/n");
                                                    char playagain = Console.ReadKey().KeyChar;

                                                    if (playagain == 'y')
                                                    {
                                                        goto DiceStart;
                                                    }

                                                    if (playagain == 'n')
                                                    {
                                                        goto Start;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Press a valid key");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("You lose");
                                                while (true)
                                                {
                                                    Console.WriteLine("Would you like to play again? y/n");
                                                    char playagain = Console.ReadKey().KeyChar;

                                                    if (playagain == 'y')
                                                    {
                                                        goto DiceStart;
                                                    }

                                                    if (playagain == 'n')
                                                    {
                                                        goto Start;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Press a valid key");
                                                    }
                                                }
                                            }
                                        }
                                    }

                                case '4':
                                    {
                                    baccaratStart:
                                        Console.Clear();
                                        Console.WriteLine("Welcome to Barracat");

                                        int[] numbers = new int[13];
                                        numbers[0] = 1;
                                        numbers[1] = 2;
                                        numbers[2] = 3;
                                        numbers[3] = 4;
                                        numbers[4] = 5;
                                        numbers[5] = 6;
                                        numbers[6] = 7;
                                        numbers[7] = 8;
                                        numbers[8] = 9;
                                        numbers[9] = 0;
                                        numbers[10] = 0;
                                        numbers[11] = 0;
                                        numbers[12] = 0;

                                        Console.WriteLine("Who is gonna win? You, Banker or Tie?");
                                        string choice = Console.ReadLine();
                                        string lowerchoice = choice.ToLower();
                                        if (lowerchoice == "you")
                                        {
                                            Console.WriteLine("What is your bet?");
                                            string num = "";

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
                                                else

                                                {
                                                    if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                    {
                                                        num = num.Substring(num.Length - 1);
                                                        Console.Write("\b \b");
                                                    }
                                                }
                                            }
                                            while (chr.Key != ConsoleKey.Enter);

                                            int betInt = Int32.Parse(num);
                                            int moneyInt = Int32.Parse(tmpbalance);
                                            int totalAfterBet = moneyInt - betInt;

                                            if (totalAfterBet < 0)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine("Bet is exceeds your current balance");
                                                Console.WriteLine("Please reset your money at the menu");
                                                goto Startover;
                                            }
                                            else
                                            {
                                                string newTotal = totalAfterBet.ToString();
                                                File.WriteAllText(balancetxt, newTotal);
                                                using StreamReader sr = File.OpenText(balancetxt);
                                                string moneyTotal = "";
                                                while ((moneyTotal = sr.ReadLine()) != null)
                                                {
                                                    Console.WriteLine("");
                                                    Console.WriteLine($"Your balance is now {moneyTotal}");
                                                    Console.WriteLine("CAUTION: if you leave while playing a ga,e that haven't ended yet you will lose your money");
                                                    break;
                                                }
                                            }

                                            Random rnd = new Random();
                                            int firstCard = numbers[rnd.Next(0, 12)];
                                            int bankerFirst = numbers[rnd.Next(0, 12)];
                                            int secoundCard = numbers[rnd.Next(0, 12)];
                                            int bankerSecound = numbers[rnd.Next(0, 12)];
                                            int total = firstCard + secoundCard;
                                            int bankerTotal = bankerFirst + bankerSecound;
                                            Console.WriteLine($"Your first card is {firstCard}");
                                            Console.WriteLine($"Your secound card is {secoundCard}");
                                            Console.WriteLine("");
                                            Console.WriteLine($"Bankers first card is {bankerFirst}");
                                            Console.WriteLine($"Bankers secound card is {bankerSecound}");

                                            if (total > 10)
                                            {
                                                string totalString = total.ToString();
                                                string total1 = totalString.Substring(1);
                                                int newTotal = Int32.Parse(total1);
                                                Console.WriteLine($"Your total is now {newTotal}");
                                            }

                                            if (bankerTotal > 10)
                                            {
                                                string bankerTotalString = bankerTotal.ToString();
                                                string bankerTotal1 = bankerTotalString.Substring(1);
                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");
                                                int newbankerTotal = Int32.Parse(bankerTotal1);
                                                Console.WriteLine($"Bankers total is now {newbankerTotal}");
                                            }
                                            else
                                            {
                                                if (total == 9)
                                                {
                                                    Console.WriteLine("Le gros!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto Startover;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (bankerTotal == 9)
                                                {
                                                    Console.WriteLine("Le gros!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (total == 8)
                                                {
                                                    Console.WriteLine("Le petit!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (bankerTotal == 8)
                                                {
                                                    Console.WriteLine("Le petit!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (total <= 5)
                                                {
                                                    int thirdCard = numbers[rnd.Next(0, 12)];
                                                    Console.WriteLine($"Your first card is {thirdCard}");
                                                    int total1 = total + thirdCard;
                                                    Console.WriteLine($"Your total is now {total1}");

                                                    if (total1 > 10)
                                                    {
                                                        string totalString = total1.ToString();
                                                        string total2 = totalString.Substring(1);
                                                        int newTotal1 = Int32.Parse(total2);
                                                        Console.WriteLine($"Your total is now {newTotal1}");

                                                        if (bankerTotal <= 4)
                                                        {
                                                            int bankerThird = numbers[rnd.Next(0, 12)];
                                                            Console.WriteLine($"Your first card is {bankerThird}");
                                                            int bankerTotal1 = bankerTotal + bankerThird;
                                                            Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                            if (total1 > bankerTotal1)
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine("You win " + moneyWin);
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto Startover;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal1 > total1)
                                                            {
                                                                Console.WriteLine("Banker win");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal1 == total1)
                                                            {
                                                                Console.WriteLine("Tie");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (total1 > bankerTotal)
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine("You win " + moneyWin);
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto Startover;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal > total1)
                                                            {
                                                                Console.WriteLine("Banker win");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal == total1)
                                                            {
                                                                Console.WriteLine("Tie");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal <= 4)
                                                    {
                                                        int bankerThird = numbers[rnd.Next(0, 12)];
                                                        Console.WriteLine($"Your first card is {bankerThird}");
                                                        int bankerTotal1 = bankerTotal + bankerThird;
                                                        Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                        if (total1 > bankerTotal1)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine("You win " + moneyWin);
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto Startover;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal1 > total1)
                                                        {
                                                            Console.WriteLine("Banker win");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal1 == total1)
                                                        {
                                                            Console.WriteLine("Tie");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (total1 > bankerTotal)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine("You win " + moneyWin);
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto Startover;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal > total1)
                                                        {
                                                            Console.WriteLine("Banker win");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal == total1)
                                                        {
                                                            Console.WriteLine("Tie");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (true)
                                                    {
                                                        Console.WriteLine("Do you want to hit or stay?");
                                                        string h = Console.ReadLine();
                                                        string newh = h.ToLower();

                                                        if (newh == "hit")
                                                        {
                                                            int thirdCard = numbers[rnd.Next(0, 12)];
                                                            Console.WriteLine($"Your third card is {thirdCard}");
                                                            int total1 = total + thirdCard;
                                                            Console.WriteLine($"Your total is now {total1}");

                                                            if (total1 > 10)
                                                            {
                                                                string totalString = total1.ToString();
                                                                string total2 = totalString.Substring(1);
                                                                int newTotal1 = Int32.Parse(total2);
                                                                Console.WriteLine($"Your total is now {newTotal1}");

                                                                if (bankerTotal <= 4)
                                                                {
                                                                    int bankerThird = numbers[rnd.Next(0, 12)];
                                                                    Console.WriteLine($"Your first card is {bankerThird}");
                                                                    int bankerTotal1 = bankerTotal + bankerThird;
                                                                    Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                    if (total1 > bankerTotal1)
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine("You win " + moneyWin);
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto Startover;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal1 > total1)
                                                                    {
                                                                        Console.WriteLine("Banker win");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal1 == total1)
                                                                    {
                                                                        Console.WriteLine("Tie");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (total1 > bankerTotal)
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine("You win " + moneyWin);
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto Startover;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal > total1)
                                                                    {
                                                                        Console.WriteLine("Banker win");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal == total1)
                                                                    {
                                                                        Console.WriteLine("Tie");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal <= 4)
                                                            {
                                                                int bankerThird = numbers[rnd.Next(0, 12)];
                                                                Console.WriteLine($"Your first card is {bankerThird}");
                                                                int bankerTotal1 = bankerTotal + bankerThird;
                                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                if (total1 > bankerTotal1)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 > total1)
                                                                {
                                                                    Console.WriteLine("Banker win");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 == total1)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (total1 > bankerTotal)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal > total1)
                                                                {
                                                                    Console.WriteLine("Banker win");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal == total1)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (newh == "stay")
                                                        {
                                                            if (bankerTotal <= 5)
                                                            {
                                                                int bankerThird = numbers[rnd.Next(0, 12)];
                                                                Console.WriteLine($"Your first card is {bankerThird}");
                                                                int bankerTotal1 = bankerTotal + bankerThird;
                                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                if (total > bankerTotal1)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 > total)
                                                                {
                                                                    Console.WriteLine("Banker win");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 == total)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Please pick one");
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (lowerchoice == "banker")
                                        {
                                            Console.WriteLine("What is your bet?");
                                            string num = "";

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
                                                else

                                                {
                                                    if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                    {
                                                        num = num.Substring(num.Length - 1);
                                                        Console.Write("\b \b");
                                                    }
                                                }
                                            }
                                            while (chr.Key != ConsoleKey.Enter);

                                            int betInt = Int32.Parse(num);
                                            int moneyInt = Int32.Parse(tmpbalance);
                                            int totalAfterBet = moneyInt - betInt;

                                            if (totalAfterBet < 0)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine("Bet is exceeds your current balance");
                                                Console.WriteLine("Please resset your money at the menu");
                                                goto Startover;
                                            }
                                            else
                                            {
                                                string newTotal = totalAfterBet.ToString();
                                                File.WriteAllText(balancetxt, newTotal);
                                                using StreamReader sr = File.OpenText(balancetxt);
                                                string moneyTotal = "";
                                                while ((moneyTotal = sr.ReadLine()) != null)
                                                {
                                                    Console.WriteLine("");
                                                    Console.WriteLine($"Your balance is now {moneyTotal}");
                                                    Console.WriteLine("CAUTION: if you leave while playing a ga,e that haven't ended yet you will lose your money");
                                                    break;
                                                }
                                            }

                                            Random rnd = new Random();
                                            int firstCard = numbers[rnd.Next(0, 12)];
                                            int bankerFirst = numbers[rnd.Next(0, 12)];
                                            int secoundCard = numbers[rnd.Next(0, 12)];
                                            int bankerSecound = numbers[rnd.Next(0, 12)];
                                            int total = firstCard + secoundCard;
                                            int bankerTotal = bankerFirst + bankerSecound;
                                            Console.WriteLine($"Your first card is {firstCard}");
                                            Console.WriteLine($"Your secound card is {secoundCard}");
                                            Console.WriteLine("");
                                            Console.WriteLine($"Bankers first card is {bankerFirst}");
                                            Console.WriteLine($"Bankers secound card is {bankerSecound}");

                                            if (total > 10)
                                            {
                                                string totalString = total.ToString();
                                                string total1 = totalString.Substring(1);
                                                int newTotal = Int32.Parse(total1);
                                                Console.WriteLine($"Your total is now {newTotal}");
                                            }

                                            if (bankerTotal > 10)
                                            {
                                                string bankerTotalString = bankerTotal.ToString();
                                                string bankerTotal1 = bankerTotalString.Substring(1);
                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");
                                                int newbankerTotal = Int32.Parse(bankerTotal1);
                                                Console.WriteLine($"Bankers total is now {newbankerTotal}");
                                            }
                                            else
                                            {
                                                if (total == 9)
                                                {
                                                    Console.WriteLine("Le gros!");

                                                    if (total > bankerTotal)
                                                    {
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto Startover;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (bankerTotal == 9)
                                                {
                                                    Console.WriteLine("Le gros!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (total == 8)
                                                {
                                                    Console.WriteLine("Le petit!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (bankerTotal == 8)
                                                {
                                                    Console.WriteLine("Le petit!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (total <= 5)
                                                {
                                                    int thirdCard = numbers[rnd.Next(0, 12)];
                                                    Console.WriteLine($"Your first card is {thirdCard}");
                                                    int total1 = total + thirdCard;
                                                    Console.WriteLine($"Your total is now {total1}");

                                                    if (total1 > 10)
                                                    {
                                                        string totalString = total1.ToString();
                                                        string total2 = totalString.Substring(1);
                                                        int newTotal1 = Int32.Parse(total2);
                                                        Console.WriteLine($"Your total is now {newTotal1}");

                                                        if (bankerTotal <= 4)
                                                        {
                                                            int bankerThird = numbers[rnd.Next(0, 12)];
                                                            Console.WriteLine($"Your first card is {bankerThird}");
                                                            int bankerTotal1 = bankerTotal + bankerThird;
                                                            Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                            if (total1 > bankerTotal1)
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine("You win " + moneyWin);
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto Startover;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal1 > total1)
                                                            {
                                                                Console.WriteLine("Banker win");

                                                                double value = 1.95;
                                                                double moneyWin = betInt * value;
                                                                string newString = File.ReadAllText(balancetxt);
                                                                double moneyTotal = Int32.Parse(newString);
                                                                double newTotal = moneyWin - moneyTotal;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(balancetxt, newStringTotal);
                                                                using StreamReader sr = File.OpenText(balancetxt);
                                                                string moneyTotal1 = "";
                                                                while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                    break;
                                                                }

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal1 == total1)
                                                            {
                                                                Console.WriteLine("Tie");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (total1 > bankerTotal)
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine("You win " + moneyWin);
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto Startover;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal > total1)
                                                            {
                                                                Console.WriteLine("Banker win");

                                                                double value = 1.95;
                                                                double moneyWin = betInt * value;
                                                                string newString = File.ReadAllText(balancetxt);
                                                                double moneyTotal = Int32.Parse(newString);
                                                                double newTotal = moneyWin - moneyTotal;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(balancetxt, newStringTotal);
                                                                using StreamReader sr = File.OpenText(balancetxt);
                                                                string moneyTotal1 = "";
                                                                while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                    break;
                                                                }

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal == total1)
                                                            {
                                                                Console.WriteLine("Tie");
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal <= 4)
                                                    {
                                                        int bankerThird = numbers[rnd.Next(0, 12)];
                                                        Console.WriteLine($"Your first card is {bankerThird}");
                                                        int bankerTotal1 = bankerTotal + bankerThird;
                                                        Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                        if (total1 > bankerTotal1)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine("You win " + moneyWin);
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto Startover;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal1 > total1)
                                                        {
                                                            Console.WriteLine("Banker win");

                                                            double value = 1.95;
                                                            double moneyWin = betInt * value;
                                                            string newString = File.ReadAllText(balancetxt);
                                                            double moneyTotal = Int32.Parse(newString);
                                                            double newTotal = moneyWin - moneyTotal;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(balancetxt, newStringTotal);
                                                            using StreamReader sr = File.OpenText(balancetxt);
                                                            string moneyTotal1 = "";
                                                            while ((moneyTotal1 = sr.ReadLine()) != null)
                                                            {
                                                                Console.WriteLine("");
                                                                Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                break;
                                                            }

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal1 == total1)
                                                        {
                                                            Console.WriteLine("Tie");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (total1 > bankerTotal)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine("You win " + moneyWin);
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto Startover;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal > total1)
                                                        {
                                                            Console.WriteLine("Banker win");

                                                            double value = 1.95;
                                                            double moneyWin = betInt * value;
                                                            string newString = File.ReadAllText(balancetxt);
                                                            double moneyTotal = Int32.Parse(newString);
                                                            double newTotal = moneyWin - moneyTotal;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(balancetxt, newStringTotal);
                                                            using StreamReader sr = File.OpenText(balancetxt);
                                                            string moneyTotal1 = "";
                                                            while ((moneyTotal1 = sr.ReadLine()) != null)
                                                            {
                                                                Console.WriteLine("");
                                                                Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                break;
                                                            }

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal == total1)
                                                        {
                                                            Console.WriteLine("Tie");
                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (true)
                                                    {
                                                        Console.WriteLine("Do you want to hit or stay?");
                                                        string h = Console.ReadLine();
                                                        string newh = h.ToLower();

                                                        if (newh == "hit")
                                                        {
                                                            int thirdCard = numbers[rnd.Next(0, 12)];
                                                            Console.WriteLine($"Your third card is {thirdCard}");
                                                            int total1 = total + thirdCard;
                                                            Console.WriteLine($"Your total is now {total1}");

                                                            if (total1 > 10)
                                                            {
                                                                string totalString = total1.ToString();
                                                                string total2 = totalString.Substring(1);
                                                                int newTotal1 = Int32.Parse(total2);
                                                                Console.WriteLine($"Your total is now {newTotal1}");

                                                                if (bankerTotal <= 4)
                                                                {
                                                                    int bankerThird = numbers[rnd.Next(0, 12)];
                                                                    Console.WriteLine($"Your first card is {bankerThird}");
                                                                    int bankerTotal1 = bankerTotal + bankerThird;
                                                                    Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                    if (total1 > bankerTotal1)
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine("You win " + moneyWin);
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto Startover;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal1 > total1)
                                                                    {
                                                                        Console.WriteLine("Banker win");

                                                                        double value = 1.95;
                                                                        double moneyWin = betInt * value;
                                                                        string newString = File.ReadAllText(balancetxt);
                                                                        double moneyTotal = Int32.Parse(newString);
                                                                        double newTotal = moneyWin - moneyTotal;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                                        string moneyTotal1 = "";
                                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                        {
                                                                            Console.WriteLine("");
                                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                            break;
                                                                        }

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal1 == total1)
                                                                    {
                                                                        Console.WriteLine("Tie");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (total1 > bankerTotal)
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine("You win " + moneyWin);
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto Startover;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal > total1)
                                                                    {
                                                                        Console.WriteLine("Banker win");

                                                                        double value = 1.95;
                                                                        double moneyWin = betInt * value;
                                                                        string newString = File.ReadAllText(balancetxt);
                                                                        double moneyTotal = Int32.Parse(newString);
                                                                        double newTotal = moneyWin - moneyTotal;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                                        string moneyTotal1 = "";
                                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                        {
                                                                            Console.WriteLine("");
                                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                            break;
                                                                        }

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal == total1)
                                                                    {
                                                                        Console.WriteLine("Tie");
                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal <= 4)
                                                            {
                                                                int bankerThird = numbers[rnd.Next(0, 12)];
                                                                Console.WriteLine($"Your first card is {bankerThird}");
                                                                int bankerTotal1 = bankerTotal + bankerThird;
                                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                if (total1 > bankerTotal1)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);


                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 > total1)
                                                                {
                                                                    Console.WriteLine("Banker win");

                                                                    double value = 1.95;
                                                                    double moneyWin = betInt * value;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    double moneyTotal = Int32.Parse(newString);
                                                                    double newTotal = moneyWin - moneyTotal;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 == total1)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (total1 > bankerTotal)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);


                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal > total1)
                                                                {
                                                                    Console.WriteLine("Banker win");

                                                                    double value = 1.95;
                                                                    double moneyWin = betInt * value;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    double moneyTotal = Int32.Parse(newString);
                                                                    double newTotal = moneyWin - moneyTotal;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal == total1)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (newh == "stay")
                                                        {
                                                            if (bankerTotal <= 5)
                                                            {
                                                                int bankerThird = numbers[rnd.Next(0, 12)];
                                                                Console.WriteLine($"Your first card is {bankerThird}");
                                                                int bankerTotal1 = bankerTotal + bankerThird;
                                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                if (total > bankerTotal1)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 > total)
                                                                {
                                                                    Console.WriteLine("Banker win");

                                                                    double value = 1.95;
                                                                    double moneyWin = betInt * value;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    double moneyTotal = Int32.Parse(newString);
                                                                    double newTotal = moneyWin - moneyTotal;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 == total)
                                                                {
                                                                    Console.WriteLine("Tie");
                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Please pick one");
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (lowerchoice == "tie")
                                        {
                                            Console.WriteLine("What is your bet?");
                                            string num = "";

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
                                                else

                                                {
                                                    if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                    {
                                                        num = num.Substring(num.Length - 1);
                                                        Console.Write("\b \b");
                                                    }
                                                }
                                            }
                                            while (chr.Key != ConsoleKey.Enter);

                                            int betInt = Int32.Parse(num);
                                            int moneyInt = Int32.Parse(tmpbalance);
                                            int totalAfterBet = moneyInt - betInt;

                                            if (totalAfterBet < 0)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine("Bet is exceeds your current balance");
                                                Console.WriteLine("Please reset your money at the menu");
                                                goto Startover;
                                            }
                                            else
                                            {
                                                string newTotal = totalAfterBet.ToString();
                                                File.WriteAllText(balancetxt, newTotal);
                                                using StreamReader sr = File.OpenText(balancetxt);
                                                string moneyTotal = "";
                                                while ((moneyTotal = sr.ReadLine()) != null)
                                                {
                                                    Console.WriteLine("");
                                                    Console.WriteLine($"Your balance is now {moneyTotal}");
                                                    Console.WriteLine("CAUTION: if you leave while playing a ga,e that haven't ended yet you will lose your money");
                                                    break;
                                                }
                                            }

                                            Random rnd = new Random();
                                            int firstCard = numbers[rnd.Next(0, 12)];
                                            int bankerFirst = numbers[rnd.Next(0, 12)];
                                            int secoundCard = numbers[rnd.Next(0, 12)];
                                            int bankerSecound = numbers[rnd.Next(0, 12)];
                                            int total = firstCard + secoundCard;
                                            int bankerTotal = bankerFirst + bankerSecound;
                                            Console.WriteLine($"Your first card is {firstCard}");
                                            Console.WriteLine($"Your secound card is {secoundCard}");
                                            Console.WriteLine("");
                                            Console.WriteLine($"Bankers first card is {bankerFirst}");
                                            Console.WriteLine($"Bankers secound card is {bankerSecound}");

                                            if (total > 10)
                                            {
                                                string totalString = total.ToString();
                                                string total1 = totalString.Substring(1);
                                                int newTotal = Int32.Parse(total1);
                                                Console.WriteLine($"Your total is now {newTotal}");
                                            }

                                            if (bankerTotal > 10)
                                            {
                                                string bankerTotalString = bankerTotal.ToString();
                                                string bankerTotal1 = bankerTotalString.Substring(1);
                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");
                                                int newbankerTotal = Int32.Parse(bankerTotal1);
                                                Console.WriteLine($"Bankers total is now {newbankerTotal}");
                                            }
                                            else
                                            {
                                                if (total == 9)
                                                {
                                                    Console.WriteLine("Le gros!");

                                                    if (total > bankerTotal)
                                                    {
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto Startover;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");

                                                        int moneyWin = betInt * 8;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        int moneyTotal = Int32.Parse(newString);
                                                        int newTotal = moneyTotal - moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (bankerTotal == 9)
                                                {
                                                    Console.WriteLine("Le gros!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");

                                                        int moneyWin = betInt * 8;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        int moneyTotal = Int32.Parse(newString);
                                                        int newTotal = moneyTotal - moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (total == 8)
                                                {
                                                    Console.WriteLine("Le petit!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");

                                                        int moneyWin = betInt * 8;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        int moneyTotal = Int32.Parse(newString);
                                                        int newTotal = moneyTotal - moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (bankerTotal == 8)
                                                {
                                                    Console.WriteLine("Le petit!");

                                                    if (total > bankerTotal)
                                                    {
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine("You win " + moneyWin);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal > total)
                                                    {
                                                        Console.WriteLine("Banker win");

                                                        double value = 1.95;
                                                        double moneyWin = betInt * value;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        double moneyTotal = Int32.Parse(newString);
                                                        double newTotal = moneyWin - moneyTotal;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal == total)
                                                    {
                                                        Console.WriteLine("Tie");

                                                        int moneyWin = betInt * 8;
                                                        string newString = File.ReadAllText(balancetxt);
                                                        int moneyTotal = Int32.Parse(newString);
                                                        int newTotal = moneyTotal - moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                        string moneyTotal1 = "";
                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                        {
                                                            Console.WriteLine("");
                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                            break;
                                                        }

                                                        while (true)
                                                        {
                                                            Console.WriteLine("Would you like to play again? y/n");
                                                            char playagain = Console.ReadKey().KeyChar;

                                                            if (playagain == 'y')
                                                            {
                                                                goto baccaratStart;
                                                            }

                                                            if (playagain == 'n')
                                                            {
                                                                goto Start;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Press a valid key");
                                                            }
                                                        }
                                                    }
                                                }

                                                if (total <= 5)
                                                {
                                                    int thirdCard = numbers[rnd.Next(0, 12)];
                                                    Console.WriteLine($"Your first card is {thirdCard}");
                                                    int total1 = total + thirdCard;
                                                    Console.WriteLine($"Your total is now {total1}");

                                                    if (total1 > 10)
                                                    {
                                                        string totalString = total1.ToString();
                                                        string total2 = totalString.Substring(1);
                                                        int newTotal1 = Int32.Parse(total2);
                                                        Console.WriteLine($"Your total is now {newTotal1}");

                                                        if (bankerTotal <= 4)
                                                        {
                                                            int bankerThird = numbers[rnd.Next(0, 12)];
                                                            Console.WriteLine($"Your first card is {bankerThird}");
                                                            int bankerTotal1 = bankerTotal + bankerThird;
                                                            Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                            if (total1 > bankerTotal1)
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine("You win " + moneyWin);
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);


                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto Startover;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal1 > total1)
                                                            {
                                                                Console.WriteLine("Banker win");

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal1 == total1)
                                                            {
                                                                Console.WriteLine("Tie");

                                                                int moneyWin = betInt * 8;
                                                                string newString = File.ReadAllText(balancetxt);
                                                                int moneyTotal = Int32.Parse(newString);
                                                                int newTotal = moneyTotal - moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(balancetxt, newStringTotal);
                                                                using StreamReader sr = File.OpenText(balancetxt);
                                                                string moneyTotal1 = "";
                                                                while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                    break;
                                                                }

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (total1 > bankerTotal)
                                                            {
                                                                int moneyWin = betInt * 2;
                                                                Console.WriteLine("You win " + moneyWin);
                                                                string newString = File.ReadAllText(tmpbalancetxt);
                                                                int moneyTotalInt = Int32.Parse(newString);
                                                                int newTotal = moneyTotalInt + moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                try
                                                                {
                                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Console.WriteLine("Couldn't encrypt winning");
                                                                    Console.WriteLine("Error message: " + e);
                                                                    Thread.Sleep(500);
                                                                }
                                                                File.WriteAllText(balancetxt, balance);


                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto Startover;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal > total1)
                                                            {
                                                                Console.WriteLine("Banker win");

                                                                double value = 1.95;
                                                                double moneyWin = betInt * value;
                                                                string newString = File.ReadAllText(balancetxt);
                                                                double moneyTotal = Int32.Parse(newString);
                                                                double newTotal = moneyWin - moneyTotal;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(balancetxt, newStringTotal);
                                                                using StreamReader sr = File.OpenText(balancetxt);
                                                                string moneyTotal1 = "";
                                                                while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                    break;
                                                                }

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal == total1)
                                                            {
                                                                Console.WriteLine("Tie");

                                                                int moneyWin = betInt * 8;
                                                                string newString = File.ReadAllText(balancetxt);
                                                                int moneyTotal = Int32.Parse(newString);
                                                                int newTotal = moneyTotal - moneyWin;
                                                                string newStringTotal = newTotal.ToString();
                                                                File.WriteAllText(balancetxt, newStringTotal);
                                                                using StreamReader sr = File.OpenText(balancetxt);
                                                                string moneyTotal1 = "";
                                                                while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                {
                                                                    Console.WriteLine("");
                                                                    Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                    break;
                                                                }

                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Would you like to play again? y/n");
                                                                    char playagain = Console.ReadKey().KeyChar;

                                                                    if (playagain == 'y')
                                                                    {
                                                                        goto baccaratStart;
                                                                    }

                                                                    if (playagain == 'n')
                                                                    {
                                                                        goto Start;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Press a valid key");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (bankerTotal <= 4)
                                                    {
                                                        int bankerThird = numbers[rnd.Next(0, 12)];
                                                        Console.WriteLine($"Your first card is {bankerThird}");
                                                        int bankerTotal1 = bankerTotal + bankerThird;
                                                        Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                        if (total1 > bankerTotal1)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine("You win " + moneyWin);
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);


                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto Startover;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal1 > total1)
                                                        {
                                                            Console.WriteLine("Banker win");

                                                            double value = 1.95;
                                                            double moneyWin = betInt * value;
                                                            string newString = File.ReadAllText(balancetxt);
                                                            double moneyTotal = Int32.Parse(newString);
                                                            double newTotal = moneyWin - moneyTotal;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(balancetxt, newStringTotal);
                                                            using StreamReader sr = File.OpenText(balancetxt);
                                                            string moneyTotal1 = "";
                                                            while ((moneyTotal1 = sr.ReadLine()) != null)
                                                            {
                                                                Console.WriteLine("");
                                                                Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                break;
                                                            }

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal1 == total1)
                                                        {
                                                            Console.WriteLine("Tie");

                                                            int moneyWin = betInt * 8;
                                                            string newString = File.ReadAllText(balancetxt);
                                                            int moneyTotal = Int32.Parse(newString);
                                                            int newTotal = moneyTotal - moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(balancetxt, newStringTotal);
                                                            using StreamReader sr = File.OpenText(balancetxt);
                                                            string moneyTotal1 = "";
                                                            while ((moneyTotal1 = sr.ReadLine()) != null)
                                                            {
                                                                Console.WriteLine("");
                                                                Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                break;
                                                            }

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (total1 > bankerTotal)
                                                        {
                                                            int moneyWin = betInt * 2;
                                                            Console.WriteLine("You win " + moneyWin);
                                                            string newString = File.ReadAllText(tmpbalancetxt);
                                                            int moneyTotalInt = Int32.Parse(newString);
                                                            int newTotal = moneyTotalInt + moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                            try
                                                            {
                                                                byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Couldn't encrypt winning");
                                                                Console.WriteLine("Error message: " + e);
                                                                Thread.Sleep(500);
                                                            }
                                                            File.WriteAllText(balancetxt, balance);


                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto Startover;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal > total1)
                                                        {
                                                            Console.WriteLine("Banker win");

                                                            double value = 1.95;
                                                            double moneyWin = betInt * value;
                                                            string newString = File.ReadAllText(balancetxt);
                                                            double moneyTotal = Int32.Parse(newString);
                                                            double newTotal = moneyWin - moneyTotal;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(balancetxt, newStringTotal);
                                                            using StreamReader sr = File.OpenText(balancetxt);
                                                            string moneyTotal1 = "";
                                                            while ((moneyTotal1 = sr.ReadLine()) != null)
                                                            {
                                                                Console.WriteLine("");
                                                                Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                break;
                                                            }

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }

                                                        if (bankerTotal == total1)
                                                        {
                                                            Console.WriteLine("Tie");

                                                            int moneyWin = betInt * 8;
                                                            string newString = File.ReadAllText(balancetxt);
                                                            int moneyTotal = Int32.Parse(newString);
                                                            int newTotal = moneyTotal - moneyWin;
                                                            string newStringTotal = newTotal.ToString();
                                                            File.WriteAllText(balancetxt, newStringTotal);
                                                            using StreamReader sr = File.OpenText(balancetxt);
                                                            string moneyTotal1 = "";
                                                            while ((moneyTotal1 = sr.ReadLine()) != null)
                                                            {
                                                                Console.WriteLine("");
                                                                Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                break;
                                                            }

                                                            while (true)
                                                            {
                                                                Console.WriteLine("Would you like to play again? y/n");
                                                                char playagain = Console.ReadKey().KeyChar;

                                                                if (playagain == 'y')
                                                                {
                                                                    goto baccaratStart;
                                                                }

                                                                if (playagain == 'n')
                                                                {
                                                                    goto Start;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Press a valid key");
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (true)
                                                    {
                                                        Console.WriteLine("Do you want to hit or stay?");
                                                        string h = Console.ReadLine();
                                                        string newh = h.ToLower();

                                                        if (newh == "hit")
                                                        {
                                                            int thirdCard = numbers[rnd.Next(0, 12)];
                                                            Console.WriteLine($"Your third card is {thirdCard}");
                                                            int total1 = total + thirdCard;
                                                            Console.WriteLine($"Your total is now {total1}");

                                                            if (total1 > 10)
                                                            {
                                                                string totalString = total1.ToString();
                                                                string total2 = totalString.Substring(1);
                                                                int newTotal1 = Int32.Parse(total2);
                                                                Console.WriteLine($"Your total is now {newTotal1}");

                                                                if (bankerTotal <= 4)
                                                                {
                                                                    int bankerThird = numbers[rnd.Next(0, 12)];
                                                                    Console.WriteLine($"Your first card is {bankerThird}");
                                                                    int bankerTotal1 = bankerTotal + bankerThird;
                                                                    Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                    if (total1 > bankerTotal1)
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine("You win " + moneyWin);
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto Startover;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal1 > total1)
                                                                    {
                                                                        Console.WriteLine("Banker win");

                                                                        double value = 1.95;
                                                                        double moneyWin = betInt * value;
                                                                        string newString = File.ReadAllText(balancetxt);
                                                                        double moneyTotal = Int32.Parse(newString);
                                                                        double newTotal = moneyWin - moneyTotal;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                                        string moneyTotal1 = "";
                                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                        {
                                                                            Console.WriteLine("");
                                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                            break;
                                                                        }

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal1 == total1)
                                                                    {
                                                                        Console.WriteLine("Tie");

                                                                        int moneyWin = betInt * 8;
                                                                        string newString = File.ReadAllText(balancetxt);
                                                                        int moneyTotal = Int32.Parse(newString);
                                                                        int newTotal = moneyTotal + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                                        string moneyTotal1 = "";
                                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                        {
                                                                            Console.WriteLine("");
                                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                            break;
                                                                        }

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (total1 > bankerTotal)
                                                                    {
                                                                        int moneyWin = betInt * 2;
                                                                        Console.WriteLine("You win " + moneyWin);
                                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                                        int moneyTotalInt = Int32.Parse(newString);
                                                                        int newTotal = moneyTotalInt + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                        try
                                                                        {
                                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Console.WriteLine("Couldn't encrypt winning");
                                                                            Console.WriteLine("Error message: " + e);
                                                                            Thread.Sleep(500);
                                                                        }
                                                                        File.WriteAllText(balancetxt, balance);

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto Startover;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal > total1)
                                                                    {
                                                                        Console.WriteLine("Banker win");

                                                                        double value = 1.95;
                                                                        double moneyWin = betInt * value;
                                                                        string newString = File.ReadAllText(balancetxt);
                                                                        double moneyTotal = Int32.Parse(newString);
                                                                        double newTotal = moneyWin - moneyTotal;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                                        string moneyTotal1 = "";
                                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                        {
                                                                            Console.WriteLine("");
                                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                            break;
                                                                        }

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }

                                                                    if (bankerTotal == total1)
                                                                    {
                                                                        Console.WriteLine("Tie");

                                                                        int moneyWin = betInt * 8;
                                                                        string newString = File.ReadAllText(balancetxt);
                                                                        int moneyTotal = Int32.Parse(newString);
                                                                        int newTotal = moneyTotal + moneyWin;
                                                                        string newStringTotal = newTotal.ToString();
                                                                        File.WriteAllText(balancetxt, newStringTotal);
                                                                        using StreamReader sr = File.OpenText(balancetxt);
                                                                        string moneyTotal1 = "";
                                                                        while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                        {
                                                                            Console.WriteLine("");
                                                                            Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                            break;
                                                                        }

                                                                        while (true)
                                                                        {
                                                                            Console.WriteLine("Would you like to play again? y/n");
                                                                            char playagain = Console.ReadKey().KeyChar;

                                                                            if (playagain == 'y')
                                                                            {
                                                                                goto baccaratStart;
                                                                            }

                                                                            if (playagain == 'n')
                                                                            {
                                                                                goto Start;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Press a valid key");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            if (bankerTotal <= 4)
                                                            {
                                                                int bankerThird = numbers[rnd.Next(0, 12)];
                                                                Console.WriteLine($"Your first card is {bankerThird}");
                                                                int bankerTotal1 = bankerTotal + bankerThird;
                                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                if (total1 > bankerTotal1)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);


                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 > total1)
                                                                {
                                                                    Console.WriteLine("Banker win");

                                                                    double value = 1.95;
                                                                    double moneyWin = betInt * value;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    double moneyTotal = Int32.Parse(newString);
                                                                    double newTotal = moneyWin - moneyTotal;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 == total1)
                                                                {
                                                                    Console.WriteLine("Tie");

                                                                    int moneyWin = betInt * 8;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    int moneyTotal = Int32.Parse(newString);
                                                                    int newTotal = moneyTotal + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (total1 > bankerTotal)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal > total1)
                                                                {
                                                                    Console.WriteLine("Banker win");

                                                                    double value = 1.95;
                                                                    double moneyWin = betInt * value;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    double moneyTotal = Int32.Parse(newString);
                                                                    double newTotal = moneyWin - moneyTotal;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal == total1)
                                                                {
                                                                    Console.WriteLine("Tie");

                                                                    int moneyWin = betInt * 8;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    int moneyTotal = Int32.Parse(newString);
                                                                    int newTotal = moneyTotal + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (newh == "stay")
                                                        {
                                                            if (bankerTotal <= 5)
                                                            {
                                                                int bankerThird = numbers[rnd.Next(0, 12)];
                                                                Console.WriteLine($"Your first card is {bankerThird}");
                                                                int bankerTotal1 = bankerTotal + bankerThird;
                                                                Console.WriteLine($"Bankers total is now {bankerTotal1}");

                                                                if (total > bankerTotal1)
                                                                {
                                                                    int moneyWin = betInt * 2;
                                                                    Console.WriteLine("You win " + moneyWin);
                                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                                    int moneyTotalInt = Int32.Parse(newString);
                                                                    int newTotal = moneyTotalInt + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                                    try
                                                                    {
                                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Console.WriteLine("Couldn't encrypt winning");
                                                                        Console.WriteLine("Error message: " + e);
                                                                        Thread.Sleep(500);
                                                                    }
                                                                    File.WriteAllText(balancetxt, balance);

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto Startover;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 > total)
                                                                {
                                                                    Console.WriteLine("Banker win");

                                                                    double value = 1.95;
                                                                    double moneyWin = betInt * value;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    double moneyTotal = Int32.Parse(newString);
                                                                    double newTotal = moneyWin - moneyTotal;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }

                                                                if (bankerTotal1 == total)
                                                                {
                                                                    Console.WriteLine("Tie");

                                                                    int moneyWin = betInt * 8;
                                                                    string newString = File.ReadAllText(balancetxt);
                                                                    int moneyTotal = Int32.Parse(newString);
                                                                    int newTotal = moneyTotal + moneyWin;
                                                                    string newStringTotal = newTotal.ToString();
                                                                    File.WriteAllText(balancetxt, newStringTotal);
                                                                    using StreamReader sr = File.OpenText(balancetxt);
                                                                    string moneyTotal1 = "";
                                                                    while ((moneyTotal1 = sr.ReadLine()) != null)
                                                                    {
                                                                        Console.WriteLine("");
                                                                        Console.WriteLine($"Your balance is now {moneyTotal1}");
                                                                        break;
                                                                    }

                                                                    while (true)
                                                                    {
                                                                        Console.WriteLine("Would you like to play again? y/n");
                                                                        char playagain = Console.ReadKey().KeyChar;

                                                                        if (playagain == 'y')
                                                                        {
                                                                            goto baccaratStart;
                                                                        }

                                                                        if (playagain == 'n')
                                                                        {
                                                                            goto Start;
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Press a valid key");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Please pick one");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Please choose one");
                                            goto baccaratStart;
                                        }
                                    }
                                    break;

                                case '5':
                                    {
                                    SlotStart:
                                        Console.Clear();
                                        Console.WriteLine("Welcome to Slots");

                                    bet:
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
                                            else

                                            {
                                                if (chr.Key == ConsoleKey.Backspace && num.Length > 0)
                                                {
                                                    num = num.Substring(num.Length - 1);
                                                    Console.Write("\b \b");
                                                }
                                            }
                                        }

                                        while (chr.Key != ConsoleKey.Enter);

                                        // Get balance from balance file
                                        try
                                        {
                                            byte[] balance_byte = Convert.FromBase64String(HttpUtility.UrlDecode(data));
                                            tmpbalance = Encoding.UTF8.GetString(balance_byte);
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("Error while decoding balance");
                                            Thread.Sleep(500);
                                            goto Startover;
                                        }

                                        int betInt = Int32.Parse(num);
                                        int moneyInt = Int32.Parse(tmpbalance);
                                        int totalAfterBet = moneyInt - betInt;

                                        // if the bet is above what user has, send user back
                                        if (totalAfterBet < 0)
                                        {
                                            Console.WriteLine("");
                                            Console.WriteLine("Bet is exceeds your current balance");
                                            Console.WriteLine("Please reset your money at the menu");
                                            Thread.Sleep(2000);
                                            goto SlotStart;
                                        }

                                        // if bet isnt above total amount then subtract bet from total and put in file
                                        else
                                        {
                                            string newTotal = totalAfterBet.ToString();
                                            File.WriteAllText(tmpbalancetxt, newTotal);
                                            //Encrypt balance
                                            try
                                            {
                                                byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                            }

                                            catch (Exception e)
                                            {
                                                Console.WriteLine("Encrypting went wrong");
                                                Console.WriteLine("Error message: " + e);
                                                Thread.Sleep(500);
                                                goto Startover;
                                            }
                                            using StreamReader sr = File.OpenText(tmpbalancetxt);
                                            string moneyTotal = "";
                                            while ((moneyTotal = sr.ReadLine()) != null)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine($"Your balance is now {moneyTotal}");
                                                Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                                Thread.Sleep(5000);
                                                break;
                                            }
                                        }

                                        Console.Clear();

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

                                        string message = "Press ESC to go back, Press Backspace to change bet";
                                        Console.WriteLine(message);

                                        Console.WriteLine("***************   X-X = 3x                                                   ");
                                        Console.WriteLine("*             *   X-X-Y-Y = 5x                                               ");
                                        Console.WriteLine("*             *   X-X-X = 10x                                                ");
                                        Console.WriteLine("*             *   X-X-X-Y-Y = 50x                                            ");
                                        Console.WriteLine("*             *   X-X-X-X = 100x                                             ");
                                        Console.WriteLine("*             *   X-X-X-X-X = 1000x                                          ");
                                        Console.WriteLine("*             *                                                              ");
                                        Console.WriteLine("***************   For each win you will get a jackpot spin and 100x your win!");

                                        Random rndicons = new Random();

                                        while (chr.Key != ConsoleKey.Escape)
                                        {
                                            ConsoleKeyInfo _char;
                                            Console.SetCursorPosition(0, 9);

                                            betInt = Int32.Parse(num);
                                            moneyInt = Int32.Parse(tmpbalance);
                                            totalAfterBet = moneyInt - betInt;
                                            string balanceTotal = "";

                                            // if the bet is above what user has, send user back
                                            if (totalAfterBet < 0)
                                            {
                                                Console.WriteLine("");
                                                Console.WriteLine("Bet is exceeds your current balance");
                                                Console.WriteLine("Please reset your money at the menu");
                                                Thread.Sleep(2000);
                                                goto SlotStart;
                                            }

                                            // if bet isnt above total amount then subtract bet from total and put in file
                                            else
                                            {
                                                string newTotal = totalAfterBet.ToString();
                                                File.WriteAllText(tmpbalancetxt, newTotal);
                                                //Encrypt balance
                                                try
                                                {
                                                    byte[] ba_byte = Encoding.UTF8.GetBytes(newTotal);
                                                    accounts.Balance = HttpUtility.UrlEncode(Convert.ToBase64String(ba_byte));
                                                }

                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Encrypting went wrong");
                                                    Console.WriteLine("Error message: " + e);
                                                    Thread.Sleep(500);
                                                    goto Startover;
                                                }

                                                using StreamReader sr = File.OpenText(tmpbalancetxt);
                                                balanceTotal = sr.ReadLine();
                                            }

                                            Console.WriteLine("Current balance: " + balanceTotal);

                                            string firstIcon = "";
                                            string secoundIcon = "";
                                            string thirdIcon = "";
                                            string fourthIcon = "";
                                            string fifthIcon = "";

                                            Console.SetCursorPosition(0, 4);
                                            Console.Write("*  ");
                                            Thread.Sleep(200);
                                            for (int i = 0; i < 5; i++)
                                            {
                                                Console.SetCursorPosition(3, 4);
                                                firstIcon = icons[rndicons.Next(0, 10)];
                                                Console.Write(firstIcon);
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(100);
                                            Console.Write(" ");

                                            for (int i = 0; i < 5; i++)
                                            {
                                                Console.SetCursorPosition(5, 4);
                                                secoundIcon = icons[rndicons.Next(0, 10)];
                                                Console.Write(secoundIcon);
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(100);
                                            Console.Write(" ");

                                            for (int i = 0; i < 5; i++)
                                            {
                                                Console.SetCursorPosition(7, 4);
                                                thirdIcon = icons[rndicons.Next(0, 10)];
                                                Console.Write(thirdIcon);
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(100);
                                            Console.WriteLine(" ");

                                            for (int i = 0; i < 5; i++)
                                            {
                                                Console.SetCursorPosition(9, 4);
                                                fourthIcon = icons[rndicons.Next(0, 10)];
                                                Console.Write(fourthIcon);
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(100);
                                            Console.Write(" ");

                                            for (int i = 0; i < 5; i++)
                                            {
                                                Console.SetCursorPosition(11, 4);
                                                fifthIcon = icons[rndicons.Next(0, 10)];
                                                Console.Write(fifthIcon);
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(500);

                                            Random jackpotNumber = new Random();
                                            Random userNumber = new Random();
                                            int userNum = 0;
                                            int jackpotNum = 0;

                                            //X-X-X-X-X
                                            if (firstIcon == secoundIcon && secoundIcon == thirdIcon && thirdIcon == fourthIcon && fourthIcon == fifthIcon)
                                            {
                                                for (int i = 0; i < 24; i++)
                                                {
                                                    userNum = userNumber.Next(0, 99) + 1;
                                                    Console.SetCursorPosition(0, 10);
                                                    Console.Write("Your number is: " + userNum);
                                                    Thread.Sleep(100);
                                                }

                                                for (int i = 0; i < 24; i++)
                                                {
                                                    jackpotNum = jackpotNumber.Next(0, 99) + 1;
                                                    Console.SetCursorPosition(0, 11);
                                                    Console.Write("The Jackpot number is: " + jackpotNum);
                                                    Thread.Sleep(100);
                                                }
                                                Thread.Sleep(1000);

                                                Console.WriteLine("");

                                                if (userNum == jackpotNum)
                                                {
                                                    Console.WriteLine("Congratulations! You won the jackpot!");
                                                    int tmpmoneyWin = betInt * 1000;
                                                    int moneyWin = tmpmoneyWin * 100;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    Thread.Sleep(1000);
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);
                                                    goto SpinAgain;
                                                }

                                                if (userNum != jackpotNum)
                                                {
                                                    Console.WriteLine("Unfortunate! You lost the jackpot!");
                                                    int moneyWin = betInt * 1000;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    Thread.Sleep(1000);
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);
                                                    goto SpinAgain;
                                                }
                                            }

                                            //X-X-X-X-Y
                                            else if (firstIcon == secoundIcon && secoundIcon == thirdIcon && thirdIcon == fourthIcon)
                                            {
                                                for (int i = 0; i < 24; i++)
                                                {
                                                    userNum = userNumber.Next(0, 99) + 1;
                                                    Console.SetCursorPosition(0, 10);
                                                    Console.Write("Your number is: " + userNum);
                                                    Thread.Sleep(100);
                                                }

                                                for (int i = 0; i < 24; i++)
                                                {
                                                    jackpotNum = jackpotNumber.Next(0, 99) + 1;
                                                    Console.SetCursorPosition(0, 11);
                                                    Console.Write("The Jackpot number is: " + jackpotNum);
                                                    Thread.Sleep(100);
                                                }
                                                Thread.Sleep(1000);

                                                Console.WriteLine("");

                                                if (userNum == jackpotNum)
                                                {
                                                    Console.WriteLine("Congratulations! You won the jackpot!");
                                                    int tmpmoneyWin = betInt * 100;
                                                    int moneyWin = tmpmoneyWin * 100;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    Thread.Sleep(1000);
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);
                                                    goto SpinAgain;
                                                }

                                                if (userNum != jackpotNum)
                                                {
                                                    Console.WriteLine("Unfortunate! You lost the jackpot!");
                                                    int moneyWin = betInt * 100;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    Thread.Sleep(1000);
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);
                                                    goto SpinAgain;
                                                }
                                            }

                                            //X-X-X-Y-Y
                                            else if (firstIcon == secoundIcon && secoundIcon == thirdIcon)
                                            {
                                                if (fourthIcon == fifthIcon)
                                                {
                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        userNum = userNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 10);
                                                        Console.Write("Your number is: " + userNum);
                                                        Thread.Sleep(100);
                                                    }

                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        jackpotNum = jackpotNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 11);
                                                        Console.Write("The Jackpot number is: " + jackpotNum);
                                                        Thread.Sleep(100);
                                                    }
                                                    Thread.Sleep(1000);

                                                    Console.WriteLine("");

                                                    if (userNum == jackpotNum)
                                                    {
                                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                                        int tmpmoneyWin = betInt * 50;
                                                        int moneyWin = tmpmoneyWin * 100;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }

                                                    if (userNum != jackpotNum)
                                                    {
                                                        Console.WriteLine("Unfortunate! You lost the jackpot!");
                                                        int moneyWin = betInt * 50;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }
                                                }

                                                else
                                                {
                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        userNum = userNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 10);
                                                        Console.Write("Your number is: " + userNum);
                                                        Thread.Sleep(100);
                                                    }

                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        jackpotNum = jackpotNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 11);
                                                        Console.Write("The Jackpot number is: " + jackpotNum);
                                                        Thread.Sleep(100);
                                                    }
                                                    Thread.Sleep(1000);

                                                    Console.WriteLine("");

                                                    if (userNum == jackpotNum)
                                                    {
                                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                                        int tmpmoneyWin = betInt * 10;
                                                        int moneyWin = tmpmoneyWin * 100;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }

                                                    if (userNum != jackpotNum)
                                                    {
                                                        Console.WriteLine("Unfortunate! You lost the jackpot!");
                                                        int moneyWin = betInt * 10;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }
                                                }
                                            }

                                            //X-X-X-Y-Z
                                            else if (firstIcon == secoundIcon && secoundIcon == thirdIcon)
                                            {
                                                for (int i = 0; i < 24; i++)
                                                {
                                                    userNum = userNumber.Next(0, 99) + 1;
                                                    Console.SetCursorPosition(0, 10);
                                                    Console.Write("Your number is: " + userNum);
                                                    Thread.Sleep(100);
                                                }

                                                for (int i = 0; i < 24; i++)
                                                {
                                                    jackpotNum = jackpotNumber.Next(0, 99) + 1;
                                                    Console.SetCursorPosition(0, 11);
                                                    Console.Write("The Jackpot number is: " + jackpotNum);
                                                    Thread.Sleep(100);
                                                }
                                                Thread.Sleep(1000);

                                                Console.WriteLine("");

                                                if (userNum == jackpotNum)
                                                {
                                                    Console.WriteLine("Congratulations! You won the jackpot!");
                                                    int tmpmoneyWin = betInt * 10;
                                                    int moneyWin = tmpmoneyWin * 100;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    Thread.Sleep(1000);
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);
                                                    goto SpinAgain;
                                                }

                                                if (userNum != jackpotNum)
                                                {
                                                    Console.WriteLine("Unfortunate! You lost the jackpot!");
                                                    int moneyWin = betInt * 10;
                                                    Console.WriteLine($"You win {moneyWin}");
                                                    Thread.Sleep(1000);
                                                    string newString = File.ReadAllText(tmpbalancetxt);
                                                    int moneyTotalInt = Int32.Parse(newString);
                                                    int newTotal = moneyTotalInt + moneyWin;
                                                    string newStringTotal = newTotal.ToString();
                                                    File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                    try
                                                    {
                                                        byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                        balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Couldn't encrypt winning");
                                                        Console.WriteLine("Error message: " + e);
                                                        Thread.Sleep(500);
                                                    }
                                                    File.WriteAllText(balancetxt, balance);
                                                    goto SpinAgain;
                                                }
                                            }

                                            //X-X-Y-Y-Z
                                            else if (firstIcon == secoundIcon)
                                            {
                                                if (thirdIcon == fourthIcon)
                                                {
                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        userNum = userNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 10);
                                                        Console.Write("Your number is: " + userNum);
                                                        Thread.Sleep(100);
                                                    }

                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        jackpotNum = jackpotNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 11);
                                                        Console.Write("The Jackpot number is: " + jackpotNum);
                                                        Thread.Sleep(100);
                                                    }
                                                    Thread.Sleep(1000);

                                                    Console.WriteLine("");

                                                    if (userNum == jackpotNum)
                                                    {
                                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                                        int tmpmoneyWin = betInt * 5;
                                                        int moneyWin = tmpmoneyWin * 100;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }

                                                    if (userNum != jackpotNum)
                                                    {
                                                        Console.WriteLine("Unfortunate! You lost the jackpot!");
                                                        int moneyWin = betInt * 5;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }
                                                }

                                                else
                                                {
                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        userNum = userNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 10);
                                                        Console.Write("Your number is: " + userNum);
                                                        Thread.Sleep(100);
                                                    }

                                                    for (int i = 0; i < 24; i++)
                                                    {
                                                        jackpotNum = jackpotNumber.Next(0, 99) + 1;
                                                        Console.SetCursorPosition(0, 11);
                                                        Console.Write("The Jackpot number is: " + jackpotNum);
                                                        Thread.Sleep(100);
                                                    }
                                                    Thread.Sleep(1000);

                                                    Console.WriteLine("");

                                                    if (userNum == jackpotNum)
                                                    {
                                                        Console.WriteLine("Congratulations! You won the jackpot!");
                                                        int tmpmoneyWin = betInt * 2;
                                                        int moneyWin = tmpmoneyWin * 100;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }

                                                    if (userNum != jackpotNum)
                                                    {
                                                        Console.WriteLine("Unfortunate! You lost the jackpot!");
                                                        int moneyWin = betInt * 2;
                                                        Console.WriteLine($"You win {moneyWin}");
                                                        Thread.Sleep(1000);
                                                        string newString = File.ReadAllText(tmpbalancetxt);
                                                        int moneyTotalInt = Int32.Parse(newString);
                                                        int newTotal = moneyTotalInt + moneyWin;
                                                        string newStringTotal = newTotal.ToString();
                                                        File.WriteAllText(tmpbalancetxt, newStringTotal);
                                                        try
                                                        {
                                                            byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(newStringTotal);
                                                            balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Couldn't encrypt winning");
                                                            Console.WriteLine("Error message: " + e);
                                                            Thread.Sleep(500);
                                                        }
                                                        File.WriteAllText(balancetxt, balance);
                                                        goto SpinAgain;
                                                    }
                                                }
                                            }

                                            SpinAgain:
                                            Console.SetCursorPosition(0, 14);
                                            Console.WriteLine("Press anything to spin again");
                                            _char = Console.ReadKey();
                                            if (_char.Key == ConsoleKey.Backspace)
                                            {
                                                goto bet;
                                            }

                                            if (_char.Key == ConsoleKey.Escape)
                                            {
                                                goto Startover;
                                            }

                                            else
                                            {
                                                Console.SetCursorPosition(0, 9);
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                                Console.WriteLine("                                                                       ");
                                            }
                                        }
                                    }
                                    break;

                                case '6':
                                    {
                                        try
                                        {
                                            if (File.Exists(balancetxt))
                                            {
                                                File.Delete(balancetxt);
                                                string stringMoney = "1000";
                                                File.WriteAllText(tmpbalancetxt, stringMoney);
                                                try
                                                {
                                                    byte[] tmpbalance_byte = Encoding.UTF8.GetBytes(stringMoney);
                                                    balance = HttpUtility.UrlEncode(Convert.ToBase64String(tmpbalance_byte));
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Couldn't encrypt new balance");
                                                    Console.WriteLine("Error message: " + e);
                                                    Thread.Sleep(500);
                                                }
                                                File.WriteAllText(balancetxt, balance);
                                            }
                                        }
                                        catch (Exception Ex)
                                        {
                                            Console.WriteLine(Ex.ToString());
                                        }

                                        using (StreamReader sr = File.OpenText(balancetxt))
                                        {
                                            while ((balance = sr.ReadLine()) != null)
                                            {
                                                Console.WriteLine($"Your balance is currently {balance}");
                                            }
                                        }
                                        goto Startover;
                                    }

                                case '0':
                                    {
                                        goto Start;
                                    }
                            }
                        }
                    }

                case '6':
                    {
                    SpecStart:
                        Console.Clear();
                        Console.WriteLine("Here is everything about your PC");
                        Console.WriteLine("***********************************");

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("1: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("OS: ");
                        ManagementObjectSearcher os = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                        foreach (ManagementObject win in os.Get())
                        {
                            string wintype = win["Caption"].ToString();
                            Console.WriteLine(wintype);
                            Console.WriteLine("");
                        }

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("2: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("CPU: ");
                        var cpu = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                        foreach (ManagementObject mo in cpu.Get())
                        {
                            Console.WriteLine(mo["Name"]);
                            string cpuid = mo["ProcessorId"].ToString();
                            Console.WriteLine("ID: " + cpuid);
                            Console.WriteLine("");
                        }

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("3: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("GPU: ");
                        ManagementObjectSearcher videocontroller = new ManagementObjectSearcher("select * from Win32_VideoController");
                        foreach (ManagementObject videocard in videocontroller.Get())
                        {
                            Console.WriteLine(videocard["Name"]);
                            string gpuid = "";
                            gpuid = videocard["DeviceID"].ToString();
                            Console.WriteLine("ID: " + gpuid);
                            Console.WriteLine("");
                        }

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("4: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Motherboard: ");
                        ManagementObjectSearcher motherboard = new ManagementObjectSearcher("select * from Win32_BaseBoard");
                        foreach (ManagementObject modbinfo in motherboard.Get())
                        {
                            Console.WriteLine(modbinfo["Product"]);
                            string modbid = "";
                            modbid = modbinfo["SerialNumber"].ToString();
                            Console.WriteLine("ID: " + modbid);
                            Console.WriteLine("");
                        }

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("5: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("RAM: ");
                        ManagementObjectSearcher ram = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
                        foreach (ManagementObject queryObj in ram.Get())
                        {
                            Console.WriteLine("Slot: {0}; Capacity: {1}Gb; Speed: {2}; Manufacturer: {3}", queryObj["BankLabel"],
                                              Math.Round(Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2),
                                               queryObj["Speed"], queryObj["Name"]);
                        }
                        Console.WriteLine("");

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("6: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Disks: ");
                        ManagementObjectSearcher dsk = new ManagementObjectSearcher("select * from Win32_LogicalDisk");
                        foreach (ManagementObject disk in dsk.Get())
                        {
                            string manu = disk["Caption"].ToString();
                            Console.WriteLine("Disks: " + manu);
                            Console.WriteLine("");
                        }

                        Console.Write("0: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Go back");
                        Console.ForegroundColor = ConsoleColor.DarkRed;

                        Console.WriteLine("Press a number between 1-6 to get more info");

                        char info = Console.ReadKey().KeyChar;
                        Console.Clear();

                        switch (info)
                        {
                            default:
                                {
                                    Console.WriteLine("Please press a valid key");
                                }
                                break;

                            case '1':
                                {
                                    foreach (ManagementObject win in os.Get())
                                    {
                                        try
                                        {
                                            string bn = win["BuildNumber"].ToString();
                                            string type = win["OSType"].ToString();
                                            string ver = win["Version"].ToString();
                                            string cap = win["Caption"].ToString();
                                            Console.WriteLine("OSType: " + type);
                                            Console.WriteLine("Version: " + ver);
                                            Console.WriteLine("OS Name: " + cap);
                                            Console.WriteLine("Build Number: " + bn);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }

                                    Console.WriteLine("");
                                    Console.WriteLine("Press any key to go back to the menu");
                                    Console.ReadKey();
                                    goto SpecStart;
                                }

                            case '2':
                                {
                                    foreach (ManagementObject cpuinfo in cpu.Get())
                                    {
                                        try
                                        {
                                            string manu = cpuinfo["Manufacturer"].ToString();
                                            string name = cpuinfo["Name"].ToString();
                                            string cpuid = cpuinfo["ProcessorId"].ToString();
                                            string cores = cpuinfo["NumberOfCores"].ToString();
                                            string speed = cpuinfo["MaxClockSpeed"].ToString();
                                            Console.WriteLine("Manufacturer: " + manu);
                                            Console.WriteLine("CPU: " + name);
                                            Console.WriteLine("ID: " + cpuid);
                                            Console.WriteLine("Cores: " + cores);
                                            Console.WriteLine("MHz: " + speed);

                                            Console.WriteLine("");
                                        }

                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }

                                    Console.WriteLine("Press any key to go back to the menu");
                                    Console.ReadKey();
                                    goto SpecStart;
                                }

                            case '3':
                                {
                                    foreach (ManagementObject videocard in videocontroller.Get())
                                    {
                                        try
                                        {
                                            string name = videocard["Name"].ToString();
                                            string sysname = videocard["SystemName"].ToString();
                                            string id = videocard["DeviceID"].ToString();
                                            string curref = videocard["CurrentRefreshrate"].ToString();
                                            string driver = videocard["DriverVersion"].ToString();

                                            Console.WriteLine("Graphiccard: " + name);
                                            Console.WriteLine("System Name: " + sysname);
                                            Console.WriteLine("ID: " + id);
                                            Console.WriteLine("Driver Version: " + driver);
                                            Console.WriteLine("Current refreshrate: " + curref);

                                            Console.WriteLine("");
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }

                                    Console.WriteLine("Press any key to go back to the menu");
                                    Console.ReadKey();
                                    goto SpecStart;
                                }

                            case '4':
                                {
                                    foreach (ManagementObject modbinfo in motherboard.Get())
                                    {
                                        try
                                        {
                                            string prod = modbinfo["Product"].ToString();

                                            Console.WriteLine("Name:" + prod);

                                            Console.WriteLine("");
                                        }
                                        catch (Exception)
                                        {
                                        }

                                    }

                                    Console.WriteLine("Press any key to go back to the menu");
                                    Console.ReadKey();
                                    goto SpecStart;
                                }

                            case '5':
                                {
                                    foreach (ManagementObject raminfo in ram.Get())
                                    {
                                        try
                                        {
                                            string slot = raminfo["BankLabel"].ToString();
                                            string manu = raminfo["Manufacturer"].ToString();
                                            string speed = raminfo["Speed"].ToString();
                                            string serial = raminfo["Name"].ToString();
                                            double cap = Math.Round(Convert.ToDouble(raminfo["Capacity"]) / 1024 / 1024 / 1024, 2);

                                            Console.WriteLine("Manufacturer: " + manu);
                                            Console.WriteLine("Size: " + cap);
                                            Console.WriteLine("Speed: " + speed);
                                            Console.WriteLine("Size: " + cap);
                                            Console.WriteLine("Serial: " + serial);
                                            Console.Write("Slot: " + slot);

                                            Console.WriteLine("");
                                        }
                                        catch (Exception)
                                        { 

                                        }
                                    }

                                    Console.WriteLine("Press any key to go back to the menu");
                                    Console.ReadKey();
                                    goto SpecStart;
                                }

                            case '6':
                                {
                                    foreach (ManagementObject disk in dsk.Get())
                                    {
                                        try
                                        {
                                            string manu = disk["Caption"].ToString();
                                            string hddid = disk["VolumeSerialNumber"].ToString();
                                            double newsize = Math.Round(Convert.ToDouble(disk["Size"]) / 1024 / 1024 / 1024, 2);
                                            Console.WriteLine("Disks: " + manu);
                                            Console.Write(disk["Name"] + " = ");
                                            Console.WriteLine(newsize + "GB");
                                            Console.WriteLine("ID: " + hddid);
                                            Console.WriteLine("");
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }

                                    Console.WriteLine("Press any key to go back to the menu");
                                    Console.ReadKey();
                                    goto SpecStart;
                                }

                            case '0':
                                {
                                    goto Start;
                                }
                        }

                    }
                    break;

                case '7':
                    {
                        Settings:
                        Console.Clear();
                        string logins = root;
                        Console.Clear();
                        Console.WriteLine("What do you want to change?");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("1: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Change presets");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("2: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Change password");
                        Console.Write("0: ");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Go back");

                        char decision = Console.ReadKey().KeyChar;
                        Console.Clear();

                        if(decision == '1')
                        {
                            Presets:
                            Console.WriteLine("Which preset has to be changed?");
                            Console.Clear();
                            Console.WriteLine("What do you want to change?");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("1: ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Preset 1");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("2: ");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("Preset 2");
                            Console.Write("3: ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Preset 3");
                            Console.Write("0: ");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("Go back");

                            char decision1 = Console.ReadKey().KeyChar;
                            Console.Clear();

                            if (decision1 == '1')
                            {
                                Console.WriteLine("What program do you want as shortcut?");
                                string program = Console.ReadLine();
                                string exe = "";
                                if (!program.EndsWith(".exe"))
                                {
                                    exe = program;
                                }
                                if (program.EndsWith(".exe"))
                                {
                                    program.Substring(program.Length - 4);
                                    exe = program;
                                }
                                File.WriteAllText(presetname_1, exe);

                                Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
                                string address = Console.ReadLine();
                                File.WriteAllText(presetaddress_1, address);
                                Console.WriteLine("Preset has been set");
                                Thread.Sleep(1000);
                                goto Start;
                            }

                            if (decision1 == '2')
                            {
                                Console.WriteLine("What program do you want as shortcut?");
                                string program = Console.ReadLine();
                                string exe = "";
                                if (!program.EndsWith(".exe"))
                                {
                                    exe = program;
                                }
                                if (program.EndsWith(".exe"))
                                {
                                    program.Substring(program.Length - 4);
                                    exe = program;
                                }
                                File.WriteAllText(presetname_2, exe);

                                Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
                                string address = Console.ReadLine();
                                File.WriteAllText(presetaddress_2, address);
                                Console.WriteLine("Preset has been set");
                                Thread.Sleep(1000);
                                goto Start;
                            }

                            if (decision1 == '3')
                            {
                                Console.WriteLine("What program do you want as shortcut?");
                                string program = Console.ReadLine();
                                string exe = "";
                                if (!program.EndsWith(".exe"))
                                {
                                    exe = program;
                                }
                                if (program.EndsWith(".exe"))
                                {
                                    program.Substring(program.Length - 4);
                                    exe = program;
                                }
                                File.WriteAllText(presetname_3, exe);

                                Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
                                string address = Console.ReadLine();
                                File.WriteAllText(presetaddress_3, address);
                                Console.WriteLine("Preset has been set");
                                Thread.Sleep(1000);
                                goto Start;
                            }

                            if (decision1 == '0')
                            {
                                Thread.Sleep(200);
                                goto Start;
                            }

                            else
                            {
                                Console.WriteLine("Please enter a valid key");
                                Thread.Sleep(1000);
                                goto Presets;
                            }
                        }

                        if (decision == '2')
                        {
                            Console.WriteLine("What do you want to change your password to?");
                            Console.Write("Old password: ");
                            string oldpw = "";

                            //Replaces input with '*'
                            ConsoleKeyInfo input = Console.ReadKey(true);
                            while (input.Key != ConsoleKey.Enter)
                            {
                                if (input.Key != ConsoleKey.Backspace)
                                {
                                    Console.Write("*");
                                    oldpw += input.KeyChar;
                                }
                                else if (input.Key == ConsoleKey.Backspace)
                                {
                                    if (!string.IsNullOrEmpty(oldpw))
                                    {
                                        // remove one character from the list of password characters
                                        oldpw = oldpw.Substring(0, oldpw.Length - 1);
                                        // get the location of the cursor
                                        int pos = Console.CursorLeft;
                                        // move the cursor to the left by one character
                                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                                        // replace it with space
                                        Console.Write(" ");
                                        // move the cursor to the left by one character again
                                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                                    }
                                }
                                input = Console.ReadKey(true);
                            }

                            string pw = oldpw;
                            try
                            {
                                byte[] pw_byte = Encoding.UTF8.GetBytes(pw);
                                oldpw = HttpUtility.UrlEncode(Convert.ToBase64String(pw_byte));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Couldn't decrypt password");
                                Console.WriteLine("Error message: " + e);
                                Thread.Sleep(500);
                                goto Start;
                            }

                            Console.WriteLine("");

                            if (accounts.Password == oldpw)
                            {
                                Console.Write("New password: ");
                                string newpw = "";
                                //Replaces input with '*'
                                ConsoleKeyInfo info1 = Console.ReadKey(true);
                                while (info1.Key != ConsoleKey.Enter)
                                {
                                    if (info1.Key != ConsoleKey.Backspace)
                                    {
                                        Console.Write("*");
                                        newpw += info1.KeyChar;
                                    }
                                    else if (info1.Key == ConsoleKey.Backspace)
                                    {
                                        if (!string.IsNullOrEmpty(oldpw))
                                        {
                                            // remove one character from the list of password characters
                                            newpw = newpw.Substring(0, newpw.Length - 1);
                                            // get the location of the cursor
                                            int pos = Console.CursorLeft;
                                            // move the cursor to the left by one character
                                            Console.SetCursorPosition(pos - 1, Console.CursorTop);
                                            // replace it with space
                                            Console.Write(" ");
                                            // move the cursor to the left by one character again
                                            Console.SetCursorPosition(pos - 1, Console.CursorTop);
                                        }
                                    }
                                    info1 = Console.ReadKey(true);
                                }
                                accounts.Password = newpw;

                                string pw1 = accounts.Password;
                                try
                                {
                                    byte[] pw_byte = Encoding.UTF8.GetBytes(pw1);
                                    accounts.Password = HttpUtility.UrlEncode(Convert.ToBase64String(pw_byte));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Couldn't decrypt password");
                                    Console.WriteLine("Error message: " + e);
                                    Thread.Sleep(500);
                                    goto Start;
                                }
                                File.AppendAllText(logins, "UN: " + accounts.Username + " PW: " + accounts.Password);

                                Console.WriteLine("Password has been changed");
                                Thread.Sleep(500);
                                goto Start;
                            }

                            if (accounts.Password != oldpw)
                            {
                                Console.WriteLine("Password is incorrect");
                                Thread.Sleep(500);
                                goto Start;
                            }
                        }

                        if (decision == '0')
                        {
                            Thread.Sleep(200);
                            goto Start;
                        }

                        else
                        {
                            Console.Write("Please press a valid key");
                            Thread.Sleep(1000);
                            goto Settings;
                        }
                    }

                case '8':
                    {
                        Console.Clear();
                        Environment.Exit(0);
                        goto Start;
                    }

                case '9':
                    {
                        Console.Clear();
                        Console.WriteLine("Your PC will now restart, press ESC to cancel. Press any other key to continue");
                        ConsoleKeyInfo esc;
                        esc = Console.ReadKey();
                        if (esc.Key == ConsoleKey.Escape)
                        {
                            goto Start;
                        }
                        else
                        {
                            Process.Start("ShutDown", "/r /t 0");
                        }
                    }
                    break;

                case '0':
                    {
                        Console.Clear();
                        Console.WriteLine(" Your PC will now shut down, press ESC to cancel. Press any other key to continue");
                        ConsoleKeyInfo esc;
                        esc = Console.ReadKey();
                        if (esc.Key == ConsoleKey.Escape)
                        {
                            goto Start;
                        }
                        else
                        {
                            Process.Start("ShutDown", "/s /t 0");
                        }
                    }
                    break;
            }
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
    internal class ConsoleSpinner
    {
        private int _currentAnimationFrame;

        public ConsoleSpinner()
        {
            SpinnerAnimationFrames = new[]
                                     {
                                         '|',
                                         '/',
                                         '-',
                                         '\\'
                                     };
        }

        public char[] SpinnerAnimationFrames { get; set; }

        public void UpdateProgress()
        {
            // Store the current position of the cursor
            var originalX = Console.CursorLeft;
            var originalY = Console.CursorTop;

            // Write the next frame (character) in the spinner animation
            Console.Write(SpinnerAnimationFrames[_currentAnimationFrame]);

            // Keep looping around all the animation frames
            _currentAnimationFrame++;
            if (_currentAnimationFrame == SpinnerAnimationFrames.Length)
            {
                _currentAnimationFrame = 0;
            }

            // Restore cursor to original position
            Console.SetCursorPosition(originalX, originalY);
        }
    }
}