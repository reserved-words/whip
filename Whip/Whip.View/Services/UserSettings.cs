﻿using GalaSoft.MvvmLight.Messaging;
using LastFmApi;
using System;
using System.Threading.Tasks;
using Whip.LastFm;
using Whip.Services.Interfaces;
using Whip.ViewModels.Messages;
using Whip.ViewModels.Windows;

namespace Whip.View
{
    public class UserSettings : IUserSettings
    {
        private readonly ILastFmApiClientService _lastFmApiClientService;
        private readonly IMessenger _messenger;

        private bool _scrobblingStatusChanged;
        private bool _shuffleStatusChanged;
        private bool _libraryUpdated;
        private bool _lastFmUsernameChanged;

        private string _lastFmErrorMessage;

        public UserSettings(IMessenger messenger, ILastFmApiClientService lastFmApiClientService)
        {
            _lastFmApiClientService = lastFmApiClientService;
            _messenger = messenger;
        }

        public event Action ScrobblingStatusChanged;
        public event Action ShufflingStatusChanged;

        public bool EssentialSettingsSet => !string.IsNullOrEmpty(MusicDirectory)
            && !string.IsNullOrEmpty(LastFmUsername)
            && !string.IsNullOrEmpty(LastFmApiKey)
            && !string.IsNullOrEmpty(LastFmApiSecret)
            && !string.IsNullOrEmpty(MainColourRgb);

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

        public string ArchiveDirectory
        {
            get { return Properties.Settings.Default.ArchiveDirectory; }
            set { Properties.Settings.Default.ArchiveDirectory = value; }
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
                    Properties.Settings.Default.LastFmApiSessionKey = null;
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

        public bool LastFmStatus
        {
            get { return Properties.Settings.Default.LastFmStatus; }
            set { Properties.Settings.Default.LastFmStatus = value; }
        }

        public bool Offline
        {
            get { return Properties.Settings.Default.Offline; }
            set { Properties.Settings.Default.Offline = value; }
        }

        public string LastFmErrorMessage
        {
            get { return _lastFmErrorMessage; }
            private set { _lastFmErrorMessage = value; }
        }

        public void Save()
        {
            if (_lastFmUsernameChanged)
            {
                _lastFmUsernameChanged = false;
                SetLastFmClients();
            }

            Properties.Settings.Default.Save();

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

        private void SetLastFmClients()
        {
            if (string.IsNullOrEmpty(LastFmUsername))
                return;

            try
            {
                _lastFmApiClientService.SetClients(LastFmApiKey, LastFmApiSecret, LastFmUsername, LastFmApiSessionKey).Wait();
                LastFmApiSessionKey = _lastFmApiClientService.AuthorizedApiClient.SessionKey;
            }
            catch (LastFmApiException ex)
            {
                if (ex.ErrorCode == ErrorCode.UserNotLoggedIn)
                {
                    _messenger.Send(new ShowDialogMessage(new MessageViewModel(_messenger, "Last.FM Error", ex.Message)));
                    LastFmUsername = string.Empty;
                    LastFmApiSessionKey = null;
                    return;
                }

                throw;
            }
        }

        public void SetInternetStatus(bool online)
        {
            if (online && Offline)
            {
                Offline = false;
                Properties.Settings.Default.Save();
            }
            else if (!online && !Offline)
            {
                Offline = true;
                Properties.Settings.Default.Save();
            }
        }

        public void SetStartupDefaults()
        {
            LastFmStatus = true;
            Offline = false;
            
            SetLastFmClients();

            Properties.Settings.Default.Save();
        }

        public void TurnOffLastFm(string errorMessage)
        {
            LastFmStatus = false;
            LastFmErrorMessage = errorMessage;
            Properties.Settings.Default.Save();
        }
    }
}