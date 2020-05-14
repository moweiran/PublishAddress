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
using Polly;

namespace ConsoleApp1
{
    public class CityManage
    {
        public void Handle(string url)
        {
            List<Base_Provinces> provinces = new List<Base_Provinces>();
            using (IDbConnection conn = DBHelper.Connection)
            {
                string sQuery = "SELECT Id,Code,ProvinceId,ProvinceName FROM Base_Provinces where IsCompleted!=1";
                conn.Open();
                provinces = conn.Query<Base_Provinces>(sQuery).ToList();
                foreach (var province in provinces)
                {
                    List<Base_Cities> citys = new List<Base_Cities>();
                    PolicyHelper.RetryForever(() =>
                    {
                        var getUrl = $"{url}{province.Id}.html";
                        Console.WriteLine($"cityUrl:{getUrl}");                       
                        HtmlDocument doc = new HtmlDocument();
                        var html = HttpServiceHelper.Get(getUrl, 2);
                        doc.LoadHtml(html);
                        HtmlNode rootNode = doc.DocumentNode;
                        var citytrs = rootNode.SelectNodes("//tr[@class='citytr']");
                        if (citytrs != null)
                        {
                            throw new Exception();
                        }
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
                                ProvinceName = province.ProvinceName,
                                IsCompleted = false
                            });
                        }
                    });
                    if (citys.Count > 0)
                    {
                        SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                        db.CommonBulkCopy(citys, null);
                        string udpateProvince = $"update Base_Provinces set IsCompleted =1 where provinceId= '{province.ProvinceId}'";
                        conn.Execute(udpateProvince);
                    }
                }
                Console.WriteLine("城市结束");
            }
        }
    }
}
