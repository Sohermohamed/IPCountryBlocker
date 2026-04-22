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
    public class LogRepository : ILogRepository
    {
        // thread-safe in-memory storage
        private readonly ConcurrentBag<BlockedAttemptLog> _logs = new();

        public void Add(BlockedAttemptLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            _logs.Add(log);
        }

        public IEnumerable<BlockedAttemptLog> GetAll()
        {
            return _logs
                   .OrderByDescending(x => x.Timestamp)
                   .ToList();
        }
    }
}
