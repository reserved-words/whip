﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Whip.Services.Interfaces.Singletons;

namespace Whip.Services.Singletons
{
    public class ConfigSettings : IConfigSettings
    {
        private const string configDirectoryName = "Whip";
        private const string configFileName = "config.xml";

        private bool _populated;

        public ConfigSettings()
        {
            PopulateSettings();
        }

        public string LastFmApiKey { get; set; }
        public string LastFmApiSecret { get; set; }
        public string BandsInTownApiKey { get; set; }
        public string YouTubeApiKey { get; set; }
        public List<string> FileExtensions { get; set; }
        public int TrackChangeDelay { get; set; }
        public int MinutesBeforeRefreshNews { get; set; }
        public int NumberOfSimilarArtistsToDisplay { get; set; }
        public int DaysBeforeUpdatingArtistWebInfo { get; set; }
        public int MinutesBeforeUpdatingTweets { get; set; }
        public string TwitterApiKey { get; set; }
        public string TwitterApiSecret { get; set; }
        public string TwitterApiAccessToken { get; set; }
        public string TwitterApiAccessTokenSecret { get; set; }

        private void PopulateSettings()
        {
            if (_populated)
                return;

            var config = XDocument.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), configDirectoryName, configFileName));
            var root = config.Element("config");

            TrackChangeDelay = Convert.ToInt16(root.Element("track_change_delay").Value);
            NumberOfSimilarArtistsToDisplay = Convert.ToInt16(root.Element("num_similar_artists").Value);
            MinutesBeforeRefreshNews = Convert.ToInt16(root.Element("minutes_before_update_news").Value);
            DaysBeforeUpdatingArtistWebInfo = Convert.ToInt16(root.Element("days_before_update_web_info").Value);
            MinutesBeforeUpdatingTweets = Convert.ToInt16(root.Element("minutes_before_update_tweets").Value);

            FileExtensions = new List<string>();
            foreach (var fileExtension in root.Element("file_extensions").Elements("file_extension"))
            {
                FileExtensions.Add(fileExtension.Value);
            }

            foreach (var apiSetting in root.Element("api_settings").Elements("api_setting"))
            {
                switch (apiSetting.Attribute("name").Value)
                {
                    case "Last.fm":
                        LastFmApiKey = apiSetting.Attribute("api_key").Value;
                        LastFmApiSecret = apiSetting.Attribute("secret").Value;
                        break;
                    case "BandsInTown":
                        BandsInTownApiKey = apiSetting.Attribute("api_key").Value;
                        break;
                    case "YouTube":
                        YouTubeApiKey = apiSetting.Attribute("api_key").Value;
                        break;
                    case "Twitter":
                        TwitterApiKey = apiSetting.Attribute("api_key").Value;
                        TwitterApiSecret = apiSetting.Attribute("secret").Value;
                        TwitterApiAccessToken = apiSetting.Attribute("token").Value;
                        TwitterApiAccessTokenSecret = apiSetting.Attribute("token_secret").Value;
                        break;
                    default:
                        break;
                }
            }

            _populated = true;
        }
    }
}
