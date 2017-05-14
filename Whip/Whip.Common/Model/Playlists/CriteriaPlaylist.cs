﻿using System;
using System.Collections.Generic;
using Whip.Common.Model.Playlists.Criteria;

namespace Whip.Common.Model
{
    public class CriteriaPlaylist : PlaylistBase
    {
        public CriteriaPlaylist(int id, string title)
            :base(id, title)
        {

        }

        public List<CriteriaGroup> CriteriaGroups { get; set; }

        public PropertyName? OrderByProperty { get; set; }

        public bool OrderByDescending { get; set; }

        public int? MaximumNumber { get; set; }
    }
}
