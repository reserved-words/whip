﻿
namespace Whip.Common.Validation
{
    public static class TrackValidation
    {
        public const string TrackTagRegexPattern = @"^[0-9a-zA-Z\-\s]*$";
        public const string TrackTagCharactersErrorMessage = "Tags should only contain alphanumeric characters, hyphens and spaces";

        public const byte MaxLengthTrackTitle = 255;
        public const byte MaxLengthAlbumTitle = 255;
        public const byte MaxLengthArtistName = 255;
        public const byte MaxLengthGrouping = 30;
        public const byte MaxLengthGenre = 30;
        public const byte MaxLengthCountry = 30;
        public const byte MaxLengthState = 30;
        public const byte MaxLengthCity = 30;
        public const int MaxLengthLyrics = 4000;
        public const int MinTrackCount = 1;
        public const int MaxTrackCount = 999;
        public const int MinDiscCount = 1;
        public const int MaxDiscCount = 99;
        public const int MaxLengthTag = 30;
    }
}
