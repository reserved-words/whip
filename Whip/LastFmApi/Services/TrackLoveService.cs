﻿using LastFmApi.Interfaces;
using LastFmApi.Internal;
using LastFmApi.Methods.Track;
using System.Threading.Tasks;

namespace LastFmApi
{
    public class TrackLoveService : ITrackLoveService
    {
        public async Task<bool> IsLovedAsync(UserApiClient client, Track track)
        {
            var isLovedMethod = new IsLovedMethod(client, track, client.Username);
            return await isLovedMethod.GetResultAsync();
        }

        public async Task LoveTrackAsync(UserApiClient client, Track track)
        {
            var loveTrackMethod = new LoveTrackMethod(client, track);
            await loveTrackMethod.PostAsync();
        }

        public async Task UnloveTrackAsync(UserApiClient client, Track track)
        {
            var unloveTrackMethod = new UnloveTrackMethod(client, track);
            await unloveTrackMethod.PostAsync();
        }
    }
}
