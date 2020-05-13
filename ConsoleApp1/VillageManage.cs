using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.DbHelper;
using Dapper;
using HtmlAgilityPack;
using Models;

namespace ConsoleApp1
{
    public class VillageManage
    {
        public List<Base_Villages> Handle(string url, List<Base_Towns> towns)
        {
            List<Base_Villages> villages = new List<Base_Villages>();
            using (IDbConnection conn = DBHelper.Connection)
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
                HtmlDocument doc =HtmlHelper.GetDocument(getUrl);
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
            SqlBulkCopyHelper db = new SqlBulkCopyHelper();
            db.CommonBulkCopy(villages, null);
            Console.WriteLine("村结束");
            return villages;
        }
    }
}
