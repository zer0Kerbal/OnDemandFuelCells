﻿//109
// 
// This code was generated by a tool. Any changes made manually will be lost
// the next time this code is regenerated.
// 

using System.Reflection;

[assembly: AssemblyFileVersion("1.2.99.109")]
[assembly: AssemblyVersion("1.2.99.0")]
[assembly: AssemblyInformationalVersion("1.2.99")]
[assembly: KSPAssembly("OnDemandFuelCells", 1,2,99)]

namespace OnDemandFuelCells
{
	public static class Version
	{
		public const int major = 1;
		public const int minor = 2;
		public const int patch = 99;
		public const int build = 0;
		public const string Number = "1.2.99.0";
#if DEBUG
        public const string Text = Number + "-zed'K BETA DEBUG";
        public const string SText = Number + "B DEBUG";
#else
        public const string Text = Number + "-zed'K";
		public const string SText = Number;
#endif
	}
}