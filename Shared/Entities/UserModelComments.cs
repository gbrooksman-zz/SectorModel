using System;

namespace SectorModel.Shared.Entities
{
    public class UserModelComment : BaseEntity
    {
        public UserModelComment()
        {

        }

        public Guid Id { get; set; }

        public Guid ModelId { get; set; }

        public Guid UserId { get; set; }

        public string Comment { get; set; }
    }
}
