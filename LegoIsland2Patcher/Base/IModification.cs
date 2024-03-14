using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoIsland2Patcher.Base
{
	public interface IModification
	{
		void ApplyModification(LegoIslandExe legoIsland);
		void RemoveModification(LegoIslandExe legoIsland, Backup backup);
		bool Exist(LegoIslandExe legoIsland, Backup backup);
	}
}
