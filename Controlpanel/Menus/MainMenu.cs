using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Controlpanel.Model;
using Controlpanel.Utilities;

namespace Controlpanel.Menus
{
    public class MainMenu
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
        
        private readonly Account _account;

        public MainMenu(Account account)
        {
            _account = account;
        }

        public void printMenu()
        {
            List<Preset> presets = _account.Presets;

            Console.Clear();
            // Menu
            Console.WriteLine("Welcome to Soerensen's Controlpanel");
            Console.WriteLine("***********************************");
            Console.WriteLine("Logged in as: " + _account.Username);
            Console.WriteLine("What do you want to do?");
            int option = presets.Count;
            for(int i = 0; i < option; i++)
            {
                Console.Write($"{i+1}: ");
                Console.Write(presets[i].Name);
            }
            Console.Write($"{option+1}: ");
            Console.WriteLine("Create Preset");
            Console.Write($"{option+2}: ");
            Console.WriteLine("Clean PC");
            Console.Write($"{option+3}: ");
            Console.WriteLine("Enter Casino");
            Console.Write($"{option+4}: ");
            Console.WriteLine("See Hardware Information //Not done");
            Console.Write($"{option+5}: ");
            Console.WriteLine("Settings");
            Console.Write($"{option+6}: ");
            Console.WriteLine("Exit Controlpanel");
            Console.Write($"{option+7}: ");
            Console.WriteLine("Restart PC");
            Console.Write($"{option+8}: ");
            Console.WriteLine("Shutdown PC");
            Console.WriteLine("***********************************");
            while(!Console.KeyAvailable)
            {
                printSystemInfo();
            }
            SelectOption();
        }

    private void SelectOption()
    {
        Console.Clear();
        var presets = _account.Presets;
        var option = presets.Count + 8; // Number of options in the menu
        var selectedOption = 0;

        while (selectedOption < 1 || selectedOption > option)
        {
            Console.WriteLine("Enter your choice (1-" + option + "):");
            var input = Console.ReadLine();

            if (int.TryParse(input, out selectedOption) && selectedOption >= 1 && selectedOption <= option)
            {
                // User selected a valid option
                break;
            }

            Console.WriteLine("Invalid choice. Please enter a number from 1 to " + option);
        }

        // Handle the selected option
        if (selectedOption <= presets.Count)
        {
            // User selected a preset
            var preset = presets[selectedOption - 1];
            Process.Start(preset.URL);
        }
        else if (selectedOption == option - 6)
        {
            // User selected "Create Preset"
            new CreatePresetMenu(_account).printMenu();
        }
        else if (selectedOption == option - 5)
        {
            // User selected "Clean PC"
            new SystemCleanup().Start();
        }
        else if (selectedOption == option - 4)
        {
            // User selected "Enter Casino"
            // TODO: Implement the logic for entering the casino
        }
        else if (selectedOption == option - 3)
        {
            // User selected "See Hardware Information"
            new SystemInfoMenu().PrintMenu();
        }
        else if (selectedOption == option - 2)
        {
            // User selected "Settings"
            new AccountMenu(_account).PrintMenu();
        }
        else if (selectedOption == option - 1)
        {
            // User selected "Exit Controlpanel"
            exit();
        }
        else if (selectedOption == option)
        {
            // User selected "Restart PC"
            restart();
        }
        else
        {
            // User selected "Shutdown PC"
            shutdown();
        }
    }

    private void printSystemInfo()
        {
            // Writes after the menu
            Console.SetCursorPosition(0, 10);

            // Gets the date and write
            string day = DateTime.Today.ToString("D");
            Console.WriteLine($"The date is {day}");

            // Gets the time and write
            string time = DateTime.Now.ToString("T");
            Console.WriteLine($"The current time is {time}");

            // Prints values
            Console.WriteLine($"CPU Load: " + getCpuProcent() + "%     ");
            Console.WriteLine($"RAM Load: " + getRamProcent() + "%      ");
            Thread.Sleep(500);
        }

        private dynamic getRamProcent()
        {
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
            dynamic RamProcent = 100 - (RamValue / TotalRaminMb) * 100;
            return Math.Round(RamProcent);
        }
        
        private dynamic getCpuProcent()
        {
            // Getting Cpu info
            PerformanceCounter cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };

            // Get first value from CPU which is always 0
            dynamic _ = cpuCounter.NextValue();
            Thread.Sleep(500);
            // Call again to get an actual value
            dynamic secondValue = cpuCounter.NextValue();
            return Math.Round(secondValue);
        }

        private void exit()
        {
            Console.Clear();
            Environment.Exit(0);
        }
        
        private void restart()
        {
            Console.Clear();
            Console.WriteLine("Your PC will now restart, press ESC to cancel. Press any other key to continue");
            ConsoleKeyInfo esc;
            esc = Console.ReadKey();
            if (esc.Key != ConsoleKey.Escape)
            {
                Process.Start("ShutDown", "/r /t 0");
            }
        }
        
        private void shutdown()
        {
            Console.Clear();
            Console.WriteLine("Your PC will now shut down, press ESC to cancel. Press any other key to continue");
            ConsoleKeyInfo esc;
            esc = Console.ReadKey();
            if (esc.Key != ConsoleKey.Escape)
            {
                Process.Start("ShutDown", "/s /t 0");
            }
        }
    }
}