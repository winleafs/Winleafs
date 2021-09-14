using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Layouts;
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
        private CustomEffect _customEffect;
		private Frame _currentFrame;
		//private Color _currentColor = Colors.White;
		//private SolidColorBrush _currentBrush;
		private Dictionary<uint, SolidColorBrush> _pallete;
		private static readonly Regex _regex = new Regex("[^0-9.]"); //regex that matches disallowed text

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
				BuildFrameList();
			}
            else
            {
                _customEffect = new CustomEffect();
				AddNewFrame();
			}
			
			BuildPallete();

			FrameSelected(_customEffect.Frames[0]);
		}

		//private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		//{
		//	if (e.NewValue != null)
		//	{
		//		_currentColor = e.NewValue.Value;
				
		//		//Try to find a brush for the selecyted color
		//		var argb = MediaColorConverter.ToInt(_currentColor);

		//		if (!_pallete.TryGetValue(argb, out var brush))
		//		{
		//			brush = new SolidColorBrush(_currentColor);
		//		}
		//		_currentBrush = brush;
		//	}
		//}

		public void FrameSelected(Frame frame)
		{
			_currentFrame = frame;

			var panelToBrushMap = new Dictionary<int, SolidColorBrush>();

			foreach (var panel in _currentFrame.PanelColors)
			{
				panelToBrushMap.Add(panel.Key, _pallete[panel.Value]);
			}
			LayoutDisplay.PanelToBrushMap = panelToBrushMap;
			LayoutDisplay.UpdateColors();
		}

		private void LayoutDisplay_PanelClicked(object sender, System.EventArgs e)
		{
			var drawablePanel = sender as DrawablePanel;

			if (drawablePanel != null)
			{
				var color = ColorPicker.SelectedColor ?? Colors.Black;

				// Try to find a brush for the selected color
				var rgb = MediaColorConverter.ToRgb(color);

				if (!_pallete.TryGetValue(rgb, out var brush))
				{
					{
						brush = new SolidColorBrush(color);
					}
					AddColorToPallete(color, brush);
				}

				// Color the panel and update the color for the panel on the Frame
				// Display black (unlit panel) as a gray
				drawablePanel.Polygon.Fill = rgb == 0 ? Brushes.LightSlateGray : brush;
				_currentFrame.PanelColors[drawablePanel.PanelId] = rgb;
			}
		}

		private void AddFrame_Click(object sender, RoutedEventArgs e)
		{
			var frame = AddNewFrame();

			FrameSelected(frame);
		}

		private void RemoveFrame_Click(object sender, RoutedEventArgs e)
		{
			if (_customEffect.Frames.Count > 1)
			{
				_customEffect.Frames.RemoveAt(_customEffect.Frames.Count - 1);
			}
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

			//Add the frame to the effect and the displayed list
            _customEffect.Frames.Add(newFrame);
			FrameList.Children.Add(new FrameUserControl(this, _customEffect.Frames.Count, newFrame));

			return newFrame;
        }

		private void BuildFrameList()
		{
			FrameList.Children.Clear();


			for (var i = 0; i < _customEffect.Frames.Count; i++)
			{
				FrameList.Children.Add(new FrameUserControl(this, i + 1, _customEffect.Frames[i]));
			}

			_currentFrame = _customEffect.Frames.Last();
		}

		private void BuildPallete()
		{
			var colorsUsed = _customEffect.Frames.SelectMany(f => f.PanelColors.Values).Distinct();

			ColorPicker.StandardColors.Clear();
			_pallete = new Dictionary<uint, SolidColorBrush>();
			
			foreach (var rgb in colorsUsed)
			{
				var mediaColor = MediaColorConverter.FromRgb(rgb);
				ColorPicker.StandardColors.Add(new Xceed.Wpf.Toolkit.ColorItem(mediaColor, string.Empty));
				_pallete.Add(rgb, new SolidColorBrush(mediaColor));
			}
		}

		private void AddColorToPallete(Color color, SolidColorBrush brush)
		{
			ColorPicker.StandardColors.Add(new Xceed.Wpf.Toolkit.ColorItem(color, string.Empty));
			_pallete.Add(MediaColorConverter.ToRgb(color), brush);
		}

        //public void DeleteFrame(Frame frame)
        //{
        //    //LayoutDisplay.UnlockPanels(step.PanelIds);

        //    _customEffect.Frames.Remove(frame);
        //    BuildFrameList();
        //}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_customEffect.Frames.Count > 0)
            {
                UserSettings.Settings.ActiveDevice.CustomEffect = _customEffect;
                UserSettings.Settings.SaveSettings();

                Close();
            }
            else
            {
                PopupCreator.Error(Layout.Resources.AtLeast1Step);
            }
        }

		private void Play_Click(object sender, RoutedEventArgs e)
		{
			

			if (float.TryParse(TransitionTextBox.Text, out var transitionSecs) && transitionSecs > 0)
			{
				//TODO Play the animation
			}
		}

		private void TransitionTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = _regex.IsMatch(e.Text);
		}
		
	
	}
}
