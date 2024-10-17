using Android.App;
using Android.Content;

namespace blecon.BGLocationTracking.Platforms.Android;

public sealed class NotificationHelper
{
    private const string FOREGROUND_CHANNEL_ID = "BGLocationTrackingChannel";
    private static readonly Context Context = global::Android.App.Application.Context;

    public Notification GetServiceStartedNotification()
    {
        Intent intent = new(Context, typeof(MainActivity));
        intent.AddFlags(ActivityFlags.SingleTop)
            .PutExtra("Title", "Message");

        var pendingIntent = PendingIntent.GetActivity(
            Context,
            0,
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var notificationBuilder = new Notification.Builder(Context, FOREGROUND_CHANNEL_ID)
            .SetContentTitle("blecon BGLocationTracking")
            .SetContentText("Ihre Position wird im Hintergrund erfasst.")
            .SetSmallIcon(_Microsoft.Android.Resource.Designer.ResourceConstant.Drawable.location_tracking_on)
            .SetContentIntent(pendingIntent)
            .SetOngoing(true);

        if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
        {
            var notificationChannel = new NotificationChannel(
                FOREGROUND_CHANNEL_ID,
                "BGLocationTracking",
                NotificationImportance.High);
            notificationChannel.SetShowBadge(true);

            var notificationManager = Context.GetSystemService(Context.NotificationService) as NotificationManager;
            if (notificationManager is not null)
            {
                notificationBuilder.SetChannelId(FOREGROUND_CHANNEL_ID);
                notificationManager.CreateNotificationChannel(notificationChannel);
            }
        }

        return notificationBuilder.Build();
    }
}
