using System;


namespace SectorModel.Shared.Entities
{

    public class Model : BaseEntity
    {
        public Model()
        {
            
        }


        public Guid UserId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime StopDate { get; set; }

        public int StartValue { get; set; }

        public int StopValue { get; set; }

        public int Version { get; set; }

        public bool IsPrivate { get; set; }

        public Guid ModelVersionId { get; set; }
    }
}