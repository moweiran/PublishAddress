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
    public class CountyManage
    {
        public List<Base_Counties> Handle(string url, List<Base_Cities> citys)
        {
            List<Base_Counties> countys = new List<Base_Counties>();
            using (IDbConnection conn = DBHelper.Connection)
            {
                string sQuery = "SELECT Id,Code,CountyId,CountyName,CityId,City_Id,CityName,ProvinceId,Province_Id,ProvinceName,IsHasChildren FROM Base_Counties";
                conn.Open();
                countys = conn.Query<Base_Counties>(sQuery).ToList();
            }
            if (countys.Count == 0)
            {
                foreach (var city in citys)
                {
                    var getUrl = $"{url}{city.ProvinceId}/{city.Id}.html";
                    Console.WriteLine($"countyUrl:{getUrl}");
                    HtmlDocument doc = HtmlHelper.GetDocument(getUrl);
                    HtmlNode rootNode = doc.DocumentNode;
                    var trs = rootNode.SelectNodes("//tr[@class='countytr']");
                    if (trs != null)
                    {
                        foreach (var tr in trs)
                        {
                            var tdas = tr.SelectNodes("./td/a[@href]");
                            if (tdas != null)
                            {
                                var href = tdas[0].Attributes["href"].Value;
                                var id = Regex.Match(href, @"[0-9]{6}").Value;
                                var code = tdas[0].InnerText;
                                var name = tdas[1].InnerText;
                                Console.WriteLine($"county:{id},{code},{name}");
                                countys.Add(new Base_Counties
                                {
                                    Id = id,
                                    Code = code,
                                    CountyId = code,
                                    CountyName = name,
                                    ProvinceId = city.ProvinceId,
                                    Province_Id = city.Province_Id,
                                    ProvinceName = city.ProvinceName,
                                    CityId = city.CityId,
                                    City_Id = city.Id,
                                    CityName = city.CityName,
                                    IsHasChildren = true
                                });
                            }
                            else
                            {
                                var code = tr.ChildNodes[0].InnerText;
                                var name = tr.ChildNodes[1].InnerText;
                                Console.WriteLine($"county:{code},{name}");
                                countys.Add(new Base_Counties
                                {
                                    Id = "",
                                    Code = code,
                                    CountyId = code,
                                    CountyName = name,
                                    ProvinceId = city.ProvinceId,
                                    Province_Id = city.Province_Id,
                                    ProvinceName = city.ProvinceName,
                                    CityId = city.CityId,
                                    City_Id = city.Id,
                                    CityName = city.CityName,
                                    IsHasChildren = false
                                });
                            }
                        }
                    }
                    else
                    {
                        var code = $"{city.Id}01000000";
                        var name = "市辖区";
                        Console.WriteLine($"county:{code},市辖区");
                        countys.Add(new Base_Counties
                        {
                            Id = "",
                            Code = code,
                            CountyId = code,
                            CountyName = name,
                            ProvinceId = city.ProvinceId,
                            Province_Id = city.Province_Id,
                            ProvinceName = city.ProvinceName,
                            CityId = city.CityId,
                            City_Id = city.Id,
                            CityName = city.CityName,
                            IsHasChildren = true
                        });
                    }

                }

                SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                db.CommonBulkCopy(countys, null);
                Console.WriteLine("区县结束");
            }
            return countys;
        }
    }
}
