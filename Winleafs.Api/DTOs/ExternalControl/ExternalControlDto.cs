using Winleafs.Api.DTOs.Shared;

namespace Winleafs.Api.DTOs.ExternalControl
{
	public class ExternalControlDto
	{
		public ExternalControlDto(string version)
		{
			Write = new WriteObjectDto()
			{
				Command = WriteCommands.Display,
				AnimType = "extControl",
				ExtControlVersion = version
			};
		}

		public WriteObjectDto Write { get; set; }
	}
}
