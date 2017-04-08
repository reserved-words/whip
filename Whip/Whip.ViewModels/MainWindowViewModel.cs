﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using Whip.Common.Singletons;
using Whip.Common.Utilities;
using Whip.Services.Interfaces;
using Whip.ViewModels.Messages;
using Whip.ViewModels.TabViewModels;
using Whip.ViewModels.Windows;

namespace Whip.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ILibraryService _libraryService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IMessenger _messenger;

        private readonly LibraryViewModel _libraryViewModel;

        private Library _library;

        public MainWindowViewModel(ILibraryService libraryService, IUserSettingsService userSettingsService, Library library,
            IMessenger messenger, LibraryViewModel libraryViewModel)
        {
            _libraryService = libraryService;
            _userSettingsService = userSettingsService;
            _library = library;
            _messenger = messenger;

            _libraryViewModel = libraryViewModel;

            ApplicationSettingsCommand = new RelayCommand(OnApplicationSettings);
            PopulateLibraryCommand = new RelayCommand(OnPopulateLibrary);
            SaveLibraryCommand = new RelayCommand(OnSaveLibrary);
        }
        
        public MainViewModel MainViewModel => new MainViewModel(_libraryViewModel);
        public SidebarViewModel SidebarViewModel => new SidebarViewModel();

        public RelayCommand ApplicationSettingsCommand { get; private set; }
        public RelayCommand PopulateLibraryCommand { get; private set; }
        public RelayCommand SaveLibraryCommand { get; private set; }

        private void OnApplicationSettings()
        {
            OnApplicationSettings(null);
        }

        private void OnApplicationSettings(Action callback)
        {
            var applicationSettingsViewModel = new ApplicationSettingsViewModel(_userSettingsService, _messenger, callback);

            _messenger.Send(new ShowDialogMessage(applicationSettingsViewModel));
        }

        private void OnPopulateLibrary()
        {
            if (!_userSettingsService.EssentialSettingsSet)
            {
                OnApplicationSettings(OnPopulateLibrary);
                return;
            }

            var progressBarViewModel = new ProgressBarViewModel("Populating Library");
            var startProgressBarMessage = new ShowDialogMessage(progressBarViewModel);

            OnPopulateLibraryAsync(progressBarViewModel);

            _messenger.Send(startProgressBarMessage);
        }

        private async void OnPopulateLibraryAsync(ProgressBarViewModel progressBarViewModel)
        {
            var progressHandler = new Progress<ProgressArgs>(progressBarViewModel.Update);
            var stopProgressBarMessage = new HideDialogMessage(progressBarViewModel.Guid);

            _library.Update(await _libraryService.GetLibraryAsync(progressHandler));

            _messenger.Send(stopProgressBarMessage);
        }

        private void OnSaveLibrary()
        {
            _libraryService.SaveLibrary(_library);
        }
    }
}
