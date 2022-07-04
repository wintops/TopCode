using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using WAPIWrapperCSharp;
using WindCommon;

namespace ConsoleApplication1
{
    class Program
    {

       const string filename= @"\data\op.txt";
        const string filename1 = @"\data\options.txt";
        const string filename0 = @"\data\symbols.txt";
        const string file_symbols1 = @"\data\symbols1.txt";
        const string tl_host = "https://api.wmcloud.com";
        //10000631.SH,10000632.SH,10000633.SH,10000634.SH,10000635.SH,10000641.SH,10000643.SH,10000657.SH,10000636.SH,10000637.SH,10000638.SH,10000639.SH,10000640.SH,10000642.SH,10000644.SH,10000658.SH,10000645.SH,10000646.SH,10000647.SH,10000648.SH,10000649.SH,10000655.SH,10000659.SH,10000650.SH,10000651.SH,10000652.SH,10000653.SH,10000654.SH,10000656.SH,10000660.SH,10000569.SH,10000555.SH,10000556.SH,10000557.SH,10000558.SH,10000559.SH,10000571.SH,10000573.SH,10000591.SH,10000595.SH,10000599.SH,10000570.SH,10000560.SH,10000561.SH,10000562.SH,10000563.SH,10000564.SH,10000572.SH,10000574.SH,10000592.SH,10000596.SH,10000600.SH,10000629.SH,10000625.SH,10000615.SH,10000616.SH,10000617.SH,10000618.SH,10000619.SH,10000661.SH,10000630.SH,10000626.SH,10000620.SH,10000621.SH,10000622.SH,10000623.SH,10000624.SH,10000662.SH";

        static void Main(string[] args)
        {
            get_tldata();
        }

        private static void get_wddata()
        {
            string symbols = System.IO.File.ReadAllText(filename0);// "510050.sh,000016.sh,IH1607.CFE,IH1608.CFE,IH1609.CFE,IH1612.CFE";
            

            WindAPI w = new WindAPI();
            w.start();

            //string symbols = "510050.sh,000016.sh,10000631.sh,IH1607.CFE,IH1608.CFE,IH1609.CFE,IH1612.CFE,10000631.SH";
            //WindData wd = w.wsd("600000.SH", "MACD", "ED-3M", "2016-07-01", "MACD_L=26;MACD_S=12;MACD_N=9;MACD_IO=1;Fill=Previous");
         
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            WindData wd = w.wset("optionchain", "date="+today+";us_code=510050.SH;option_var=全部;month=全部;call_put=全部");
            
            string str = WindDataMethod.WindDataToString(wd, "wsq");

            
            System.IO.File.WriteAllText(filename1, str);
            Console.Write(str);

            string ops="";
            using (StreamReader sr = new StreamReader(filename1))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string[] parts = sr.ReadLine().Split('\t');
                    ops=ops+','+ parts[4];
                }
            }

            symbols = symbols + ops;
            Console.Write(symbols);

            wd = w.wsq(symbols, "rt_open,rt_high,rt_low,rt_last,rt_vol ","");
            str = WindDataMethod.WindDataToString(wd, "wsq");
            System.IO.File.WriteAllText(filename, str);

            w.wsq(symbols, "rt_open,rt_high,rt_low,rt_last,rt_vol ","", wsqCallback);
            //"IH1607.CFE,IH1608.CFE,IH1609.CFE,IH1612.CFE", "rt_last,rt_vol,rt_latest",
            //Timer t = new Timer(TimerCallback, null, 0, 2000);
            Console.WriteLine("waiting for data " + DateTime.Now);


            Console.Read();

            w.stop();
        }


        private static void decoderesult(ref string result, System.Net.HttpWebResponse response, string encoding)
        {
            using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
            {
                using (StreamReader StreamReaderreader = new StreamReader(stream, Encoding.GetEncoding("gb2312")))
                {
                    result = StreamReaderreader.ReadToEnd();
                }
            }
        }


        private static void tl_request(string req, string param, string fileName)
        {


            string url = tl_host + "/data/v1/api/" + req + ".csv?" + param;

            System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string token = "c8ea1aaeddfe3b750e15f538223f67b9e24499f96265b06d0500a2d891d99b8d";//此处更换token
            request.Headers["Authorization"] = "Bearer " + token;
            request.Headers["Accept-Encoding"] = "gzip";//数据压缩传输
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            string responseBody = string.Empty;
            if (encoding == "gzip") { 
            decoderesult(ref responseBody, response, encoding);
            }
            else using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseBody = reader.ReadToEnd();
                }

            File.WriteAllText(fileName, responseBody);
            response.Close();
        }

        private static void get_tldata()
        {
            string symbols = System.IO.File.ReadAllText(file_symbols1);
            string today = DateTime.Now.ToString("yyyyMMdd");

            Console.WriteLine("ready data " + DateTime.Now);
            tl_request("market/getTickRTSnapshot", "tradeDate=" + today + "&securityID=" + symbols, @"\data\TickRTSnapshot.txt");
            tl_request("market/getOptionTickRTSnapshot", "", @"\data\OptionTickRTSnapshot.txt");
            tl_request("market/getFutureTickRTSnapshot", "", @"\data\FutureTickRTSnapshot.txt");//instrumentID=IH1607,IH1608,IH1609,IH1612,
            tl_request("options/getOptVar", "exchangeCD=XSHG", @"\data\OptVar.txt");
            tl_request("future/getFutu", "exchangeCD=XSGE", @"\data\Futu.txt");

            Console.WriteLine("get data " + DateTime.Now);
            Console.Read();

        }

        private static  void wsqCallback(WindData wd)
        {
            string str = WindDataMethod.WindDataToString(wd, "wsq");
            System.IO.File.WriteAllText(filename, str);
            //Console.Write(str);
            
            // Display the date/time when this method got called.
            Console.WriteLine("update: " + DateTime.Now);
           
            // Force a garbage collection to occur for this demo.
            GC.Collect();
        }


        private static void TimerCallback(Object o)
        {
            // Display the date/time when this method got called.
            Console.WriteLine("In TimerCallback: " + DateTime.Now);
            // Force a garbage collection to occur for this demo.
            GC.Collect();
        }
    }
}
