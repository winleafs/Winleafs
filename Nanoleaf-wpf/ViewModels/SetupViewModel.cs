using Nanoleaf_Models.Models;
using System.Collections.ObjectModel;

namespace Nanoleaf_wpf.ViewModels
{
    public class SetupViewModel
    {
        public string Name { get; set; }
        public ObservableCollection<Device> Devices { get; set; }

        public SetupViewModel()
        {
            Devices = new ObservableCollection<Device>();
        }
    }
}
