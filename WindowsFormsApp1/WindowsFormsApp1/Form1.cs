using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;

// Use HiveOS API
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Timer myTimer = new System.Windows.Forms.Timer();
        DateTime TimeNow;
        byte Count;
        Boolean IsOk=true;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Your secret API key, you sign the message with it;
            // @var string 64 hex chars;
            String SECRET_KEY = "bd041ca7901bd51a2ac865b8562d28f27ecc78836fae8e03a149da22fa895d22";

            // This is a public key
            // @var string 64 hex chars
            String PUBLIC_KEY = "8420bc5541866675a4c719d9ecb5670073ff8d9fd27f4e378eb5b31b4d487b27";

            // Api connection URL
            // Please use HTTPS, not HTTP for calls for better security.
            // @var string
            String HTTPS_URL = "https://api.hiveos.farm/worker/eypiay.php";

            richTextBox1.Text = "";
            // const String CorrectAuthText = "Авторизация прошла успешно";

            var postData = "";
            //var json= "{\"metod\":\"getRigs\",\"public_key\":\"8420bc5541866675a4c719d9ecb5670073ff8d9fd27f4e378eb5b31b4d487b27\"}";
            postData = "?method=getRigs&public_key=8420bc5541866675a4c719d9ecb5670073ff8d9fd27f4e378eb5b31b4d487b27";

            var data = Encoding.ASCII.GetBytes(postData);
            string SubString = EncodeHMAC(SECRET_KEY, data);

            HTTPS_URL += postData;
            var request = (WebRequest)WebRequest.Create(HTTPS_URL);
            request.ContentType = "application/form-data"; // x - www - form - urlencoded";
            request.Method = "POST";

            richTextBox1.Text = postData + Convert.ToChar(13);
            richTextBox1.Text += "HMAC:" + SubString;
            richTextBox1.Text += Convert.ToChar(13);

            postData = "";
            data = Encoding.ASCII.GetBytes(postData);

            //  request.Headers["HMAC"] = SubString;
            request.ContentLength = data.Length;
            string GettingString, Sid;
            GettingString = " ";
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();

            }


            string answer = "";

            try
            {
                var response = (WebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                answer = responseString.Substring(0);

            }
            catch (WebException ex)
            {

                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                GettingString += Convert.ToChar(13) + "Error ";
                int code = (int)httpResponse.StatusCode;
                GettingString += code;
                // Console.WriteLine("Статусный код ошибки: {0} - {1}",(int)httpResponse.StatusCode, httpResponse.StatusCode);
            }

            richTextBox1.Text += GettingString + Convert.ToChar(13) + answer;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            myTimer.Tick += new EventHandler(timer1_Tick);
            myTimer.Interval = 1 * 60 * 1000;
            myTimer.Start();
            label3.Text = "Работаю";
            TimeNow = DateTime.Now;
            string sTimeNow = TimeNow.ToString();
            label4.Text = sTimeNow.Substring(11,5);
            GetAndShowSpeed();
        }
        public void GetAndShowSpeed()
        {
            string Answer = GetNiceHashAPI(textBoxLogin.Text);
            richTextBox1.Text = Answer;
            int iNumStart,iNumEnd;
            string sFind = "\"a\":";
            iNumStart = Answer.IndexOf(sFind);
            string subAnswer, Speed;
            if (iNumStart>0)
            {
                iNumEnd = Answer.IndexOf("\"}",iNumStart);
            subAnswer = Answer.Substring(iNumStart+ 5,iNumEnd-iNumStart-5 );
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
                if (Count>3) { MessageBox.Show("Ферма стоит!!!"); }
            }

            if (IsOk)
                {
                label2.Text = "Текущая скорость:" + Speed + " H/sec";
                Count = 0;
                myTimer.Stop();
                myTimer.Interval = 5 * 60 * 1000;
                myTimer.Start();
                }
            else
            {
                label2.Text = Speed;
                label2.Text += " " + Count.ToString() + " Проход";
            }

           
        }

        private string GetNiceHashAPI(string BTCaddr)
        {
            string HTTPS_URL = "https://api.nicehash.com/api";


            var postData = "?method=stats.provider.workers";
            postData += "&addr=" + BTCaddr;
            HTTPS_URL += postData;

            var data = Encoding.ASCII.GetBytes(postData);

            richTextBox1.Text = postData;
            richTextBox1.Text += Convert.ToChar(13);

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

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            var request = (HttpWebRequest)WebRequest.Create("http://hosting.logictrail.com.ua/oauth.html");
            var postData = "lang = ru";
            postData += "&user=ZSTteplovoz";
            postData += "&response_type=token";
            postData += "&client_id=Wialon";
            postData += "&redirect_uri=http://hosting.logictrail.com.ua/login.html";
            postData += "&access_type=100";// 0x100
            postData += "&activation_time=0";
            postData += "&duration=2592000";
            postData += "&flags=6";
            postData += "&login=ZSTteplovoz";
            postData += "&passw=ZSTteplovoz";
            postData += "&sign=" + label3.Text;

            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application / x - www - form - urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            string GettingString, Sid;
            GettingString = responseString.Substring(0);
            richTextBox1.Text = GettingString;
            Sid = "";
        }
        public string EncodeHMAC(string InKey, byte[] Data)
        {
            Encoding enc = Encoding.UTF8;

            byte[] key = enc.GetBytes(InKey);
            // byte[] data = enc.GetBytes(Data);

            HMACSHA256 sha256_HMAC = new HMACSHA256(key);

            byte[] hashValue = sha256_HMAC.ComputeHash(Data);

            StringBuilder hex = new StringBuilder(hashValue.Length);
            foreach (byte b in hashValue)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeNow = DateTime.Now;
            string sTimeNow = TimeNow.ToString();
            label4.Text = sTimeNow.Substring(11,5);
            GetAndShowSpeed();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           myTimer.Stop();
            label3.Text = "Остановлен";
        }
    }
}
