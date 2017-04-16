﻿using Whip.Common;
using Whip.ViewModels.Utilities;

namespace Whip.ViewModels.TabViewModels
{
    public class NewsViewModel : TabViewModelBase
    {
        public NewsViewModel()
            :base(TabType.News, IconType.Rss, "News")
        {

        }
    }
}