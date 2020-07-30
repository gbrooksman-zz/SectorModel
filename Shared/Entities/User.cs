using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SectorModel.Shared.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            
        }

        public string Email { get; set; }

        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        public bool IsActive { get; set; }
    }
}
