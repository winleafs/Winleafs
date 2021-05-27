namespace Winleafs.Api.DTOs.States
{
	public class SetValueDto
	{
		public SetValueDto(int value)
		{
			Value = value;
		}

		public int Value { get; set; }
	}
}
