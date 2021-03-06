﻿using System.Collections.Generic;
using Whip.Common.Enums;
using Whip.Common.Interfaces;
using Whip.Common.Model.Playlists.Criteria;

namespace Whip.Common.Model
{
    public class CriteriaPlaylist : Playlist, ITrackCriteria
    {
        public CriteriaPlaylist(int id, string title, bool favourite)
            :base(PlaylistType.Criteria, id, title, favourite)
        {
            CriteriaGroups = new List<CriteriaGroup>();
        }

        public List<CriteriaGroup> CriteriaGroups { get; set; }

        public PropertyName? OrderByProperty { get; set; }

        public bool OrderByDescending { get; set; }

        public int? MaxTracks { get; set; }
    }
}
