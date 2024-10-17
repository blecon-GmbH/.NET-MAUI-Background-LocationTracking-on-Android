using blecon.BGLocationTracking.ViewModels;

namespace blecon.BGLocationTracking.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        BindingContext = mainPageViewModel;
    }
}
