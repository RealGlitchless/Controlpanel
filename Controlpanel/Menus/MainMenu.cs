using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Controlpanel.Menus.Casino;
using Controlpanel.Model;

namespace Controlpanel.Menus;

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

    public void PrintMenu()
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
            Console.WriteLine(presets[i].Name);
        }
        Console.Write($"{option+1}: ");
        Console.WriteLine("Create Preset");
        Console.Write($"{option+2}: ");
        Console.WriteLine("Clean PC");
        Console.Write($"{option+3}: ");
        Console.WriteLine("Enter Casino");
        Console.Write($"{option+4}: ");
        Console.WriteLine("See Hardware Information");
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
            Console.SetCursorPosition(0, option + 13);
            PrintSystemInfo();
        }
        SelectOption();
    }

    private void SelectOption()
    {
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
        else if (selectedOption == option - 7)
        {
            // User selected "Create Preset"
            new CreatePresetMenu(_account).PrintMenu();
        }
        else if (selectedOption == option - 6)
        {
            // User selected "Clean PC"
            SystemCleanupMenu.Start();
        }
        else if (selectedOption == option - 5)
        {
            // User selected "Enter Casino"
            new CasinoMenu(_account).PrintMenu();
        }
        else if (selectedOption == option - 4)
        {
            // User selected "See Hardware Information"
            SystemInfoMenu.PrintMenu();
        }
        else if (selectedOption == option - 3)
        {
            // User selected "Settings"
            new AccountMenu(_account).PrintMenu();
        }
        else if (selectedOption == option - 2)
        {
            // User selected "Exit Controlpanel"
            Exit();
        }
        else if (selectedOption == option - 1)
        {
            // User selected "Restart PC"
            Restart();
        }
        else if (selectedOption == option)
        {
            // User selected "Shutdown PC"
            Shutdown();
        }
        else
        {
            Console.WriteLine("Invalid input");
        }
    }

    private static void PrintSystemInfo()
    {
        // Gets the date and write
        string day = DateTime.Today.ToString("D");
        Console.WriteLine($"The date is {day}");

        // Gets the time and write
        string time = DateTime.Now.ToString("T");
        Console.WriteLine($"The current time is {time}");

        // Prints values
        Console.WriteLine("CPU Load: " + GetCpuProcent() + "%     ");
        Console.WriteLine("RAM Load: " + GetRamProcent() + "%      ");
        Thread.Sleep(500);
    }

    private static dynamic GetRamProcent()
    {
        // Get available RAM in MB
        PerformanceCounter ramCounter = new("Memory", "Available MBytes");
        var availableRamInMb = ramCounter.NextValue();

        // Getting total of ram
        GetPhysicallyInstalledSystemMemory(out _);

        // Get total RAM in MB
        GetPhysicallyInstalledSystemMemory(out var totalRamInKb);
        var totalRamInMb = Convert.ToInt32(totalRamInKb / 1024);

        // Calculate and return RAM usage percentage
        var ramPercentage = Math.Round((1 - availableRamInMb / totalRamInMb) * 100);
        return ramPercentage;
    }
        
    private static dynamic GetCpuProcent()
    {
        // Getting Cpu info
        PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");

        // Get first value from CPU which is always 0
        var _ = cpuCounter.NextValue();

        // Wait for the counter to get an actual value
        Thread.Sleep(500);

        // Get the second value from the CPU counter
        var secondValue = cpuCounter.NextValue();
        var cpuPercentage = Math.Round(secondValue);

        return cpuPercentage;
    }

    private static void Exit()
    {
        Console.Clear();
        Environment.Exit(0);
    }
        
    private static void Restart()
    {
        Console.Clear();
        Console.WriteLine("Your PC will now restart, press ESC to cancel. Press any other key to continue");
        var esc = Console.ReadKey();
        if (esc.Key != ConsoleKey.Escape)
        {
            Process.Start("ShutDown", "/r /t 0");
        }
    }
        
    private static void Shutdown()
    {
        Console.Clear();
        Console.WriteLine("Your PC will now shut down, press ESC to cancel. Press any other key to continue");
        var esc = Console.ReadKey();
        if (esc.Key != ConsoleKey.Escape)
        {
            Process.Start("ShutDown", "/s /t 0");
        }
    }
}