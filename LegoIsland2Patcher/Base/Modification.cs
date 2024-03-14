using System.IO;
using static LegoIsland2Patcher.Utilities;

namespace LegoIsland2Patcher.Base
{
	public class Modification : IModification
	{
		private static (string Info, string Data, string Patch, string Installed) ModPaths(string directory)
		{
			return (
				Path.Combine(directory, "info.txt"),
				Path.Combine(directory, "_data"),
				Path.Combine(directory, "patch.txt"),
				Path.Combine(directory, "installed")
			);
		}

		public static Modification GetModification(string directory)
		{
			var (InfoPath, DataPath, PatchPath, InstalledPath) = ModPaths(directory);

			if (!File.Exists(InfoPath))
			{
				return null;
			}

			string[] info = File.ReadAllLines(InfoPath);

			var mod = new Modification()
			{
				Name = info[0],
				Author = info[1],
				Description = info[2],
				Installed = File.Exists(InstalledPath)
			};

			if (File.Exists(DataPath))
			{
				mod.HasData = true;
				mod.DataPath = DataPath;
			}

			if (File.Exists(PatchPath))
			{
				mod.HasPatch = true;
				string[] data = File.ReadAllText(PatchPath).Split(';');

				mod.Offset = HexToLong(data[0]);
				mod.Patch = ToBytes(data[1]);

				if (data.Length > 2)
				{
					mod.Originals = ToBytes(data[2]);
				}
			}

			return mod;
		}

		public string Name { get; set; }
		public string Description { get; set; }
		public string Author { get; set; }
		public bool HasPatch { get; set; }
		public long Offset { get; set; }
		public byte[] Patch { get; set; }
		public byte[] Originals { get; set; }
		public bool HasData { get; set; }
		public string DataPath { get; set; }
		public bool Installed { get; set; }

		public void ApplyModification(LegoIslandExe legoIsland)
		{
			if (HasPatch)
			{
				using var fs = legoIsland.WriteFile();

				fs.Seek(Offset, SeekOrigin.Begin);

				foreach (var p in Patch)
				{
					fs.WriteByte(p);
				}
			}
		}

		public void RemoveModification(LegoIslandExe legoIsland, Backup backup)
		{
			if (HasPatch)
			{
				using var fs = legoIsland.WriteFile();
				using var bck = backup.ReadFile();

				fs.Seek(Offset, SeekOrigin.Begin);

				for (int i = 0; i < Patch.Length; i++)
				{
					fs.WriteByte((byte)bck.ReadByte());
				}
			}
		}

		public bool Exist(LegoIslandExe legoIsland, Backup backup)
		{
			return Installed;
		}
	}
}
