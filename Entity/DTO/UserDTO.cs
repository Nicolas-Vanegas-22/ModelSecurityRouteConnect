using System;

namespace Entity.DTO
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public object UserId { get; set; }
    }
}
