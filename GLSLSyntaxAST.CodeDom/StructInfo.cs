using System.Collections.Generic;
using Irony.Parsing;

namespace GLSLSyntaxAST.CodeDom
{
	public class StructInfo
	{
		public StructInfo ()
		{
			Members = new List<StructMember> ();
			StructType = GLSLStructType.Struct;
		}

		public GLSLStructType StructType { get; set; }
		public string Name {get;set;}
		public LayoutInformation Layout {get;set;}
		public List<StructMember> Members {get; private set;}
	}
}

