using HtmlAgilityPack;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public static class HtmlHelper
    {
        public static HtmlDocument GetDocument(string url)
        {
            GC.Collect();
            HtmlDocument doc = new HtmlDocument();
            Policy.Handle<Exception>()   //指定需要重试的异常类型
                .RetryForever((ex, count, context) =>
                {     //指定发生异常重试的次数
                    Console.WriteLine($"重试次数{count},异常{ex.Message}");
                })
                .Execute(() =>
                {
                    var html = HttpServiceHelper.Get(url, 2);
                    doc.LoadHtml(html);
                });    //要执行的方法
            Thread.Sleep(100);
            return doc;
        }
    }
}
