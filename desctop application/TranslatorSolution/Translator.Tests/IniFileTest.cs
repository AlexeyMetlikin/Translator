using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translator.Abstract;
using Translator.Entities;
using Translator.Settings;

namespace Translator.Tests
{
    [TestClass]
    public class IniFileTest
    {
        [TestMethod]
        public void CanTakeKeyFromValidIniFile()
        {
            Random rnd = new Random();
            string key = "";
            for (int i = 0; i < 20; i++)
            {
                key = String.Concat(key, rnd.Next(10).ToString());
            }

            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=" + key);
            file.Close();

            IApiTranslator API = new TranslateAPI("Host");
            IniFile config = new IniFile("settings.ini");

            if (config.KeyExists("Key", "API-Key"))
            {
                API.API_Key = config.ReadINI("API-Key", "Key");
            }

            Assert.AreEqual(key, API.API_Key);

            File.Delete("settings.ini");
        }

        [TestMethod]
        public void CannotTakeKeyFromInvalidIniFile()
        {
            IApiTranslator API = new TranslateAPI("Host");
            IniFile config = new IniFile("settings.ini");

            if (config.KeyExists("Key", "API-Key"))
            {
                API.API_Key = config.ReadINI("API-Key", "Key");
            }

            Assert.IsNull(API.API_Key);
        }

        [TestMethod]
        public void CannotTakeKeyFromIniWithInvalidSection()
        {
            Random rnd = new Random();
            string key = "";
            for (int i = 0; i < 20; i++)
            {
                key = String.Concat(key, rnd.Next(10).ToString());
            }

            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=" + key);
            file.Close();

            IApiTranslator API = new TranslateAPI("Host");
            IniFile config = new IniFile("settings.ini");

            if (config.KeyExists("Key", "API"))
            {
                API.API_Key = config.ReadINI("API", "Key");
            }

            Assert.IsNull(API.API_Key);
        }

        [TestMethod]
        public void CannotTakeKeyFromIniWithInvalidKey()
        {
            Random rnd = new Random();
            string key = "";
            for (int i = 0; i < 20; i++)
            {
                key = String.Concat(key, rnd.Next(10).ToString());
            }

            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=" + key);
            file.Close();

            IApiTranslator API = new TranslateAPI("Host");
            IniFile config = new IniFile("settings.ini");

            if (config.KeyExists("API-Key", "API-Key"))
            {
                API.API_Key = config.ReadINI("API-Key", "API-Key");
            }

            Assert.IsNull(API.API_Key);
        }
    }
}
