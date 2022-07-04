using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        string filename;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            filename = @"\\88888-PC\data\op.txt";
        }


        static void decoderesult(ref string result, System.Net.HttpWebResponse response)
        {
            using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
            {
                using (StreamReader StreamReaderreader = new StreamReader(stream))
                {
                    result = StreamReaderreader.ReadToEnd();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            DateTime modification = File.GetLastWriteTime(filename);
            label1.Text = modification.ToString();
            //string sourcePath = @"C:\inetpub\wwwroot";
            //string destinationPath = @"G:\ProjectBO\ForFutureAnalysis";
            //string sourceFileName = "startingStock.xml";
            //string destinationFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml"; // Don't mind this. I did this because I needed to name the copied files with respect to time.

            //string sourceFile = System.IO.Path.Combine(sourcePath, sourceFileName);
            // string destinationFile = System.IO.Path.Combine(destinationPath, destinationFileName);

            if(File.GetLastWriteTime("op.txt")!= modification)    System.IO.File.Copy( filename , "op.txt", true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new Form1()).Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string host = "https://api.wmcloud.com/data/v1";
            string url = "/api/market/getMktEqud.csv?tradeDate=20160802";// &endDate=&secID=&ticker=000001&tradeDate=";//000001日线数据
            url = host + url;
            System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string token = "c8ea1aaeddfe3b750e15f538223f67b9e24499f96265b06d0500a2d891d99b8d";//此处更换token
            request.Headers["Authorization"] = "Bearer " + token;
            request.Headers["Accept-Encoding"] = "gzip";//数据压缩传输
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            string responseBody = string.Empty;
            decoderesult(ref responseBody, response);
            //Console.Write
               MessageBox.Show(responseBody);
            //MessageBox.Show(response.ContentLength.ToString());
            response.Close();
        }
    }
}
