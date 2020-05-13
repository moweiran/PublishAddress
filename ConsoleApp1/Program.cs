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
                List<Base_Provinces> provinces = new List<Base_Provinces>();
                List<Base_Cities> citys = new List<Base_Cities>();
                List<Base_Counties> countys = new List<Base_Counties>();
                List<Base_Towns> towns = new List<Base_Towns>();
                List<Base_Villages> villages = new List<Base_Villages>();

                var provinceManage = new ProvinceManage();
                provinces = provinceManage.Handle(url);

                var cityManage = new CityManage();
                citys= cityManage.Handle(url, provinces );

                var countyManage = new CountyManage();
                countys= countyManage.Handle(url, citys );

                var townManage = new TownManage();
                towns = townManage.Handle(url, countys);

                var villageManage = new VillageManage();
                villages= villageManage.Handle(url, towns);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }
            Console.ReadLine();
        }
    }
}
