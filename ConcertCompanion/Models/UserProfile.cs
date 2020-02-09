using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcertCompanion.Models
{
    public class Artist
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IList<Artist> TopArtists { get; set; }
    }
}
