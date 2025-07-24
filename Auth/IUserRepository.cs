using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelsWebApi.Auth
{
    public interface IUserRepository
    {
        UserDto GetUser(UserModel userModel);
    }
}