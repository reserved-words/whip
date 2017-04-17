﻿using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using Whip.Common;
using Whip.Services.Interfaces;
using Whip.ViewModels.Messages;
using Whip.ViewModels.Utilities;

namespace Whip.ViewModels.SidebarViewModels
{
    public class SettingsIconsViewModel
    {
        private readonly IMessenger _messenger;
        private readonly IUserSettings _userSettings;

        public SettingsIconsViewModel(IMessenger messenger, IUserSettings userSettings)
        {
            _messenger = messenger;
            _userSettings = userSettings;

            PopulateIconCommands();

            _userSettings.ScrobblingStatusChanged += ScrobblingStatusChanged;
            _userSettings.ShufflingStatusChanged += ShufflingStatusChanged;
        }

        private void ShufflingStatusChanged()
        {
            IconCommands[1].On = _userSettings.ShuffleOn;
        }

        private void ScrobblingStatusChanged()
        {
            IconCommands[0].On = _userSettings.Scrobbling;
        }

        public List<CommandIcon> IconCommands { get; private set; }

        private void PopulateIconCommands()
        {
            IconCommands = new List<CommandIcon>
            {
                new CommandIcon(IconType.LastFmSquare, "Scrobbling", OnToggleScrobbling, _userSettings.Scrobbling, IconType.Headphones, "Not Scrobbling"),
                new CommandIcon(IconType.Random, "Shuffling", OnToggleShuffle, _userSettings.ShuffleOn, IconType.LongArrowRight, "Playing In Order"),
                new CommandIcon(IconType.VolumeOff, "Mute", OnMute),
                new CommandIcon(IconType.VolumeDown, "Volume Down", OnVolumeDown),
                new CommandIcon(IconType.VolumeUp, "Volume Up", OnVolumeUp),
                new CommandIcon(IconType.Cog, "Settings", OnSettings),
                new CommandIcon(IconType.PaintBrush, "Switch Colour", OnSwitchColour)
            };
        }

        private void OnSwitchColour(CommandIcon commandIcon)
        {
            // throw new NotImplementedException();
        }

        private void OnSettings(CommandIcon commandIcon)
        {
            _messenger.Send(new EditSettingsMessage());
        }

        private void OnVolumeUp(CommandIcon commandIcon)
        {
            // throw new NotImplementedException();
        }

        private void OnVolumeDown(CommandIcon commandIcon)
        {
            // throw new NotImplementedException();
        }

        private void OnMute(CommandIcon commandIcon)
        {
            // throw new NotImplementedException();
        }

        private void OnToggleShuffle(CommandIcon commandIcon)
        {
            _userSettings.ShuffleOn = !_userSettings.ShuffleOn;
            _userSettings.Save();
            commandIcon.On = _userSettings.ShuffleOn;
        }

        private void OnToggleScrobbling(CommandIcon commandIcon)
        {
            _userSettings.Scrobbling = !_userSettings.Scrobbling;
            _userSettings.Save();
            commandIcon.On = _userSettings.Scrobbling;
        }
    }
}