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
                var url = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2019/";
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var provinceManage = new ProvinceManage();
                provinceManage.Handle(url);
                var cityManage = new CityManage();
                cityManage.Handle(url );
                var countyManage = new CountyManage();
                countyManage.Handle(url );
                var townManage = new TownManage();
                townManage.Handle(url);
                var villageManage = new VillageManage();
                villageManage.Handle(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }
            Console.ReadLine();
        }
    }
}
