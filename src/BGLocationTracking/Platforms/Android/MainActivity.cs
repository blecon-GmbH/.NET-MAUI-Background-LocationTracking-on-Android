using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using blecon.BGLocationTracking.Infrastructure.Messages;
using blecon.BGLocationTracking.Platforms.Android;
using CommunityToolkit.Mvvm.Messaging;

namespace blecon.BGLocationTracking;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const int REQUEST_CODE = 5649;
    private Intent? _serviceIntent;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        _serviceIntent = new Intent(this, typeof(LocationForegroundService));
        SetServiceMethods();

        if (Build.VERSION.SdkInt >= BuildVersionCodes.M && !Android.Provider.Settings.CanDrawOverlays(this))
        {
            var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission);
            intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }
    }

    private void SetServiceMethods()
    {
        WeakReferenceMessenger.Default.Register<StartForegroundServiceRequestedMessage>(this, (_, _) =>
        {
            if (!IsServiceRunning(typeof(LocationForegroundService)))
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    StartForegroundService(_serviceIntent);
                }
                else
                {
                    StartService(_serviceIntent);
                }
            }
        });

        WeakReferenceMessenger.Default.Register<StopForegroundServiceRequestedMessage>(this, (_, _) =>
        {
            if (IsServiceRunning(typeof(LocationForegroundService)))
            {
                StopService(_serviceIntent);
            }
        });
    }

    private bool IsServiceRunning(Type serviceType)
    {
        if (GetSystemService(ActivityService) is not ActivityManager activityManager)
        {
            return false;
        }

        // GetRunningServices is marked as deprecated but still works. May break in the future. Be aware!
        foreach (var service in activityManager.GetRunningServices(int.MaxValue)!)
        {
            if (service.Service!.ClassName == Java.Lang.Class.FromType(serviceType).CanonicalName)
            {
                return true;
            }
        }

        return false;
    }
}
