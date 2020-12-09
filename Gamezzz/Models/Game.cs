using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gamezzz.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }        
        [Required]
        public List<string> Category { get; set; }
        [Required]
        [Range(1990, 2020)]
        public string YearOfRelease { get; set; }

        public string photoName { get; set; }
       
    }
}
