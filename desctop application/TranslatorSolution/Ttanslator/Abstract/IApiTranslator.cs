using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator.Abstract
{
    public interface IApiTranslator : IApi
    {
        string API_Key { get; set; }                // Свойство получения/заполнения ключа API
        List<KeyValuePair<string, string>> Languages { get; }   // Свойство получения списка доступных языков  
        void FillLanguages(string languages);       // Заполнение списка доступных языков
    }
}
