using System.IO;

namespace LegoIsland2Patcher.Base
{
	public static class Patch
	{
		public static void ApplyPatch(LegoIslandExe exe, byte[] bytes, long offset, int length)
		{
			// Length should be the same size as bytes[] assuming that its not another way of adding more 0 bytes at the end
			// or limiting the size of bytes being written
			if (bytes.Length < length)
			{
				return;
			}

			using var fs = exe.WriteFile();
			fs.Seek(offset, SeekOrigin.Begin);

			foreach (var item in bytes)
			{
				fs.WriteByte(item);
			}
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
			fs.Seek(offset, SeekOrigin.Begin);

			for (var i = 0; i < length; i++)
			{
				fs.WriteByte((byte)bck.ReadByte());
			}
		}
	}
}
