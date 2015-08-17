using System;
using Irony.Parsing;

namespace GLSLSyntaxAST
{
	[Language("GLSL","0.1", "GLSL shading language")]	
	public class CorrectGLSLGrammar: Grammar
	{
		public CorrectGLSLGrammar () : base(false)
		{
			var multiLine = new CommentTerminal ("MULTI-LINE", "/*", "*/");
			var singleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");

			//Temporarily, treat preprocessor instructions like comments
			var ppInstruction = new CommentTerminal("ppInstruction", "#","\r", "\n");
			this.NonGrammarTerminals.Add(singleLineComment);
			this.NonGrammarTerminals.Add(ppInstruction);
			this.NonGrammarTerminals.Add (multiLine);

			string LEFT_PAREN = "(";
			string RIGHT_PAREN = ")";
			string LEFT_BRACE = "{";
			string RIGHT_BRACE = "}";
			string SEMICOLON = ";";
			var PRECISION = ToTerm ("precision", "PRECISION");
			IDENTIFIER = TerminalFactory.CreateCSharpIdentifier("IDENTIFIER");
			var COMMA = ToTerm(",", "COMMA");
			var EQUAL = ToTerm ("=", "EQUAL");

			var translation_unit = new NonTerminal("translation_unit"); // done
			var external_declaration = new NonTerminal("external_declaration"); // done
			var function_definition = new NonTerminal ("function_definition"); // done
			Declaration = new NonTerminal ("declaration"); // done
			var function_prototype = new NonTerminal ("function_prototype"); // done
			var init_declarator_list = new NonTerminal ("init_declarator_list"); // done
			BlockStructure = new NonTerminal ("block_structure");
			var precision_qualifier = new NonTerminal ("precision_qualifier");
			TypeQualifier = new NonTerminal ("type_qualifier");
			var type_specifier = new NonTerminal ("type_specifier");
			ArraySpecifier = new NonTerminal ("array_specifier");
			var identifier_list = new NonTerminal ("identifier_list");
			var compound_statement_no_new_scope = new NonTerminal ("compound_statement_no_new_scope"); // done
			var statement_list = new NonTerminal("statement_list"); 
			var function_declarator = new NonTerminal ("function_declarator"); // done
			var function_header = new NonTerminal ("function_header"); // done
			var function_header_with_parameters = new NonTerminal ("function_header_with_parameters"); // done
			var parameter_declaration = new NonTerminal ("parameter_declaration");
			FullySpecifiedType = new NonTerminal ("fully_specified_type");
			SingleDeclaration = new NonTerminal ("single_declaration");
			var initializer = new NonTerminal ("initializer"); // done 
			var assignment_expression = new NonTerminal ("assignment_expression"); // done
			var initializer_list = new NonTerminal ("initializer_list");
			var conditional_expression = new NonTerminal ("conditional_expression");
			var unary_expression = new NonTerminal ("unary_expression");
			var assignment_operator = new NonTerminal ("assignment_operator");
			var struct_declaration_list = new NonTerminal ("struct_declaration_list"); // done
			var struct_declaration = new NonTerminal ("struct_declaration");  // done
			var struct_declarator_list = new NonTerminal ("struct_declarator_list"); // done
			var struct_declarator = new NonTerminal ("struct_declarator");  

			// Place Rules Here
			this.Root = translation_unit;

			//translation_unit.Rule = external_declaration | translation_unit + external_declaration;
			translation_unit.Rule = Eof | MakePlusRule(translation_unit, external_declaration);

			external_declaration.Rule =  function_definition | Declaration;

			Declaration.Rule = function_prototype + SEMICOLON 
				| init_declarator_list + SEMICOLON 
				| PRECISION + precision_qualifier + type_specifier + SEMICOLON
				| BlockStructure + SEMICOLON
				| BlockStructure + IDENTIFIER + SEMICOLON
				| BlockStructure + IDENTIFIER + ArraySpecifier + SEMICOLON
				| TypeQualifier + SEMICOLON 
				| TypeQualifier + IDENTIFIER + SEMICOLON
				| TypeQualifier + IDENTIFIER + identifier_list + SEMICOLON;

			function_definition.Rule = function_prototype + compound_statement_no_new_scope;

			compound_statement_no_new_scope.Rule = LEFT_BRACE + RIGHT_BRACE | LEFT_BRACE + statement_list + RIGHT_BRACE;

			function_prototype.Rule = function_declarator + RIGHT_PAREN;

			function_declarator.Rule = function_header 	
				| function_header_with_parameters;

			function_header.Rule = FullySpecifiedType + IDENTIFIER + LEFT_PAREN;

			init_declarator_list.Rule = SingleDeclaration
				| init_declarator_list + COMMA + IDENTIFIER
				| init_declarator_list + COMMA + IDENTIFIER + ArraySpecifier
				| init_declarator_list + COMMA + IDENTIFIER + ArraySpecifier + EQUAL + initializer
				| init_declarator_list + COMMA + IDENTIFIER + EQUAL + initializer;

			function_header_with_parameters.Rule = function_header + parameter_declaration
				| function_header_with_parameters + COMMA + parameter_declaration;

			initializer.Rule = assignment_expression 
				| LEFT_BRACE + initializer_list + RIGHT_BRACE
				| LEFT_BRACE + initializer_list + COMMA + RIGHT_BRACE;

			assignment_expression.Rule = conditional_expression	| unary_expression  + assignment_operator + assignment_expression;

			BlockStructure.Rule = TypeQualifier + IDENTIFIER + LEFT_BRACE + struct_declaration_list + RIGHT_BRACE;

			struct_declaration_list.Rule = MakePlusRule (struct_declaration_list, struct_declaration);

			struct_declaration.Rule = type_specifier + struct_declarator_list + SEMICOLON 
				| TypeQualifier + type_specifier + struct_declarator_list  + SEMICOLON;

			struct_declarator_list.Rule = MakePlusRule (struct_declarator_list, COMMA, struct_declarator);
		}

		NonTerminal FullySpecifiedType {
			get;
			set;
		}

		NonTerminal Declaration {
			get;
			set;
		}

		IdentifierTerminal IDENTIFIER {
			get;
			set;
		}

		NonTerminal BlockStructure {
			get;
			set;
		}

		NonTerminal TypeQualifier {
			get;
			set;
		}

		NonTerminal ArraySpecifier {
			get;
			set;
		}

		NonTerminal SingleDeclaration {
			get;
			set;
		}
	}
}

