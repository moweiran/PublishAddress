using ConsoleApp1.DbHelper;
using Dapper;
using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ProvinceManage
    {
        public List<Models.Base_Provinces> Handle(string url)
        {
            List<Models.Base_Provinces> provinces = new List<Base_Provinces>();
            using (IDbConnection conn = DBHelper.Connection)
            {
                string sQuery = "SELECT Id,Code,ProvinceId,ProvinceName FROM Base_Provinces";
                conn.Open();
                provinces = conn.Query<Base_Provinces>(sQuery).ToList();
            }
            if (provinces.Count == 0)
            {
                HtmlDocument doc = HtmlHelper.GetDocument($"{url}index.html");
                HtmlNode rootNode = doc.DocumentNode;
                var provinceas = rootNode.SelectNodes("//tr[@class='provincetr']/td/a[@href]");
                foreach (var provincea in provinceas)
                {
                    var href = provincea.Attributes["href"].Value;
                    var id = Regex.Match(href, @"[0-9]{2}").Value;
                    var name = provincea.InnerText;
                    var code = id;
                    Console.WriteLine($"province:{id},{code},{name}");
                    provinces.Add(new Base_Provinces
                    {
                        Id = id,
                        Code = code,
                        ProvinceId = code,
                        ProvinceName = name,
                    });
                }
                SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                db.CommonBulkCopy(provinces, null);
                Console.WriteLine("省份结束");
            }
            return provinces;
        }
    }
}
