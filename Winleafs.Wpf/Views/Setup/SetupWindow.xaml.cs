using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Winleafs.Wpf.ViewModels;
using NLog;
using Winleafs.Models;
using Winleafs.Nanoleaf;
using Winleafs.Nanoleaf.Detection;
using Winleafs.Orchestration;
using Winleafs.Wpf.Views.MainWindows;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Setup
{
	/// <summary>
	/// Interaction logic for Setup.xaml
	/// </summary>
	public partial class SetupWindow : Window
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly SetupViewModel _setupViewModel;
		private readonly List<Device> _discoveredDevices;
		private readonly MainWindow _parent;
		private readonly DeviceDiscoveryService _discoveryService;
		private NanoleafClient _nanoleafClient;
		private Device _selectedDevice;

		public SetupWindow(MainWindow parent = null)
		{
			_parent = parent;

			InitializeComponent();

			_setupViewModel = new SetupViewModel();
			_discoveredDevices = new List<Device>();

			_discoveryService = new DeviceDiscoveryService();
			_discoveryService.DeviceDiscovered += DeviceDiscovered;
			_discoveryService.Start();
		}

		public void Finish_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_setupViewModel.Name))
			{
				PopupCreator.Error(Setup.Resources.NameCannotBeEmpty);
				return;
			}
			else if (UserSettings.HasSettings() && UserSettings.Settings.Devices.Any(d => d.Name.ToLower().Equals(_setupViewModel.Name)))
			{
				PopupCreator.Error(Setup.Resources.NameAlreadyExists);
				return;
			}

			_selectedDevice.Name = _setupViewModel.Name;

			UserSettings.Settings.AddDevice(_selectedDevice);

			_logger.Info($"Successfully added device {_selectedDevice.Name}");
			_discoveryService.Stop();

			if (_parent != null)
			{
				OrchestratorCollection.AddOrchestratorForDevice(_selectedDevice);
				_parent.DeviceAdded(_selectedDevice);
				_parent.SelectedDevice = _selectedDevice.Name;
			}
			else
			{
				UserSettings.Settings.SetActiveDevice(_selectedDevice.Name); //The first added device must always be active
				App.NormalStartup(null);
			}

			Close();
		}

		public void Pair_Click(object sender, RoutedEventArgs e)
		{
			Task.Run(Pair);
		}

		private async Task Pair()
		{
			try
			{
				var authToken = await _nanoleafClient.AuthorizationEndpoint.GetAuthTokenAsync();

				await _nanoleafClient.IdentifyEndpoint.Identify();

				var effects = await _nanoleafClient.EffectsEndpoint.GetEffectsListAsync();

				Dispatcher.Invoke(() =>
				{
					_selectedDevice.AuthToken = authToken;
					_selectedDevice.LoadEffectsFromNameList(effects);

					AuthorizeDevice.Visibility = Visibility.Hidden;
					NameDevice.Visibility = Visibility.Visible;
				});
			}
			catch
			{
				PopupCreator.Error(Setup.Resources.UnknownError);
			}
		}



		public void Cancel_Click(object sender, RoutedEventArgs e)
		{
			_discoveryService.Stop();
			if (!UserSettings.HasSettings() || UserSettings.Settings.Devices.Count == 0)
			{ //If the user has no settings or devices, cancel click is a shutdown
				Application.Current.Shutdown();
			}
			else
			{
				Close();
			}
		}

		public void Continue_Click(object sender, RoutedEventArgs e)
		{
			if (DiscoverDevice.Devices.SelectedItem == null)
			{
				return;
			}

			AuthorizeDevice.Visibility = Visibility.Visible;
			DiscoverDevice.Visibility = Visibility.Hidden;

			_selectedDevice = (Device)DiscoverDevice.Devices.SelectedItem;
			_logger.Info($"Selected following device: {_selectedDevice.IPAddress}:{_selectedDevice.Port}");

			_nanoleafClient = new NanoleafClient(_selectedDevice.IPAddress, _selectedDevice.Port);
		}

		private void DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
		{
			_discoveredDevices.Add(e.Device);
			BuildDeviceList();
		}

		private void BuildDeviceList()
		{
			Dispatcher.Invoke(() =>
			{
				_setupViewModel.Devices.Clear();

				foreach (var device in _discoveredDevices)
				{
					_setupViewModel.Devices.Add(device);
				}
			});
		}

		private void AuthorizeDevice_Loaded(object sender, RoutedEventArgs e)
		{
			AuthorizeDevice.ParentWindow = this;
			AuthorizeDevice.DataContext = _setupViewModel;
		}

		private void DiscoverDevice_Loaded(object sender, RoutedEventArgs e)
		{
			DiscoverDevice.ParentWindow = this;
			DiscoverDevice.DataContext = _setupViewModel;
		}

		private void NameDevice_Loaded(object sender, RoutedEventArgs e)
		{
			NameDevice.ParentWindow = this;
			NameDevice.DataContext = _setupViewModel;
		}
	}
}
