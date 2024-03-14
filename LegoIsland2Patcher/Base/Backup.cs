using System.IO;

namespace LegoIsland2Patcher.Base
{
	public class Backup : LegoIslandExe
	{
		public static Backup BackupLegoIsland(LegoIslandExe legoIsland)
		{
			string backupPath = Path.Combine("backup", legoIsland.Version.ExeName);
			var backup = new Backup(legoIsland, backupPath);

			if (File.Exists(backupPath))
			{
				return backup;
			}

			if (!Directory.Exists("backup"))
			{
				Directory.CreateDirectory("backup");
				File.WriteAllBytes("backup/_Do Not Delete This Folder", new byte[0]);
			}

			using var fs = legoIsland.ReadFile();
			using var bck = File.OpenWrite(backupPath);

			fs.CopyTo(bck);

			return backup;
		}

		public LegoIslandExe Original { get; set; }

		public Backup(LegoIslandExe legoIsland, string path) : base(legoIsland.Version, path)
		{
			Original = legoIsland;
		}

		public void Restore()
		{
			using var fs = Original.WriteFile();
			using var bck = ReadFile();

			bck.CopyTo(fs);
		}
	}
}
