using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsletterFunction.Models;

namespace NewsletterFunction.Services
{
    public interface IUserService
    {
        List<User> CheckSubscribers();
    }
}
