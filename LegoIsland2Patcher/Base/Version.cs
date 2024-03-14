using System.IO;

namespace LegoIsland2Patcher.Base
{
	public class Version
	{
		public static readonly Version[] Versions =
		{
			new Version("English Version 1",    0x52, "LEGO Island 2.exe", 0x529D, 7,  21, 0xA495, 0x2A870),
			new Version("English Version 2",    0x7A, "LEGO Island 2.exe", 0x529D, 7,  21, 0xA495, 0x2A870),
			new Version("Spanish Version",      0xCF, "Isola LEGO 2.exe",  0x52FD, 7,  21, 0xA765, 0x3DFA0),
			new Version("Dutch Version",        0x98, "LEGO eiland 2.exe", 0x52D6, 12, 31, 0xCAC3, 0x43C70),
			new Version("German Version",       0x31, "LEGO Insel 2.exe",  0x52FD, 7,  21, 0xA765, 0x3E1A0),
			new Version("Swedish Version",      0x08, "LEGO Island 2.exe", 0x52D6, 12, 31, 0xCAC3, 0x43C70),
			new Version("Norwegian Version",    0xF6, "LEGO Island 2.exe", 0x52D6, 12, 31, 0xCAC3, 0x43C70),
			new Version("French Version",       0x81, "L'île LEGO 2.exe",  0x52FD, 7,  21, 0xA765, 0x3E1E0),
			new Version("Unidentified Version", 0x52, "LEGO Island 2.exe", 0x529D, 7,  21, 0xA495, 0x2A870)
		};

		public static Version CheckVersion(string path)
		{
			foreach (var version in Versions)
			{
				if (File.Exists(Path.Combine(path, version.ExeName)))
				{
					return version;
				}
			}

			return null;
		}

		public string Label { get; set; }
		public byte CheckByte { get; set; } //Byte value at position 0x128
		public string ExeName { get; set; }
		public long ResolutionOffset { get; set; } //See https://www.rockraidersunited.com/topic/7653-widescreen-hack-high-resolution/?tab=comments#comment-129128
		public int ResSep { get; set; } //Distance from W to H
		public int ResDis { get; set; } //Distance from one resolution to the next
		public long FovOffset { get; set; } //Search for 00 00 40 3F
		public long LoadOffset { get; set; } //Long loading fix. Search for 90 90 90 90 90 FF. Use the position of FF.

		public Version(string label, byte checkByte, string exeName, long resOff, int resSep, int resDis, long fovOff, long loadOff)
		{
			Label = label;
			CheckByte = checkByte;
			ExeName = exeName;
			ResolutionOffset = resOff;
			ResSep = resSep;
			ResDis = resDis;
			FovOffset = fovOff;
			LoadOffset = loadOff;
		}
	}
}
