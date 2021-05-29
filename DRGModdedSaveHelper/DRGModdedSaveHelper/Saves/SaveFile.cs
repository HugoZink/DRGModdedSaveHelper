using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DRGModdedSaveHelper.Saves
{
    /// <summary>
    /// Represents a single save file.
    /// </summary>
    public class SaveFile
    {
        public string Path { get; private set; }

        public string UserId { get; private set; }

        public bool Modded { get; private set; } = false;

        public bool Experimental { get; private set; } = false;

        public bool Backup { get; private set; } = false;

        public byte[] Contents
        {
            get
            {
                return File.ReadAllBytes(Path);
            }
            private set
            {
                File.WriteAllBytes(Path, value);
            }
        }

        public DateTime Modified
        {
            get
            {
                return File.GetLastWriteTimeUtc(Path);
            }
        }

        public SaveFile(string path)
        {
            if(path == null)
            {
                throw new ArgumentNullException("path", "Save path cannot be null!");
            }

            Path = path;

            string filename = System.IO.Path.GetFileName(path);
            string[] filenameComponents = filename.Split('_');
            UserId = filenameComponents[0];

            foreach(string component in filenameComponents)
            {
                if(component.Equals("Experimental", StringComparison.OrdinalIgnoreCase))
                {
                    Experimental = true;
                }
                if (component.Equals("Modded", StringComparison.OrdinalIgnoreCase))
                {
                    Modded = true;
                }
                if (component.Equals("Backup", StringComparison.OrdinalIgnoreCase) || component.Equals("ExternalBackup", StringComparison.OrdinalIgnoreCase))
                {
                    Backup = true;
                }
            }
        }

        /// <summary>
        /// Copies the contents of this savegame to otherSave.
        /// </summary>
        /// <param name="otherSave">The savegame to copy the data towards.</param>
        public void CopyContentsToOtherSave(SaveFile otherSave)
        {
            otherSave.Contents = this.Contents;
        }
    }
}
