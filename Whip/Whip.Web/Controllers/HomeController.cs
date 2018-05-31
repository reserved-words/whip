﻿using System.Web.Mvc;
using Whip.Common.Interfaces;
using Whip.Common.Singletons;
using Whip.Services.Interfaces;
using Whip.Services.Interfaces.Singletons;
using Whip.Web.Interfaces;
using Whip.Web.Models;

namespace Whip.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IPlaylistService _playlistsService;
        private readonly ICloudService _cloudService;

        public HomeController(IPlaylist playlist, Library library, IPlaylistService playlistsService, 
            ICloudService cloudService, ITrackRepository trackRepository)
            :base(trackRepository, cloudService, playlist, library)
        {
            _cloudService = cloudService;
            _playlistsService = playlistsService;
            _trackRepository = trackRepository;
        }

        public ActionResult Index()
        {
            var playlists = _playlistsService.GetAll();

            var model = new PlaylistsViewModel();

            foreach (var playlist in playlists.CriteriaPlaylists)
            {
                model.CriteriaPlaylists.Add(new PlaylistViewModel { Id = playlist.Id, Title = playlist.Title });
            }

            foreach (var playlist in playlists.OrderedPlaylists)
            {
                model.OrderedPlaylists.Add(new PlaylistViewModel { Id = playlist.Id, Title = playlist.Title });
            }

            foreach (var playlist in playlists.FavouriteQuickPlaylists)
            {
                model.StandardPlaylists.Add(new PlaylistViewModel { Id = playlist.Id, Title = playlist.Title });
            }

            return View(model);
        }

        public ActionResult StandardPlaylist(int id)
        {
            var playlist = _playlistsService.GetQuickPlaylist(id, Library);
            return Play(playlist.Item1.Title, playlist.Item2, null, true);
        }

        public ActionResult OrderedPlaylist(int id)
        {
            var playlist = _playlistsService.GetOrderedPlaylist(id, Library);
            return Play(playlist.Item1.Title, playlist.Item2, null, false);
        }

        public ActionResult CriteriaPlaylist(int id)
        {
            var playlist = _playlistsService.GetCriteriaPlaylist(id, Library);
            return Play(playlist.Item1.Title, playlist.Item2, null, true);
        }

        private JsonResult GetCurrentTrack()
        {
            var track = Playlist.CurrentTrack;

            return new JsonResult
            {
                Data = track == null
                    ? null
                    : new
                    {
                        Description = track.Title + " by " + track.Artist.Name,
                        Url = _cloudService.GetTrackUrl(track),
                        ArtworkUrl = _cloudService.GetArtworkUrl(track.Disc.Album)
                    }
            };
        }

        [HttpPost]
        public JsonResult GetNextTrack()
        {
            Playlist.MoveNext();
            return GetCurrentTrack();
        }

        [HttpPost]
        public JsonResult GetPreviousTrack()
        {
            Playlist.MoveNext();
            return GetCurrentTrack();
        }
    }
}