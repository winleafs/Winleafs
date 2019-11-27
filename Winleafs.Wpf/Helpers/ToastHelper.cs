using System;
using System.Linq;
using System.Windows;
using NLog;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Winleafs.Wpf.Enums;

namespace Winleafs.Wpf.Helpers
{
    public static class ToastHelper
    {
        public static void ShowNotification(string body, ToastLogLevel customLogLevel,
            Corner corner = Corner.BottomRight, double secondsViewable = 3)
        {
            var notificationWindow = Application.Current?.Windows?.Cast<Window>()
                .FirstOrDefault(window => window != null && window.IsActive);

            // App either hasn't started or doesn't have a main window
            if (notificationWindow == null)
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"Failed to log message: \"{body}\"");
                return;
            }

#pragma warning disable CA2000 // Dispose objects before losing scope
            var notifier = new Notifier(configuration =>
            {
                configuration.PositionProvider = new WindowPositionProvider(
                    parentWindow: notificationWindow,
                    corner: corner,
                    offsetX: 10,
                    offsetY: 10);

                configuration.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(secondsViewable),
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
