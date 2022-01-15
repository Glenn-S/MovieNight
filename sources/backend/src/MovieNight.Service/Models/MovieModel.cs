using System;

namespace MovieNight.Service.Models
{
    public class MovieModel :
        IAuditModel
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
        
        // Audit Information
        public AuditModel AuditInformation { get; set; }
    }
}
