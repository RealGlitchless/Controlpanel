using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Controlpanel.Utilities
{
    public class SystemCleanup
    {
        // Importing shell32.dll
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        public void Start()
        {
            Console.Clear();
            DirectoryInfo tempDirectory = FindTemp();
            float initialSize = GetTempSize(tempDirectory);
            
            EmptyTemp(tempDirectory);
            
            float newSize = GetTempSize(tempDirectory);
            float deletedSize = initialSize - newSize;

            Console.WriteLine("Deleting temp files and folders");
            string sizeMessage = $"There has been deleted {deletedSize:N0}MB from the Temp folder";

            if (deletedSize >= 1024)
            {
                decimal gbSize = Math.Round(Convert.ToDecimal(deletedSize / 1024), 2);
                sizeMessage = $"There has been deleted {gbSize:N2}GB from the Temp folder";
            }

            Console.WriteLine(sizeMessage);

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
            Console.WriteLine("Everything is done and your PC has been cleaned!");
            Console.WriteLine("Press any key to return to the menu");
            Console.ReadKey();
        }

        private static void EmptyTemp(DirectoryInfo temp)
        {
            ConsoleSpinner spinner = new ConsoleSpinner();

            foreach (FileInfo file in temp.EnumerateFiles())
            {
                spinner.UpdateProgress();
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
        
        private DirectoryInfo FindTemp()
        {
            Console.WriteLine("Finding Temp folder");
            DirectoryInfo temp = new DirectoryInfo(Path.GetTempPath());
            Console.WriteLine("Temp folder found");
            return temp;
        }
        
        private float GetTempSize(DirectoryInfo temp)
        {
            return temp.GetFiles("*", SearchOption.AllDirectories).Aggregate<FileInfo, long>(0, (current, fi) => current + fi.Length);
        }
    }
}