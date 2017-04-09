﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whip.Services.Interfaces;

namespace Whip.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        public bool EssentialSettingsSet => !string.IsNullOrEmpty(MusicDirectory);

        public string MusicDirectory
        {
            get { return Properties.Settings.Default.MusicDirectory; }
            set { Properties.Settings.Default.MusicDirectory = value; }
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
            set { Properties.Settings.Default.LastFmUsername = value; }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
