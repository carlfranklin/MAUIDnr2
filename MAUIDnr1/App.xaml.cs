namespace MAUIDnr1;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
#if WINDOWS
        // from https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/windows?view=net-maui-7.0
		window.Width = 1280;
		window.Height= 720;
#endif
        return window;
    }
}
