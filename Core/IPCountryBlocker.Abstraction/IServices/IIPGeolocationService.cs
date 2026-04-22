using IPCountryBlocker.Shared.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCountryBlocker.Abstraction.IServices
{
    public interface IIPGeolocationService
    {
        Task<IPInfoDto?> GetIPInfoAsync(string ipAddress);

    }
}
