using IPCountryBlocker.Abstraction.IRepositories;
using IPCountryBlocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPCountryBlocker.Service.Services
{
    public class CountryService
    {
        private readonly ICountryRepository _countryRepository;

        private static readonly HashSet<string> ValidIsoCountryCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", 
            "BE", "BZ", "BJ", "BM", "BT", "BO", "BQ", "BA", "BW", "BV", "BR", "IO", "BN", "BG", "BF", "BI", "KV", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CN",
            "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "CI", "HR", "CU", "CW", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "SZ", "ET",
            "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", 
            "VA", "HN", "HK", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", 
            "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MO", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", 
            "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "MK", "MP", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", 
            "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SX", 
            "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", 
            "TM", "TC", "TV", "UG", "UA", "AE", "GB", "UM", "US", "UY", "UZ", "VU", "VE", "VN", "VG", "VI", "WF", "EH", "YE", "ZM", "ZW"
        };

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        private bool IsValidCountryCode(string code) =>
            !string.IsNullOrWhiteSpace(code) && 
            Regex.IsMatch(code, "^[a-zA-Z]{2}$") &&
            ValidIsoCountryCodes.Contains(code);

        public bool BlockCountry(string countryCode, string? countryName = null)
        {
            if (!IsValidCountryCode(countryCode)) return false;

            countryCode = countryCode.ToUpper();
            if (_countryRepository.Exists(countryCode)) return false;

            return _countryRepository.Add(new Country 
            { 
                Code = countryCode, 
                Name = string.IsNullOrWhiteSpace(countryName) ? "Unknown" : countryName 
            });
        }

        public bool UnblockCountry(string countryCode)
        {
            if (!IsValidCountryCode(countryCode)) return false;
            return _countryRepository.Remove(countryCode.ToUpper());
        }

        public IEnumerable<Country> GetBlockedCountries(int page, int pageSize, string? searchQuery)
        {
            var query = _countryRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                query = query.Where(c => 
                    c.Code.ToLower().Contains(searchQuery) || 
                    c.Name.ToLower().Contains(searchQuery));
            }

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public bool BlockCountryTemporarily(string countryCode, int durationMinutes)
        {
            if (!IsValidCountryCode(countryCode) || durationMinutes < 1 || durationMinutes > 1440) 
                return false;
                
            return _countryRepository.AddTemporal(countryCode.ToUpper(), durationMinutes);
        }

        public bool IsCountryBlocked(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode)) return false;
            var code = countryCode.ToUpper();
            return _countryRepository.Exists(code) || _countryRepository.IsTemporarilyBlocked(code);
        }

        public void CleanupExpiredTemporalBlocks()
        {
            _countryRepository.RemoveExpiredTemporalBlocks();
        }
    }
}
