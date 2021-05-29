using DRGModdedSaveHelper.Config;
using DRGModdedSaveHelper.Saves;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DRGModdedSaveHelper.Watcher
{
    public class DRGWatcher
    {
        private SaveHelperConfig config;

        private Process process;

        private SaveFiles saves;

        public DRGWatcher(SaveHelperConfig config, string gamePath, string savePath)
        {
            this.config = config;

            saves = new SaveFiles(savePath);

            if (config.VerboseLogging)
            {
                Console.WriteLine("Vanilla saves:");
                foreach (var save in saves.GetVanillaSaves())
                {
                    Console.WriteLine(save.Path);
                }

                Console.WriteLine();

                Console.WriteLine("Modded saves:");
                foreach (var save in saves.GetModdedSaves())
                {
                    Console.WriteLine(save.Path);
                }

                Console.WriteLine();

                Console.WriteLine("Experimental saves:");
                foreach (var save in saves.GetExperimentalSaves())
                {
                    Console.WriteLine(save.Path);
                }
            }

            process = new Process();
            process.StartInfo.FileName = "steam://rungameid/548430";
        }

        public void StartDRG()
        {
            // Start DRG
            // Since we have to use a steam:// launch code, we cannot wait for it to exit (steam will immediately return exit code 0 while the game is still open).
            // Instead, sleep until it actually starts.
            Console.WriteLine("Starting Deep Rock Galactic.");
            process.Start();

            bool started = false;
            while(!started)
            {
                // FSD.exe seems to be a "secondary" process that launches the real game (such as FSD-Win64-Shipping.exe)
                // However, it stays open while the game is open, so it's good enough.
                var drg = Process.GetProcessesByName("FSD");
                if(drg != null && drg.Length > 0)
                {
                    process = drg[0];
                    started = true;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Deep Rock Galactic is running! Waiting for exit...");
            // Block indefinitely until DRG exits
            process.WaitForExit();
            Console.WriteLine("DRG closed. Copying save files...");

            var userIds = saves.GetUserIds();
            foreach (string userId in userIds)
            {
                if (config.CopyStrategy == CopyStrategy.Newest)
                {
                    saves.CopyNewestToOldest(userId);
                }
                else if (config.CopyStrategy == CopyStrategy.AlwaysModded)
                {
                    saves.CopyModdedToVanilla(userId);
                }
                else if (config.CopyStrategy == CopyStrategy.AlwaysVanilla)
                {
                    saves.CopyVanillaToModded(userId);
                }
                else
                {
                    Console.WriteLine("Invalid/unknown copyStrategy specified in the config JSON! Possible values are newest, alwaysModded, alwaysVanilla.");
                    Console.WriteLine("Nothing was copied for now. Please review the DRGModdedSaveHelper.json file, and try again.");
                    break;
                }
            }
        }
    }
}
