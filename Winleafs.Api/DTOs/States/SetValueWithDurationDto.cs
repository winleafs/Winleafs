namespace Winleafs.Api.DTOs.States
{
	public class SetValueWithDurationDto
	{
		public SetValueWithDurationDto(int value, int? duration = null)
		{
			Value = value;
			Duration = duration;
		}

		public int Value { get; set; }

		public int? Duration { get; set; }
	}
}
