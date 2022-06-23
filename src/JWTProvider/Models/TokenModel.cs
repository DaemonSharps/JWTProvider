using System;
using System.ComponentModel.DataAnnotations;

namespace JWTProvider.Models
{
    public class TokenModel
    {
        [Required]
        public string AccessToken { get; set; }

        public Guid RefreshToken { get; set; }
    }
}
