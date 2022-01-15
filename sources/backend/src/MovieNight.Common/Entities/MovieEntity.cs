using System;

namespace MovieNight.Common.Entities
{
    public class MovieEntity :
        IAudit
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public decimal Rating { get; set; }
        public TimeSpan FilmLength { get; set; } // total number of minutes
        // Eventually add in tables for directors, stars, writers
        public DateTime ReleaseDate { get; set; }

        // AuditFields
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
