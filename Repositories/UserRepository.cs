using ApiAuth.Models;

namespace ApiAuth.Repositories
{
    public static class UserRepository
    {
        public static User Get (
            string username,
            string password
        )
        {
            var users = new List<User> { 
                new User{ Id = 1, Username = "Batman", Password = "Batman", Role = "manager"},
                new User{ Id = 2, Username = "Robin", Password = "Robin", Role = "employee"}
            };

            return users.FirstOrDefault(x => 
                x.Username.ToLower() == username.ToLower() 
                && x.Password == password
            );
        }
    }
}
