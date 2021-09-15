using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winleafs.Models.Models.Effects;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Wpf.Api.Effects
{
	/// <summary>
	/// Used to build a CustomEffectCommand to send to the API
	/// </summary>
	public class CustomEffectCommandBuilder
	{

		public CustomEffectCommand Build(CustomEffect customEffect, string Name)
		{
			// If a name has been passed the custom effect will be added to the device
			var customEffectCommand = new CustomEffectCommand();
			customEffectCommand.Command = "add";
			customEffectCommand.AnimName = Name;



			return customEffectCommand;
		}

		public CustomEffectCommand Build(CustomEffect customEffect)
		{
			// If no name has been passed the custom effect will just be dispayed on the device
			var customEffectCommand = new CustomEffectCommand();
			customEffectCommand.Command = "display";

			BuildBody(customEffect, customEffectCommand);

			return customEffectCommand;
		}

		private void BuildBody(CustomEffect customEffect, CustomEffectCommand customEffectCommand)
		{
			if (customEffect.IsStatic)
			{
				customEffectCommand.AnimType = "static";
			}
			else
			{
				customEffectCommand.AnimType = "custom";
				customEffectCommand.Loop = customEffect.IsLoop;
			}

			//TODO Build animdata
		}
	}
}
