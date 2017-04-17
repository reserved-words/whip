﻿using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using Whip.Common;
using Whip.Common.Model;
using Whip.ViewModels.Utilities;
using GalaSoft.MvvmLight.Messaging;
using Whip.ViewModels.Messages;
using Whip.Services.Interfaces.Singletons;

namespace Whip.ViewModels.TabViewModels
{
    public class CurrentPlaylistViewModel : TabViewModelBase
    {
        private readonly IMessenger _messenger;
        private readonly IPlaylist _playlist;

        private Track _selectedTrack;

        public CurrentPlaylistViewModel(IPlaylist playlist, IMessenger messenger)
            :base(TabType.CurrentPlaylist, IconType.ListOl, "Current Playlist")
        {
            _messenger = messenger;
            _playlist = playlist;

            _playlist.ListUpdated += OnPlaylistUpdated;

            EditTrackCommand = new RelayCommand(OnEditTrack);
            PlayCommand = new RelayCommand(OnPlay);
        }

        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand EditTrackCommand { get; private set; }

        public string PlaylistName => _playlist.PlaylistName;

        public Track SelectedTrack
        {
            get { return _selectedTrack; }
            set { Set(ref _selectedTrack, value); }
        }

        public List<Track> Tracks => _playlist.Tracks;

        private void OnEditTrack()
        {
            _messenger.Send(new EditTrackMessage(SelectedTrack));
        }

        private void OnPlay()
        {
            if (SelectedTrack != null)
            {
                _messenger.Send(new MoveToTrackMessage(SelectedTrack));
            }
        }

        private void OnPlaylistUpdated()
        {
            RaisePropertyChanged(nameof(PlaylistName));
            RaisePropertyChanged(nameof(Tracks));
        }
    }
}
