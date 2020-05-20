using ICSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.Core
{
	

	public static class IMMainMenuExtensions
	{
		private static readonly string MenuTableName = "SYS_LOGS";
		private static readonly string MenuBeforeTool = "Tools";

		public static string BuildToolName(this IMMainMenu mainMenu, string parentToolName, string toolName)
		{
			return string.Concat(parentToolName, "\\", toolName);
		}
		public static void InsertItem(this IMMainMenu mainMenu, string parentToolName, string toolName, Action action)
		{
			mainMenu.InsertItem(mainMenu.BuildToolName(parentToolName, toolName), () => action(), MenuTableName);
		}
		public static void SetLocation(this IMMainMenu mainMenu)
		{
			mainMenu.SetInsertLocation(mainMenu.BuildToolName(MenuBeforeTool, ""), IMMainMenu.InsertLocation.Before);
		}
	}
}
