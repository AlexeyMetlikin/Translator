using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translator.Abstract;
using Translator.Entities;
using Translator.Settings;
using System.Net.Http;

namespace Translator.Tests
{
    [TestClass]
    public class FormMethodsTest
    {
        [TestMethod]
        public void CanDetectLang()
        {
            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=trnsl.1.1.20170705T051451Z.be2ac132d81993cd.d05ddc65d1cb87498519a919b46278f54265b15c");
            file.Close();

            TranslatorForm form = new TranslatorForm();

            string lang = form.TryDetectLang("Привет");

            Assert.AreEqual(lang, "Русский");

            File.Delete("settings.ini");
        }

        [TestMethod]
        public void CanTranslateText()
        {
            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=trnsl.1.1.20170705T051451Z.be2ac132d81993cd.d05ddc65d1cb87498519a919b46278f54265b15c");
            file.Close();

            TranslatorForm form = new TranslatorForm();

            string result = form.Translate("Test", "en", "ru");

            Assert.AreEqual(result, "Тест");

            File.Delete("settings.ini");
        }

        [TestMethod]
        public void CanTranslateTextWithAutoDetectLang()
        {
            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=trnsl.1.1.20170705T051451Z.be2ac132d81993cd.d05ddc65d1cb87498519a919b46278f54265b15c");
            file.Close();

            TranslatorForm form = new TranslatorForm();

            string result = form.Translate("Test", null, "ru");

            Assert.AreEqual(result, "Тест");

            File.Delete("settings.ini");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotTranslateTextWithoutLang()
        {
            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=trnsl.1.1.20170705T051451Z.be2ac132d81993cd.d05ddc65d1cb87498519a919b46278f54265b15c");
            file.Close();

            TranslatorForm form = new TranslatorForm();

            string result = form.Translate("Test", null, null);

            Assert.AreEqual(result, "");
        }
    }
}
