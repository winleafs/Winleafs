namespace Winleafs.Api.DTOs.States
{
	public class SetStateDto
	{
		public SetStateDto(bool value)
		{
			On = new StateValueDto(value);
		}

		public StateValueDto On { get; set; }
	}
}
