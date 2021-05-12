using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoNotifier
{
    class LogManager
    {
        private string filePath;
        private static LogManager instance;

        public static LogManager GetInstance()
        {
            if (instance == null)
                instance = new LogManager();
            return instance;
        }

        public LogManager()
        {
            string startupPath = Directory.GetCurrentDirectory();
            filePath = startupPath + "\\CryptoCreationLog.txt";
        }

        public void Log(string message)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
        }
}
}
