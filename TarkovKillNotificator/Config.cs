using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Diagnostics;

namespace TarkovKillNotificator
{
    class Config
    {
        private IniData configs;
        private FileIniDataParser parser;
        private String path;

        public Config(String paths = "")
        {
            if (String.IsNullOrEmpty(paths)) path = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) + ".ini";
            else path = paths;

            parser = new FileIniDataParser();

            if (File.Exists(path))
            {
                try
                {
                    configs = parser.ReadFile(path);
                }
                catch
                {
                    Reset();
                }
            }else
            {
                Reset();
            }


        }

        public void Reset()
        {
            configs = new IniData();

            configs["Default"]["HighlightsPath"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Temp", "Highlights", "Escape From Tarkov");
            configs["Default"]["CustomSoundPath"] = "";

            configs["Default"]["CustomSound"] = "False";
            configs["Default"]["AlwaysTop"] = "True";
            configs["Default"]["PlaySound"] = "True";

            parser.WriteFile(path, configs);
        }

        public void Save()
        {
            try
            {
                parser.WriteFile(path, configs);
            }
            catch
            {
                Debug.Print("Cannot save config file.");
            }
        }

        public string Get(String key)
        {
            return configs["Default"][key];
        }

        public void Set(String key, String value)
        {
            configs["Default"][key] = value;
            parser.WriteFile(path, configs);
        }
    }
}
