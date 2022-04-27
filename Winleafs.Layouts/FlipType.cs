using Winleafs.Models.Enums;

namespace Winleafs.Layouts
{
    public enum FlipType
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        HorizontalVertical = 3
    }

    public static class FlipTypeHelper
    {
        public static FlipType ScreenMirrorFlipToFlipType(ScreenMirrorFlip screenMirrorFlip)
        {
            switch (screenMirrorFlip)
            {
                case ScreenMirrorFlip.None:
                    return FlipType.None;
                case ScreenMirrorFlip.Horizontal:
                    return FlipType.Horizontal;
                case ScreenMirrorFlip.Vertical:
                    return FlipType.Vertical;
                case ScreenMirrorFlip.HorizontalVertical:
                    return FlipType.HorizontalVertical;
                default:
                    throw new NotImplementedException($"Conversion for {nameof(ScreenMirrorFlip)}.{screenMirrorFlip} not implemented.");
            }
        }
    }
}
