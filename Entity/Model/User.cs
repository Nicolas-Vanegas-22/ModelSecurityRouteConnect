using System;

namespace Entity.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email  { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Password { get; set; }
        public string DeleteAt { get; set; }
        public string CreateAt { get; set; }
    }
}