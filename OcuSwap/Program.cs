using System;

namespace OcuSwap
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize with a title and line, waiting for user to hit a key
            Console.Title = "OcuSwap";
            Console.WriteLine("Press any key to go dark :D");
            Console.ReadKey();

            // Kill Oculus Client before we do anything
            if (System.Diagnostics.Process.GetProcessesByName("OculusClient").Length > 0)
            {
                foreach (var process in System.Diagnostics.Process.GetProcessesByName("OculusClient"))
                {
                    process.Kill();
                }
            }

            string theVoidPath;

            // Checking paths and asking for another path if not found
            bool pathsExist = CheckPaths();
            if (pathsExist)
            {
                theVoidPath = @"C:\Program Files\Oculus\Support\oculus-dash\dash\assets\raw\textures\environment\the_void";
            } else
            {
                Console.WriteLine("\nLooks like Oculus software isn't installed to the default location.\nPlease enter the location to your Oculus path and press Enter.");
                theVoidPath = Console.ReadLine();
            }

            string darkDotDDS = theVoidPath + @"\grid_plane_004.dds";
            string lightGridDDS = theVoidPath + @"\grid_plane_006.dds";

            // Beginning to overwrite
            // TODO: Check if we actually have already swapped or not

            Console.WriteLine("Overwriting Dark Grid to Light Grid");
            System.IO.File.Move(darkDotDDS, lightGridDDS, true);
            Console.WriteLine("Done! Make sure to start your Oculus app again. Press any key to exit.");
            Console.ReadKey();
        }

        static bool CheckPaths()
        {
            if (System.IO.Directory.Exists(@"C:\Program Files\Oculus\Support\oculus-dash\dash\assets\raw\textures\environment\the_void"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
