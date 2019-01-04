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

        public void LoadDummy()
        {
            Devices = new ObservableCollection<Device>();
            Devices.Add(new Device() { Name = "test1", IpAddress = "111.1.11.1" });
            Devices.Add(new Device() { Name = "test2", IpAddress = "123.222.222.222" });
        }
    }
}
