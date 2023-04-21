using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Controlpanel.Utilities;

namespace Controlpanel.Menus;

public static class SystemCleanupMenu
{
    // Importing shell32.dll
    [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
    private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

    public static void Start()
    {
        Console.Clear();
        DirectoryInfo tempDirectory = FindTemp();
        float initialSize = GetTempSize(tempDirectory);

        EmptyTemp(tempDirectory);

        float newSize = GetTempSize(tempDirectory);

        string sizeMessage = GetDeletedSize(initialSize, newSize);

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
        else
        {
            Console.WriteLine("Files in the recycle bin has been deleted");
            Thread.Sleep(1500);
        }

        Console.WriteLine("Everything is done and your PC has been cleaned!");
        Console.WriteLine("Press any key to return to the menu");
        Console.ReadKey();
    }

    private static void EmptyTemp(DirectoryInfo temp)
    {
        ConsoleSpinner spinner = new();

        foreach (FileInfo file in temp.EnumerateFiles())
        {
            spinner.UpdateProgress();

            // Try to delete the file
            Console.WriteLine(
                TryDeleteFile(file) ? $"Deleted file: {file.Name}" : $"Failed to delete file: {file.Name}");
        }
        // Delete folders
        foreach (DirectoryInfo dir in temp.EnumerateDirectories())
        {
            // Try to delete the directory
            Console.WriteLine(TryDeleteDirectory(dir)
                ? $"Deleted directory: {dir.Name}"
                : $"Failed to delete directory: {dir.Name}");
        }
    }

    private static bool TryDeleteFile(FileInfo file)
    {
        try
        {
            file.Delete();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool TryDeleteDirectory(DirectoryInfo dir)
    {
        try
        {
            dir.Delete(true);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static DirectoryInfo FindTemp()
    {
        Console.WriteLine("Finding Temp folder");
        DirectoryInfo temp = new(Path.GetTempPath());
        Console.WriteLine("Temp folder found");
        return temp;
    }

    private static float GetTempSize(DirectoryInfo temp)
    {
        return temp.GetFiles("*", SearchOption.AllDirectories).Aggregate<FileInfo, long>(0, (current, fi) => current + fi.Length);
    }

    private static string GetDeletedSize(float initialSize, float newSize)
    {
        float deletedSize = initialSize - newSize;

        string sizeMessage = deletedSize switch
        {
            <= 0 => "No files were deleted",
            < 1000000 => $"Deleted {deletedSize / 1000} KB",
            < 1000000000 => $"Deleted {deletedSize / 1000000} MB",
            < 100000000000 => $"Deleted {deletedSize / 1000000000} GB",
            _ => ""
        };

        return sizeMessage;
    }
}