namespace Winleafs.Api.DTOs.Effects
{
	public class SelectEffectDto
	{
		public SelectEffectDto(string select)
		{
			Select = select;
		}

		public string Select { get; set; }
	}
}
