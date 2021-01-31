using Gamezzz.Data;
using Gamezzz.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gamezzz.Services
{
    public class FavouritesService : IFavouritesService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _usersDb;
        private readonly IApiService _apiService;

        public FavouritesService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager
            , ApplicationDbContext usersDb, IApiService apiService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usersDb = usersDb;
            _apiService = apiService;
        }

        [HttpPost]
        public async Task AddToFavourites(string userId,int gameId)// async Task<IActionResult>
        {
            var result = IsFavourite(userId, gameId).Result;
            //Console.WriteLine(result);
            if (!result)
            {
                var user = await _usersDb.Users.FindAsync(userId);

                if (user.favouriteGames != null && !user.favouriteGames.Equals(""))
                    user.favouriteGames += (" " + gameId);
                else user.favouriteGames = gameId.ToString();

                await _usersDb.SaveChangesAsync();

                Console.WriteLine(user.UserName + " " + gameId);//_userManager.FindByIdAsync(userId).UserName);    
            }

                    
        }

        [HttpDelete]
        public async Task RemoveFromFavourites(string userId,int gameId)
        {
            var user = await _usersDb.Users.FindAsync(userId);

            string[] dividedFavouriteGames = user.favouriteGames.Split(' ');

            List<string> dividedFavouriteGamesList = new List<string>(dividedFavouriteGames);

            for (int i = 0; i < dividedFavouriteGamesList.Count; i++)
            {
                Console.WriteLine(dividedFavouriteGames[i]);

                if (dividedFavouriteGamesList[i].Contains(gameId.ToString()))
                { 
                    dividedFavouriteGamesList.RemoveAt(i);
                    break;
                }
            }
            var result = String.Join(" ", dividedFavouriteGamesList.ToArray());

            user.favouriteGames = result;

            await _usersDb.SaveChangesAsync();

        }

        [HttpGet]
        public async Task<bool> IsFavourite(string userId,int gameId) //async Task<bool>
        {
            var user = await _usersDb.Users.FindAsync(userId);


            var favouriteGames = user.favouriteGames;

                if (favouriteGames!=null && favouriteGames.Contains(gameId.ToString()))
                    return true;
                else return false;          
           
           
            
        }

        [HttpGet]
        public async Task<List<Game>> GetFavourites(string favouriteGames)
        {
            if (favouriteGames != null && favouriteGames != "")
            {
                var gamesToSearch = new List<Game>();
                var gamesToReturn = new List<Game>();

                int[] dividedFavouriteGames = Array.ConvertAll(favouriteGames.Split(' '), int.Parse);


                gamesToSearch = await _apiService.GetAllToList();

                for (int i = 0; i < dividedFavouriteGames.Length; i++)
                {
                    var result = gamesToSearch.Find(x => x.Id.Equals((dividedFavouriteGames[i])));

                    if (result != null)
                        gamesToReturn.Add(result);
                }

                return gamesToReturn;
            }
            else return null;

        }
    }
}
