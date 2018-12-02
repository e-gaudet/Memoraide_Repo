using Memoraide_WebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memoraide_WebApp.Data
{
    public class MemoraideDbContext : IdentityDbContext<User>
    {
        public MemoraideDbContext(DbContextOptions<MemoraideDbContext> options)
            :base (options)
        {

        }
    }
}
