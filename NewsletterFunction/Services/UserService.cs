using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsletterFunction.Models;

namespace NewsletterFunction.Services
{
    internal class UserService : IUserService
    {
        private readonly FuncDBContext _db;

        public UserService(FuncDBContext db)
        {
            _db = db;
        }

        public List<User> CheckSubscribers()
        {
            return _db.Users.Where(u => u.Newsletter).ToList();
        }
    }
}
