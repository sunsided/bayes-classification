using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsSpam
{
    class Program
    {
        static void Main(string[] args)
        {
            const string path = @".\data\SMSSpamCollection";
            var fileInfo = new FileInfo(path);
            var dataSet = new SmsDataReader(fileInfo);
            foreach (var sms in dataSet)
            {
                Console.WriteLine(sms.Content);
            }

            if (!Debugger.IsAttached) return;
            Console.WriteLine("Press key to exit.");
            Console.ReadKey(true);
        }
    }
}
