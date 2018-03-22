using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Modules
{
    class APIKeys
    {
        public static string DiscordAPIKey { get; private set; }
        public static string AlterVistaAPIKey { get; private set; }
        public static string RiotAPIKey { get; private set; }

        public static void RetrieveAPIKey()
        {
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"\keys.txt");

            foreach (string i in lines)
            {
                string[] temp = i.Split(new string[] { ":" }, StringSplitOptions.None);

                switch (temp[0])
                {
                    case "discord":
                        DiscordAPIKey = temp[1];
                        break;
                    case "altervista":
                    AlterVistaAPIKey = temp[1];
                        break;
                    case "riot":
                        RiotAPIKey = temp[1];
                        break;
                    default:
                        break;
                }
            }
        }        
    }
}
