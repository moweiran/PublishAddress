using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Base_Towns
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string TownId { get; set; }
        public string TownName { get; set; }
        public string CountyId { get; set; }
        public string County_Id { get; set; }
        public string City_Id { get; set; }
        public string CityId { get; set; }
        public string ProvinceId { get; set; }
        public string Province_Id { get; set; }
        public string ProvinceName { get; internal set; }
        public string CityName { get; internal set; }
        public string CountyName { get; internal set; }
        public bool? IsCompleted { get; set; }
    }
}
