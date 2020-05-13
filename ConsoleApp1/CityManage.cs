using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleApp1.DbHelper;
using Dapper;
using HtmlAgilityPack;
using Models;

namespace ConsoleApp1
{
    public class CityManage
    {
        public List<Base_Cities> Handle(string url, List<Base_Provinces> provinces)
        {
            List<Base_Cities> citys = new List<Base_Cities>();
            using (IDbConnection conn = DBHelper.Connection)
            {
                string sQuery = "SELECT Id,Code,CityId,CityName,ProvinceId,Province_Id,ProvinceName FROM Base_Cities";
                conn.Open();
                citys = conn.Query<Base_Cities>(sQuery).ToList();
            }
            if (citys.Count == 0)
            {
                foreach (var province in provinces)
                {
                    var getUrl = $"{url}{province.Id}.html";
                    Console.WriteLine($"cityUrl:{getUrl}");
                    HtmlDocument doc = HtmlHelper.GetDocument(getUrl);
                    HtmlNode rootNode = doc.DocumentNode;
                    var citytrs = rootNode.SelectNodes("//tr[@class='citytr']");
                    foreach (var citytr in citytrs)
                    {
                        var cityas = citytr.SelectNodes("./td/a[@href]");
                        var href = cityas[0].Attributes["href"].Value;
                        var id = Regex.Match(href, @"[0-9]{4}").Value;
                        var code = cityas[0].InnerText;
                        var name = cityas[1].InnerText;
                        Console.WriteLine($"city:{id},{code},{name}");
                        citys.Add(new Base_Cities
                        {
                            Id = id,
                            Code = code,
                            CityId = code,
                            CityName = name,
                            ProvinceId = province.ProvinceId,
                            Province_Id = province.Id,
                            ProvinceName = province.ProvinceName
                        });
                    }
                }
                SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                db.CommonBulkCopy(citys, null);
                Console.WriteLine("城市结束");
            }
            return citys;
        }
    }
}
