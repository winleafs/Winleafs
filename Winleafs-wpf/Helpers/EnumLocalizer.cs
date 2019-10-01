using System;
using Winleafs.Wpf.Views;

namespace Winleafs.Wpf.Helpers
{
    public static class EnumLocalizer
    {
        /// <summary>
        /// Localizes the enum value using the <see cref="EnumResources"/>.
        /// </summary>
        /// <param name="enumValue">The value to be localized.</param>
        /// <returns>The localized string for the <see cref="enumValue"/>.</returns>
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
