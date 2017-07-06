using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Translator.Settings
{
    public class IniFile
    {
        private string _path; //Имя файла.

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string _default, StringBuilder retVal, int size, string filePath);

        // записываем пусть до файла и его имя.
        public IniFile(string iniPath)
        {
            _path = new FileInfo(iniPath).FullName.ToString();
        }

        // читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string ReadINI(string section, string key)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section.ToLower(), key.ToLower(), "", retVal, 255, _path);
            return retVal.ToString();
        }

        // проверяем, есть ли ключ в секции
        public bool KeyExists(string key, string section = null)
        {
            return ReadINI(section, key).Length > 0;
        }
    }
}