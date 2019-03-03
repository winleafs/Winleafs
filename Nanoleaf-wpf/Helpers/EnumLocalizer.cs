using System;
using Winleafs.Wpf.Views;

namespace Winleafs.Wpf.Helpers
{
    public static class EnumLocalizer
    {
        public static string GetLocalizedEnum(Enum enumValue)
        {
            try
            {
                return EnumResources.ResourceManager.GetString(enumValue.ToString());
            }
            catch
            {
                return enumValue.ToString();
            }
        }
    }
}
