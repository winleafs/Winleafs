using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Layouts;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Layout
{
    /// <summary>
    /// Interaction logic for PercentageProfileWindow.xaml
    /// </summary>
    public partial class CreateEffectWindow : Window
    {
        private CustomEffect _customEffect;

        public CreateEffectWindow()
        {
            InitializeComponent();

			LayoutDisplay.EnableClick();
			LayoutDisplay.InitializeResizeTimer();
			LayoutDisplay.DrawLayout();
            LayoutDisplay.DisableColorTimer();

            if (UserSettings.Settings.ActiveDevice.CustomEffect != null)
            {
                var serialized = JsonConvert.SerializeObject(UserSettings.Settings.ActiveDevice.CustomEffect); //Deep copy the custom effect when editing
                _customEffect = JsonConvert.DeserializeObject<CustomEffect>(serialized);

				BuildFrameList();
            }
            else
            {
                _customEffect = new CustomEffect();
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (LayoutDisplay.SelectedPanelIds.Count <= 0)
            {
                return;
            }
            
            var frame = new Frame();

            foreach (var panelId in LayoutDisplay.PanelIds)
            {
                frame.PanelColors.Add(panelId, Color.Black);
            }                

            _customEffect.Frames.Add(frame);

            BuildFrameList();

            //LayoutDisplay.LockPanels(LayoutDisplay.SelectedPanelIds);

            LayoutDisplay.ClearSelectedPanels();
            
        }

		private void BuildFrameList()
		{
			FrameList.Children.Clear();

			for (var i = 0; i < _customEffect.Frames.Count; i++)
			{
				FrameList.Children.Add(new FrameUserControl(this, i + 1, _customEffect.Frames[i]));
			}
		}

		public void HighlightPanels(HashSet<int> panelIds)
        {
            LayoutDisplay.HighlightPanels(panelIds);
        }

        public void UnhighlightPanels(HashSet<int> panelIds)
        {
            LayoutDisplay.UnhighlightPanels(panelIds);
        }

        public void DeleteFrame(Frame frame)
        {
            //LayoutDisplay.UnlockPanels(step.PanelIds);

            _customEffect.Frames.Remove(frame);
            BuildFrameList();
        }

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
