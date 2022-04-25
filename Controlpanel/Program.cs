using System;
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
            Console.Title = "Soerensen's Controlpanel";

            Console.CursorVisible = false;

            // Useless login feature that nobody will use. Also very unsecure, dont use personal pw here
            string root = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logins";

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            Accounts accounts = new Accounts();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Do you want to 'login' or 'create login'?");
                string decision = Console.ReadLine();
                decision.ToLower();
                if (decision == "create")
                {
                    var (username, balance) = Create();
                    accounts.Username = Encoder(username);
                    accounts.Balance = Encoder(balance);
                }
                else if (decision == "login")
                {
                    var (username, balance) = Login();
                    accounts.Username = Encoder(username);
                    accounts.Balance = Encoder(balance);
                    break;
                }
                else
                {
                    Console.WriteLine("Please type 'login' or 'create login'");
                    Thread.Sleep(500);
                }
            }

            while (true)
            {
                Console.Clear();
                string username = Decoder(accounts.Username);
                Menu(username);
                string presetdir = $"{root}\\{accounts.Username}\\Presets";
                string presetfolder_1 = $"{presetdir}\\Preset 1";
                string presetname_1 = $"{presetfolder_1}\\ProgramName";
                string presetaddress_1 = $"{presetfolder_1}\\address";
                string presetfolder_2 = $"{presetdir}\\Preset 2";
                string presetname_2 = $"{presetfolder_2}\\ProgramName";
                string presetaddress_2 = $"{presetfolder_2}\\address";
                string presetfolder_3 = $"{presetdir}\\Preset 3";
                string presetname_3 = $"{presetfolder_3}\\ProgramName";
                string presetaddress_3 = $"{presetfolder_3}\\address";
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
                    PerformanceCounter cpuCounter = new PerformanceCounter
                    {
                        CategoryName = "Processor",
                        CounterName = "% Processor Time",
                        InstanceName = "_Total"
                    };

                    // Getting ram info
                    PerformanceCounter ramCounter = new PerformanceCounter
                    {
                        CategoryName = "Memory",
                        CounterName = "Available MBytes",
                        InstanceName = String.Empty
                    };
                    dynamic RamValue = ramCounter.NextValue();

                    // Getting total of ram
                    GetPhysicallyInstalledSystemMemory(out long memKb);
                    int memKbInt = Convert.ToInt32(memKb);
                    // Converting to mb
                    int TotalRaminMb = memKbInt / 1024;

                    // Convert to procent
                    dynamic RamComp = RamValue / TotalRaminMb;
                    dynamic NewRamComp = RamComp * 100;
                    dynamic RamProcent = 100 - NewRamComp;

                    // Get first value from CPU which is always 0
                    dynamic _ = cpuCounter.NextValue();
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
                    case '1':
                        {
                            Console.Clear();
                            if (File.Exists(presetaddress_1))
                            {
                                try
                                {
                                    string address_ = File.ReadAllText(presetaddress_1);
                                    Process.Start(address_);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("File doesn't exist, change preset under 'Settings'");
                                    Thread.Sleep(2000);
                                }
                            }

                            Console.WriteLine("What program do you want as shortcut?");
                            string program = Console.ReadLine();
                            if (!program.EndsWith(".exe"))
                            {
                                program = program[^4..];
                            }
                            File.WriteAllText(presetname_1, program);

                            Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
                            string address = Console.ReadLine();
                            File.WriteAllText(presetaddress_1, address);
                            Console.WriteLine("Preset has been set");
                            Thread.Sleep(1000);
                        }
                        continue;

                    case '2':
                        {
                            Console.Clear();
                            if (File.Exists(presetaddress_2))
                            {
                                try
                                {
                                    string address_ = File.ReadAllText(presetaddress_2);
                                    Process.Start(address_);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("File doesn't exist, change preset under 'Settings'");
                                    Thread.Sleep(2000);
                                }
                            }

                            Console.WriteLine("What program do you want as shortcut?");
                            string program = Console.ReadLine();
                            if (!program.EndsWith(".exe"))
                            {
                                program = program[^4..];
                            }
                            File.WriteAllText(presetname_2, program);

                            Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
                            string address = Console.ReadLine();
                            File.WriteAllText(presetaddress_2, address);
                            Console.WriteLine("Preset has been set");
                            Thread.Sleep(1000);
                        }
                        continue;

                    case '3':
                        {
                            Console.Clear();

                            if (File.Exists(presetaddress_3))
                            {
                                try
                                {
                                    string address_ = File.ReadAllText(presetaddress_3);
                                    Process.Start(address_);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("File doesn't exist, change preset under 'Settings'");
                                    Thread.Sleep(2000);
                                }
                            }

                            Console.WriteLine("What program do you want as shortcut?");
                            string program = Console.ReadLine();
                            if (program.EndsWith(".exe"))
                            {
                                program = program[^4..];
                            }
                            File.WriteAllText(presetname_3, program);

                            Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
                            string address = Console.ReadLine();
                            File.WriteAllText(presetaddress_3, address);
                            Console.WriteLine("Preset has been set");
                            Thread.Sleep(1000);
                        }
                        continue;

                    case '4':
                        {
                            // Find path to temp folder
                            Console.Clear();
                            (DirectoryInfo temp, float size) = FindTemp();

                            // Delete files
                            Console.WriteLine("Deleting temp files and folders");
                            EmptyTemp(temp);

                            // Getting size of temp again
                            float delsize = 0;
                            foreach (FileInfo fi in temp.GetFiles("*", SearchOption.AllDirectories))
                            {
                                delsize += fi.Length;
                            }

                            // Convert kb to mb
                            float delmbsize = delsize / 1000000;

                            // Comparing the old and new size
                            float ttldelsize = size - delmbsize;

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
                                Console.WriteLine("Files in the recycle bin has been deleted");
                                Thread.Sleep(1500);
                            }

                            // Done
                            Console.Clear();
                            Console.WriteLine("Everything is done and your PC has been cleaned!");
                            Console.WriteLine("Press any key to return to the menu");
                            Console.ReadKey();
                        }
                        continue;

                    case '5':
                        {
                            Casino(username);
                        }
                        continue;

                    case '6':
                        {
                            while (true)
                            {
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
                                    Console.WriteLine(win["Caption"].ToString() + "\n");
                                }

                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write("2: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("CPU: ");
                                var cpu = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                                foreach (ManagementObject mo in cpu.Get())
                                {
                                    Console.WriteLine(mo["Name"]);
                                    Console.WriteLine($"ID: {mo["ProcessorId"]}\n");
                                }

                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write("3: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("GPU: ");
                                ManagementObjectSearcher videocontroller = new ManagementObjectSearcher("select * from Win32_VideoController");
                                foreach (ManagementObject videocard in videocontroller.Get())
                                {
                                    Console.WriteLine(videocard["Name"]);
                                    Console.WriteLine($"ID: {videocard["DeviceID"]}\n");
                                }

                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write("4: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Motherboard: ");
                                ManagementObjectSearcher motherboard = new ManagementObjectSearcher("select * from Win32_BaseBoard");
                                foreach (ManagementObject modbinfo in motherboard.Get())
                                {
                                    Console.WriteLine(modbinfo["Product"]);
                                    Console.WriteLine($"ID: {modbinfo["SerialNumber"]}\n");
                                }

                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write("5: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("RAM: ");
                                ManagementObjectSearcher ram = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
                                foreach (ManagementObject queryObj in ram.Get())
                                {
                                    var capacity = Math.Round(Convert.ToDouble(queryObj["Capacity"])) / 1024 / 1024 / 1024;
                                    Console.WriteLine($"Slot: {queryObj["BankLabel"]}\nCapacity: {capacity}Gb\n");
                                }

                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write("6: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Disks: ");
                                ManagementObjectSearcher dsk = new ManagementObjectSearcher("select * from Win32_LogicalDisk");
                                foreach (ManagementObject disk in dsk.Get())
                                {
                                    Console.WriteLine($"Disks: {disk["Caption"]}");
                                    double size = Math.Round(Convert.ToDouble(disk["Size"]) / 1024 / 1024 / 1024, 2);
                                    Console.WriteLine($"Size: {size}GB\n");
                                }

                                Console.Write("0: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Go back");
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Press a number between 1-6 to get more info");
                                char info = Console.ReadKey().KeyChar;

                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.White;

                                switch (info)
                                {
                                    default:
                                        {
                                            Console.WriteLine("Please press a valid key");
                                        }
                                        continue;

                                    case '1':
                                        {
                                            foreach (ManagementObject win in os.Get())
                                            {
                                                try
                                                {
                                                    Console.WriteLine("OSType: " + win["OSType"]);
                                                    Console.WriteLine("Version: " + win["Version"]);
                                                    Console.WriteLine("OS Name: " + win["Caption"]);
                                                    Console.WriteLine("Build Number: " + win["BuildNumber"]);
                                                }

                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }

                                            Console.WriteLine("");
                                            Console.WriteLine("\nPress any key to go back to the menu");
                                            Console.ReadKey();
                                            continue;
                                        }

                                    case '2':
                                        {
                                            foreach (ManagementObject cpuinfo in cpu.Get())
                                            {
                                                try
                                                {
                                                    Console.WriteLine("Manufacturer: " + cpuinfo["Manufacturer"]);
                                                    Console.WriteLine("CPU: " + cpuinfo["Name"]);
                                                    Console.WriteLine("ID: " + cpuinfo["ProcessorId"]);
                                                    Console.WriteLine("Cores: " + cpuinfo["NumberOfCores"]);
                                                    Console.WriteLine("MHz: " + cpuinfo["MaxClockSpeed"]);
                                                }

                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }

                                            Console.WriteLine("\nPress any key to go back to the menu");
                                            Console.ReadKey();
                                            continue;
                                        }

                                    case '3':
                                        {
                                            foreach (ManagementObject videocard in videocontroller.Get())
                                            {
                                                try
                                                {
                                                    Console.WriteLine("Graphic Card: " + videocard["Name"]);
                                                    Console.WriteLine("System Name: " + videocard["SystemName"]);
                                                    Console.WriteLine("ID: " + videocard["DeviceID"]);
                                                    Console.WriteLine("Driver Version: " + videocard["DriverVersion"]);
                                                    Console.WriteLine("Current refreshrate: " + videocard["CurrentRefreshrate"]);
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }

                                            Console.WriteLine("\nPress any key to go back to the menu");
                                            Console.ReadKey();
                                            continue;
                                        }

                                    case '4':
                                        {
                                            foreach (ManagementObject modbinfo in motherboard.Get())
                                            {
                                                try
                                                {
                                                    Console.WriteLine("Name:" + modbinfo["Product"]);
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }

                                            }

                                            Console.WriteLine("\nPress any key to go back to the menu");
                                            Console.ReadKey();
                                            continue;
                                        }

                                    case '5':
                                        {
                                            foreach (ManagementObject raminfo in ram.Get())
                                            {
                                                try
                                                {
                                                    double cap = Math.Round(Convert.ToDouble(raminfo["Capacity"]) / 1024 / 1024 / 1024, 2);

                                                    Console.WriteLine("Manufacturer: " + raminfo["Manufacturer"]);
                                                    Console.WriteLine("Speed: " + raminfo["Speed"]);
                                                    Console.WriteLine("Size: " + cap);
                                                    Console.WriteLine("Serial: " + raminfo["Name"]);
                                                    Console.Write("Slot: " + raminfo["BankLabel"]);
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }

                                            Console.WriteLine("Press any key to go back to the menu");
                                            Console.ReadKey();
                                            continue;
                                        }

                                    case '6':
                                        {
                                            foreach (ManagementObject disk in dsk.Get())
                                            {
                                                try
                                                {
                                                    double newsize = Math.Round(Convert.ToDouble(disk["Size"]) / 1024 / 1024 / 1024, 2);

                                                    Console.WriteLine("Disks: " + disk["Caption"]);
                                                    Console.Write(disk["Name"] + " = ");
                                                    Console.WriteLine(newsize + "GB");
                                                    Console.WriteLine("ID: " + disk["VolumeSerialNumber"]);
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }

                                            Console.WriteLine("Press any key to go back to the menu");
                                            Console.ReadKey();
                                            continue;
                                        }

                                    case '0':
                                        {
                                            break;
                                        }
                                }
                                break;
                            }
                        }
                        continue;

                    case '7':
                        {
                            while (true)
                            {
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

                                char decs = Console.ReadKey().KeyChar;
                                Console.Clear();

                                if (decs == '1')
                                {
                                    while (true)
                                    {
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
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.Write("0: ");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine("Go back");

                                        char decs1 = Console.ReadKey().KeyChar;
                                        Console.Clear();

                                        if (decs1 == '1')
                                        {
                                            Preset(presetaddress_1);
                                            break;
                                        }

                                        if (decs1 == '2')
                                        {
                                            Preset(presetaddress_2);
                                            break;
                                        }

                                        if (decs1 == '3')
                                        {
                                            Preset(presetaddress_3);
                                            break;
                                        }

                                        if (decs1 == '0')
                                        {
                                            break;
                                        }

                                        else
                                        {
                                            Console.WriteLine("Please enter a valid key");
                                            Thread.Sleep(1000);
                                            continue;
                                        }
                                    }
                                    continue;
                                }

                                if (decs == '2')
                                {
                                    Console.WriteLine("What do you want to change your password to?");
                                    Console.Write("Old password: ");
                                    string oldpw = PwRedact();

                                    var saved_pw = File.ReadLines(root);
                                    if (saved_pw.Equals(oldpw) == false)
                                    {
                                        Console.WriteLine("Password is incorrect");
                                        Thread.Sleep(500);
                                        continue;
                                    }

                                    Console.Write("New password: ");
                                    string newpw = PwRedact();

                                    File.AppendAllText(root, "UN: " + accounts.Username + " PW: " + Encoder(newpw));

                                    Console.WriteLine("Password has been changed");
                                    Thread.Sleep(500);
                                    continue;


                                }

                                if (decs == '0')
                                {
                                    Thread.Sleep(200);
                                    break;
                                }

                                else
                                {
                                    Console.Write("Please press a valid key");
                                    Thread.Sleep(1000);
                                    continue;
                                }
                            }
                        }
                        continue;

                    case '8':
                        {
                            Console.Clear();
                            Environment.Exit(0);
                        }
                        break;

                    case '9':
                        {
                            Console.Clear();
                            Console.WriteLine("Your PC will now restart, press ESC to cancel. Press any other key to continue");
                            ConsoleKeyInfo esc;
                            esc = Console.ReadKey();
                            if (esc.Key == ConsoleKey.Escape)
                            {
                                break;
                            }
                            Process.Start("ShutDown", "/r /t 0");
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
                                break;
                            }
                            Process.Start("ShutDown", "/s /t 0");
                        }
                        break;
                }
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

        static (string username, string balance) Login()
        {
            Console.WriteLine("Logging in");
            //Gets username
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string encoded_un = Encoder(username); 

            //Gets password
            Console.Write("Password: ");
            string password = PwRedact();

            Console.WriteLine("");
            string root = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logins";
            string userdir = $"{root}\\{encoded_un}";


            //Check if username exist
            //If username doesnt exist
            if (!Directory.Exists(userdir))
            {
                Console.WriteLine("Username or password doesnt exist");
                Thread.Sleep(500);
                return (null, null);
            }

            string logins = $"{userdir}\\login";
            string casinodir = $"{userdir}\\Casino";
            string balancetxt = $"{casinodir}\\Balance";
            //Gets pw info
            string infos = File.ReadAllText(logins);

            //If pw isnt correct
            if (!infos.Contains($"PW: {Encoder(password)}"))
            {
                Console.WriteLine("Username or password doesnt exist");
                Thread.Sleep(500);
                return (null, null);
            }

            string balance;
            //Checks if balance file exist
            //If balance file doesnt exist
            if (!File.Exists(balancetxt))
            {
                balance = "1000";
                //Make a new balance file
                Directory.CreateDirectory(balancetxt);

                //Write encrypted balance to file
                File.WriteAllText(balancetxt, Encoder(balance));
            }
            balance = File.ReadAllText(balancetxt);

            Console.WriteLine("You have logged in succesfully as: " + username);
            return (username, balance);
        }

        static (string username, string balance) Create()
        {
            //Get username and pw
            Console.WriteLine("Creating account");
            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = PwRedact();

            Console.WriteLine("");

            string root = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logins";
            string userdir = $"{root}\\{Encoder(username)}";

            if (Directory.Exists(userdir))
            {
                Console.WriteLine("Username is taken");
                Thread.Sleep(500);
                return (null, null);
            }

            //Create sub folder
            Directory.CreateDirectory(userdir);
            Console.WriteLine("");
            string balance = Encoder(1000.ToString());

            string casinodir = $"{userdir}\\Casino";
            string logins = $"{userdir}\\login";
            string balancetxt = $"{casinodir}\\Balance";

            string presetdir = $"{userdir}\\Presets";
            string presetfolder1 = $"{presetdir}\\Preset 1";
            string presetfolder2 = $"{presetdir}\\Preset 2";
            string presetfolder3 = $"{presetdir}\\Preset 3";

            Directory.CreateDirectory(casinodir);
            Directory.CreateDirectory(presetdir);
            Directory.CreateDirectory(presetfolder1);
            Directory.CreateDirectory(presetfolder2);
            Directory.CreateDirectory(presetfolder3);

            File.AppendAllText(logins, "UN: " + username + " PW: " + password);
            File.AppendAllText(balancetxt, balance);

            Console.WriteLine("Account has been made, please sign in");
            Thread.Sleep(500);
            return (username, balance);
        }

        static string Encoder(string input)
        {
            byte[] input_bt = Encoding.UTF8.GetBytes(input);
            return HttpUtility.UrlEncode(Convert.ToBase64String(input_bt));
        }

        static string Decoder(string input)
        {
            byte[] input_bt = Convert.FromBase64String(HttpUtility.UrlDecode(input));
            return Encoding.UTF8.GetString(input_bt);
        }

        static void Menu(string username)
        {
            Presets preset = new Presets();
            string root = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logins";
            string presetdir = $"{root}\\{Encoder(username)}\\Presets";
            string presetfolder_1 = $"{presetdir}\\Preset 1";
            string presetname_1 = $"{presetfolder_1}\\ProgramName";
            string presetaddress_1 = $"{presetfolder_1}\\address";
            string presetfolder_2 = $"{presetdir}\\Preset 2";
            string presetname_2 = $"{presetfolder_2}\\ProgramName";
            string presetaddress_2 = $"{presetfolder_2}\\address";
            string presetfolder_3 = $"{presetdir}\\Preset 3";
            string presetname_3 = $"{presetfolder_3}\\ProgramName";
            string presetaddress_3 = $"{presetfolder_3}\\address";

            // Menu
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Welcome to Soerensen's Controlpanel");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("***********************************");
            Console.WriteLine("Logged in as: " + username);
            Console.WriteLine("What do you want to do?");
            Console.Write("1: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (File.Exists(presetaddress_1))
            {
                string preset1Content = File.ReadAllText(presetname_1);
                preset.Preset1 = preset1Content;
                Console.WriteLine("Open " + preset.Preset1);
            }
            else
            {
                Console.WriteLine("Set shortcut 1");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("2: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (File.Exists(presetaddress_2))
            {
                string preset2Content = File.ReadAllText(presetname_2);
                preset.Preset2 = preset2Content;
                Console.WriteLine("Open " + preset.Preset2);
            }
            else
            {
                Console.WriteLine("Set shortcut 2");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("3: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (File.Exists(presetaddress_3))
            {
                string preset3Content = File.ReadAllText(presetname_3);
                preset.Preset3 = preset3Content;
                Console.WriteLine("Open " + preset.Preset3);
            }
            else
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
        }
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
            Console.ForegroundColor = ConsoleColor.DarkRed;
            // When hitting 2 times and then stand dealer wins when dealer is lower
            Console.WriteLine("Black Jack // Not working");
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
                                }
                                while (chr.Key != ConsoleKey.Enter);

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
                                Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
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

                                if(!Int32.TryParse(Console.ReadLine(), out int bet))
                                {
                                    Console.WriteLine("Invalid value");
                                    Thread.Sleep(500);
                                    continue;
                                }

                                // Get balance from balance file
                                PlaceBet(balancetxt, bet);
                                Console.WriteLine("");
                                Console.WriteLine($"Your balance is now {GetBalance(balancetxt)}");
                                Console.WriteLine("CAUTION: if you leave while playing a game that haven't ended yet, you will lose your money");
                                Thread.Sleep(5000);

                                ConsoleKeyInfo chara = Console.ReadKey(true);
                                while (chara.Key != ConsoleKey.Escape)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Press ESC to go back, Press Backspace to change bet");
                                    Console.WriteLine("***************   X-X = 3x                                                   ");
                                    Console.WriteLine("*             *   X-X-Y-Y = 5x                                               ");
                                    Console.WriteLine("*             *   X-X-X = 10x                                                ");
                                    Console.WriteLine("*             *   X-X-X-Y-Y = 50x                                            ");
                                    Console.WriteLine("*             *   X-X-X-X = 100x                                             ");
                                    Console.WriteLine("*             *   X-X-X-X-X = 1000x                                          ");
                                    Console.WriteLine("*             *                                                              ");
                                    Console.WriteLine("***************   For each win you will get a jackpot spin and 100x your win!");

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
                                    if (firstIcon == secondIcon && secondIcon == thirdIcon && thirdIcon == fourthIcon && fourthIcon == fifthIcon)
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
        static void Preset(string root)
        {
            Console.WriteLine("What program do you want as shortcut?");
            string program = Console.ReadLine();
            if (program.EndsWith(".exe"))
            {
                program = program[^4..];
            }
            File.WriteAllText(root, program);

            Console.WriteLine(@"What is the address? e.g. C:\Program Files (x86)\Google\Chrome\Application");
            string address = Console.ReadLine();
            File.WriteAllText(root, address);
            Console.WriteLine("Preset has been set");
            Thread.Sleep(1000);
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

        static string PwRedact()
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
                else if (input.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(pw))
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
                }
                input = Console.ReadKey(true);
            }
            return pw;
        }

        static (DirectoryInfo temp, float size) FindTemp()
        {
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
            return (temp, size);
        }

        static void EmptyTemp(DirectoryInfo temp)
        {
            var s = new ConsoleSpinner();

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