using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translator.Abstract;
using Translator.Entities;
using Translator.Settings;
using System.Net.Http;
using System.Collections.Generic;

namespace Translator.Tests
{
    [TestClass]
    public class FormMethodsTest
    {
        [TestMethod]
        public void CanGetLanguages()
        {
            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=Key");
            file.Close();

            IApiTranslator API = new TranslateAPIUnderTest("host");
            TranslatorForm form = new TranslatorForm(API);

            Assert.AreEqual(API.Languages.Count, 3);
            Assert.AreEqual(API.Languages[0], new KeyValuePair<string, string>("en", "Английский"));
            Assert.AreEqual(API.Languages[1], new KeyValuePair<string, string>("ru", "Русский"));
            Assert.AreEqual(API.Languages[2], new KeyValuePair<string, string>("de", "Немецкий"));

            File.Delete("settings.ini");
        }

        [TestMethod]
        public void CanDetectLang()
        {
            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=Key");
            file.Close();

            IApiTranslator API = new TranslateAPIUnderTest("host");

            TranslatorForm form = new TranslatorForm(API);

            string lang = form.TryDetectLang("Привет");

            Assert.IsNotNull(lang);
            Assert.AreEqual(lang, "Русский");

            File.Delete("settings.ini");
        }

        [TestMethod]
        public void CanTranslateText()
        {
            StreamWriter file = new StreamWriter(new FileStream("settings.ini", FileMode.Create, FileAccess.Write));
            file.WriteLine("[API-Key]");
            file.WriteLine("Key=Key");
            file.Close();

            IApiTranslator API = new TranslateAPIUnderTest("host");

            TranslatorForm form = new TranslatorForm(API);

            string result = form.Translate("Test", "en", "ru");

            Assert.AreEqual(result, "Тест");

            File.Delete("settings.ini");
        }

        private class TranslateAPIUnderTest : IApiTranslator
        {
            public string API_Key { get; set; }

            public string Host { get; }

            public List<KeyValuePair<string, string>> Languages { get; }

            public TranslateAPIUnderTest(string host)
            {
                this.Host = host;
                Languages = new List<KeyValuePair<string, string>>();
            }

            public void FillLanguages(string languages)
            {
                Languages.Add(new KeyValuePair<string, string>("en", "Английский"));
                Languages.Add(new KeyValuePair<string, string>("ru", "Русский"));
                Languages.Add(new KeyValuePair<string, string>("de", "Немецкий"));
            }

            public string SendRequest(string requestType, string request, List<KeyValuePair<string, string>> pars)
            {
                switch (request)
                {
                    case "/api/v1.5/tr.json/detect":
                        return "{\"code\":200,\"lang\":\"ru\"}";

                    case "/api/v1.5/tr.json/translate":
                        return "{\"code\":200,\"lang\":\"ru-en\",\"text\":[Тест]";

                    case "/api/v1.5/tr.json/getLangs":
                        return "{\"dirs\":[\"ru-en\",\"ru-de\",\"en-ru\",\"en-de\",\"de-en\",\"de-ru\"],\"langs\":{\"ru\":\"Русский\",\"de\":\"Немецкий\",\"en\":\"Английский\"}}";

                    default:
                        return null;
                }
            }
        }
    }
}
