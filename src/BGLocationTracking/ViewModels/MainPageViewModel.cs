using blecon.BGLocationTracking.Infrastructure.Messages;
using blecon.BGLocationTracking.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace blecon.BGLocationTracking.ViewModels;

public sealed partial class MainPageViewModel : BaseViewModel
{
    private const string START = "Start";
    private const string STOP = "Stop";

    private readonly IPermissionService _permissionService;

    [ObservableProperty]
    private string _startStopButtonText = START;

    private bool _isLocationTrackingEnabled;

    public MainPageViewModel(IPermissionService permissionService)
    {
        _permissionService = permissionService;

        WeakReferenceMessenger.Default.Register<LocationMessage>(this, HandleLocationMessage);
    }

    public ObservableCollection<string> LocationChanges { get; } = [];

    [RelayCommand]
    public async Task StartStop()
    {
        if (await _permissionService.CheckAndRequestLocationAlwaysPermission() != PermissionStatus.Granted)
        {
            return;
        }

        if (_isLocationTrackingEnabled)
        {
            WeakReferenceMessenger.Default.Send(new StopForegroundServiceRequestedMessage());
            StartStopButtonText = START;
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new StartForegroundServiceRequestedMessage());
            StartStopButtonText = STOP;
        }

        _isLocationTrackingEnabled = !_isLocationTrackingEnabled;
    }

    private void HandleLocationMessage(object recipient, LocationMessage locationMessage)
    {
        var locationDetails = $"[{DateTime.Now:HH:mm:ss}] Latitude: {locationMessage.Latitude} - Longitude: {locationMessage.Longitude}";

        LocationChanges.Add(locationDetails);
        Debug.WriteLine(locationDetails);
    }
}
