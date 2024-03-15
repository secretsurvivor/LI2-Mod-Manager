using System.IO;

namespace LegoIsland2Patcher.Base
{
	public static class Patch
	{
		public static void ApplyPatch(LegoIslandExe exe, byte[] bytes, long offset, int length)
		{
			using var fs = exe.WriteFile();
			fs.Seek(offset, SeekOrigin.Begin);
			fs.Write(bytes, 0, length);
		}

		public static void ApplyPatch(LegoIslandExe exe, byte[] bytes, long offset)
		{
			ApplyPatch(exe, bytes, offset, bytes.Length);
		}

		public static void RemovePatch(LegoIslandExe exe, BackupExe backup, long offset, int length)
		{
			using var bck = backup.ReadFile();
			using var fs = exe.WriteFile();

			bck.Seek(offset, SeekOrigin.Begin);

			byte[] backupBytes = new byte[length];
			bck.Read(backupBytes, 0, length);

			fs.Seek(offset, SeekOrigin.Begin);
			fs.Write(backupBytes, 0, length);
		}
	}
}
