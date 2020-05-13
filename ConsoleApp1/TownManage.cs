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
    public class TownManage
    {
        public void Handle(string url)
        {
            List<Base_Counties> countys = new List<Base_Counties>();
            using (IDbConnection conn = DBHelper.Connection)
            {
                string sQuery = "SELECT Id,Code,CountyId,CountyName,CityId,City_Id,CityName,ProvinceId,Province_Id,ProvinceName,IsHasChildren FROM Base_Counties where IsCompleted!=1";
                conn.Open();
                countys = conn.Query<Base_Counties>(sQuery).ToList();
                foreach (var county in countys)
                {
                    List<Base_Towns> towns = new List<Base_Towns>();
                    if (!county.IsHasChildren)
                    {
                        continue;
                    }
                    var getUrl = string.Empty;
                    if (string.IsNullOrWhiteSpace(county.Id))
                    {
                        getUrl = $"{url}{county.ProvinceId}/{county.City_Id}.html";
                    }
                    else if (county.IsHasChildren)
                    {
                        getUrl = $"{url}{county.ProvinceId}/{county.City_Id.Substring(2, 2)}/{county.Id}.html";
                    }
                    Console.WriteLine($"townUrl:{getUrl}");
                    HtmlDocument doc = HtmlHelper.GetDocument(getUrl);
                    HtmlNode rootNode = doc.DocumentNode;
                    var trs = rootNode.SelectNodes("//tr[@class='towntr']");
                    if (trs != null)
                    {
                        foreach (var tr in trs)
                        {
                            var tdas = tr.SelectNodes("./td/a[@href]");
                            var href = tdas[0].Attributes["href"].Value;
                            var id = Regex.Match(href, @"[0-9]{9}").Value;
                            var code = tdas[0].InnerText;
                            var name = tdas[1].InnerText;
                            Console.WriteLine($"town:{id},{code},{name}");
                            towns.Add(new Base_Towns
                            {
                                Id = id,
                                Code = code,
                                TownId = code,
                                TownName = name,
                                ProvinceId = county.ProvinceId,
                                Province_Id = county.Province_Id,
                                ProvinceName = county.ProvinceName,
                                CityId = county.CityId,
                                City_Id = county.City_Id,
                                CityName = county.CityName,
                                CountyId = county.CountyId,
                                County_Id = county.Id,
                                CountyName = county.CountyName,
                                IsCompleted = false
                            });
                        }
                    }
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(towns, null);
                    string updateCounty = $"update Base_Counties set IsCompleted =1 where CountyId= '{county.CountyId}'";
                    conn.Execute(updateCounty);
                }
                Console.WriteLine("乡镇结束");
            }
        }
    }
}
