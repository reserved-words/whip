﻿using System.Web.Mvc;
using System.Web.UI;
using Whip.Services.Interfaces;
using Whip.Services.Interfaces.Singletons;
using Whip.Web.Models;
using Whip.Web.Interfaces;

namespace Whip.Web.Controllers
{
    public class CurrentPlaylistController : BaseController
    {
        private readonly ICloudService _cloudService;

        public CurrentPlaylistController(ICloudService cloudService, IPlaylist playlist, IErrorLoggingService logger, IPlaySettings playSettings)
            : base(cloudService, playlist, logger, playSettings)
        {
            _cloudService = cloudService;
        }

        [OutputCache(Duration = 1800, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            var tracklist = new TrackListViewModel(Playlist.Tracks, 1, 30, GetViewModel);

            var model = new PlayViewModel(Playlist.PlaylistName, tracklist);

            return PartialView("_Index", model);
        }

        [HttpPost]
        public new JsonResult GetCurrentTrack()
        {
            return base.GetCurrentTrack();
        }

        [HttpPost]
        public JsonResult GetNextTrack()
        {
            Playlist.MoveNext();
            ClearTrackCache();
            return GetCurrentTrack();
        }

        [HttpPost]
        public JsonResult GetPreviousTrack()
        {
            Playlist.MovePrevious();
            ClearTrackCache();
            return GetCurrentTrack();
        }

        [HttpPost]
        public JsonResult Restart()
        {
            return Play();
        }
    }
}