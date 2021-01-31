using Gamezzz.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gamezzz.Services
{
    public class ApiService : IApiService
    {
        [HttpGet]
        public async Task<List<Game>> GetAllToList()
        {
            
            var _gameList = new List<Game>();
            var client = new HttpClient();
            Uri requestUri;
            Uri next;

            requestUri = new Uri("https://api.rawg.io/api/games");

           
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,

                    RequestUri = requestUri,
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
                    next = new Uri(o.next.ToString());

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
                Console.WriteLine(next);
                requestUri = next;
            

            return _gameList;
          
        }

        public async Task<List<Game>> FindFavourites(string favouriteGames)
        {
            var gamesToSearch = new List<Game>();
            var gamesToReturn = new List<Game>();

            int[] dividedFavouriteGames = Array.ConvertAll(favouriteGames.Split(' '),int.Parse);

            gamesToSearch = await GetAllToList();

            for(int i = 0; i < dividedFavouriteGames.Length; i++)
            {
                var result = gamesToSearch.Find(x => x.Id.Equals( (dividedFavouriteGames[i]) ) );

                if (result != null)
                    gamesToReturn.Add(result);
            }

            return gamesToReturn;
        }

    }

}
