﻿using GalaSoft.MvvmLight.Messaging;
using System;
using Whip.Services.Interfaces;
using Whip.ViewModels.Messages;

namespace Whip.Services
{
    public class UserSettings : IUserSettings
    {
        private readonly IMessenger _messenger;

        private bool _scrobblingStatusChanged;
        private bool _shuffleStatusChanged;
        private bool _libraryUpdated;
        private bool _lastFmUsernameChanged;

        public UserSettings(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public event Action ScrobblingStatusChanged;
        public event Action ShufflingStatusChanged;

        public bool EssentialSettingsSet => !string.IsNullOrEmpty(MusicDirectory)
            && !string.IsNullOrEmpty(LastFmUsername)
            && !string.IsNullOrEmpty(LastFmApiKey)
            && !string.IsNullOrEmpty(LastFmApiSecret);

        public string MusicDirectory
        {
            get { return Properties.Settings.Default.MusicDirectory; }
            set
            {
                if (value != Properties.Settings.Default.MusicDirectory)
                {
                    Properties.Settings.Default.MusicDirectory = value;
                    _libraryUpdated = true;
                }
            }
        }

        public string MainColourRgb
        {
            get { return Properties.Settings.Default.MainColourRgb; }
            set { Properties.Settings.Default.MainColourRgb = value; }
        }

        public string LastFmApiKey
        {
            get { return Properties.Settings.Default.LastFmApiKey; }
            set { Properties.Settings.Default.LastFmApiKey = value; }
        }

        public string LastFmApiSecret
        {
            get { return Properties.Settings.Default.LastFmApiSecret; }
            set { Properties.Settings.Default.LastFmApiSecret = value; }
        }

        public string LastFmApiSessionKey
        {
            get { return Properties.Settings.Default.LastFmApiSessionKey; }
            set { Properties.Settings.Default.LastFmApiSessionKey = value; }
        }

        public string LastFmUsername
        {
            get { return Properties.Settings.Default.LastFmUsername; }
            set
            {
                if (value != Properties.Settings.Default.LastFmUsername)
                {
                    Properties.Settings.Default.LastFmUsername = value;
                    _lastFmUsernameChanged = true;
                }
            }
        }

        public bool Scrobbling
        {
            get { return Properties.Settings.Default.Scrobbling; }
            set
            {
                if (value != Properties.Settings.Default.Scrobbling)
                {
                    Properties.Settings.Default.Scrobbling = value;
                    _scrobblingStatusChanged = true;
                }
            }
        }

        public bool ShuffleOn
        {
            get { return Properties.Settings.Default.ShuffleOn; }
            set
            {
                if (value != Properties.Settings.Default.ShuffleOn)
                {
                    Properties.Settings.Default.ShuffleOn = value;
                    _shuffleStatusChanged = true;
                }
            }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
            
            if (_lastFmUsernameChanged)
            {
                _lastFmUsernameChanged = false;
                // Update session key
            }

            if (_scrobblingStatusChanged)
            {
                _scrobblingStatusChanged = false;
                ScrobblingStatusChanged?.Invoke();
            }

            if (_shuffleStatusChanged)
            {
                _shuffleStatusChanged = false;
                ShufflingStatusChanged?.Invoke();
            }

            if (_libraryUpdated && !string.IsNullOrEmpty(MusicDirectory))
            {
                _libraryUpdated = false;
                _messenger.Send(new LibraryUpdateRequest());
            }
        }
    }
}