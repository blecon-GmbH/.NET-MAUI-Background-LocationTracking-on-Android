using blecon.BGLocationTracking.Infrastructure.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace blecon.BGLocationTracking.Services;

public sealed class LocationService : ILocationService
{
    private readonly GeolocationRequest _geolocationRequest = new(GeolocationAccuracy.Best);

    public async Task Run(CancellationToken cancellationToken)
        => await Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000);

                var location = await Geolocation.GetLocationAsync(_geolocationRequest, cancellationToken);
                if (location is null)
                {
                    return;
                }

                WeakReferenceMessenger.Default.Send(new LocationMessage(location.Latitude, location.Longitude));
            }
        }, cancellationToken);
}
