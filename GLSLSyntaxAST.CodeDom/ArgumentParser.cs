using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public class ArgumentParser
	{
		public class ShaderFilesNotSuppliedException : Exception			
		{
			public ShaderFilesNotSuppliedException () : base("Shader files not supplied")
			{
				
			}
		}

		public class FileSwitchNotSuppliedException : Exception			
		{
			public FileSwitchNotSuppliedException () : base("-F switch was not provided")
			{

			}
		}

		public class ValueNotSuppliedException : Exception			
		{			
			public ValueNotSuppliedException (string flag)
				: base(string.Format("Value for '{0}' switch not supplied", flag))
			{

			}
		}

		private class ArgumentSwitch
		{
			public ArgumentSwitch (string value)
			{
				OriginalString = value;
				Flag = value.ToUpperInvariant();
			}
			public string OriginalString { get; private set; }
			public string Flag { get; private set; }
			public int? Index { get; set; }
			public int ValueIndex { get; set; }
			public string Value {get;set;}

			public bool WasSupplied()
			{
				return Index.HasValue;
			}
		}

		public ArgumentParser ()
		{
			Namespace = string.Empty;
			AssemblyFileName = string.Empty;
			SourceFileName = string.Empty;
			GenerateCode = false;
			GenerateAssembly = false;
		}

		public string Namespace { get; private set; }
		public string AssemblyFileName { get; private set; }
		public string SourceFileName { get; private set; }
		public bool GenerateCode { get; private set; } 
		public bool GenerateAssembly { get; private set; } 

		public string[] Parse(string [] args)
		{
			var fileSwitch = new ArgumentSwitch("-F");
			var ns = new ArgumentSwitch ("-n");
			var compiler = new ArgumentSwitch ("-c");
			var assembly = new ArgumentSwitch ("-a");

			var argSwitches = new ArgumentSwitch[] {
				ns,
				assembly,
				compiler,
			};

			for (int i = 0 ; i < args.Length; ++i)
			{
				if (fileSwitch.Flag == args [i].ToUpperInvariant ())
				{
					fileSwitch.Index = i;
				} 
				else
				{
					foreach (var s in argSwitches)
					{
						if (s.Flag == args [i].ToUpperInvariant ())
						{
							s.Index = i;
							s.ValueIndex = i + 1;
						}
					}
				}
			}

			if (!fileSwitch.WasSupplied())
			{
				throw new FileSwitchNotSuppliedException ();
			}

			if (fileSwitch.Index.Value >= args.Length)
			{
				throw new ShaderFilesNotSuppliedException ();
			}

			foreach (var s in argSwitches)
			{
				if (s.WasSupplied())
				{
					if (s.ValueIndex >= fileSwitch.Index)
					{
						throw new ValueNotSuppliedException (s.OriginalString);
					}
					else
					{
						var strValue = args [s.ValueIndex];

						foreach (var r in argSwitches)
						{
							if (r.Flag == strValue.ToUpperInvariant ())
							{
								throw new ValueNotSuppliedException (s.OriginalString);
							}
						}

						s.Value = args [s.ValueIndex];
					}
				}
			}

			Namespace = ns.Value ?? Namespace;

			if (compiler.WasSupplied ())
			{
				GenerateCode = true;
				SourceFileName = compiler.Value;
			}

			if (assembly.WasSupplied ())
			{
				GenerateAssembly = true;
				AssemblyFileName = assembly.Value;
			}

			var output = new List<string> ();
			for (int i = fileSwitch.Index.Value + 1; i < args.Length; ++i)
			{
				output.Add(args[i]);
			}
			return output.ToArray ();
		}
	}
}

