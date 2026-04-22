using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCountryBlocker.Domain.Entities
{
    public class BlockedAttemptLog
    {
        public string IPAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string CountryCode { get; set; }
        public bool IsBlocked { get; set; }
        public string UserAgent { get; set; }
    }
}
