using Newtonsoft.Json;

namespace Winleafs.Api.DTOs.States
{
	public class SetStateValuesDto
	{
		public SetStateValuesDto()
		{
		}

		public SetStateValuesDto(int hue, int saturation)
		{
			Hue = new SetValueDto(hue);
			Saturation = new SetValueDto(saturation);
		}

		public SetStateValuesDto(int hue, int saturation, int brightness)
		{
			Brightness = new SetValueWithDurationDto(brightness, null);
			Hue = new SetValueDto(hue);
			Saturation = new SetValueDto(saturation);
		}

		public SetValueDto Hue { get; set; }

		[JsonProperty("sat")]
		public SetValueDto Saturation { get; set; }

		public SetValueWithDurationDto Brightness { get; set; }

		public static SetStateValuesDto SetBrightness(int value, int? duration = null)
		{
			return new SetStateValuesDto()
			{
				Brightness = new SetValueWithDurationDto(value, duration)
			};
		}

		public static SetStateValuesDto SetHue(int value)
		{
			return new SetStateValuesDto()
			{
				Hue = new SetValueDto(value)
			};
		}

		public static SetStateValuesDto SetSaturation(int value)
		{
			return new SetStateValuesDto()
			{
				Saturation = new SetValueDto(value)
			};
		}
	}
}
