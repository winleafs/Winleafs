using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Layouts;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Layout
{

    /// <summary>
    /// Interaction logic for PercentageProfileWindow.xaml
    /// </summary>
    public partial class CreateEffectWindow : Window
    {
		public class FrameListItem
		{
			public FrameListItem(Frame frame, int ordinal)
			{
				Frame = frame;
				Name = Name = $"{Layout.Resources.Frame} {ordinal}";
			}

			public string Name { get; set; }
			public Frame Frame { get; set; }
		}

		private IList<FrameListItem> _frameListItems;
		private CustomEffect _customEffect;
		private Frame _currentFrame;
		//private Color _currentColor = Colors.White;
		//private SolidColorBrush _currentBrush;
		private Dictionary<uint, SolidColorBrush> _rgbToBrushMap;
		private static readonly Regex _numericRegex = new Regex("[^0-9.]"); 

		public CreateEffectWindow()
        {
            InitializeComponent();

			LayoutDisplay.EnableClick();
			LayoutDisplay.InitializeResizeTimer();
			LayoutDisplay.DrawLayout();
            LayoutDisplay.DisableColorTimer();
			LayoutDisplay.MultiSelectEnabled = false;
			LayoutDisplay.PanelClicked += LayoutDisplay_PanelClicked;
			ColorPicker.SelectedColor = Colors.White;

            if (UserSettings.Settings.ActiveDevice.CustomEffect != null)
            {
                var serialized = JsonConvert.SerializeObject(UserSettings.Settings.ActiveDevice.CustomEffect); //Deep copy the custom effect when editing
                _customEffect = JsonConvert.DeserializeObject<CustomEffect>(serialized);
			}
            else
            {
                _customEffect = new CustomEffect();
			}

			BuildRgbToBrushMap();

			BuildFrameList();

			

			//FrameSelected(_customEffect.Frames[0]);
		}
				
		//public void FrameSelected(Frame frame)
		//{
		//	_currentFrame = frame;

		//	var panelToBrushMap = new Dictionary<int, SolidColorBrush>();

		//	foreach (var panel in _currentFrame.PanelColors)
		//	{
		//		panelToBrushMap.Add(panel.Key, _rgbToBrushMap[panel.Value]);
		//	}
		//	LayoutDisplay.PanelToBrushMap = panelToBrushMap;
		//	LayoutDisplay.UpdateColors();
		//}

		private void LayoutDisplay_PanelClicked(object sender, System.EventArgs e)
		{
			var drawablePanel = sender as DrawablePanel;

			if (drawablePanel != null)
			{
				var color = ColorPicker.SelectedColor ?? Colors.Black;

				// Try to find a brush for the selected color
				var rgb = ColorFormatConverter.ToRgb(color);

				if (!_rgbToBrushMap.TryGetValue(rgb, out var brush))
				{
					AddToRgbToBrushMap(color);
				}

				// Color the panel and update the color for the panel on the Frame
				drawablePanel.Polygon.Fill = _rgbToBrushMap[rgb];
				_currentFrame.PanelColors[drawablePanel.PanelId] = rgb;
			}
		}

		private void AddFrame_Click(object sender, RoutedEventArgs e)
		{
			AddNewFrame();
		}

		private void RemoveFrame_Click(object sender, RoutedEventArgs e)
		{
			if (FrameListBox.Items.Count < 2)
			{
				return;
			}

			var lastFrameListItem = _frameListItems.Last(); 

			// If the last item is selected, select the one before it 
			if (FrameListBox.SelectedIndex == FrameListBox.Items.Count - 1)
			{
				FrameListBox.SelectedIndex--;
			}

			_frameListItems.Remove(lastFrameListItem);
			_customEffect.Frames.Remove(lastFrameListItem.Frame);
		}

		private void FrameListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var selectedFrame = (FrameListBox.SelectedItem as FrameListItem)?.Frame;
			if (selectedFrame == null)
			{
				return;
			}

			_currentFrame = selectedFrame; 
			var panelToBrushMap = new Dictionary<int, SolidColorBrush>();

			foreach (var panel in _currentFrame.PanelColors)
			{
				panelToBrushMap.Add(panel.Key, _rgbToBrushMap[panel.Value]);
			}
			LayoutDisplay.PanelToBrushMap = panelToBrushMap;
			LayoutDisplay.UpdateColors();
		}

		private Frame AddNewFrame()
		{ 
			// Copy the previous frame's panel colors where we can
			var prevFrame = _customEffect.Frames.LastOrDefault();
            var newFrame = new Frame();

            foreach (var panelId in LayoutDisplay.PanelIds)
            {
				uint color = 0;
				if (prevFrame != null && prevFrame.PanelColors.TryGetValue(panelId, out var prevColor))
				{
					color = prevColor;
				}

				newFrame.PanelColors.Add(panelId, color);
            }                

			// Add the frame to the effect and the displayed list and select it
            _customEffect.Frames.Add(newFrame);
			var newFrameListItem = new FrameListItem(newFrame, _frameListItems.Count + 1);
			_frameListItems.Add(newFrameListItem);
			FrameListBox.SelectedItem = newFrameListItem;

			return newFrame;
        }

		private void BuildFrameList()
		{
			_frameListItems = new ObservableCollection<FrameListItem>();

			for (var i = 0; i < _customEffect.Frames.Count; i++)
			{
				_frameListItems.Add(new FrameListItem(_customEffect.Frames[i], i));
			}

			FrameListBox.ItemsSource = _frameListItems;

			if (_customEffect.Frames.Count == 0)
			{
				AddNewFrame();
			}

			FrameListBox.SelectedItem = _frameListItems.First();
		}

		private void BuildRgbToBrushMap()
		{
			var colorsUsed = _customEffect.Frames.SelectMany(f => f.PanelColors.Values).Append(0U).Distinct();

			ColorPicker.StandardColors.Clear();
			_rgbToBrushMap = new Dictionary<uint, SolidColorBrush>();
			
			foreach (var rgb in colorsUsed)
			{
				AddToRgbToBrushMap(ColorFormatConverter.ToMediaColor(rgb));
			}
		}

		private void AddToRgbToBrushMap(Color color)
		{
			ColorPicker.StandardColors.Add(new Xceed.Wpf.Toolkit.ColorItem(color, string.Empty));
			_rgbToBrushMap.Add(ColorFormatConverter.ToRgb(color), color == Colors.Black ? Brushes.LightSlateGray : new SolidColorBrush(color));
		}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
			//TODO Check for losing changes
            Close();
        }

        private void SaveProgress_Click(object sender, RoutedEventArgs e)
        {
                UserSettings.Settings.ActiveDevice.CustomEffect = _customEffect;
                UserSettings.Settings.SaveSettings();
        }

		private async void PlayOnDevice_Click(object sender, RoutedEventArgs e)
		{
			if (float.TryParse(TransitionTextBox.Text, out var transitionSecs) && transitionSecs > 0)
			{
				PopupCreator.Error(Layout.Resources.ValidTransistionTime);
				return;
			}

			_customEffect.IsLoop = LoopCheckBox.IsChecked ?? false;
			var customEffectCommandBuilder = new CustomEffectCommandBuilder(_customEffect);
			var customEffectCommand = customEffectCommandBuilder.BuildDisplayCommand(transitionSecs);

			var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);

			//TODO async so as not hang UI thread, but consider ContinueWith to handle errors
			//which are lost by the use of async void
			await orchestrator.ExecuteCustomEffectCommand(customEffectCommand);
		}

		private async void SaveToDevice_Click(object sender, RoutedEventArgs e)
		{
			//Always save progress as well
			UserSettings.Settings.ActiveDevice.CustomEffect = _customEffect;
			UserSettings.Settings.SaveSettings();

			if (float.TryParse(TransitionTextBox.Text, out var transitionSecs) && transitionSecs > 0)
			{
				PopupCreator.Error(Layout.Resources.ValidTransistionTime);
				return;
			}

			if (NameTextBox.Text.Trim().Length < 1)
			{
				PopupCreator.Error(Layout.Resources.ValidName);
				return;
			}
			
			_customEffect.IsLoop = LoopCheckBox.IsChecked ?? false;
			var customEffectCommandBuilder = new CustomEffectCommandBuilder(_customEffect);
			var customEffectCommand = customEffectCommandBuilder.BuildAddCommand(transitionSecs, NameTextBox.Text.Trim());

			var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);

			//TODO async so as not hang UI thread, but consider ContinueWith to handle errors
			//which are lost by the use of async void
			await orchestrator.ExecuteCustomEffectCommand(customEffectCommand);
		}

		private void TransitionTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = _numericRegex.IsMatch(e.Text);
		}
	}
}
