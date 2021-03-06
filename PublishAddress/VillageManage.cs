﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.DbHelper;
using Dapper;
using HtmlAgilityPack;
using Models;

namespace ConsoleApp1
{
    public class VillageManage
    {
        public void Handle(string url)
        {
            List<Base_Towns> towns = new List<Base_Towns>();
            using (IDbConnection conn = DBHelper.Connection)
            {
                string sQuery = "SELECT Id,Code,TownId,TownName,CountyId,County_Id,CountyName,CityId,City_Id,CityName,ProvinceId,Province_Id,ProvinceName FROM Base_Towns where IsCompleted!=1";
                conn.Open();
                towns = conn.Query<Base_Towns>(sQuery).ToList();
                Parallel.ForEach(towns, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (town) =>
                {
                    List<Base_Villages> villages = new List<Base_Villages>();
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
                    HtmlDocument doc = new HtmlDocument();
                    var html = HttpServiceHelper.PolicyGet(getUrl);
                    doc.LoadHtml(html);
                    HtmlNode rootNode = doc.DocumentNode;
                    var villagetrs = rootNode.SelectNodes("//tr[@class='villagetr']");
                    foreach (var tr in villagetrs)
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
                    if (villages.Count > 0)
                    {
                        SqlBulkCopyHelper db = new SqlBulkCopyHelper();
                        db.CommonBulkCopy(villages, null);
                        string updateTown = $"update Base_Towns set IsCompleted =1 where TownId= '{town.TownId}'";
                        conn.Execute(updateTown);
                    }
                });
                Console.WriteLine("村结束");
            }
        }
    }
}
