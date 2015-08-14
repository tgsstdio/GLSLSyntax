using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public class StructInfo
	{
		public StructInfo ()
		{
			Members = new List<StructMember> ();
		}
		public string Name {get;set;}
		public LayoutInformation Layout {get;set;}
		public List<StructMember> Members {get; private set;}
	}
}

