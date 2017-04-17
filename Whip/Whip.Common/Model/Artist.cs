﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whip.Common.Model
{
    public class Artist
    {
        public Artist()
        {
            Albums = new List<Album>();
            Tracks = new List<Track>();
        }

        public string Name { get; set; }
        public string Genre { get; set; }
        public string Grouping { get; set; }
        public City City { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string LastFm { get; set; }
        public string Wikipedia { get; set; }

        public List<Album> Albums { get; set; }
        public List<Track> Tracks { get; set; }

        public override bool Equals(object a)
        {
            var artist = a as Artist;
            if (artist == null)
                return false;

            return artist.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
