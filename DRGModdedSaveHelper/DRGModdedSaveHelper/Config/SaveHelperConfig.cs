using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DRGModdedSaveHelper.Config
{
    public class SaveHelperConfig
    {
        public CopyStrategy CopyStrategy { get; set; }

        public bool KeepConsoleWindowOpen { get; set; }

        public bool VerboseLogging { get; set; }

        private static JsonSerializerOptions serializerOpts = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        public static JsonSerializerOptions SerializerOptions
        {
            get
            {
                return serializerOpts;
            }
        }

        public static SaveHelperConfig GetConfigFromJson(string json)
        {
            var config = JsonSerializer.Deserialize<SaveHelperConfig>(json, SerializerOptions);

            return config;
        }
    }
}
