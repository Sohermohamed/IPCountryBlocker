using System.ComponentModel.DataAnnotations;

namespace IPCountryBlocker.Shared.DTO_s
{
    public class TemporalBlockRequest
    {
        [Required]
        [RegularExpression("^[a-zA-Z]{2}$", ErrorMessage = "Country code must be exactly 2 alphabetic characters.")]
        public string CountryCode { get; set; }

        [Required]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int DurationMinutes { get; set; }
    }
}
