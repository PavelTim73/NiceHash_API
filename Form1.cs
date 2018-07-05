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


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            // const String CorrectAuthText = "Авторизация прошла успешно";
            var request = (HttpWebRequest)WebRequest.Create("http://hosting.logictrail.com.ua/login.html");
          //  var request = (HttpWebRequest)WebRequest.Create("http://hosting.logictrail.com.ua/wialon/ajax.html");
            var postData = "lang=ru";
           // postData += "&svc=core/get_account_data";
            postData += "&user=" +textBoxLogin.Text;
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application / x - www - form - urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            { stream.Write(data, 0, data.Length);
              stream.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            string GettingString, Sid;
            GettingString = responseString.Substring(0);
            richTextBox1.Text = GettingString;
            int iNum;
            iNum = GettingString.IndexOf("sign");
            Sid = GettingString.Substring(iNum + 13, 44 );
            label3.Text = Sid;
        }

        private void button2_Click(object sender, EventArgs e)
        {
           webBrowser1.Navigate("https://hosting.logictrail.com.ua");
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
            { stream.Write(data, 0, data.Length);
              stream.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            string GettingString, Sid;
            GettingString = responseString.Substring(0);
            richTextBox1.Text = GettingString;
            Sid = "";
        }
    }
}
