using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Services
{
    public interface IAccountRepository
    {
        Task<IdentityResult> AddUserAsync(IdentityUser user, string password);
        Task<IdentityUser> GetUserAsync(string userId);
        bool Save();
    }
}
