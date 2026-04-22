using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace IPCountryBlocker.Shared.DTO_s
{
    public class IPInfoDto
    {
        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }
        
        [JsonPropertyName("country_name")]
        public string CountryName { get; set; }
        
        [JsonPropertyName("org")]
        public string ISP { get; set; }
    }
}
