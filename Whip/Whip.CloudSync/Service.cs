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
        private readonly ILibrarySettings _settings;
        private readonly SyncData _syncData;

        private int _consecutiveFailures;

        public Service(ITrackRepository repository, ICloudService cloudService, IErrorLoggingService logger, ILibrarySettings settings,
            SyncData syncData)
        {
            _cloudService = cloudService;
            _logger = logger;
            _repository = repository;
            _settings = settings;
            _syncData = syncData;
        }

        public void Run()
        {
            var timeLastUpdated = _syncData.GetTimeLastSynced();
            _syncData.SetTimeLastSynced(DateTime.Now);

            var tracksToUpload = GetTracksToUpload(timeLastUpdated);

            _cloudService.UploadFile(Path.Combine(_settings.DataDirectory, "library.xml"));
            _cloudService.UploadFile(Path.Combine(_settings.DataDirectory, "playlists.xml"));

            _consecutiveFailures = 0;

            foreach (var track in tracksToUpload)
            {
                UploadTrack(track);

                if (_consecutiveFailures > MaxConsecutiveErrorsAllowed)
                {
                    LogTooManyFailures();
                    return;
                }
            }
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
            ex.Data.Add("Title", track.Title);
            ex.Data.Add("Artist", track.Artist.Name);
            _logger.Log(ex);
        }
    }
}
