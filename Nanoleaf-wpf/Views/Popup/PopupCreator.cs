using System.Linq;
using System.Windows;

namespace Winleafs.Wpf.Views.Popup
{
    public static class PopupCreator
    {
        public static void CreatePopup(string title, string body, bool blocking = false)
        {        
            if (!IsWindowOpen())
            {
                if (blocking)
                {
                    new Popup(body, title).ShowDialog(); //ShowDialog makes it block the calling window
                }
                else
                {
                    new Popup(body, title).Show();
                }
            }
        }

        public static void CreateErrorPopup(string body)
        {
            if (!IsWindowOpen())
            {
                CreatePopup(PopupResource.Error, body, true);
            }
        }

        public static void CreateSuccessPopup(string body, bool blocking = false)
        {
            if (!IsWindowOpen())
            {
                CreatePopup(PopupResource.Success, body, true);
            }
        }

        public static bool IsWindowOpen()
        {
            return Application.Current.Windows.OfType<Popup>().Any();
        }
    }
}
