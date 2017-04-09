﻿using Whip.Common.Interfaces;
using WMPLib;

namespace WmpPlayer
{
    public class Player : IPlayer
    {
        private readonly WindowsMediaPlayer _player = new WindowsMediaPlayer();

        public Player()
        {
            _player.settings.volume = 50;
        }

        public void Pause()
        {
            _player.controls.pause();
        }

        public void Play(string filepath)
        {
            _player.URL = filepath;
            _player.controls.currentPosition = 0;
            _player.controls.play();
        }

        public void Resume()
        {
            _player.controls.play();
        }
    }
}
