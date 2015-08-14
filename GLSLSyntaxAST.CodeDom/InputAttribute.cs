using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class InputAttribute
	{
		public string Name {get;set;}
		public string Direction {get;set;}
		public LayoutInformation Layout {get;set;}
		public string TypeString;
		public Type ClosestType;
	}
}

