using System;
using System.Linq;
using System.Management;

namespace Controlpanel.Menus;

public static class SystemInfoMenu
{
    public static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("Here is everything about your PC.");
        Console.WriteLine("***********************************");
        PrintSystemInfo();
        Console.WriteLine("***********************************\n");
        Console.WriteLine("Press any key to go back");
        var _ = Console.ReadKey().KeyChar;
            
    }

        
    private static void PrintSystemInfo()
    {
        Console.WriteLine("\n*************** OS ****************\n");
        PrintOSInfo();
        Console.WriteLine("\n*************** CPU ***************\n");
        PrintCPUInfo();
        Console.WriteLine("\n*************** GPU ***************\n");
        PrintGPUInfo();
        Console.WriteLine("\n*********** Motherboard ***********\n");
        PrintMotherboardInfo();
        Console.WriteLine("\n*************** RAM ***************\n");
        PrintRAMInfo();
        Console.WriteLine("\n*************** HDD ***************\n");
        PrintDiskInfo();
    }

    private static void PrintOSInfo()
    {
        var osQuery = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        var os = osQuery.Get().Cast<ManagementObject>().FirstOrDefault();

        if (os == null) return;
        Console.WriteLine("OS Name: " + os["Caption"]);
        Console.WriteLine("Version: " + os["Version"]);
        Console.WriteLine("Build Number: " + os["BuildNumber"]);
    }

    private static void PrintCPUInfo()
    {
        var cpuQuery = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        var cpu = cpuQuery.Get().Cast<ManagementObject>().FirstOrDefault();

        if (cpu == null) return;
        Console.WriteLine("Name: " + cpu["Name"]);
        Console.WriteLine("Manufacturer: " + cpu["Manufacturer"]);
        Console.WriteLine("Number of cores: " + cpu["NumberOfCores"]);
        Console.WriteLine("Number of logical processors: " + cpu["NumberOfLogicalProcessors"]);
        Console.WriteLine("Max clock speed: " + cpu["MaxClockSpeed"]);
    }

    private static void PrintGPUInfo()
    {
        var gpuQuery = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
        var gpu = gpuQuery.Get().Cast<ManagementObject>().FirstOrDefault();

        if (gpu == null) return;
        Console.WriteLine("Name: " + gpu["Name"]);
        Console.WriteLine("Manufacturer: " + gpu["AdapterCompatibility"]);
        Console.WriteLine("Video Processor: " + gpu["VideoProcessor"]);
        Console.WriteLine("Video Memory Type: " + gpu["VideoMemoryType"]);
        Console.WriteLine("Video Memory: " + gpu["AdapterRAM"]);
    }

    private static void PrintMotherboardInfo()
    {
        var motherboardQuery = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
        var motherboard = motherboardQuery.Get().Cast<ManagementObject>().FirstOrDefault();

        if (motherboard == null) return;
        Console.WriteLine("Product: " + motherboard["Product"]);
        Console.WriteLine("Manufacturer: " + motherboard["Manufacturer"]);
        Console.WriteLine("Serial Number: " + motherboard["SerialNumber"]);
        Console.WriteLine("Version: " + motherboard["Version"]);
    }

    private static void PrintRAMInfo()
    {
        var ramQuery = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
        var ramModules = ramQuery.Get().Cast<ManagementObject>();

        foreach (var ramModule in ramModules)
        {
            Console.WriteLine("Name: " + ramModule["Name"]);
            Console.WriteLine("Manufacturer: " + ramModule["Manufacturer"]);
            Console.WriteLine("Capacity: " + ramModule["Capacity"]);
            Console.WriteLine("Speed: " + ramModule["Speed"]);
            Console.WriteLine("");
        }
    }

    private static void PrintDiskInfo()
    {
        var diskQuery = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
        var disks = diskQuery.Get().Cast<ManagementObject>();

        foreach (var disk in disks)
        {
            Console.WriteLine("Caption: " + disk["Caption"]);
            Console.WriteLine("Manufacturer: " + disk["Manufacturer"]);
            Console.WriteLine("Model: " + disk["Model"]);
            Console.WriteLine("Serial Number: " + disk["SerialNumber"]);
            Console.WriteLine("Size: " + disk["Size"]);
            Console.WriteLine("Interface Type: " + disk["InterfaceType"]);
            Console.WriteLine("");
        }
    }
}