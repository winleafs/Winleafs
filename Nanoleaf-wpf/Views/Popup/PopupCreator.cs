using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winleafs.Wpf.Views.Popup
{
    public static class PopupCreator
    {
        public static void CreatePopup(string title, string body)
        {
            new Popup(body, title).Show();
        }

        public static void CreateErrorPopup(string body)
        {
            CreatePopup(PopupResource.Error, body);
        }

        public static void CreateSuccessPopup(string body)
        {
            CreatePopup(PopupResource.Success, body);
        }
    }
}
