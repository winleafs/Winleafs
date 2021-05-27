namespace Winleafs.Api.DTOs.Shared
{
	public class WriteObjectDto
	{
		public string Command { get; set; }

		public string AnimName { get; set; }

		public string AnimType { get; set; }

		public string ExtControlVersion { get; set; }

		public WriteObjectDto()
		{
		}

		public WriteObjectDto(string command, string animName, string extControlVersion = null)
		{
			Command = command;
			AnimName = animName;
			ExtControlVersion = extControlVersion;
		}
	}
}
