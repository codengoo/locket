﻿using System.ComponentModel.DataAnnotations;

namespace locket.DTOs
{
    public class AuthDto
    {
        public class ISignInByUsername
        {
            [Required]
            [StringLength(16, MinimumLength = 8)]
            public string Username { get; set; }
            [Required]
            [StringLength(36, MinimumLength = 8)]
            public string Password { get; set; }
        }

        public class IReturnSignInByUser
        {
            public Guid Uid { get; set; }
            public string Username { get; set; }
        }

        public class ILoginByUsername
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
        }
    }
}
