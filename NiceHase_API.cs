using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Timers;
using System.Net;
using System.IO;


namespace ConsoleApp1
{
    class Program
    {
        static Timer myTimer = new System.Timers.Timer();
        static DateTime TimeNow;
        static byte Count;
        static Boolean IsOk = true;
        static string wallet = "35HUFFdAKdVpKFXm9Yj7N3FQGFVWwcPvRt";
        static void Main(string[] args)
        {    
            myTimer.Elapsed += timer1_Tick;
            myTimer.Interval = 1 * 60 * 1000;
            myTimer.Start();
            Console.WriteLine("Работаю");
            TimeNow = DateTime.Now;
            string sTimeNow = TimeNow.ToString();
            Console.WriteLine(sTimeNow.Substring(11, 5));
            GetAndShowSpeed(wallet);
            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();

        }
        public static void GetAndShowSpeed(string Wallet)
        {
            string Answer = GetNiceHashAPI(Wallet);
          //  richTextBox1.Text = Answer;
            int iNumStart, iNumEnd;
            string sFind = "\"a\":";
            iNumStart = Answer.IndexOf(sFind);
            string subAnswer, Speed;
            if (iNumStart > 0)
            {
                iNumEnd = Answer.IndexOf("\"}", iNumStart);
                subAnswer = Answer.Substring(iNumStart + 5, iNumEnd - iNumStart - 5);
                Speed = subAnswer;

            }
            else
            {
                Speed = "ФЕРМА СТОИТ!";
                IsOk = false;
                Count += 1;
                myTimer.Stop();
                myTimer.Interval = 1 * 60 * 1000;
                myTimer.Start();
                if (Count > 3) { Console.WriteLine("Ферма стоит!!!"); }
            }

            if (IsOk)
            {
                Console.WriteLine("Текущая скорость: {0} H/sec", Speed);
                Count = 0;
                myTimer.Stop();
                myTimer.Interval = 5 * 60 * 1000;
                myTimer.Start();
            }
            else
            {
                Console.WriteLine( Speed);
                Console.WriteLine(" {0} Проход",Count.ToString());
            }


        }

        private static string GetNiceHashAPI(string BTCaddr)
        {
            string HTTPS_URL = "https://api.nicehash.com/api";


            var postData = "?method=stats.provider.workers";
            postData += "&addr=" + BTCaddr;
            HTTPS_URL += postData;

            var data = Encoding.ASCII.GetBytes(postData);

          //  richTextBox1.Text = postData;
          //  richTextBox1.Text += Convert.ToChar(13);

            var request = (WebRequest)WebRequest.Create(HTTPS_URL);
            request.ContentType = "application/json"; // x - www - form - urlencoded";
            request.Method = "POST";

            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            var response = (WebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();
            string answer = responseString.Substring(0);
            return answer;
        }

        private static void timer1_Tick(object sender, EventArgs e)
        {
            TimeNow = DateTime.Now;
            string sTimeNow = TimeNow.ToString();
            Console.WriteLine( sTimeNow.Substring(11, 5));
            GetAndShowSpeed(wallet);
        }

    }
}
