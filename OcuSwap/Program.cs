using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace OcuSwap
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize with title and main menu

            Console.Title = "OcuSwap - 0.1.0";
            Console.WriteLine("OcuSwap is now starting...");

            bool showMenu;
            do
            {
                showMenu = MainMenu();
            } while (showMenu);
        }

        static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to OcuSwap!\n\nSelect a task:\n1. Change Home environment (Dark Mode)\n2. Adjust Home horizon intensity\n3. Change Air Link panel image (Quest 1)\n4. Exit (typing anything else will exit as well)\n");
            Console.Write("Type a number and press Enter: ");
            switch (Console.ReadLine())
            {
                case "1":
                    return DarkMode();
                case "2":
                    return HorizonChange();
                case "3":
                    return AirLinkPanel();
                default:
                    return false;
            }
        }

        // This checks if the Oculus path exists in the registry and if so returns that, otherwise asks for the user to input it themselves.
        static string CheckPaths()
        {
            RegistryKey oculusKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Oculus VR, LLC\Oculus");
            string basePath = (string)oculusKey.GetValue("Base");
            if (Directory.Exists(basePath))
            {
                return basePath;
            }
            else
            {
                Console.WriteLine("\nLooks like Oculus software isn't installed in a way that we could locate it.\nPlease enter the location to your Oculus path and press Enter.\nExample: C:\\Program Files\\Oculus");
                return Console.ReadLine();
            }
        }

        static void KillOculus()
        {
            // Kill Oculus Dash before we do anything
            Console.Clear();
            if (Process.GetProcessesByName("OculusDash").Length > 0)
            {
                Console.WriteLine("Hey! We see the Oculus Dashboard is open.\nWe need to close this to apply the patches, so if you're in VR, take off your headset.\n\nPress any key to have us close the Oculus Dashboard...");
                Console.ReadKey();
                Console.WriteLine("We're closing the Oculus Dashboard so we can do our things...");
                foreach (var process in Process.GetProcessesByName("OculusDash"))
                {
                    process.Kill();
                }
                Console.Clear();
            }
        }

        static bool FinishedPrompt()
        {
            Console.Clear();
            Console.WriteLine("We're finished! Open Oculus Home to see your changes.\nWhat would you like to do?\n\n1. Return to main menu\n2. Exit\n");
            Console.Write("Type a number and press Enter: ");
            switch (Console.ReadLine())
            {
                case "1":
                    return true;
                default:
                    return false;
            }
        }

        // HERE BEGINS THE TASKS :D

        static bool DarkMode()
        {
            Console.Clear();
            string voidPath = CheckPaths() + @"\Support\oculus-dash\dash\assets\raw\textures\environment\the_void";
            string darkGridDDS = voidPath + @"\grid_plane_007.dds";
            string darkDotDDS = voidPath + @"\grid_plane_004.dds";
            string greyDotsDDS = voidPath + @"\grid_plane_003.dds";
            string lightGridDDS = voidPath + @"\grid_plane_006.dds";

            // Checking if our backup of white grid texture exists, if not backs up for restoration
            string lightGridDDSBackup = lightGridDDS + ".old";
            if (!File.Exists(lightGridDDSBackup))
            {
                File.Copy(lightGridDDS, lightGridDDSBackup);
            }

            string selectedDDS;
            string selectedDDSMessage;

            Console.WriteLine("So, what would you like to change the Oculus Home environment to?\n\n1. Black environment with grid\n2. Black environment with dots (my favorite)\n3. Grey environment with dots\n4. Restore white grid\n5. Back to main menu\n");
            Console.Write("Type a number and press Enter: ");
            switch (Console.ReadLine())
            {
                case "1":
                    selectedDDS = darkGridDDS;
                    selectedDDSMessage = "Black environment with grid";
                    break;
                case "2":
                    selectedDDS = darkDotDDS;
                    selectedDDSMessage = "Black environment with dots";
                    break;
                case "3":
                    selectedDDS = greyDotsDDS;
                    selectedDDSMessage = "Grey environment with dots";
                    break;
                case "4":
                    selectedDDS = lightGridDDSBackup;
                    selectedDDSMessage = "Restore white grid";
                    break;
                default:
                    return true;
            }

            Console.Clear();
            Console.WriteLine("You have selected: " + selectedDDSMessage);
            Console.WriteLine("Ready to apply?\n\n1. Apply\n2. Back to main menu\n");
            Console.Write("Type a number and press Enter: ");
            switch (Console.ReadLine())
            {
                case "1":
                    break;
                default:
                    return true;
            }
            KillOculus();
            Console.WriteLine("Applying your selected environment...");
            File.Copy(selectedDDS, lightGridDDS, true);
            return FinishedPrompt();
        }

        static bool HorizonChange()
        {
            string voidShaderPath = CheckPaths() + @"\Support\oculus-dash\dash\data\shaders\theVoid\theVoidUniforms.glsl";

            // Backup shader file just in case
            string voidShaderBackup = voidShaderPath + ".old";
            if (!File.Exists(voidShaderBackup))
            {
                File.Copy(voidShaderPath, voidShaderBackup);
            }

            string shaderContents = File.ReadAllText(voidShaderPath);

            string userSetIntensity;
            do
            {
                Console.Clear();
                Console.WriteLine("So, what would you like to change the Oculus Home horizon intensity to to?\n\nSuggested values\n0.0012 - default intensity\n0.00004 - suitable for dark environment (recommended)\n\nEnter your value as a decimal number with \".\" as the seperator.\n");
                Console.Write("Type a number and press Enter: ");

                userSetIntensity = Console.ReadLine();
                userSetIntensity = Regex.Match(userSetIntensity, @"[+-]?([0-9]*[.])?[0-9]+").ToString();
            } while (string.IsNullOrEmpty(userSetIntensity));

            string voidShaderPatched = Regex.Replace(shaderContents, @"(?<=float u_fogDensity = ).*?(?=;)", userSetIntensity);

            Console.Clear();
            Console.WriteLine("You have set: " + userSetIntensity);
            Console.WriteLine("Ready to apply?\n\n1. Apply\n2. Back to main menu\n");
            Console.Write("Type a number and press Enter: ");
            switch (Console.ReadLine())
            {
                case "1":
                    break;
                default:
                    return true;
            }
            KillOculus();
            Console.WriteLine("Applying your horizon intensity...");
            File.WriteAllText(voidShaderPath, voidShaderPatched);
            return FinishedPrompt();
        }

        static bool AirLinkPanel()
        {
            Console.Clear();

            string panelPath = CheckPaths() + @"\Support\oculus-dash\dash\data\js\assets\src\apps\dash\asset_files\experience-panels\oculus-airlink.png";

            // Checking if our backup of Quest 2 panel exists, if not backs up for restoration
            string quest2PanelBackup = panelPath + ".old";
            if (!File.Exists(quest2PanelBackup))
            {
                File.Copy(panelPath, quest2PanelBackup);
            }

            string selectedPanelMessage;
            bool applyPanel;

            Console.WriteLine("So, what would you like to change the Air Link panel to?\n\n1. Quest 1 image\n2. Restore Quest 2 image\n3. Back to main menu\n");
            Console.Write("Type a number and press Enter: ");
            switch (Console.ReadLine())
            {
                case "1":
                    applyPanel = true;
                    selectedPanelMessage = "Quest 1 image";
                    break;
                case "2":
                    applyPanel = false;
                    selectedPanelMessage = "Restore Quest 2 image";
                    break;
                default:
                    return true;
            }

            Console.Clear();
            Console.WriteLine("You have selected: " + selectedPanelMessage);
            Console.WriteLine("Ready to apply?\n\n1. Apply\n2. Back to main menu\n");
            Console.Write("Type a number and press Enter: ");
            switch (Console.ReadLine())
            {
                case "1":
                    break;
                default:
                    return true;
            }

            KillOculus();
            Console.WriteLine("Applying your panel image...");
            if (applyPanel)
            {
                File.WriteAllBytes(panelPath, OcuSwap.Resources.oculus_airlink);
            } else
            {
                File.Copy(quest2PanelBackup, panelPath, true);
            }
            return FinishedPrompt();
        }

    }
}
