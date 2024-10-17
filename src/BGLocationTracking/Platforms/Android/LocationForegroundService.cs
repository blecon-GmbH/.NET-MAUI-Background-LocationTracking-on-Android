using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using blecon.BGLocationTracking.Infrastructure.Messages;
using blecon.BGLocationTracking.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace blecon.BGLocationTracking.Platforms.Android;

[Service(Name = "de.blecon.bglocationtracking.LocationForegroundService")]
public sealed class LocationForegroundService : Service
{
    private const int SERVICE_RUNNING_NOTIFICATION_ID = 10001;
    private CancellationTokenSource? _cancellationTokenSource;

    public override IBinder? OnBind(Intent? intent)
        => null;

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(
        Intent? intent,
        [GeneratedEnum] StartCommandFlags flags,
        int startId)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var notification = new NotificationHelper().GetServiceStartedNotification();

        if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
        {
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }
        else
        {
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification, ForegroundService.TypeLocation);
        }

        Task.Run(() =>
        {
            try
            {
                var locationService = IPlatformApplication.Current?.Services.GetService<ILocationService>();
                if (locationService is null)
                {
                    throw new InvalidOperationException($"Could not resolve required service of type {nameof(ILocationService)}");
                }

                locationService.Run(_cancellationTokenSource.Token).Wait();
            }
            catch (System.OperationCanceledException)
            {
                // ignore expected exception when the task is cancelled
            }
            finally
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    WeakReferenceMessenger.Default.Send(new StopForegroundServiceRequestedMessage());
                }
            }
        }, _cancellationTokenSource.Token);

        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        if (_cancellationTokenSource is not null)
        {
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            _cancellationTokenSource.Cancel();
        }

        base.OnDestroy();
    }
}
