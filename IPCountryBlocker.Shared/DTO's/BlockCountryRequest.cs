using System.ComponentModel.DataAnnotations;

namespace IPCountryBlocker.Shared.DTO_s
{
    public class BlockCountryRequest
    {
        [Required]
        [RegularExpression("^[a-zA-Z]{2}$", ErrorMessage = "Country code must be exactly 2 alphabetic characters.")]
        public string CountryCode { get; set; }
        
        public string? CountryName { get; set; } 
    }
}
