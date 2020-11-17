using Gamezzz.Data;
using Gamezzz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gamezzz.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _usersDb;
        private readonly ApplicationDbGamesContext _gamesDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        [BindProperty]
        public Game Game { get; set; }

        public GamesController(ApplicationDbContext usersDb, ApplicationDbGamesContext gamesDb, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _usersDb = usersDb;
            _gamesDb = gamesDb;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        /* public IActionResult Upsert(int? id)
         {
             Game = new Game();
             if (id == null)
             {
                 //create
                 return View(Game);
             }
             //update
             Game = _gamesDb.Games.FirstOrDefault(u => u.Id == id);
             if (Game == null)
             {
                 return NotFound();
             }
             return View(Game);
         }*/
        #region API Calls

        [HttpPost]
        public async Task<IActionResult> AddToFavourites(int gameId)//, string userName)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var gameToAdd = await _gamesDb.Games.FirstOrDefaultAsync(u => u.Id == gameId);
                if (gameToAdd == null)
                {
                    return Json(new { success = false, message = "Error while adding" });
                }
                var userId = _userManager.GetUserId(HttpContext.User);
                var user = _userManager.FindByIdAsync(userId).Result;
                user.favouriteGames += (gameToAdd.Title + ", ");//.Result.Title + ", ");

                // await _userManager.UpdateAsync(user);
                await _usersDb.SaveChangesAsync();


                RedirectToAction("Index");
                return Json(new { success = true, message = "Added to favourites!" });
            }
            else
            {
                return Json(new { success = false, message = "Please log in to add." });
            }

        }

        [HttpGet]
        public async Task<bool> IsFavourite(int gameId)
        {                
            var userId = _userManager.GetUserId(HttpContext.User);
            if (userId == null) return false;
            
            var gameToFind = await _gamesDb.Games.FirstOrDefaultAsync(u => u.Id == gameId);

            var user = _userManager.FindByIdAsync(userId).Result;

            if (user.favouriteGames.Contains(gameToFind.Title)) return true;
            else return false;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            
            return Json(new { data = await _gamesDb.Games.ToListAsync() });
        }

        [HttpGet]
        public async Task onGet(int id)
        {
            Game = await _gamesDb.Games.FindAsync(id);
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
        #endregion
    }
}
