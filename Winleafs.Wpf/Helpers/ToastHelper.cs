using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Winleafs.Wpf.Enums;

namespace Winleafs.Wpf.Helpers
{
    public static class ToastHelper
    {
        public static void ShowNotification(string body, ToastLogLevel customLogLevel)
        {
            // App either hasn't started or doesn't have a main window
            if (Application.Current?.MainWindow == null)
            {
                return;
            }

#pragma warning disable CA2000 // Dispose objects before losing scope
            var notifier = new Notifier(configuration =>
            {
                configuration.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                configuration.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(1));

                configuration.Dispatcher = Application.Current.Dispatcher;
            });
#pragma warning restore CA2000 // Dispose objects before losing scope

            switch (customLogLevel)
            {
                case ToastLogLevel.Information:
                    notifier.ShowInformation(body);
                    break;
                case ToastLogLevel.Success:
                    notifier.ShowSuccess(body);
                    break;
                case ToastLogLevel.Warning:
                    notifier.ShowWarning(body);
                    break;
                case ToastLogLevel.Error:
                    notifier.ShowError(body);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(customLogLevel), customLogLevel, null);
            }
        }
    }
}
