using Microsoft.VisualStudio.TestTools.UnitTesting;
using Translator.Abstract;
using Translator.Entities;
using System.Collections.Generic;
using System.Net.Http;

namespace Translator.Tests
{
    [TestClass]
    public class TranslatorAPITest
    {
        [TestMethod]
        public void CanGetResponsetWithValidKey()
        {
            IApiTranslator API = new TranslateAPI("https://translate.yandex.net");
            API.API_Key = "trnsl.1.1.20170705T051451Z.be2ac132d81993cd.d05ddc65d1cb87498519a919b46278f54265b15c";

            var pars = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "ui", "ru" ),
                    new KeyValuePair<string, string>( "key", API.API_Key )
                };
            string response = API.SendRequest("POST", "/api/v1.5/tr.json/getLangs", pars);
            API.FillLanguages(response);

            Assert.IsTrue(response.Contains("\"dirs\""));
            Assert.IsTrue(response.Contains("\"langs\""));
            Assert.IsNotNull(API.Languages);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void CannotGetResponsesWithInvalidKey()
        {
            IApiTranslator API = new TranslateAPI("https://translate.yandex.net");
            API.API_Key = "Key";

            var pars = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "ui", "ru" ),
                    new KeyValuePair<string, string>( "key", API.API_Key )
                };
            string response = API.SendRequest("POST", "/api/v1.5/tr.json/getLangs", pars);
        }

        [TestMethod]
        public void CanFillLanguagesFromValidString()
        {
            string languages = "\"dirs\":[\"ru-en\",\"en-de\"],\"langs\":{\"ru\":\"Русский\",\"en\":\"Английский\",\"de\":\"Немецкий\"}";
            IApiTranslator API = new TranslateAPI("Host");

            API.FillLanguages(languages);

            Assert.AreEqual(API.Languages[0].Key, "ru");
            Assert.AreEqual(API.Languages[1].Key, "en");
            Assert.AreEqual(API.Languages[2].Key, "de");

            Assert.AreEqual(API.Languages[0].Value, "Русский");
            Assert.AreEqual(API.Languages[1].Value, "Английский");
            Assert.AreEqual(API.Languages[2].Value, "Немецкий");
        }

        [TestMethod]
        [ExpectedException(typeof(System.IndexOutOfRangeException))]
        public void CannotFillLanguagesFromInvalidString()
        {
            string languages = "langs: ru: Русский, en: Английский, de: Немецкий";
            IApiTranslator API = new TranslateAPI("Host");

            API.FillLanguages(languages);
        }
    }
}
