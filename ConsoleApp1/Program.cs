using ConsoleApp1.DbHelper;
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
                List<ProvinceItem> provinces = new List<ProvinceItem>();
                List<Base_Provinces> dbProvinces = new List<Base_Provinces>();
                List<CityItem> citys = new List<CityItem>();
                List<Base_Cities> dbCitys = new List<Base_Cities>();
                List<CountyItem> countys = new List<CountyItem>();
                List<Base_Counties> dbCountys = new List<Base_Counties>();
                List<TownItem> towns = new List<TownItem>();
                List<Base_Towns> dbTowns = new List<Base_Towns>();
                List<VillageItem> villages = new List<VillageItem>();
                List<Base_Villages> dbVillages = new List<Base_Villages>();

                HtmlDocument doc = GetDocument($"{url}index.html");
                var rootNode = doc.DocumentNode;
                var provinceas = rootNode.SelectNodes("//tr[@class='provincetr']/td/a[@href]");
                foreach (var provincea in provinceas)
                {
                    var href = provincea.Attributes["href"].Value;
                    var id = Regex.Match(href, @"[0-9]{2}").Value;
                    var name = provincea.InnerText;
                    var code = id;
                    Console.WriteLine($"province:{id},{code},{name}");
                    provinces.Add(new ProvinceItem
                    {
                        Id = id,
                        Name = name,
                        Code = code
                    });
                    dbProvinces.Add(new Base_Provinces
                    {
                        ProvinceName = name,
                        ProvinceId = decimal.Parse(code)
                    });
                }

                foreach (var province in provinces)
                {
                    var getUrl = $"{url}{province.Id}.html";
                    Console.WriteLine($"cityUrl:{getUrl}");
                    doc = GetDocument(getUrl);
                    rootNode = doc.DocumentNode;
                    var citytrs = rootNode.SelectNodes("//tr[@class='citytr']");
                    foreach (var citytr in citytrs)
                    {
                        var cityas = citytr.SelectNodes("./td/a[@href]");
                        var href = cityas[0].Attributes["href"].Value;
                        var id = Regex.Match(href, @"[0-9]{4}").Value;
                        var code = cityas[0].InnerText;
                        var name = cityas[1].InnerText;
                        Console.WriteLine($"city:{id},{code},{name}");
                        citys.Add(new CityItem
                        {
                            Id = id,
                            Code = code,
                            Name = name,
                            ProvinceId = province.Id,
                            ProvinceName = province.Name
                        });
                        dbCitys.Add(new Base_Cities
                        {
                            CityName = name,
                            CityId = decimal.Parse(code),
                            ProvinceId = decimal.Parse(province.Code)
                        });
                    }
                }

                foreach (var city in citys)
                {
                    var getUrl = $"{url}{city.ProvinceId}/{city.Id}.html";
                    Console.WriteLine($"countyUrl:{getUrl}");
                    doc = GetDocument(getUrl);
                    rootNode = doc.DocumentNode;
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
                                countys.Add(new CountyItem
                                {
                                    Id = id,
                                    Code = code,
                                    Name = name,
                                    ProvinceId = city.ProvinceId,
                                    ProvinceName = city.ProvinceName,
                                    CityId = city.Id,
                                    CityName = city.Name,
                                    IsHasChildren = true
                                });
                                dbCountys.Add(new Base_Counties
                                {
                                    CountyName = name,
                                    CountyId = decimal.Parse(code),
                                    CityId = decimal.Parse(city.Code)
                                });
                            }
                            else
                            {
                                var code = tr.ChildNodes[0].InnerText;
                                var name = tr.ChildNodes[1].InnerText;
                                Console.WriteLine($"county:{code},{name}");
                                countys.Add(new CountyItem
                                {
                                    Id = "",
                                    Code = code,
                                    Name = name,
                                    ProvinceId = city.ProvinceId,
                                    ProvinceName = city.ProvinceName,
                                    CityId = city.Id,
                                    CityName = city.Name,
                                    IsHasChildren = false
                                });
                                dbCountys.Add(new Base_Counties
                                {
                                    CountyName = name,
                                    CountyId = decimal.Parse(code),
                                    CityId = decimal.Parse(city.Code)
                                });
                            }
                        }
                    }
                    else
                    {
                        var code = $"{city.Id}01000000";
                        var name = "市辖区";
                        Console.WriteLine($"county:{code},市辖区");
                        countys.Add(new CountyItem
                        {
                            Id = "",
                            Code = code,
                            Name = name,
                            ProvinceId = city.ProvinceId,
                            ProvinceName = city.ProvinceName,
                            CityId = city.Id,
                            CityName = city.Name,
                            IsHasChildren = true
                        });
                        dbCountys.Add(new Base_Counties
                        {
                            CountyName = name,
                            CountyId = decimal.Parse(code),
                            CityId = decimal.Parse(city.Code)
                        });
                    }

                }

                foreach (var county in countys)
                {
                    if (!county.IsHasChildren)
                    {
                        continue;
                    }
                    var getUrl = string.Empty;
                    if (string.IsNullOrWhiteSpace(county.Id))
                    {
                        getUrl = $"{url}{county.ProvinceId}/{county.CityId}.html";
                    }
                    else if (county.IsHasChildren)
                    {
                        getUrl = $"{url}{county.ProvinceId}/{county.CityId.Substring(2, 2)}/{county.Id}.html";
                    }
                    Console.WriteLine($"townUrl:{getUrl}");
                    doc = GetDocument(getUrl);
                    rootNode = doc.DocumentNode;
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
                            towns.Add(new TownItem
                            {
                                Id = id,
                                Code = code,
                                Name = name,
                                ProvinceId = county.ProvinceId,
                                ProvinceName = county.ProvinceName,
                                CityId = county.CityId,
                                CityName = county.CityName,
                                CountyId = county.Id,
                                CountyName = county.Name
                            });
                            dbTowns.Add(new Base_Towns
                            {
                                TownName = name,
                                TownId = decimal.Parse(code),
                                CountyId = decimal.Parse(county.Code)
                            });
                        }
                    }
                }

                foreach (var town in towns)
                {
                    var getUrl = string.Empty;
                    if (string.IsNullOrWhiteSpace(town.CountyId))
                    {
                        getUrl = $"{url}{town.ProvinceId}/{town.CityId.Substring(2, 2)}/{town.Id}.html";
                    }
                    else
                    {
                        getUrl = $"{url}{town.ProvinceId}/{town.CityId.Substring(2, 2)}/{town.CountyId.Substring(4, 2)}/{town.Id}.html";
                    }
                    Console.WriteLine($"villageUrl:{getUrl}");
                    doc = GetDocument(getUrl);
                    rootNode = doc.DocumentNode;
                    var trs = rootNode.SelectNodes("//tr[@class='villagetr']");
                    foreach (var tr in trs)
                    {
                        var tds = tr.SelectNodes("./td");
                        var code = tds[0].InnerText;
                        var name = tds[2].InnerText;
                        Console.WriteLine($"village:{code},{name}");
                        villages.Add(new VillageItem { Id = "", Code = code, Name = name, ProvinceId = town.ProvinceId, CityId = town.CityId, CountyId = town.Id, TownId = town.Id });
                        dbVillages.Add(new Base_Villages
                        {
                            VillageName = name,
                            VillageId = decimal.Parse(code),
                            ProvinceId = decimal.Parse(town.ProvinceId),
                            ProvinceName = town.ProvinceName,
                            CityId = decimal.Parse(town.CityId),
                            CityName = town.CityName,
                            CountyId = decimal.Parse(town.CountyId),
                            CountyName = town.CountyName,
                            TownId = decimal.Parse(town.Code),
                            TownName = town.Name
                        });
                    }
                }

                Task.Run(() =>
                {
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(dbProvinces, null);
                    Console.WriteLine("省份结束");
                });
                Task.Run(() =>
                {
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(dbCitys, null);
                    Console.WriteLine("城市结束");
                });
                Task.Run(() =>
                {
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(dbCountys, null);
                    Console.WriteLine("区县结束");
                });
                Task.Run(() =>
                {
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(dbTowns, null);
                    Console.WriteLine("乡镇结束");
                });
                Task.Run(() =>
                {
                    SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                    db.CommonBulkCopy(dbVillages, null);
                    Console.WriteLine("村结束");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }
            Console.ReadLine();
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
                    var html = HttpServiceHelper.Get(url, 3);
                    doc.LoadHtml(html);

                });    //要执行的方法
            Thread.Sleep(500);
            return doc;
        }

        private static Task<string> GetHtml(string url)
        {
            System.GC.Collect();
            using (MyWebClient webClient = new MyWebClient())
            {
                webClient.Encoding = Encoding.GetEncoding("GB2312");
                var s = webClient.DownloadStringTaskAsync(url);
                webClient.Dispose();
                return s;
            }
        }

        private static void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
        }
    }

    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 2 * 1000;
            return w;
        }
    }

    class ProvinceItem
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    class CityItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
    }

    class CountyItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public bool IsHasChildren { get; internal set; }
    }

    class TownItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string CountyId { get; set; }
        public string CountyName { get; set; }
    }

    class VillageItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ProvinceId { get; set; }
        public string CityId { get; set; }
        public string CountyId { get; set; }
        public string TownId { get; set; }
    }
}
