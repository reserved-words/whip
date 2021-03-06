﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Whip.Common.Model;
using Whip.Services.Interfaces;

namespace Whip.CloudSync
{
    public class Service
    {
        private const int MaxConsecutiveErrorsAllowed = 5;

        private readonly IErrorLoggingService _logger;
        private readonly ITrackRepository _repository;
        private readonly ICloudService _cloudService;
        private readonly ITaggingService _taggingService;
        private readonly SyncData _syncData;

        private int _consecutiveFailures;

        public Service(ITrackRepository repository, ICloudService cloudService, IErrorLoggingService logger, SyncData syncData, ITaggingService taggingService)
        {
            _cloudService = cloudService;
            _logger = logger;
            _repository = repository;
            _syncData = syncData;
            _taggingService = taggingService;
        }

        public void Run()
        {
            var timeLastUpdated = _syncData.GetTimeLastSynced();
            var newSyncTime = DateTime.Now;
            
            var tracksToUpload = GetTracksToUpload(timeLastUpdated);
            Console.WriteLine("Tracks to upload: {0}", tracksToUpload.Count);

            Console.WriteLine("Uploading library");
            _cloudService.UploadFile(_syncData.GetPath("library.xml"));

            Console.WriteLine("Uploading playlists");
            _cloudService.UploadFile(_syncData.GetPath("playlists.xml"));
            
            _consecutiveFailures = 0;

            foreach (var track in tracksToUpload)
            {
                Console.WriteLine("Uploading {0} by {1}", track.Title, track.Artist.Name);
                UploadTrack(track);

                if (_consecutiveFailures > MaxConsecutiveErrorsAllowed)
                {
                    LogTooManyFailures();
                    return;
                }
            }

            var albumsSynced = tracksToUpload.Select(t => t.Disc).Select(d => d.Album).Distinct();
            foreach (var album in albumsSynced)
            {
                Console.WriteLine("Uploading artwork for {0} by {1}", album.Title, album.Artist.Name);
                UploadArtwork(album);

                if (_consecutiveFailures > MaxConsecutiveErrorsAllowed)
                {
                    LogTooManyFailures();
                    return;
                }
            }

            Console.WriteLine("Updating last sync time");
            _syncData.SetTimeLastSynced(newSyncTime);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private List<Track> GetTracksToUpload(DateTime timeLastUpdated)
        {
            var library = _repository.GetLibrary();

            return library.Artists
                .SelectMany(a => a.Albums)
                .SelectMany(a => a.Discs)
                .SelectMany(d => d.Tracks)
                .Where(t => t.File.DateModified >= timeLastUpdated)
                .ToList();
        }

        private void UploadArtwork(Album album)
        {
            try
            {
                album.Artwork = _taggingService.GetArtworkBytes(album.Discs.First().Tracks.First().File.FullPath);
                _cloudService.UploadArtwork(album);
                _consecutiveFailures = 0;
            }
            catch (Exception ex)
            {
                LogError(ex, album);
                _consecutiveFailures++;
            }
        }

        private void UploadTrack(Track track)
        {
            try
            {
                _cloudService.UploadTrack(track);
                _consecutiveFailures = 0;
            }
            catch (Exception ex)
            {
                LogError(ex, track);
                _consecutiveFailures++;
            }
        }

        private void LogTooManyFailures()
        {
            _logger.Log(new ApplicationException($"Exceeded allowable number of consecutive failures ({MaxConsecutiveErrorsAllowed})"));
        }

        private void LogError(Exception ex, Track track)
        {
            ex.Data.Add("Message", "Failed to upload track");
            ex.Data.Add("Title", track.Title);
            ex.Data.Add("Artist", track.Artist.Name);
            _logger.Log(ex);
        }

        private void LogError(Exception ex, Album album)
        {
            ex.Data.Add("Message", "Failed to upload artwork");
            ex.Data.Add("Title", album.Title);
            ex.Data.Add("Artist", album.Artist.Name);
            _logger.Log(ex);
        }
    }
}
