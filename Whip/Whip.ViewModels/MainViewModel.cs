﻿using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using Whip.Common.Interfaces;
using Whip.Common.Model;
using Whip.ViewModels.Messages;
using Whip.ViewModels.TabViewModels;
using Whip.ViewModels.Utilities;
using Whip.ViewModels.Windows;
using static Whip.Common.Resources;

namespace Whip.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public event Action FavouritePlaylistsUpdated = delegate { };

        private readonly IMessenger _messenger;
        private readonly EditTrackViewModel _editTrackViewModel;
        private readonly EditCriteriaPlaylistViewModel _editCriteriaPlaylistViewModel;
        private readonly EditOrderedPlaylistViewModel _editOrderedPlaylistViewModel;
        private readonly EditSettingsViewModel _settingsViewModel;

        private TabViewModelBase _selectedTab;
        private TabViewModelBase _returnToTab;
        private Track _currentTrack;

        private bool _selectedTabSetByViewModel;

        public MainViewModel(
            DashboardViewModel dashboardViewModel,
            LibraryViewModel libraryViewModel,
            PlaylistsViewModel playlistsViewModel, 
            CurrentPlaylistViewModel currentPlaylistViewModel,
            CurrentTrackViewModel currentTrackViewModel,
            ArtistViewModel artistViewModel,
            LastFmViewModel lastFmViewModel,
            NewsViewModel newsViewModel,
            SearchViewModel searchViewModel,
            ArchiveViewModel archiveViewModel,
            PlayHistoryViewModel playHistoryViewModel,
            SystemInfoViewModel systemInfoViewModel,
            EditSettingsViewModel settingsViewModel,
            EditTrackViewModel editTrackViewModel,
            EditCriteriaPlaylistViewModel editCriteriaPlaylistViewModel,
            EditOrderedPlaylistViewModel editOrderedPlaylistViewModel,
            IMessenger messenger,
            IShowTabRequestHandler showTabRequester)
        {
            Tabs = new List<TabViewModelBase>
            {
                dashboardViewModel,
                libraryViewModel,
                playlistsViewModel,
                currentPlaylistViewModel,
                currentTrackViewModel,
                artistViewModel,
                lastFmViewModel,
                newsViewModel,
                searchViewModel,
                archiveViewModel,
                playHistoryViewModel,
                systemInfoViewModel,
                settingsViewModel,
                editTrackViewModel,
                editCriteriaPlaylistViewModel,
                editOrderedPlaylistViewModel,
            };

            _editCriteriaPlaylistViewModel = editCriteriaPlaylistViewModel;
            _editOrderedPlaylistViewModel = editOrderedPlaylistViewModel;
            _editTrackViewModel = editTrackViewModel;
            _settingsViewModel = settingsViewModel;
            
            _messenger = messenger;

            _returnToTab = dashboardViewModel;

            SetSelectedTab(dashboardViewModel);

            ChangeTabCommand = new RelayCommand<TabViewModelBase>(OnChangingTab, CanChangeTab);

            showTabRequester.ShowEditTrackTab += ShowEditTrackTab;
            showTabRequester.ShowSettingsTab += ShowSettingsTab;
            showTabRequester.ShowEditCriteriaPlaylistTab += ShowEditCriteriaPlaylistTab;
            showTabRequester.ShowEditOrderedPlaylistTab += ShowEditOrderedPlaylistTab;

            _editTrackViewModel.FinishedEditing += OnFinishedEditing;
            _settingsViewModel.FinishedEditing += OnFinishedEditing;
            _editCriteriaPlaylistViewModel.FinishedEditing += OnFinishedEditing;
            _editOrderedPlaylistViewModel.FinishedEditing += OnFinishedEditing;

            playlistsViewModel.FavouritePlaylistsUpdated += OnFavouritePlaylistsUpdated;
        }

        private void OnFavouritePlaylistsUpdated()
        {
            FavouritePlaylistsUpdated.Invoke();
        }

        private void OnFinishedEditing(EditableTabViewModelBase sender)
        {
            sender.IsVisible = false;
            SetSelectedTab(_returnToTab);
        }

        private bool CanChangeTab(TabViewModelBase newTab)
        {
            if (_selectedTabSetByViewModel)
            {
                _selectedTabSetByViewModel = false;
                return true;
            }

            _returnToTab = newTab;

            var editable = _selectedTab as EditableTabViewModelBase;

            if (editable == null)
                return true;

            if (editable.Modified)
            {
                var confirmationViewModel = new ConfirmationViewModel(_messenger, UnsavedChangesTitle, UnsavedChangesText, ConfirmationViewModel.ConfirmationType.YesNo);

                _messenger.Send(new ShowDialogMessage(confirmationViewModel));

                if (!confirmationViewModel.Result)
                    return false;
            }

            editable.OnCancel();
            return true;
        }

        private void SetSelectedTab(TabViewModelBase tab)
        {
            _selectedTabSetByViewModel = true;
            SelectedTab = tab;
        }

        private void ShowSettingsTab()
        {
            _returnToTab = SelectedTab;
            _settingsViewModel.Reset();
            _settingsViewModel.IsVisible = true;
            SetSelectedTab(_settingsViewModel);
        }

        private void ShowEditTrackTab(Track track)
        {
            _returnToTab = SelectedTab;
            _editTrackViewModel.Edit(track);
            _editTrackViewModel.IsVisible = true;
            SetSelectedTab(_editTrackViewModel);
        }

        private void ShowEditCriteriaPlaylistTab(CriteriaPlaylist playlist)
        {
            _returnToTab = SelectedTab;
            _editCriteriaPlaylistViewModel.Edit(playlist);
            _editCriteriaPlaylistViewModel.IsVisible = true;
            SetSelectedTab(_editCriteriaPlaylistViewModel);
        }

        private void ShowEditOrderedPlaylistTab(OrderedPlaylist playlist)
        {
            _returnToTab = SelectedTab;
            _editOrderedPlaylistViewModel.Edit(playlist);
            _editOrderedPlaylistViewModel.IsVisible = true;
            SetSelectedTab(_editOrderedPlaylistViewModel);
        }

        private void OnChangingTab(TabViewModelBase newTab) { }

        public List<TabViewModelBase> Tabs { get; }

        public RelayCommand<TabViewModelBase> ChangeTabCommand { get; }

        public TabViewModelBase SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab == value)
                    return;

                _selectedTab?.OnHide();
                value.OnShow(_currentTrack);
                Set(ref _selectedTab, value);
            }
        }

        public void OnCurrentTrackChanged(Track track)
        {
            _currentTrack = track;
            SelectedTab.OnCurrentTrackChanged(track);
        }
        
        public void SelectTab(TabType key)
        {
            SetSelectedTab(Tabs.Single(t => t.Key == key));
        }
    }
}
