using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelsWebApi.Auth
{
    public class UserRepository: IUserRepository
    {
        private List<UserDto> _users =>new()
        {
            new UserDto("Mark", "123"),
            new UserDto("Phil", "5432"),
            new UserDto("Alice", "666")
        };
        public UserDto GetUser(UserModel userModel) =>
                _users.FirstOrDefault(u =>
                string.Equals(u.UserName, userModel.UserName) &&
                string.Equals(u.Password , userModel.Password)) ??
                throw  new Exception();

        
        
    }
}