using System;

namespace SectorModel.Shared.Entities
{  
    public class UserModel : BaseEntity
    {
        public UserModel()
        {
            
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }       

        public string Name { get; set; }

        public bool Active {get; set;}

        public DateTime StartDate {get; set;}

        public DateTime StopDate { get; set; }

        public int StartValue { get; set; } 

        public int StopValue { get; set; }

        public int Version { get; set; }

        public bool IsPrivate { get; set; }

        public Guid ModelVersionId { get; set; }
    }
}
