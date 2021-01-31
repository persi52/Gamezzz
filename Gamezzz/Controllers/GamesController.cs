using Gamezzz.Data;
using Gamezzz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Newtonsoft.Json;
using Gamezzz.Services;
using System.Security.Claims;

namespace Gamezzz.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _usersDb;
        private readonly ApplicationDbGamesContext _gamesDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IApiService _apiService;
        private readonly IFavouritesService _favouritesService;

        [BindProperty]
        public Game Game { get; set; }

        public GamesController(ApplicationDbContext usersDb, ApplicationDbGamesContext gamesDb, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IApiService apiService, IFavouritesService favouritesService)
        {
            _usersDb = usersDb;
            _gamesDb = gamesDb;
            _userManager = userManager;
            _signInManager = signInManager;           
            _apiService = apiService;
            _favouritesService = favouritesService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        #region API Calls             

        [HttpGet]
        public async Task<IActionResult> GetAllToList()
        {
            List<Game> _gameList =   _apiService.GetAllToList().Result;
            //System.Text.Json.JsonSerializer.Serialize(_gameList);
            
            return Json(new { data = _gameList });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavourites()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Game> _gameList = _favouritesService.GetFavourites(user.favouriteGames).Result;
            //System.Text.Json.JsonSerializer.Serialize(_gameList);
            
            return Json(new { data = _gameList });
            
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromFavourites(int gameId)
        {
            var user = await _userManager.GetUserAsync(User);
                     
            await _favouritesService.RemoveFromFavourites(user.Id, gameId);

            RedirectToAction("Index");

            return Json(new { success = true, message = "Removed from favourites!" });
            
        }




        [HttpGet]
        public async Task onGet(int id)
        {
            Game = await _gamesDb.Games.FindAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavourites(int gameId)
        {
            if (_signInManager.IsSignedIn(User))
            {
                //var user = _usersDb.Users.FirstOrDefault<AppUser>(User);
                
                var user = await _userManager.GetUserAsync(User);

                if (!IsFavourite(gameId).Result)
                {
                    await _favouritesService.AddToFavourites(user.Id, gameId);
                    RedirectToAction("Index");
                    return Json(new { success = true, message = "Added to favourites!" });
                }
                else return Json(new { success = true, message = "Game already is favourite" });
            }
            else
            {                
                return Json(new { success = false, message = "Please log in to add." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var gameFromDb = await _gamesDb.Games.FirstOrDefaultAsync(u => u.Id == id);
            if (gameFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _gamesDb.Games.Remove(gameFromDb);
            await _gamesDb.SaveChangesAsync();
            RedirectToAction("Index");
            return Json(new { success = true, message = "Deleted successfully" });
        }

        [HttpGet]
        public async Task<bool> IsFavourite(int gameId) //async Task<bool>
        {
            var user = await _userManager.GetUserAsync(User);
            return await _favouritesService.IsFavourite(user.Id, gameId);
        }

        #endregion
    }
}
