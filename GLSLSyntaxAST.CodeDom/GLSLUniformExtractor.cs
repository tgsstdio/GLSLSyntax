using System;
using System.IO;
using Irony.Parsing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Text;

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

		private bool IsUniform (StructInfo info, ParseTreeNode parent)
		{
			ParseTreeNode typeQualifier = parent.ChildNodes.Find (p => p.Term == mLanguage.TypeQualifier);
			if (typeQualifier == null )
			{
				return false;
			}

			ParseTreeNode storageQualifier = typeQualifier.ChildNodes.Find (p => p.Term == mLanguage.StorageQualifier);
			if (storageQualifier == null)
			{
				return false;
			}

			ParseTreeNode uniformTag = storageQualifier.ChildNodes.Find (p => p.Term == mLanguage.UNIFORM);
			if (uniformTag == null)
			{
				return false;
			}

			info.Layout = new LayoutInformation ();
			ExtractLayout (info.Layout, typeQualifier);					
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

		bool ExtractUniformArrayDetails (StructInfo info, ParseTreeNode node)
		{
			var arraySpecifier = node.ChildNodes.Find (p => p.Term == mLanguage.ArraySpecifier);
			if (arraySpecifier == null)
				return false;
			
			var uniformName = node.ChildNodes.Find (p => p.Term == mLanguage.IDENTIFIER);
			if (uniformName == null)
			{
				return false;
			}

			var constNode = arraySpecifier.ChildNodes.Find (p => p.Term == mLanguage.ConstantExpression);
			if (constNode == null)
			{
				return false;
			}

			var uniform = new StructMember ();
			uniform.Name = uniformName.Token.ValueString;
			uniform.ArrayDetails = new ArraySpecification ();
			uniform.ArrayDetails.StructType = info;
			uniform.ArrayDetails.ArraySize = (int) constNode.ChildNodes[0].Token.Value;

			var key = uniform.Name;
			if (!mUniforms.ContainsKey (key))
			{
				mUniforms.Add (key, uniform);
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

			if (node.Term == mLanguage.Declaration)
			{
				var parent = node.ChildNodes.Find (p => p.Term == mLanguage.BlockStructure);
				if (parent == null)
				{
					return 0;
				}

				var info = new StructInfo ();

				// first child is uniform keyword
				if (!IsUniform (info, parent))
				{
					return 0;
				}

				var typeName = parent.ChildNodes.Find (p => p.Term == mLanguage.IDENTIFIER);
				if (typeName == null)
				{
					return 0;
				}

				// second child is struct type name
				info.ExtractName (typeName);

				// third child is list of member inside
				if (!ExtractMembers (info, parent.ChildNodes [2]))
				{
					return 0;
				}

				int changes = 0;

				var key = info.Name;
				if (!mBlocks.ContainsKey (key))
				{
					mBlocks.Add (key, info);
					++changes;
				}

				if (ExtractUniformArrayDetails(info, node))
				{
					++changes;
				}

				return changes;
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
			temp.ExtractName (specifier.ChildNodes [1]);

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

					if (typeQualifier == null)
					{
						return 0;
					}

					if (specifier != null)
					{
						var temp = new StructInfo ();
						temp.Layout = new LayoutInformation (); 
						ExtractLayout (temp.Layout, typeQualifier);					
						return ExtractStructMembers (temp, specifier);
					}

					ParseTreeNode storageQualifier = typeQualifier.ChildNodes.Find (p => p.Term == mLanguage.StorageQualifier);
					if (storageQualifier == null)
					{
						return 0;
					}

					var inDirection = storageQualifier.ChildNodes.Find (p => p.Term == mLanguage.InTerm);

					var outDirection = storageQualifier.ChildNodes.Find (p => p.Term == mLanguage.OutTerm);

					var inoutDirection = storageQualifier.ChildNodes.Find (p => p.Term == mLanguage.InOutTerm);

					if (inDirection == null && outDirection == null && inoutDirection == null)
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

		public string ExpressTree (string code)
		{
			var builder = new StringBuilder ();
			var tree = mCompiler.Parse (code);
			ExpressTreeNode (builder, tree.Root, 0);
			return builder.ToString ();
		}

		private void ExpressTreeNode(StringBuilder builder, ParseTreeNode node, int level)
		{
			if (node == null)
			{
				return;
			}

			builder.Append (new string (' ', level));
			builder.Append (node.ToString ());
			builder.Append ("\n");
			foreach (ParseTreeNode child in node.ChildNodes)
			{
				ExpressTreeNode(builder, child, level + 1);
			}
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

