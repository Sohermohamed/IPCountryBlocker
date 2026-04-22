using IPCountryBlocker.Abstraction.IRepositories;
using IPCountryBlocker.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCountryBlocker.Persistence.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ConcurrentDictionary<string, Country> _countries = new();
        private readonly ConcurrentDictionary<string, DateTime> _temporalBlocks = new();

        public bool Add(Country country)
        {
            return _countries.TryAdd(country.Code, country);
        }

        public bool Remove(string countryCode)
        {
            return _countries.TryRemove(countryCode, out _);
        }

        public bool Exists(string countryCode)
        {
            return _countries.ContainsKey(countryCode);
        }

        public IEnumerable<Country> GetAll()
        {
            return _countries.Values;
        }

        public bool AddTemporal(string countryCode, int durationMinutes)
        {
            if (_temporalBlocks.TryGetValue(countryCode, out var expiry) && expiry > DateTime.UtcNow)
            {
                return false; 
            }

            _temporalBlocks[countryCode] = DateTime.UtcNow.AddMinutes(durationMinutes);
            return true;
        }

        public bool IsTemporarilyBlocked(string countryCode)
        {
            return _temporalBlocks.TryGetValue(countryCode, out var expiry) && expiry > DateTime.UtcNow;
        }

        public void RemoveExpiredTemporalBlocks()
        {
            var expiredKeys = _temporalBlocks
                .Where(x => x.Value <= DateTime.UtcNow)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _temporalBlocks.TryRemove(key, out _);
            }
        }
    }
}
