using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gamezzz.Models
{
    public class GameViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        [Range(1990, 2020)]
        public int YearOfRelease { get; set; }
        [Required]
        public IFormFile gameImage { get; set; }
    }
}
