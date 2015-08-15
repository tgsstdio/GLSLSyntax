using System;
using System.IO;
using Irony.Parsing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;

namespace GLSLSyntaxAST.CodeDom
{
	public class GLSLUniformExtractor : IGLSLUniformExtractor
	{
		public GLSLUniformExtractor(IGLSLTypeLookup lookup)
		{
			mTypeLookup = lookup;
			mLayouts = new StringCollection ();
			mUniforms = new Dictionary<string, StructMember> ();
			mBlocks = new Dictionary<string, StructInfo> ();
			mAttributes = new Dictionary<string, InputAttribute> ();
			mLanguage = new GLSLGrammar ();
			mCompiler = new Parser (mLanguage);
		}

		private StringCollection mLayouts;
		private GLSLGrammar mLanguage;
		private Parser mCompiler;
		public void Initialize ()
		{
			mLayouts.Add ("std140");
			mLayouts.Add ("std430");
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

		private Dictionary<string, InputAttribute> mAttributes;
		public ICollection<InputAttribute> Attributes
		{
			get{
				return mAttributes.Values;
			}
		}

		private bool CheckForUniformTag (StructInfo info, ParseTreeNode child)
		{
			if (child.Term != mLanguage.TypeQualifier )
			{
				return false;
			}

			var qualifierlist = child.ChildNodes.Find ((p) => p.Term == mLanguage.LayoutQualifierIdList);
			if (qualifierlist == null)
			{
				return false;
			}

			info.Layout = new LayoutInformation ();
			ExtractLayout (info.Layout, qualifierlist);					

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

		public void ExtractStructBody (StructInfo info, ParseTreeNode child)
		{
			var members = child.ChildNodes.FindAll (p => p.Term.Name == "struct_declaration");
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
		}

		private bool ExtractMembers (StructInfo info, ParseTreeNode child)
		{
			if (child.Term.Name == "struct_declaration_list")
			{
				ExtractStructBody (info, child);

				return true;
			} 
			else
			{
				return false;
			}
		}


		private int FindBlocks(ParseTreeNode node, int level)
		{
			if (node == null)
			{
				return 0;
			}


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
			return 0;
		}

		private void ExtractLayout(LayoutInformation info, ParseTreeNode parent)
		{
			if (parent != null)
			{
				foreach (var child in parent.ChildNodes)
				{
					if (child.Term == mLanguage.LayoutQualifier)
					{						
						foreach (var node in child.ChildNodes)
						{
							if (node.Term == mLanguage.LayoutQualifierIdList)
							{
								ExtractLayoutInfo (info, node);
							}
						}
					}
				}
			}
		}

		public void ExtractLayoutInfo (LayoutInformation info, ParseTreeNode parent )
		{
			foreach (var node in parent.ChildNodes)
			{
				if (node.Term == mLanguage.LayoutQualifierId)
				{						
					var firstNode = node.ChildNodes [0];
					if (firstNode.Term == mLanguage.IDENTIFIER)
					{
						var strValue = firstNode.Token.ValueString;						
						if (node.ChildNodes.Count == 1)
						{
							if (mLayouts.Contains (strValue))
							{
								info.Format = strValue;
							}
						}
						else if (node.ChildNodes.Count == 3)
						{
							var thirdNode = node.ChildNodes [2];
							if (node.ChildNodes [1].Token.ValueString == "=" &&
								thirdNode.Term == mLanguage.ConstantExpression)
							{
								info.Location = (int) thirdNode.ChildNodes[0].Token.Value;
							}
						}
					}
				}
			}
		}

		private int ExtractStructMembers (StructInfo temp, ParseTreeNode specifier)
		{
			if (specifier == null)
			{
				return 0;
			}

			// second child is struct type name
			if (!ExtractName (temp, specifier.ChildNodes [1]))
			{
				return 0;
			}
			// third child is list of member inside
			if (ExtractMembers (temp, specifier.ChildNodes [2]))
			{
				// TODO : case sensitive ????
				var key = temp.Name.ToLowerInvariant ();
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

		private int FindStructsAndAttributes(ParseTreeNode parent, int level)
		{
			if (parent == null)
			{
				return 0;
			}

			if (parent.Term == mLanguage.SingleDeclaration)
			{
				var node = parent.ChildNodes[0];
				if (node.Term == mLanguage.FullySpecifiedType)
				{
					var specifier = node.ChildNodes.Find (p => p.Term == mLanguage.StructSpecifier);
					var typeQualifier = node.ChildNodes.Find (p => p.Term == mLanguage.TypeQualifier);
					if (specifier != null)
					{
						var temp = new StructInfo ();
						temp.Layout = new LayoutInformation (); 
						ExtractLayout (temp.Layout, typeQualifier);					
						return ExtractStructMembers (temp, specifier);
					}

					var inDirection = typeQualifier.ChildNodes.Find (p => p.Term == mLanguage.InTerm);

					var outDirection = typeQualifier.ChildNodes.Find (p => p.Term == mLanguage.OutTerm);

					if (inDirection == null && outDirection == null)
					{
						return 0;
					}

					var attribute = new InputAttribute ();
					attribute.Direction = (inDirection != null) ? "in" : ((outDirection != null) ? "out" : null);
					attribute.Layout = new LayoutInformation ();
					ExtractLayout (attribute.Layout, typeQualifier);	
					var secondNode = node.ChildNodes [1];
					if (!mLanguage.TypesTerms.Contains (secondNode.Token.KeyTerm))
					{
						return 0;
					}

					attribute.TypeString = secondNode.Token.ValueString;
					attribute.ClosestType = mTypeLookup.FindClosestType (attribute.TypeString);

					var nameSibiling = parent.ChildNodes [1];
					attribute.Name = nameSibiling.Token.ValueString;

					// IGNORE
					if (!mAttributes.ContainsKey (attribute.Name))
					{
						mAttributes.Add (attribute.Name, attribute);
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
				foreach (ParseTreeNode child in parent.ChildNodes)
				{
					total += FindStructsAndAttributes (child, level + 1);
				}
				return total;
			}
		}

		public int Extract (string code)
		{
			var tree = mCompiler.Parse (code);
			int total = FindStructsAndAttributes (tree.Root, 0);
			total += FindBlocks (tree.Root, 0);	
			total += FindUniforms (tree.Root, 0);

			return total;
		}

		public void DebugCode (string code)
		{
			var tree = mCompiler.Parse (code);
			if (tree.ParserMessages.Count > 0)
			{
				foreach (var m in tree.ParserMessages)
				{
					Debug.WriteLine ("PARSER : {0} [{1}] {2}", m.Level, m.Location, m.Message);
				}
			}
			DebugNode (tree.Root, 0);
		}

		private void DebugNode(ParseTreeNode node, int level)
		{
			if (node == null)
			{
				return;
			}

			Debug.WriteLine(new string(' ', level) + node.ToString ());
			foreach (ParseTreeNode child in node.ChildNodes)
			{
				DebugNode(child, level + 1);
			}
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

