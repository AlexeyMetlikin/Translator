using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator.Abstract
{
    public interface IApi
    {
        string Host { get; }    // Наименование сервера
        string SendRequest(string requestType, string request, List<KeyValuePair<string, string>> pars);    // Функция для отправки запроса на сервер
    }
}
