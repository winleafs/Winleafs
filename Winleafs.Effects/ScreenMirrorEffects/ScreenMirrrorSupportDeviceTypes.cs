using Winleafs.Models.Enums;

namespace Winleafs.Effects.ScreenMirrorEffects
{
    public static class ScreenMirrrorSupportDeviceTypes
    {
        public static readonly HashSet<DeviceType> Types = new HashSet<DeviceType> { DeviceType.Aurora, DeviceType.Canvas, DeviceType.Shapes };
    }
}
