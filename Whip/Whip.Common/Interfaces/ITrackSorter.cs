﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whip.Common.Model;

namespace Whip.Common.Interfaces
{
    public interface ITrackSorter
    {
        IEnumerable<Track> Sort(IEnumerable<Track> tracks);
    }

    public interface IDefaultTrackSorter : ITrackSorter
    {
    }

    public interface IRandomTrackSorter : ITrackSorter
    {
    }
}
