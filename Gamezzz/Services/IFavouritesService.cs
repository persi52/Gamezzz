using Gamezzz.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gamezzz.Services
{
    public interface IFavouritesService
    {
        public Task AddToFavourites(string userId,int gameId);
        public Task<bool> IsFavourite(string userId,int gameId);
        public Task<List<Game>> GetFavourites(string favouriteGames);
        public Task RemoveFromFavourites(string userId, int gameId);
    }
}
