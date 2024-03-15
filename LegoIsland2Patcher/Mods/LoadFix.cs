using LegoIsland2Patcher.Base;
using static LegoIsland2Patcher.Utilities;

namespace LegoIsland2Patcher.Mods
{
	public class LoadFix : IModification
	{
		readonly byte[] nullBytes = { 0xC3, 0x90, 0x90, 0x90, 0x90, 0x90 };

		public void ApplyModification(LegoIslandExe legoIsland)
		{
			Patch.ApplyPatch(legoIsland, nullBytes, legoIsland.Version.LoadOffset);
		}

		public bool Exist(LegoIslandExe legoIsland, BackupExe backup)
		{
			return BytesMatch(legoIsland, legoIsland.Version.LoadOffset, nullBytes);
		}

		public void RemoveModification(LegoIslandExe legoIsland, BackupExe backup)
		{
			Patch.RemovePatch(legoIsland, backup, legoIsland.Version.LoadOffset, nullBytes.Length);
		}
	}
}
