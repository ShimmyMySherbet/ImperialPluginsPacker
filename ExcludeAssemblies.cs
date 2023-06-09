using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PackForIP
{
	public static class ExcludeAssemblies
	{

		/// <summary>
		/// List of regex symbols for assmblies to exclude from the Imperial Plugins package
		/// These assemblies are found within Unturned, and shouldn't be included
		/// </summary>

		public static readonly List<string> RegexSymbols_Global = new List<string>()
		{
			"Newtonsoft\\.Json\\.dll",
			"mscor.",
			"HighlightingSystem\\.dll",
			"StandardAssets\\.dll",
			"SteelSeries.",
			"Assembly-CSharp.",
			"Astar.",
			"BattlEye.",
			".steamworks\\.net.",
			"Mono\\.Posix.",
			"Mono\\.Security.",
			"Pathfinding.",
			"Razer.",
			"SDG.",
			"System.",
			"Unity.",
			"OpenMod."
		};
		public static readonly List<string> RegexSymbols_Rocketmod = new List<string>()
		{
			"Rocket\\.API\\.dll",
			"Rocket\\.Core\\.dll",
			"Rocket\\.Unturned\\.dll"
		};

		public static readonly List<string> RegexSymbols_Openmod = new List<string>()
		{
			"0Harmony\\.dll",
			"AutoFac.",
			"Cronos.",
			"dnlib.",
			"DotNet\\.Glob.",
			"InlineIL.",
			"JetBrains\\.Anno.",
			"Microsoft.",
			"MoreLinq.",
			"NetEscap.",
			"Nito.",
			"Nuget.",
			"ReadLine.",
			"RuntimeNullables.",
			"Semver.",
			"Serilog.",
			"SmallFormat.",
			"YamlDotNet.",
		};

		public static List<Regex> BuildRocketmodRegices()
			=> RegexSymbols_Global.Concat(RegexSymbols_Rocketmod).Select(x => new Regex(x)).ToList();

		public static List<Regex> BuildOpenmodRegices()
			=> RegexSymbols_Global.Concat(RegexSymbols_Openmod).Select(x => new Regex(x)).ToList();
	}
}