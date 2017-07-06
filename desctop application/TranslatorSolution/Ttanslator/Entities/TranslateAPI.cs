using System;
using System.Collections.Generic;
using System.Net.Http;
using Translator.Abstract;
using System.Text.RegularExpressions;
using System.Net;

namespace Translator.Entities
{
    public class TranslateAPI : IApiTranslator
    {
        public string Host { get; }
        public string API_Key { get; set; }
        public List<KeyValuePair<string, string>> Languages { get; }

        public TranslateAPI(string host)
        {
            this.Host = host;
            Languages = new List<KeyValuePair<string, string>>();
        }

        public string SendRequest(string requestType, string request, List<KeyValuePair<string, string>> pars)
        {
            string result = null;

            switch (requestType.ToUpper())
            {
                case "POST":
                    FormUrlEncodedContent content = null;
                    if (pars != null)
                    {
                        content = new FormUrlEncodedContent(pars);
                    }
                    result = SendPOST(request, content);
                    break;

                default:
                    throw new HttpRequestException("Не распознан метод запроса '" + requestType + "'");
            }

            return result;
            
        }

        private string SendPOST(string request, FormUrlEncodedContent content)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(Host + request, content).Result;
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public void FillLanguages(string response)
        {
            Regex reg = new Regex("\"langs\":{");
            string languages = reg.Split(response)[1].Split('}')[0];
            foreach (var lang in languages.Split(','))
            {
                Languages.Add(new KeyValuePair<string, string>(lang.Split(':')[0].Replace("\"", ""), lang.Split(':')[1].Replace("\"", "")));
            }
        }
    }
}
