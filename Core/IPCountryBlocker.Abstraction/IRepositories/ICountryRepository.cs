using IPCountryBlocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCountryBlocker.Abstraction.IRepositories
{
    public interface ICountryRepository
    {
        bool Add(Country country);
        bool Remove(string countryCode);
        bool Exists(string countryCode);
        IEnumerable<Country> GetAll();
        bool AddTemporal(string countryCode, int durationMinutes);
        bool IsTemporarilyBlocked(string countryCode);
        void RemoveExpiredTemporalBlocks();
    }
}
