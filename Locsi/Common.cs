using System;
using System.IO;
using Newtonsoft.Json;


namespace Locsi
{
    public class Common
    {
        private static Common instance;

        public static Common Instance{
            get
            {
                if (instance == null)
                {
                    instance = new Common();
                    LoadData();
                }
                return instance;
            }
        }

        public string LocsiHostname = "192.168.4.1";


        public static void SaveData()
        {
            Common inst = instance;
            string fileName = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "locsi.txt");
            string jsonString = JsonConvert.SerializeObject(inst);
            File.WriteAllText(fileName, jsonString);
        }

        public static void LoadData()
        {
            string fileName = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "locsi.txt");
            string jsonString;
            try
            {
                jsonString = File.ReadAllText(fileName);
                instance = JsonConvert.DeserializeObject<Common>(jsonString);
            }
            catch
            {
                SaveData();
            }
        }

    }
}