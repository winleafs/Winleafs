namespace Winleafs.Api.DTOs.States
{
	public class StateValueDto
	{
		public StateValueDto(bool value)
		{
			Value = value;
		}

		public bool Value { get; set; }
	}
}
