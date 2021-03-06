﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Whip.Common;
using Whip.Services.Interfaces;
using Whip.Services.Interfaces.Singletons;
using Whip.Web.ExtensionMethods;
using Whip.Web.Interfaces;
using Whip.Web.Models;

namespace Whip.Web.Controllers
{
    public class LibraryController : BaseController
    {
        private readonly ICloudService _cloudService;
        private readonly Interfaces.ILibraryService _library;

        public LibraryController(IPlaylist playlist, IErrorLoggingService logger, ICloudService cloudService, Interfaces.ILibraryService library, IPlaySettings playSettings)
            : base(cloudService, playlist, logger, playSettings)
        {
            _cloudService = cloudService;
            _library = library;
        }

        [OutputCache(Duration = 3600, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            var model = new LibraryViewModel();
            return PartialView("_Index", model);
        }

        [OutputCache(Duration = 3600, VaryByParam = "category", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Artists(string category)
        {
            var artists = _library.Library.Artists
                .Where(a => a.Category() == category && a.Albums.Any())
                .OrderBy(a => a.Sort)
                .Select(a => new ArtistViewModel(a, Url.Action("PlayArtist", new { name = a.Name }), Url.Action("Artist", new { name = a.Name })))
                .ToList();

            return PartialView("_Artists", new LibraryArtistListViewModel(category, artists));
        }

        public ActionResult Artist(string name)
        {
            var artist = _library.Library.Artists.Single(a => a.Name == name);
            var model = new LibraryArtistViewModel(
                artist, 
                Url.Action(nameof(PlayArtist), new { name = artist.Name }),
                a => Url.Action(nameof(PlayAlbum), new { artist = artist.Name, title = a.Title, releaseType = a.ReleaseType }), 
                a => Url.Action(nameof(Album), new { artist = artist.Name, title = a.Title, releaseType = a.ReleaseType }));
            return PartialView("_Artist", model);
        }

        public ActionResult Album(string artist, string title, ReleaseType releaseType)
        {
            var albumArtist = _library.Library.Artists.Single(a => a.Name == artist);
            var album = albumArtist.Albums.Single(a => a.Title == title && a.ReleaseType == releaseType);
            var model = new LibraryAlbumViewModel(album,
                Url.Action(nameof(PlayAlbum), new {artist, title, releaseType}),
                _cloudService.GetArtworkUrl(album),
                _cloudService.GetTrackUrl,
                _cloudService.GetArtworkUrl);
            return PartialView("_Album", model);
        }

        public JsonResult ShuffleAll()
        {
            return Play("Library", _library.Tracks);
        }

        public JsonResult PlayArtist(string name)
        {
            var artist = _library.Library.Artists.Single(a => a.Name == name);
            return Play(artist.Name, artist.Albums.SelectMany(a => a.Discs.SelectMany(d => d.Tracks)).ToList());
        }

        public JsonResult PlayAlbum(string artist, string title, ReleaseType releaseType)
        {
            var album = _library.Library.Artists.Single(a => a.Name == artist)
                .Albums.Single(a => a.ReleaseType == releaseType
                    && a.Title == title);
            return Play(
                $"{title} by {artist}", 
                album.Discs
                    .OrderBy(d => d.DiscNo)
                    .SelectMany(d => d.Tracks)
                    .OrderBy(t => t.TrackNo)
                    .ToList(), 
                null, 
                false);
        }
    }
}