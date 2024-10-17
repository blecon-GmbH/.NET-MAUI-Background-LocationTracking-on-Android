namespace blecon.BGLocationTracking.Services;

public sealed class PermissionService : IPermissionService
{
    public async Task<PermissionStatus> CheckAndRequestLocationAlwaysPermission(bool showRationale = false)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        if (status == PermissionStatus.Granted)
        {
            return status;
        }

        await Application.Current.MainPage.DisplayAlert(
            "Standortberechtigung",
            "Diese App benötigt während der Laufzeit dauerhaften Zugriff auf Ihren Standort.",
            "OK");

        return await Permissions.RequestAsync<Permissions.LocationAlways>();
    }
}
