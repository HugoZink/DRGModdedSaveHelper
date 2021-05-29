using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using DRGModdedSaveHelper.Saves;
using DRGModdedSaveHelper.Config;
using DRGModdedSaveHelper.Watcher;

namespace DRGModdedSaveHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string configPath = currentDirectory + "DRGModdedSaveHelper.json";
            SaveHelperConfig config;
            try
            {
                string json = File.ReadAllText(configPath);
                config = SaveHelperConfig.GetConfigFromJson(json);
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine("Config not found, generating a fresh one.");
                config = GenerateDefaultConfig();
            }

            // Get save directory from program arguments.
            // If not specified, use the default save directory relative to FSD.exe.
            string saveDirectory;
            if(args.Length > 0)
            {
                saveDirectory = args[0];
            }
            else
            {
                saveDirectory = currentDirectory + "FSD\\Saved\\SaveGames";
            }

            try
            {
                // Create the process watcher
                var watcher = new DRGWatcher(config, currentDirectory, saveDirectory);
                // Start DRG and wait indefinitely until it closes
                watcher.StartDRG();

                if (config.KeepConsoleWindowOpen)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to quit...");
                    Console.ReadKey();
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("ERROR: Could not find savegame directory!");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("Tried to find saves at path: {0}", saveDirectory);
                Console.WriteLine("Make sure this program is run in your Deep Rock Galactic directory (next to FSD.exe).");
                Console.WriteLine("You can manually specify your savegame directory by running this program with the save path as first argument.");
                Console.WriteLine();
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
            }
        }

        private static SaveHelperConfig GenerateDefaultConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "DRGModdedSaveHelper.json";
            var config = new SaveHelperConfig();
            config.CopyStrategy = CopyStrategy.Newest;
            config.KeepConsoleWindowOpen = false;
            config.VerboseLogging = false;

            string json = JsonSerializer.Serialize(config, SaveHelperConfig.SerializerOptions);
            File.WriteAllText(configPath, json);

            return config;
        }
    }
}
