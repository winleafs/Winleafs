using System.Text;
using Winleafs.Effects.Helpers;
using Winleafs.Models.Effects;
using Winleafs.Models.Layouts;

namespace Winleafs.Effects.CustomEffects
{
    /// <summary>
    /// Used to build a CustomEffectCommand to send to the API
    /// </summary>
    public class CustomEffectCommandBuilder
    {
        private readonly CustomEffect _customEffect;
        private int _transitionTime;

        public CustomEffectCommandBuilder(CustomEffect customEffect)
        {
            _customEffect = customEffect;
        }

        public CustomEffectCommand BuildAddCommand(float transitionSecs, string name)
        {
            //If a name has been passed the custom effect will be added to the device
            var customEffectCommand = new CustomEffectCommand();
            customEffectCommand.Command = "add";
            customEffectCommand.AnimName = name;

            //Convert seconds to tenths of a second, the time unit used by commands
            _transitionTime = (int)Math.Floor(transitionSecs * 10);

            BuildBody(customEffectCommand);

            return customEffectCommand;
        }

        public CustomEffectCommand BuildDisplayCommand(float transitionSecs)
        {
            //If no name has been passed the custom effect will just be dispayed on the device
            var customEffectCommand = new CustomEffectCommand();
            customEffectCommand.Command = "display";

            //Convert seconds to tenths of a second, the time unit used by commands
            _transitionTime = (int)Math.Floor(transitionSecs * 10);

            BuildBody(customEffectCommand);

            return customEffectCommand;
        }

        private void BuildBody(CustomEffectCommand customEffectCommand)
        {
            if (_customEffect.IsStatic)
            {
                customEffectCommand.AnimType = "static";
            }
            else
            {
                customEffectCommand.AnimType = "custom";
                customEffectCommand.Loop = _customEffect.IsLoop;
            }

            var animData = new StringBuilder();
            animData.Append(_customEffect.Frames[0].PanelColors.Count);

            foreach (var panelId in _customEffect.Frames[0].PanelColors.Keys)
            {
                animData.Append(BuildPanelAnimData(panelId));
            }

            customEffectCommand.AnimData = animData.ToString();

            //Set pallete so used colurs are displayed in apps (docs say max of 20)
            var rgbs = _customEffect.Frames.SelectMany(f => f.PanelColors.Values).Distinct().Take(20);
            foreach (var rgb in rgbs)
            {
                customEffectCommand.Palette.Add(ColorFormatConverter.ToPalette(rgb));
            }
        }

        private string BuildPanelAnimData(int panelId)
        {
            //Example taken from docs. This method returns one of the panel rows
            //(semicolons and newlines added for clarity only):
            //numPanels;
            //panelId0; numFrames0; RGBWT01; RGBWT02; ... RGBWT0n(0);
            //panelId1; numFrames1; RGBWT11; RGBWT12; ... RGBWT1n(1); ... ...
            //panelIdN; numFramesN; RGBWTN1; RGBWTN2; ... RGBWTNn(N);
            //RGB=color, W=0, Txx is transition time is tenths of a second

            var sb = new StringBuilder();
            uint? prevRgb = null;

            var totalFrames = 0;
            var sameColorFrameCount = 0;

            foreach (var rgb in _customEffect.Frames.Select(f => f.PanelColors[panelId]))
            {
                if (prevRgb == null || rgb != prevRgb.Value)
                {
                    if (prevRgb != null && sameColorFrameCount > 0)
                    {
                        sb.AppendFormat(" {0} {1} {2} 0 {3}",
                        (prevRgb >> 16) & 255,  // Get the value for red by shifting 2 bytes right
                        (prevRgb >> 8) & 255,   // Get the value for green by shifting one byte right
                        prevRgb & 255,          // Get the value for blue by taking the lowest byte
                        sameColorFrameCount * _transitionTime);

                        totalFrames++;
                    }

                    sb.AppendFormat(" {0} {1} {2} 0 {3}",
                        (rgb >> 16) & 255,
                        (rgb >> 8) & 255,
                        rgb & 255,
                        _transitionTime);

                    sameColorFrameCount = 0;
                    totalFrames++;
                }
                else
                {
                    sameColorFrameCount++;
                }

                prevRgb = rgb;
            }

            //Prepend panelId numframes
            return string.Format(" {0} {1}{2}", panelId, totalFrames, sb);
        }

    }
}
