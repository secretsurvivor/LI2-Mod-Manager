using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			var mod = new Modification()
			{
				Installed = File.Exists(InstalledPath)
			};

			if (File.Exists(InfoPath))
			{
				string[] info = File.ReadAllLines(InfoPath);

				mod.Name = info[0];
				mod.Author = info[1];
				mod.Description = info[2];
			}
			else
			{
				// Use folder name as Mod name
				mod.Name = Path.GetFileName(Path.GetDirectoryName(directory));
				mod.Author = "Unknown";
				mod.Description = "No description available.";
			}

			int validCounter = 0;

			if (File.Exists(DataPath))
			{
				mod.HasData = true;
				mod.DataPath = DataPath;

				validCounter++;
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

				validCounter++;
			}

			if (validCounter < 1)
			{ // Invalid Mod
				return null;
			}

			return mod;
		}

		public static IList<Modification> GetAvailableMods()
		{
			if (!Directory.Exists("mods"))
			{
				return new List<Modification>();
			}

			var modList = new List<Modification>();

			foreach (var dir in Directory.GetDirectories("mods"))
			{
				var mod = GetModification(dir);

				if (mod != null)
				{
					modList.Add(mod);
				}
			}

			return modList;
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
				Base.Patch.ApplyPatch(legoIsland, Patch, Offset);
			}

			if (HasData)
			{
				// TODO: Apply _data packages
			}
		}

		public void RemoveModification(LegoIslandExe legoIsland, BackupExe backup)
		{
			if (HasPatch)
			{
				Base.Patch.RemovePatch(legoIsland, backup, Offset, Patch.Length);
			}

			if (HasData)
			{
				// TODO: Remove _data packages
			}
		}

		public bool Exist(LegoIslandExe legoIsland, BackupExe backup)
		{
			return Installed;
		}
	}
}
