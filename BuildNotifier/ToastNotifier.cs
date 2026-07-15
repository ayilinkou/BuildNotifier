using Microsoft.Toolkit.Uwp.Notifications;

public static class ToastNotifier
{
    public static void Show(string title, string message, bool success)
    {
        new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .Show();
    }
}