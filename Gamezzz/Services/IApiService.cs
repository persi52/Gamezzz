using Gamezzz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gamezzz.Services
{
    public interface IApiService
    {
        public Task<List<Game>> GetAllToList();
    }
}
