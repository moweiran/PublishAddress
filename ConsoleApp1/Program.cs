using ConsoleApp1.DbHelper;
using Dapper;
using HtmlAgilityPack;
using Models;
using Polly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var url = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2018/";
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<Base_Provinces> provinces = new List<Base_Provinces>();
                List<Base_Cities> citys = new List<Base_Cities>();
                List<Base_Counties> countys = new List<Base_Counties>();
                List<Base_Towns> towns = new List<Base_Towns>();
                List<Base_Villages> villages = new List<Base_Villages>();
                using (IDbConnection conn = Connection)
                {
                    string sQuery = "SELECT Id,Code,ProvinceId,ProvinceName FROM Base_Provinces";
                    conn.Open();
                    provinces = conn.Query<Base_Provinces>(sQuery).ToList();
                }
                if (provinces.Count == 0)
                {
                    HtmlDocument doc = GetDocument($"{url}index.html");
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
                    Task.Run(() =>
                    {
                        SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                        db.CommonBulkCopy(provinces, null);
                        Console.WriteLine("省份结束");
                    });
                }

                using (IDbConnection conn = Connection)
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
                        HtmlDocument doc = GetDocument(getUrl);
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

                    Task.Run(() =>
                    {
                        SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                        db.CommonBulkCopy(citys, null);
                        Console.WriteLine("城市结束");
                    });
                }

                using (IDbConnection conn = Connection)
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
                        HtmlDocument doc = GetDocument(getUrl);
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
                    Task.Run(() =>
                    {
                        SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                        db.CommonBulkCopy(countys, null);
                        Console.WriteLine("区县结束");
                    });
                }
                using (IDbConnection conn = Connection)
                {
                    string sQuery = "SELECT Id,Code,TownId,TownName,CountyId,County_Id,CountyName,CityId,City_Id,CityName,ProvinceId,Province_Id,ProvinceName FROM Base_Towns";
                    conn.Open();
                    towns = conn.Query<Base_Towns>(sQuery).ToList();
                }
                if (towns.Count == 0)
                {
                    foreach (var county in countys)
                    {
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
                        HtmlDocument doc = GetDocument(getUrl);
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
                                    CountyName = county.CountyName
                                });
                            }
                        }
                    }

                    Task.Run(() =>
                    {
                        SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                        db.CommonBulkCopy(towns, null);
                        Console.WriteLine("乡镇结束");
                    });
                }
                using (IDbConnection conn = Connection)
                {
                    conn.Open();
                    string sql = "truncate table Base_Villages";
                    conn.Execute(sql);
                }
                foreach (var town in towns)
                {
                    var getUrl = string.Empty;
                    if (string.IsNullOrWhiteSpace(town.County_Id))
                    {
                        getUrl = $"{url}{town.ProvinceId}/{town.CityId.Substring(2, 2)}/{town.Id}.html";
                    }
                    else
                    {
                        getUrl = $"{url}{town.ProvinceId}/{town.City_Id.Substring(2, 2)}/{town.County_Id.Substring(4, 2)}/{town.Id}.html";
                    }
                    Console.WriteLine($"villageUrl:{getUrl}");
                    HtmlDocument doc = GetDocument(getUrl);
                    HtmlNode rootNode = doc.DocumentNode;
                    var trs = rootNode.SelectNodes("//tr[@class='villagetr']");
                    foreach (var tr in trs)
                    {
                        var tds = tr.SelectNodes("./td");
                        var code = tds[0].InnerText;
                        var name = tds[2].InnerText;
                        Console.WriteLine($"village:{code},{name}");
                        villages.Add(new Base_Villages
                        {
                            VillageId = code,
                            VillageName = name,
                            TownId = town.TownId,
                            TownName = town.TownName,
                            CountyId = town.CountyId,
                            CountyName = town.CountyName,
                            CityId = town.CityId, 
                            CityName = town.CityName,
                            ProvinceId = town.ProvinceId,
                            ProvinceName = town.ProvinceName
                        });
                    }
                }
                Task.Run(() =>
                {
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(villages, null);
                    Console.WriteLine("村结束");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }
            Console.ReadLine();
        }
        static IDbConnection Connection
        {
            get
            {
                return new SqlConnection(@"Server=.\sql2008r2;Initial Catalog=Address;User ID=sa;Password=sasa;Connection Timeout=300;MultipleActiveResultSets=True;");
            }
        }

        private static HtmlDocument GetDocument(string url)
        {
            System.GC.Collect();
            HtmlDocument doc = new HtmlDocument();
            Policy.Handle<Exception>()   //指定需要重试的异常类型
                .RetryForever((ex, count, context) =>
                {     //指定发生异常重试的次数
                    Console.WriteLine($"重试次数{count},异常{ex.Message}");
                })
                .Execute(() =>
                {
                    var html = HttpServiceHelper.Get(url, 1);
                    doc.LoadHtml(html);
                });    //要执行的方法
            //Thread.Sleep(100);
            return doc;
        }
    }
}
