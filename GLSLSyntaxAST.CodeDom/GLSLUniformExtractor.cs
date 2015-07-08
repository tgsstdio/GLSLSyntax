using System;
using System.IO;
using Irony.Parsing;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public class GLSLUniformExtractor : IGLSLUniformExtractor
	{
		public GLSLUniformExtractor(IGLSLTypeLookup lookup)
		{
			mTypeLookup = lookup;
		}

		public void Initialize ()
		{
			mUniforms = new Dictionary<string, StructMember> ();
			mBlocks = new Dictionary<string, StructInfo> ();
		}

		private IGLSLTypeLookup mTypeLookup;
		private Dictionary<string, StructMember> mUniforms;

		public ICollection<StructMember> Uniforms {
			get {
				return mUniforms.Values;
			}
		}

		private Dictionary<string, StructInfo> mBlocks;
		public ICollection<StructInfo> Blocks 
		{
			get {
				return mBlocks.Values;
			}
		}

		private bool CheckForUniformTag (StructInfo info, ParseTreeNode child)
		{
			if (child.Term.Name != "type_qualifier")
			{
				return false;
			}

			var layoutQualifier = child.ChildNodes [0];
			ParseTreeNode qualifierlist = layoutQualifier.ChildNodes.Find ((p) => p.Term.Name == "layout_qualifier_id_list");

			if (qualifierlist != null)
			{
				const string FORMAT_REQUIRED = "std140";
				if (qualifierlist.ChildNodes.Find ((p) => p.Token.ValueString == FORMAT_REQUIRED) == null)
				{
					return false;
				} 
				else
				{
					info.LayoutFormat = FORMAT_REQUIRED;
				}
			}
			else
			{
				return false;
			}

			var uniformTag = child.ChildNodes [1].Token;
			if (uniformTag.ValueString != "uniform")
			{
				return false;
			}
			return true;
		}

		private bool ExtractName (StructInfo info, ParseTreeNode child)
		{
			info.Name = child.Token.ValueString;
			return true;
		}

		private bool ExtractMembers (StructInfo info, ParseTreeNode child)
		{
			if (child.Term.Name == "struct_declaration_list")
			{
				var members = child.ChildNodes.FindAll ((p) => p.Term.Name == "struct_declaration");

				info.Members = new List<StructMember> ();
				foreach (var member in members)
				{
					var temp = new StructMember ();
					temp.TypeString = member.ChildNodes [0].Token.ValueString;
					temp.ClosestType = mTypeLookup.FindClosestType (temp.TypeString);

					var declarator = member.ChildNodes [1];
					if (declarator.Term.Name == "struct_declarator")
					{
						// ASSUME type is first child
						temp.Name = declarator.ChildNodes [0].Token.ValueString;
					}

					info.Members.Add (temp);
				}
				return true;
			} 
			else
			{
				return false;
			}
		}


		private int FindBlocks(ParseTreeNode node, int level)
		{
			if (node.Term.Name == "block_structure")
			{
				var temp = new StructInfo ();

				// first child is uniform keyword
				bool isValid = CheckForUniformTag(temp, node.ChildNodes[0]);
				if (isValid)
				{
					// second child is struct type name
					if (!ExtractName (temp, node.ChildNodes [1]))
					{
						return 0;
					}

					// third child is list of member inside
					if (ExtractMembers (temp, node.ChildNodes [2]))
					{
						// TODO : case sensitive ????
						var key = temp.Name.ToLowerInvariant();
						if (!mBlocks.ContainsKey (key))
						{
							mBlocks.Add (key, temp);
						}

						return 1;
					}
					else
					{
						return 0;
					}
				}
				else
				{
					return 0;
				}

			} 
			else
			{
				int total = 0;
				foreach (ParseTreeNode child in node.ChildNodes)
				{
					total += FindBlocks (child, level + 1);
				}
				return total;
			}
		}

		private int FindUniforms(ParseTreeNode node, int level)
		{
			// TODO : method not implemented
			return 0;
		}

		private int FindStructs(ParseTreeNode node, int level)
		{
			// TODO : method not implemented 
			return 0;
		}

		public int Extract (string code)
		{
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser (lang);
			var tree = compiler.Parse (code);
			int total = FindStructs (tree.Root, 0);
			total += FindBlocks (tree.Root, 0);	
			total += FindUniforms (tree.Root, 0);

			return total;
		}

		public int Extract (System.IO.Stream stream)
		{
			using (var reader = new StreamReader (stream))
			{
				return Extract (reader.ReadToEnd ());
			}
		}
	}
}

