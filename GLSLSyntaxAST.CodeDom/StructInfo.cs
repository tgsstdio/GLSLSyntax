using System.Collections.Generic;
using Irony.Parsing;

namespace GLSLSyntaxAST.CodeDom
{
	public class StructInfo
	{
		public StructInfo ()
		{
			Members = new List<StructMember> ();
		}

		public bool ExtractName (ParseTreeNode child)
		{
			Name = child.Token.ValueString;
			return true;
		}

		public string Name {get;set;}
		public LayoutInformation Layout {get;set;}
		public List<StructMember> Members {get; private set;}
	}
}

