using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ConsoleApp1
{
    public class HttpServiceHelper
    {
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }
        public static string RandomUseAgent()
        {
            var list = new List<string>();
            list.Add("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.163 Safari/535.1");
            list.Add("Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
            list.Add("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:6.0) Gecko/20100101 Firefox/6.0");
            list.Add("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50");
            list.Add("Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.9.168 Version/11.50");
            list.Add("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 2.0.50727; SLCC2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; Tablet PC 2.0; .NET4.0E)");
            list.Add("Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; InfoPath.3)");
            list.Add("Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; GTB7.0)");
            list.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)");
            list.Add("Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)");
            list.Add("Mozilla/5.0 (Windows; U; Windows NT 6.1; ) AppleWebKit/534.12 (KHTML, like Gecko) Maxthon/3.0 Safari/534.12");
            list.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E)");
            list.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E; SE 2.X MetaSr 1.0)");
            var max = list.Count - 1;
            var r = new Random().Next(max);
            return list[r];
        }
        public static string Get(string url, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader sr = null;
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 512;
                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.KeepAlive = false;
                request.Timeout = timeout * 1000;
                request.Headers.Add(HttpRequestHeader.UserAgent, RandomUseAgent());
                //获取服务端返回
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    //获取服务端返回数据
                    using (sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312")))
                    {

                        result = sr.ReadToEnd().Trim();
                    }
                    //using (sr = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    //{

                    //    result = sr.ReadToEnd().Trim();
                    //}
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            }
            return result;
        }

        public static string PolicyGet(string url)
        {
            var response = string.Empty;
            PolicyHelper.RetryForever(() =>
            {
                response = Get(url, 2000);
            });
            return response;
        }
    }
}
