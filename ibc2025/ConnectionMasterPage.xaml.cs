namespace ibc2025;

public partial class ConnectionMasterPage : Page
{
    public ConnectionMasterPage()
    {
        InitializeComponent();
        PageBackground.Background = App.DailyBackground;
        MasterServer.MirrorDiscover();
    }
}