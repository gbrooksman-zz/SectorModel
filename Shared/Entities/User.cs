using System;


namespace SectorModel.Shared.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            
        }

        public Guid Id { get; set; }       

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool Active { get; set; }
    }
}
