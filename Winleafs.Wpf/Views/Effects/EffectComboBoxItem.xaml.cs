using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Winleafs.Models.Enums;
using Winleafs.Wpf.ViewModels;

namespace Winleafs.Wpf.Views.Effects
{
	/// <summary>
	/// Interaction logic for EffectComboBoxItem.xaml
	/// </summary>
	public partial class EffectComboBoxItem : UserControl
	{
		public EffectComboBoxItem()
		{
			InitializeComponent();
		}

		private void DrawColoredBorder(EffectComboBoxItemViewModel dataContext)
		{
			//Remove duplicate colors, palettes from Nanoleaf can contain the same color multiple times
			var colors = dataContext.Colors.Distinct();

			if (!colors.Any())
			{
				return;
			}

			var borderParts = new int[colors.Count()];

			//Divide the border into equal sized parts
			for (var i = 0; i < colors.Count(); i++)
			{
				borderParts[i] = dataContext.Width / colors.Count();
			}

			//Divide up the remainder
			for (var i = 0; i < dataContext.Width % colors.Count(); i++)
			{
				borderParts[i] += 1;
			}

			//Create the borders
			var marginLeft = 0;
			var marginRight = dataContext.Width;

			for (var i = 0; i < colors.Count(); i++)
			{
				ContentGrid.Children.Add(new Border
				{
					BorderBrush = new SolidColorBrush(colors.ElementAt(i)),
					BorderThickness = new Thickness(0, 8, 0, 0),
					Margin = new Thickness(marginLeft, 0, marginRight - borderParts[i], 0)
				});

				marginLeft += borderParts[i];
				marginRight -= borderParts[i];
			}
		}

		private void SetIcon(EffectComboBoxItemViewModel dataContext)
		{
			//Display icons when the effect is either color or rhythm
			switch (dataContext.EffectType)
			{
				case EffectType.Color:
					ColorIcon.Visibility = Visibility.Visible;
					break;
				case EffectType.Rhythm:
					RhythmIcon.Visibility = Visibility.Visible;
					break;
			}
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DataContext is EffectComboBoxItemViewModel)
			{
				//Cast is safe since we always now this is the type of the data context
				var dataContext = (EffectComboBoxItemViewModel)DataContext;

				DrawColoredBorder(dataContext);
				SetIcon(dataContext);
			}
		}
	}
}
