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
        public void Handle(string url)
        {
            List<Base_Cities> citys = new List<Base_Cities>();           
            using (IDbConnection conn = DBHelper.Connection)
            {
                string sQuery = "SELECT Id,Code,CityId,CityName,ProvinceId,Province_Id,ProvinceName FROM Base_Cities where IsCompleted!=1";
                conn.Open();
                citys = conn.Query<Base_Cities>(sQuery).ToList();
                foreach (var city in citys)
                {
                    List<Base_Counties> countys = new List<Base_Counties>();
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
                                    IsHasChildren = true,
                                    IsCompleted = false
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
                                    IsHasChildren = false,
                                    IsCompleted = false
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
                            IsHasChildren = true,
                            IsCompleted = false
                        });
                    }
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(countys, null);
                    string updateCity = $"update Base_Cities set IsCompleted =1 where CityId= '{city.CityId}'";
                    conn.Execute(updateCity);
                }
                Console.WriteLine("区县结束");
            }
        }
    }
}
