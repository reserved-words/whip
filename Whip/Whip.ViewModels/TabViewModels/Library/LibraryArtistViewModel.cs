﻿using GalaSoft.MvvmLight;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Whip.Common.Model;
using Whip.Services.Interfaces;
using Whip.Services.Interfaces.Singletons;

namespace Whip.ViewModels.TabViewModels.Library
{
    public class LibraryArtistViewModel : ViewModelBase
    {
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IArtistInfoService _webArtistInfoService;
        private readonly IConfigSettings _configSettings;

        public LibraryArtistViewModel(IArtistInfoService webArtistInfoService, IImageProcessingService imageProcessingService, IConfigSettings configSettings)
        {
            _imageProcessingService = imageProcessingService;
            _webArtistInfoService = webArtistInfoService;
            _configSettings = configSettings;
        }

        private Artist _artist;
        private BitmapImage _image;
        private bool _loadingArtistImage;

        public Artist Artist
        {
            get { return _artist; }
            set
            {
                if (value == null || value == _artist)
                    return;

                Set(ref _artist, value);
                Task.Run(PopulateLastFmInfo);
            }
        }

        public bool LoadingArtistImage
        {
            get { return _loadingArtistImage; }
            set { Set(ref _loadingArtistImage, value); }
        }

        public BitmapImage Image
        {
            get { return _image; }
            private set { Set(ref _image, value); }
        }

        public string Wiki => Artist?.WebInfo?.Wiki;

        private async Task PopulateLastFmInfo()
        {
            if (Artist == null)
                return;

            LoadingArtistImage = true;

            await _webArtistInfoService.PopulateArtistInfo(Artist, _configSettings.NumberOfSimilarArtistsToDisplay);

            Image = await _imageProcessingService.GetImageFromUrl(Artist?.WebInfo.ExtraLargeImageUrl);
            LoadingArtistImage = false;
            RaisePropertyChanged(nameof(Wiki));
        }
    }
}
