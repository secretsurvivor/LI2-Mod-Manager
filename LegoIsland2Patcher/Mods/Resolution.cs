using LegoIsland2Patcher.Base;
using System;
using System.IO;

namespace LegoIsland2Patcher.Mods
{
	public class Resolution : IModification
	{
		public static Resolution GetCurrentResolution(LegoIslandExe legoIsland, BackupExe backup)
		{
			var res = new Resolution();

			res.Exist(legoIsland, backup);

			return res;
		}

		public long Height { get; set; }
		public long Width { get; set; }
		public bool CustomAspect { get; set; } = false;

		public (long[] Width, long[] Height) GenerateResolutionOffsets(LegoIslandExe legoIsland)
		{
			int resSep = legoIsland.Version.ResSep;
			int resDis = legoIsland.Version.ResDis;

			long[] resOffsetW = {
				legoIsland.Version.ResolutionOffset,
				legoIsland.Version.ResolutionOffset + resDis,
				legoIsland.Version.ResolutionOffset + (resDis * 2),
				legoIsland.Version.ResolutionOffset + (resDis * 3)
			};

			return (
				resOffsetW,
				new long[] {
					resOffsetW[0] + resSep,
					resOffsetW[1] + resSep,
					resOffsetW[2] + resSep,
					resOffsetW[3] + resSep
				}
			);
		}

		public void ApplyModification(LegoIslandExe legoIsland)
		{
			var (resOffsetW, resOffsetH) = GenerateResolutionOffsets(legoIsland);

			float customAspect = CustomAspect ? (float)Height / (float)Width : 3f / 4f;

			for (int i = 0; i < 4; i++)
			{
				Patch.ApplyPatch(legoIsland, BitConverter.GetBytes(Height), resOffsetH[i]);
				Patch.ApplyPatch(legoIsland, BitConverter.GetBytes(Width), resOffsetW[i]);
			}

			Patch.ApplyPatch(legoIsland, BitConverter.GetBytes(customAspect), legoIsland.Version.FovOffset);
		}

		public void RemoveModification(LegoIslandExe legoIsland, BackupExe backup)
		{
			var (resOffsetW, resOffsetH) = GenerateResolutionOffsets(legoIsland);

			for (int i = 0; i < 4; i++)
			{
				Patch.RemovePatch(legoIsland, backup, resOffsetW[i], 4);
				Patch.RemovePatch(legoIsland, backup, resOffsetH[i], 4);
			}

			Patch.RemovePatch(legoIsland, backup, legoIsland.Version.FovOffset, 4);
		}

		public bool Exist(LegoIslandExe legoIsland, BackupExe backup)
		{
			var (resOffsetW, resOffsetH) = GenerateResolutionOffsets(legoIsland);

			using var lego = legoIsland.ReadFile();
			using var bck = backup.ReadFile();

			bool difRes = lego.CompareBytes(bck, legoIsland.Version.FovOffset, 4);

			if (difRes)
			{
				lego.Seek(resOffsetW[0], SeekOrigin.Begin);
				Width = lego.ReadLong();

				lego.Seek(resOffsetH[0], SeekOrigin.Begin);
				Height = lego.ReadLong();
			}

			CustomAspect = lego.CompareBytes(bck, resOffsetW[0], 4);

			return difRes;
		}
	}
}
