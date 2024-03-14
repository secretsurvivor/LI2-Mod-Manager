using System.IO;

namespace LegoIsland2Patcher.Base
{
	public class LegoIslandExe
	{
		public Version Version { get; set; }
		public string LegoIslandFilePath { get; set; }

		public LegoIslandExe(Version version, string legoIslandFilePath)
		{
			Version = version;
			LegoIslandFilePath = legoIslandFilePath;
		}

		public FileStream ReadFile()
		{
			return File.OpenRead(LegoIslandFilePath);
		}

		public FileStream WriteFile()
		{
			return File.OpenWrite(LegoIslandFilePath);
		}
	}
}
