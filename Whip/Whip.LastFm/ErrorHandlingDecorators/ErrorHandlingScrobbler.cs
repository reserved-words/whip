﻿using System;
using System.Threading.Tasks;
using Whip.Common.Enums;
using Whip.Common.Model;
using Whip.Services.Interfaces;

namespace Whip.Services
{
    public class ErrorHandlingScrobbler : IScrobbler
    {
        private const int MinimumTrackDuration = 31;

        private readonly IAsyncMethodInterceptor _asyncMethodInterceptor;
        private readonly IScrobbleCacher _scrobbleCacher;
        private readonly IScrobbler _scrobbler;
        private readonly IUserSettings _userSettings;

        private Track _track;
        private int _duration;

        public ErrorHandlingScrobbler(IScrobbler scrobbler, IUserSettings userSettings, IAsyncMethodInterceptor asyncMethodInterceptor,
            IScrobbleCacher scrobbleCacher)
        {
            _asyncMethodInterceptor = asyncMethodInterceptor;
            _scrobbleCacher = scrobbleCacher;
            _scrobbler = scrobbler;
            _userSettings = userSettings;

            _userSettings.ScrobblingStatusChanged += OnScrobblingStatusChanged;
        }

        private async void OnScrobblingStatusChanged()
        {
            if (_track == null)
                return;
            
            await UpdateNowPlayingAsync(_track, _userSettings.Scrobbling ? _duration : MinimumTrackDuration);
        }

        public async Task<bool> ScrobbleAsync(Track track, DateTime timePlayed)
        {
            if (!_userSettings.Scrobbling)
                return true;
            
            var success = await _asyncMethodInterceptor.TryMethod(
                _scrobbler.ScrobbleAsync(track, timePlayed),
                false,
                WebServiceType.LastFm,
                "ScrobbleAsync (Track: " + track.Title + " by " + track.Artist.Name + ")");

            if (!success)
            {
                _scrobbleCacher.Cache(track, timePlayed);
            }

            return success;
        }

        public async Task<bool> UpdateNowPlayingAsync(Track track, int duration)
        {
            _track = track;
            _duration = duration;

            if (!_userSettings.Scrobbling)   
                return true;
            
            return await _asyncMethodInterceptor.TryMethod(
                _scrobbler.UpdateNowPlayingAsync(track, duration),
                false,
                WebServiceType.LastFm,
                "UpdateNowPlayingAsync (Track: " + track.Title + " by " + track.Artist.Name + ")");
        }
    }
}
