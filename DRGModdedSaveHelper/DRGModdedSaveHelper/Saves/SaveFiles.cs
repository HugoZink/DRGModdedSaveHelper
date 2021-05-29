using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRGModdedSaveHelper.Saves
{
    /// <summary>
    /// Represents all the save files that are currently in the savegames directory.
    /// </summary>
    public class SaveFiles : IEnumerable<SaveFile>
    {
        private Dictionary<string, SaveFile> saveFiles;

        private string saveDir;

        public SaveFiles(string directory)
        {
            if(directory == null)
            {
                throw new ArgumentNullException("directory", "Savegame directory path cannot be null!");
            }

            saveDir = directory;

            RescanSaveDirectory();
        }

        /// <summary>
        /// Rescans the savegame folder. Useful if savegames were added or deleted during runtime.
        /// </summary>
        public void RescanSaveDirectory()
        {
            saveFiles = new Dictionary<string, SaveFile>();

            string[] savePaths = Directory.GetFiles(saveDir, "*.sav");

            foreach (string path in savePaths)
            {
                saveFiles[path] = new SaveFile(path);
            }
        }

        /// <summary>
        /// Gets all vanilla saves that were found in the savegames folder.
        /// </summary>
        public IEnumerable<SaveFile> GetVanillaSaves()
        {
            return saveFiles.Values.Where(s => !s.Modded && !s.Experimental && !s.Backup);
        }

        /// <summary>
        /// Gets all modded saves that were found in the savegames folder.
        /// </summary>
        public IEnumerable<SaveFile> GetModdedSaves()
        {
            return saveFiles.Values.Where(s => s.Modded && !s.Experimental && !s.Backup);
        }

        /// <summary>
        /// Gets all experimental saves that were found in the savegames folder.
        /// </summary>
        public IEnumerable<SaveFile> GetExperimentalSaves()
        {
            return saveFiles.Values.Where(s => !s.Modded && s.Experimental && !s.Backup);
        }

        public SaveFile GetVanillaSaveByUserId(string userId)
        {
            return GetVanillaSaves().Where(s => s.UserId == userId).FirstOrDefault();
        }

        public SaveFile GetModdedSaveByUserId(string userId)
        {
            return GetModdedSaves().Where(s => s.UserId == userId).FirstOrDefault();
        }

        public SaveFile GetExperimentalSaveByUserId(string userId)
        {
            return GetExperimentalSaves().Where(s => s.UserId == userId).FirstOrDefault();
        }

        /// <summary>
        /// Takes a user ID's vanilla and modded save, and copies the newest save to the oldest.
        /// </summary>
        /// <param name="userId">The User ID to copy the saves for.</param>
        public void CopyNewestToOldest(string userId)
        {
            var vanillaSave = GetVanillaSaveByUserId(userId);
            if(vanillaSave == null)
            {
                Console.WriteLine("Vanilla save for user {0} not found! Skipping...", userId);
                return;
            }

            var moddedSave = GetModdedSaveByUserId(userId);
            if (moddedSave == null)
            {
                Console.WriteLine("Modded save for user {0} not found! Skipping...", userId);
                return;
            }

            if(vanillaSave.Modified == moddedSave.Modified)
            {
                Console.WriteLine("Vanilla and Modded saves for user {0} have identical Last Modified times, skipping...", userId);
                return;
            }
            else if(moddedSave.Modified > vanillaSave.Modified)
            {
                Console.WriteLine("Modded is newer than vanilla, copying modded to vanilla for user {0}.", userId);
                moddedSave.CopyContentsToOtherSave(vanillaSave);
            }
            else
            {
                Console.WriteLine("Vanilla is newer than modded, copying vanilla to modded for user {0}.", userId);
                vanillaSave.CopyContentsToOtherSave(moddedSave);
            }
        }

        /// <summary>
        /// Takes a user ID's modded save, and copies it to vanilla.
        /// </summary>
        /// <param name="userId">The User ID to copy the saves for.</param>
        public void CopyModdedToVanilla(string userId)
        {
            var vanillaSave = GetVanillaSaveByUserId(userId);
            if (vanillaSave == null)
            {
                Console.WriteLine("Vanilla save not found for user {0}! Skipping...", userId);
                return;
            }

            var moddedSave = GetModdedSaveByUserId(userId);
            if (moddedSave == null)
            {
                Console.WriteLine("Modded save not found for user {0}! Skipping...", userId);
                return;
            }

            Console.WriteLine("Copying modded save to vanilla for user {0}.", userId);
            moddedSave.CopyContentsToOtherSave(vanillaSave);
        }

        /// <summary>
        /// Takes a user ID's vanilla save, and copies it to modded.
        /// </summary>
        /// <param name="userId">The User ID to copy the saves for.</param>
        public void CopyVanillaToModded(string userId)
        {
            var vanillaSave = GetVanillaSaveByUserId(userId);
            if (vanillaSave == null)
            {
                Console.WriteLine("Vanilla save not found for user {0}! Skipping...", userId);
                return;
            }

            var moddedSave = GetModdedSaveByUserId(userId);
            if (moddedSave == null)
            {
                Console.WriteLine("Modded save not found for user {0}! Skipping...", userId);
                return;
            }

            Console.WriteLine("Copying modded save to vanilla for user {0}.", userId);
            vanillaSave.CopyContentsToOtherSave(moddedSave);
        }

        /// <summary>
        /// Get a set of all unique user ID's found in the save folder.
        /// </summary>
        public HashSet<string> GetUserIds()
        {
            var ids = new HashSet<string>();
            foreach(var save in saveFiles.Values)
            {
                ids.Add(save.UserId);
            }
            return ids;
        }

        public IEnumerator<SaveFile> GetEnumerator()
        {
            return saveFiles.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return saveFiles.Values.GetEnumerator();
        }
    }
}
