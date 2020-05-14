using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Base_Cities
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string ProvinceId { get; set; }
        public string Province_Id { get; set; }
        public string ProvinceName { get; internal set; }
        public bool? IsCompleted { get; set; }
    }
}
