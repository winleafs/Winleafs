using Nanoleaf_wpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nanoleaf_wpf.ViewModels
{
    public class SetupViewModel
    {
        public ObservableCollection<Device> Devices { get; set; }

        public SetupViewModel()
        {
            Devices = new ObservableCollection<Device>();
        }
    }
}
