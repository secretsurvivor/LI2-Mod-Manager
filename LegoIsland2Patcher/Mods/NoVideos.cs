using LegoIsland2Patcher.Base;
using System.IO;
using System.Xml.Linq;
using static LegoIsland2Patcher.Utilities;

namespace LegoIsland2Patcher.Mods
{
	public class NoVideos : IModification
	{
		//No intro videos seem to be the same across versions
		readonly byte[] nullBytes = { 0x90, 0x90, 0x90, 0x90, 0x90 };
		readonly long[] offsets = { 0x2AEA, 0x2B20, 0x2B56 };

		public void ApplyModification(LegoIslandExe legoIsland)
		{
			foreach (long off in offsets)
			{
				Patch.ApplyPatch(legoIsland, nullBytes, off);
			}
		}

		public bool Exist(LegoIslandExe legoIsland, BackupExe backup)
		{
			foreach (long offset in offsets)
			{
				if (!BytesMatch(legoIsland, offset, nullBytes))
				{
					return false;
				}
			}

			return true;
		}

		public void RemoveModification(LegoIslandExe legoIsland, BackupExe backup)
		{
			foreach (long off in offsets)
			{
				Patch.RemovePatch(legoIsland, backup, off, nullBytes.Length);
			}
		}
	}
}
