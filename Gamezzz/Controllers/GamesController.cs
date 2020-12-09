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

namespace Gamezzz.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _usersDb;
        private readonly ApplicationDbGamesContext _gamesDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        [BindProperty]
        public Game Game { get; set; }

        public GamesController(ApplicationDbContext usersDb, ApplicationDbGamesContext gamesDb, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IWebHostEnvironment hostEnvironment)
        {
            _usersDb = usersDb;
            _gamesDb = gamesDb;
            _userManager = userManager;
            _signInManager = signInManager;
            webHostEnvironment = hostEnvironment;
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
        public async Task<IActionResult> GetAllToList()
        {
            var _gameList = new List<Game>();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://rawg-video-games-database.p.rapidapi.com/games"),
                Headers =
            {
            { "x-rapidapi-key", "40bd4196d2msh88b635a99b5963ap158479jsn932f246e0e82" },
            { "x-rapidapi-host", "rawg-video-games-database.p.rapidapi.com" },
            },
            };
            //int i = 0;
            using (var response = await client.SendAsync(request))
            {
                DateTime date = new DateTime();
                List<string> categories;
                string genre;

                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();

                dynamic o = JsonConvert.DeserializeObject(body);

                for (int j = 0; j < o.results.Count; j++)
                {
                    categories = new List<string>();
                    date = o.results[j].released;
                    int genresCount = o.results[j].genres.Count;                    

                        for (int i = 0; i < genresCount; i++)
                    {
                        genre = " " + o.results[j].genres[i].name;
                        categories.Add(genre);
                    }

                    Game game = new Game
                    {
                        Id = o.results[j].id,
                        Title = o.results[j].name,
                        Category = categories,
                        YearOfRelease = date.ToString("yyyy"),
                        photoName = o.results[j].background_image
                    };                    
                    _gameList.Add(game);
                }
                
            }

           
            System.Text.Json.JsonSerializer.Serialize(_gameList);
            Console.WriteLine(_gameList[0].photoName);
            return Json(new { data = _gameList});
            
        }


        [HttpGet]
        public async Task onGet(int id)
        {
            Game = await _gamesDb.Games.FindAsync(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(GameViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                Game game = new Game
                {
                    Title = model.Title,               
                                                      
                    photoName = uniqueFileName,
                };
                _gamesDb.Add(game);
                await _gamesDb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
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

        private string UploadedFile(GameViewModel model)
        {
            string uniqueFileName = null;

            if (model.gameImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.gameImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.gameImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        #endregion
    }
}
