using ConsoleApp1.DbHelper;
using Dapper;
using HtmlAgilityPack;
using Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
        //static void Main(string[] args)
        //{
        //    try
        //    {
        //        var url = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2019/";
        //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //        var provinceManage = new ProvinceManage();
        //        provinceManage.Handle(url);
        //        var cityManage = new CityManage();
        //        cityManage.Handle(url );
        //        var countyManage = new CountyManage();
        //        countyManage.Handle(url );
        //        var townManage = new TownManage();
        //        townManage.Handle(url);
        //        var villageManage = new VillageManage();
        //        villageManage.Handle(url);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"ex.Message:{ex.Message}");
        //    }
        //    Console.ReadLine();
        //}

        static void Main(string[] args)
        {
            try
            {
                ChromeOptions op = new ChromeOptions();
                //op.AddArguments("--headless"); 启用无头浏览器
                // op.AddArguments("--window-size=1920,1080");

                var path = AppDomain.CurrentDomain.BaseDirectory.ToString();
                ChromeDriver driver = new ChromeDriver(path, op);
                string Url = $"http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2020/index.html";
                driver.Navigate().GoToUrl(Url);
                var modelHtmlList = driver.FindElementsByXPath(".//tr[@class='provincetr']");
                Dictionary<string, string> provinces = new Dictionary<string, string>();
                Dictionary<string, Base_Cities> cities = new Dictionary<string, Base_Cities>();
                Dictionary<string, Base_Counties> counties = new Dictionary<string, Base_Counties>();
                Dictionary<string, Base_Towns> towns = new Dictionary<string, Base_Towns>();
                Dictionary<string, Base_Villages> villages = new Dictionary<string, Base_Villages>();
                foreach (var modelHtml in modelHtmlList)
                {
                    CheckWarn(modelHtml);

                    var provinceHrefs = modelHtml.FindElements(By.XPath("./td/a"));
                    foreach (var provinceHref in provinceHrefs)
                    {
                        provinces.Add(provinceHref.GetAttribute("href"), provinceHref.Text);
                    }
                }

                if (provinces.Count > 0)
                {
                    foreach (var province in provinces)
                    {
                        driver.Navigate().GoToUrl(province.Key);
                        var cityModelHtmlList = driver.FindElementsByXPath(".//tr[@class='citytr']");
                        foreach (var item in cityModelHtmlList)
                        {
                            CheckWarn(item);

                            var modelHtml = item.FindElements(By.XPath("./td/a"));
                            if(modelHtml.Count!=2)
                            {
                                continue;
                            }
                            var codeHtml = modelHtml[0];
                            var nameHtml = modelHtml[1];
                            var href = codeHtml.GetAttribute("href");

                            cities.Add(href, new Base_Cities()
                            {
                                Code = codeHtml.Text,
                                CityName = nameHtml.Text
                            });
                        }

                    }
                }
                if (cities.Count > 0)
                {
                    foreach (var city in cities)
                    {
                        driver.Navigate().GoToUrl(city.Key);
                        var countyModelHtmlList = driver.FindElementsByXPath(".//tr[@class='countytr']");
                        foreach (var item in countyModelHtmlList)
                        {
                            CheckWarn(item);

                            var modelHtml = item.FindElements(By.XPath("./td/a"));
                            if (modelHtml.Count != 2)
                            {
                                continue;
                            }
                            var codeHtml = modelHtml[0];
                            var nameHtml = modelHtml[1];
                            var href = codeHtml.GetAttribute("href");

                            counties.Add(href, new Base_Counties()
                            {
                                Code = codeHtml.Text,
                                CountyName = nameHtml.Text
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }
            Console.ReadLine();
        }

        private static void CheckWarn(IWebElement modelHtml)
        {
            try
            {
                var warncontenter = modelHtml.FindElement(By.ClassName("warncontenter"));
                Thread.Sleep(1000 * 60);
            }
            catch (Exception ex)
            {

            }
        }
    }


}
