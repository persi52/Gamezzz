using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Gamezzz.Models
{
    public class AppUser : IdentityUser
    {   
       public string favouriteGames { get; set; }
    }
}
