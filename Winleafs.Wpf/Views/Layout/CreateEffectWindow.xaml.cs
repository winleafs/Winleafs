using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Layouts;
using Winleafs.Wpf.Api.Layouts;
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
		private Color _currentColor = Colors.White;
		private SolidColorBrush _currentBrush;

		public CreateEffectWindow()
        {
            InitializeComponent();

			LayoutDisplay.EnableClick();
			LayoutDisplay.InitializeResizeTimer();
			LayoutDisplay.DrawLayout();
            LayoutDisplay.DisableColorTimer();
			LayoutDisplay.PanelClicked += LayoutDisplay_PanelClicked;
			ColorPicker.SelectedColor = _currentColor;

            if (UserSettings.Settings.ActiveDevice.CustomEffect != null)
            {
                var serialized = JsonConvert.SerializeObject(UserSettings.Settings.ActiveDevice.CustomEffect); //Deep copy the custom effect when editing
                _customEffect = JsonConvert.DeserializeObject<CustomEffect>(serialized);

				BuildFrameList();
				BuildPallete();
            }
            else
            {
                _customEffect = new CustomEffect();
            }
        }

		private void LayoutDisplay_PanelClicked(object sender, System.EventArgs e)
		{
			var drawablePanel = sender as DrawablePanel;

			if (drawablePanel != null)
			{
				drawablePanel.Polygon.Fill = _currentBrush;

				//Convert from System.Windows.Media to System.Drawing.Color and update the Frame
				_currentFrame.PanelColors[drawablePanel.PanelId] = 
					System.Drawing.Color.FromArgb(_currentColor.A, _currentColor.R, _currentColor.G, _currentColor.B);
			}
		}

		public void FrameSelected(Frame frame)
		{
			//TODO Consider whether the pallette to be added will have a collection of Brushes that will
			//enable building a separate dictiobnary of panelId->Brush
			_currentFrame = frame;
			//LayoutDisplay.UpdateColors(_currentFrame.PanelColors);
		}

		private void Plus_Click(object sender, RoutedEventArgs e)
        {
			//Copy the previous frame's panel colors where we can
			var prevFrame = _customEffect.Frames.LastOrDefault();
            var newFrame = new Frame();

            foreach (var panelId in LayoutDisplay.PanelIds)
            {
				var color = System.Drawing.Color.Black;
				if (prevFrame != null && prevFrame.PanelColors.TryGetValue(panelId, out var prevColor))
				{
					color = prevColor;
				}

				newFrame.PanelColors.Add(panelId, color);
            }                

			//Add the frame to the effect and the displayed list
            _customEffect.Frames.Add(newFrame);
			FrameList.Children.Add(new FrameUserControl(this, _customEffect.Frames.Count, newFrame));
			//BuildFrameList();

			//LayoutDisplay.ClearSelectedPanels();
            
        }

		private void BuildFrameList()
		{
			FrameList.Children.Clear();

			for (var i = 0; i < _customEffect.Frames.Count; i++)
			{
				FrameList.Children.Add(new FrameUserControl(this, i + 1, _customEffect.Frames[i]));
			}
		}

		private void BuildPallete()
		{
			var colorsUsed = _customEffect.Frames.SelectMany(f => f.PanelColors.Values).OrderBy(c => c.GetHue()).Distinct();


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
    }
}
