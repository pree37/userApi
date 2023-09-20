using System.ComponentModel.DataAnnotations;

namespace usersDemo.Models
{
    public class User
    {
 

        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Salt { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FirstName { get; set; }

        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;


    }

    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }

    public class login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }


    public class auth
    {
        public string Username { get; set; }
        public string Role { get; set; }

    }


}
