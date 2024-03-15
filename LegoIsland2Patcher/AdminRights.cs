using LegoIsland2Patcher.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LegoIsland2Patcher
{
	public static class AdminRights
	{
		public static bool IsAdministrator()
		{
			// Check if the current user has administrative privileges
			using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
			var principal = new System.Security.Principal.WindowsPrincipal(identity);

			return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
		}

		public static bool RequireAdminPrivileges(string path)
		{
			try
			{
				using var fs = File.OpenWrite(path);
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				return true;
			}
		}

		public static bool RequireAdminPrivileges(LegoIslandExe legoIsland)
		{
			return RequireAdminPrivileges(legoIsland.LegoIslandFilePath);
		}

		public static bool RestartAsAdministrator()
		{
			// Get the path of the current executable
			string exePath = Assembly.GetEntryAssembly().Location;

			// Start a new process with elevated permissions
			ProcessStartInfo startInfo = new ProcessStartInfo(exePath)
			{
				UseShellExecute = true,
				Verb = "runas" // This verb launches the process as administrator
			};

			try
			{
				Process.Start(startInfo);

				return true;
			}
			catch (System.ComponentModel.Win32Exception)
			{
				// The user declined the elevation request
				return false;
			}
		}

		public static void CloseCurrentInstance()
		{
			// Close the original instance of the program
			Process.GetCurrentProcess().Kill();
		}
	}
}
