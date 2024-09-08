using System.ComponentModel.DataAnnotations;

namespace locket.DTOs
{
    public class AuthDto
    {
        public class ISignInByUsername {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
        }
    }
}
