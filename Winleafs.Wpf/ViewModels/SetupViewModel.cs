using System.Collections.ObjectModel;

using Winleafs.Models.Models;

namespace Winleafs.Wpf.ViewModels
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
