using Winleafs.Api.DTOs.Shared;

namespace Winleafs.Api.DTOs.Effects
{
	public class GetEffectDetailsDto
	{
		public GetEffectDetailsDto(string effectName)
		{
			Write = new WriteObjectDto(WriteCommands.Request, effectName);
		}

		public WriteObjectDto Write { get; set; }
	}
}
