using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gamezzz.Models
{
    public class ApplicationDbGamesContext : DbContext
    {
        public ApplicationDbGamesContext(DbContextOptions<ApplicationDbGamesContext> options) : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }
    }
}
