using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsletterFunction.Models;

namespace NewsletterFunction
{
    public class FuncDBContext : IdentityDbContext<User>
    {
        public FuncDBContext(DbContextOptions<FuncDBContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
    }
}
