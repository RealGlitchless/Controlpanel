using System;
using System.Management;

namespace Controlpanel.Menus
{
    public class SystemInfoMenu
    {
        public void PrintMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Here is everything about your PC");
                Console.WriteLine("***********************************");
                Console.WriteLine("1: OS");
                Console.WriteLine("2: CPU");
                Console.WriteLine("3: GPU");
                Console.WriteLine("4: Motherboard");
                Console.WriteLine("5: RAM");
                Console.WriteLine("6: Disks");
                Console.WriteLine("0: Go back");
                Console.WriteLine("Press a number between 1-6 to get more info");
                Console.WriteLine("***********************************\n");
                char option = Console.ReadKey().KeyChar;
                if (option == '0')
                    break;
                Console.SetCursorPosition(0, 11);
                PrintOptions(option);
                Console.WriteLine("***********************************\n");
                Console.WriteLine("Press anything to return to menu");
                char _ = Console.ReadKey().KeyChar;
            }
        }

        private void PrintOptions(char option)
        {
            switch (option)
            {
                case '1':
                    PrintOS();
                    break;

                case '2':
                    PrintCPU();
                    break;

                case '3':
                    PrintGPU();
                    break;

                case '4':
                    PrintMotherboard();
                    break;

                case '5':
                    PrintRAM();
                    break;

                case '6':
                    PrintDisk();
                    break;

                default:
                    Console.WriteLine("Please press a valid key");
                    break;
            }
        }
        
        private void PrintOS()
        {
            ManagementObjectSearcher os = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementBaseObject win in os.Get())
            {
                Console.WriteLine("OS Name: " + win["Caption"]);
                Console.WriteLine("Version: " + win["Version"]);
                Console.WriteLine("Build Number: " + win["BuildNumber"]);
            }
        }
        
        private void PrintCPU()
        {
            ManagementObjectSearcher cpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementBaseObject cpuinfo in cpu.Get())
            {
                Console.WriteLine("Name: " + cpuinfo["Name"]);
                Console.WriteLine("Manufacturer: " + cpuinfo["Manufacturer"]);
                Console.WriteLine("Number of cores: " + cpuinfo["NumberOfCores"]);
                Console.WriteLine("Number of logical processors: " + cpuinfo["NumberOfLogicalProcessors"]);
                Console.WriteLine("Max clock speed: " + cpuinfo["MaxClockSpeed"]);
            }
        }
        
        private void PrintGPU()
        {
            ManagementObjectSearcher videocontroller =
                new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementBaseObject videocard in videocontroller.Get())
            {
                Console.WriteLine("Name: " + videocard["Name"]);
                Console.WriteLine("Manufacturer: " + videocard["AdapterCompatibility"]);
                Console.WriteLine("Video Processor: " + videocard["VideoProcessor"]);
                Console.WriteLine("Video Memory Type: " + videocard["VideoMemoryType"]);
                Console.WriteLine("Video Memory: " + videocard["AdapterRAM"]);
            }
        }
        
        private void PrintMotherboard()
        {
            ManagementObjectSearcher motherboard = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (ManagementBaseObject modbinfo in motherboard.Get())
            {
                Console.WriteLine("Product: " + modbinfo["Product"]);
                Console.WriteLine("Manufacturer: " + modbinfo["Manufacturer"]);
                Console.WriteLine("Serial Number: " + modbinfo["SerialNumber"]);
                Console.WriteLine("Version: " + modbinfo["Version"]);
            }
        }
        
        private void PrintRAM()
        {
            ManagementObjectSearcher ram = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementBaseObject raminfo in ram.Get())
            {
                Console.WriteLine("Name: " + raminfo["Name"]);
                Console.WriteLine("Manufacturer: " + raminfo["Manufacturer"]);
                Console.WriteLine("Capacity: " + raminfo["Capacity"]);
                Console.WriteLine("Speed: " + raminfo["Speed"]);
            }
        }
        
        private void PrintDisk()
        {
            ManagementObjectSearcher dsk = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementBaseObject disk in dsk.Get())
            {
                Console.WriteLine("Caption: " + disk["Caption"]);
                Console.WriteLine("Manufacturer: " + disk["Manufacturer"]);
                Console.WriteLine("Model: " + disk["Model"]);
                Console.WriteLine("Serial Number: " + disk["SerialNumber"]);
                Console.WriteLine("Size: " + disk["Size"]);
            }
        }
    }
}