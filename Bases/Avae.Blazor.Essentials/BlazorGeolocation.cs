using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avae.Blazor.Essentials
{
    internal class BlazorGeolocation(BlazorNative.Device.IGeolocation geolocation) : IGeolocation
    {
        public bool IsListeningForeground => throw new NotImplementedException();

        public bool IsEnabled => true;

        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;

        public async Task<Location?> GetLastKnownLocationAsync()
        {
            var result = await geolocation.GetCurrentPositionAsync();
            if (result.Status == BlazorNative.Core.GeolocationStatus.Granted &&
                result.Position.HasValue)
                return new Location()
                {
                    Accuracy = result.Position.Value.Accuracy,
                    Altitude = result.Position.Value.Altitude,
                    Latitude = result.Position.Value.Latitude,
                    Longitude = result.Position.Value.Longitude,
                    //= result.Position.Value.TimestampUnixMs
                };
            return null;
        }

        public Task<Location?> GetLocationAsync(GeolocationRequest request, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StartListeningForegroundAsync(GeolocationListeningRequest request)
        {
            throw new NotImplementedException();
        }

        public void StopListeningForeground()
        {
            throw new NotImplementedException();
        }
    }
}
