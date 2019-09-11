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

        public static string Get(string url,int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                //request.Timeout = timeout * 1000;
                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();
                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));
                result = sr.ReadToEnd().Trim();
                sr.Close();
                sr.Dispose();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                //Logger.Instance.Error("HttpService" + "Thread - caught ThreadAbortException - resetting.");
                //Logger.Instance.Error("Exception message: {0}" + e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                //Logger.Instance.Error("HttpService" + e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    //Logger.Instance.Error("HttpService" + "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    //Logger.Instance.Error("HttpService" + "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
    }
}
