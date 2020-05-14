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
            PolicyHelper.RetryForever(() =>
            {
                var html = HttpServiceHelper.Get(url, 2);
                doc.LoadHtml(html);
            });
            Thread.Sleep(100);
            return doc;
        }
    }
}
