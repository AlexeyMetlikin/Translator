using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Translator.Abstract;
using Translator.Entities;
using Translator.Settings;

namespace Translator
{
    public partial class TranslatorForm : Form
    {
        // config-файл
        private IniFile config = new IniFile("settings.ini");

        // создаем экземпляр класса TranslateAPI
        private IApiTranslator API;
        public TranslatorForm(IApiTranslator API)
        {
            InitializeComponent();
            this.API = API;
            ReadKey();                  // Считываем API-key из config-файл  
            GetLanguages();             // Получаем список языков из API
        }

        private void ReadKey()
        {
            if (config.KeyExists("Key", "API-Key"))
            {
                API.API_Key = config.ReadINI("API-Key", "Key");
            }
        }

        private void GetLanguages()
        {
            var pars = new List<KeyValuePair<string, string>>                   // Заполняем параметры для выполнения запроса
                {
                    new KeyValuePair<string, string>( "ui", "ru" ),             // На каком языке получить список
                    new KeyValuePair<string, string>( "key", API.API_Key )      // Ключ API
                };
            string response = Send("POST", "/api/v1.5/tr.json/getLangs", pars); // Выполняем запрос к API на получение списка языков

            if (response != null)   // Если ответ получен
            {
                try
                {
                    API.FillLanguages(response);    // Заполняем список языков в TranslateAPI
                    FillComboBoxes();               // Заполняем сomboBox'ы из TranslateAPI
                }
                catch (IndexOutOfRangeException exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }

        private void FillComboBoxes()
        {
            if (API.Languages != null)  // Если список языков не пуст
            {
                foreach (var lang in API.Languages)
                {
                    comboBox1.Items.Add(lang.Value);
                    comboBox2.Items.Add(lang.Value);
                }
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf("Английский");    // По умолчанию английский язык текущий
                comboBox2.SelectedIndex = comboBox2.Items.IndexOf("Русский");       // По умолчанию язык для перевода - русский
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(TryDetectLang(richTextBox1.Text));
                }
                richTextBox2.Text = Translate(richTextBox1.Text,
                                              API.Languages.Find(l => l.Value == comboBox1.Text).Key,
                                              API.Languages.Find(l => l.Value == comboBox2.Text).Key);
            }
            catch (ArgumentNullException exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        // Выполнить перевод текста translateFrom с языка langFrom на язык langTo
        public string Translate(string translateFrom, string langFrom, string langTo)
        {
            string lang = langFrom;
            if (langTo == null)
            {
                throw new ArgumentNullException("Не задан язык перевода");
            }
            if (lang == null)
            {
                lang = langTo;
            }
            else
            {
                lang += "-" + langTo;
            }

            string translation = "";

            // Если текст запроса превышает 10000 символов - разбить строку на подстроки и переводить  
            for (int i = 0; i < translateFrom.Length; i += 10000)   
            {
                var text = translateFrom;
                if (text.Length > 9999)
                {
                    text = text.Substring(i, i + 9999);
                }
                var pars = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "lang", lang ),       // Языки, с какого и на какой переводить в формате 'en-ru'   
                        new KeyValuePair<string, string>( "option", "1" ),      // Определяет, правильно ли указан язык, с которого нужно перевести
                        new KeyValuePair<string, string>( "text", text ),       // Текст для перевода
                        new KeyValuePair<string, string>( "key", API.API_Key )  // Ключ
                    };
                var response = Send("POST", "/api/v1.5/tr.json/translate", pars);   // Выполняем запрос
                
                if (response != null)
                {
                    translation += new Regex("\"text\":").Split(response)[1].Split('[', ']')[1].Replace("\"", "");  // Если запрос успешно выполнен - добавить перевод в результирующую строку
                }
            }
            return translation;
        }

        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)  // Если язык не выбран - определить автоматически
            {
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(TryDetectLang(richTextBox1.Text));
            }
        }

        // Определить язык исходного текста
        public string TryDetectLang(string textForDetect)   
        {
            var pars = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "hint", API.Languages.Find(l => l.Value == textForDetect).Key ),  // наиболее вероятный язык (языки) 'en,ru,...'
                    new KeyValuePair<string, string>( "text", textForDetect),   // Текст для определения языка
                    new KeyValuePair<string, string>( "key", API.API_Key )      // Ключ
                };
            string response = API.SendRequest("POST", "/api/v1.5/tr.json/detect", pars);

            if (response != null)
            {
                var langKey = new Regex("\"lang\":").Split(response)[1].Split('}')[0].Replace("\"", "");    // Если запрос успешно выполнен - получим язык текста
                return API.Languages.Find(l => l.Key == langKey).Value;
            }
            return null;
        }

        private string Send(string requestType, string request, List<KeyValuePair<string, string>> pars)
        {
            try
            {
                return API.SendRequest(requestType, request, pars);
            }
            catch (FormatException exp)
            {
                MessageBox.Show(exp.Message);
            }
            catch (HttpRequestException exp)
            {
                MessageBox.Show("Ошибка в результате запроса: " + exp.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            return null;
        }
    }
}
