using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DataBase
{
    public class UserRole
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<User> Users { get; set; } = new List<User>();
    }
}
