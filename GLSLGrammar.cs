using System.Collections.Generic;
using Irony.Parsing;
using System;
using System.Linq;

namespace GLSLSyntaxAST
{
	[Language("GLSL","0.1", "GLSL shading language")]
	public class GLSLGrammar : Grammar
	{
		public GLSLGrammar () : base(false)
		{		
			var multiLine = new CommentTerminal ("MULTI-LINE", "/*", "*/");
			var SingleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
			NonGrammarTerminals.Add(SingleLineComment);
			//Temporarily, treat preprocessor instructions like comments
			var ppInstruction = new CommentTerminal("ppInstruction", "#","\r", "\n");
			NonGrammarTerminals.Add(ppInstruction);

			this.NonGrammarTerminals.Add (multiLine);

			string RIGHT_PAREN = ")";
			string LEFT_BRACE = "{";
			string RIGHT_BRACE = "}";
			string SEMICOLON = ";";
			var COMMA = ToTerm(",", "COMMA");
			var QUESTION = ToTerm ("?", "QUESTION");
			var COLON = ToTerm (":", "COLON");
			var OR_OP = ToTerm ("||", "OR_OP");
			var XOR_OP = ToTerm ("^^", "XOR_OP");
			var AND_OP = ToTerm ("&&", "AND_OP");
			var VERTICAL_BAR = ToTerm ("|", "VERTICAL_BAR");
			var CARET = ToTerm ("^", "CARET");
			var AMPERSAND = ToTerm ("&", "AMPERSAND");
			var EQ_OP = ToTerm ("==","EQ_OP");
			var NE_OP = ToTerm ("!=","NE_OP");
			var LEFT_ANGLE = ToTerm ("<","LEFT_ANGLE");
			var RIGHT_ANGLE = ToTerm (">","RIGHT_ANGLE");
			var LE_OP = ToTerm ("<=","LE_OP");
			var GE_OP = ToTerm (">=", "GE_OP");
			var LEFT_OP = ToTerm ("<<","LEFT_OP");
			var RIGHT_OP = ToTerm (">>", "RIGHT_OP");
			var PLUS = ToTerm ("+", "PLUS");
			var DASH = ToTerm ("-", "DASH");
			var STAR = ToTerm ("*", "STAR");
			var SLASH = ToTerm ("/", "SLASH");
			var PERCENT = ToTerm ("%", "PERCENT");
			var INC_OP = ToTerm ("++", "INC_OP");
			var DEC_OP = ToTerm ("--", "DEC_OP");
			var BANG = ToTerm ("!", "BANG");
			var TILDE = ToTerm ("~", "TILDE");
			var EQUAL = ToTerm ("=", "EQUAL");
			var MUL_ASSIGN = ToTerm ("*=", "MUL_ASSIGN");
			var DIV_ASSIGN = ToTerm ("/=", "DIV_ASSIGN");
			var MOD_ASSIGN = ToTerm ("%=", "MOD_ASSIGN");
			var ADD_ASSIGN = ToTerm ("+=", "ADD_ASSIGN");
			var SUB_ASSIGN = ToTerm ("-=", "SUB_ASSIGN");
			var LEFT_ASSIGN = ToTerm ("<<=", "LEFT_ASSIGN");
			var RIGHT_ASSIGN = ToTerm (">>=", "RIGHT_ASSIGN");
			var AND_ASSIGN = ToTerm ("&=", "AND_ASSIGN");
			var XOR_ASSIGN = ToTerm ("^=", "XOR_ASSIGN");
			var OR_ASSIGN = ToTerm ("|=", "OR_ASSIGN");
			var IF_STM = ToTerm ("if", "IF");
			string LEFT_PAREN = "(";
			var ELSE = ToTerm ("else", "ELSE");
			var SWITCH_STM = ToTerm ("switch", "SWITCH");
			var CASE_STM = ToTerm ("case", "CASE");
			var DEFAULT_STM = ToTerm ("default", "DEFAULT");
			var WHILE = ToTerm ("while", "WHILE");
			var FOR_STM = ToTerm ("for", "FOR");
			var DO_STM = ToTerm ("do", "DO");
			IDENTIFIER = TerminalFactory.CreateCSharpIdentifier("IDENTIFIER");

			var VOID_STM = ToTerm ("void", "VOID");
			var FLOAT_TKN = ToTerm ("float", "FLOAT");
			var DOUBLE_TKN = ToTerm ("double", "DOUBLE");
			var INT_TKN = ToTerm ("int", "INT");
			INT_TKN.Precedence = 2;
			var UINT_TKN = ToTerm ("uint", "UINT");
			var BOOL_TKN = ToTerm ("bool", "BOOL");
			var VEC2 = ToTerm ("vec2", "VEC2");
			var VEC3 = ToTerm ("vec3", "VEC3");
			var VEC4 = ToTerm ("vec4", "VEC4");
			var DVEC2 = ToTerm ("dvec2", "DVEC2");
			var DVEC3 = ToTerm ("dvec3", "DVEC3");
			var DVEC4 = ToTerm ("dvec4", "DVEC4");
			var BVEC2 = ToTerm ("bvec2", "BVEC2");
			var BVEC3 = ToTerm ("bvec3", "BVEC3");
			var BVEC4 = ToTerm ("bvec4", "BVEC4");
			var IVEC2 = ToTerm ("ivec2", "IVEC2");
			var IVEC3 = ToTerm ("ivec3", "IVEC3");
			var IVEC4 = ToTerm ("ivec4", "IVEC4");
			var UVEC2 = ToTerm ("uvec2", "UVEC2");
			var UVEC3 = ToTerm ("uvec3", "UVEC3");
			var UVEC4 = ToTerm ("uvec4", "UVEC4");
			var MAT2 = ToTerm ("mat2", "MAT2");
			var MAT3 = ToTerm ("mat3", "MAT3");
			var MAT4 = ToTerm ("mat4", "MAT4");
			var MAT2X2 = ToTerm ("mat2x2", "MAT2X2");
			var MAT2X3 = ToTerm ("mat2x3", "MAT2X3");
			var MAT2X4 = ToTerm ("mat2x4", "MAT2X4");
			var MAT3X2 = ToTerm ("mat3x2", "MAT3X2");
			var MAT3X3 = ToTerm ("mat3x3", "MAT3X3");
			var MAT3X4 = ToTerm ("mat3x4", "MAT3X4");
			var MAT4X2 = ToTerm ("mat4x2", "MAT4X2");
			var MAT4X3 = ToTerm ("mat4x3", "MAT4X3");
			var MAT4X4 = ToTerm ("mat4x4", "MAT4X4");
			var DMAT2 = ToTerm ("dmat2", "DMAT2");
			var DMAT3 = ToTerm ("dmat3", "DMAT3");
			var DMAT4 = ToTerm ("dmat4", "DMAT4");
			var DMAT2X2 = ToTerm ("dmat2x2", "DMAT2X2");
			var DMAT2X3 = ToTerm ("dmat2x3", "DMAT2X3");
			var DMAT2X4 = ToTerm ("dmat2x4", "DMAT2X4");
			var DMAT3X2 = ToTerm ("dmat3x2", "DMAT3X2");
			var DMAT3X3 = ToTerm ("dmat3x3", "DMAT3X3");
			var DMAT3X4 = ToTerm ("dmat3x4", "DMAT3X4");
			var DMAT4X2 = ToTerm ("dmat4x2", "DMAT4X2");
			var DMAT4X3 = ToTerm ("dmat4x3", "DMAT4X3");
			var DMAT4X4 = ToTerm ("dmat4x4", "DMAT4X4");
			var ATOMIC_UINT = ToTerm ("atomic_uint", "ATOMIC_UINT");
			var SAMPLER1D = ToTerm ("sampler1d", "SAMPLER1D");
			var SAMPLER2D = ToTerm ("sampler2d", "SAMPLER2D");
			var SAMPLER3D = ToTerm ("sampler3d", "SAMPLER3D");
			var SAMPLERCUBE = ToTerm ("samplercube", "SAMPLERCUBE");
			var SAMPLER1DSHADOW = ToTerm ("sampler1dshadow", "SAMPLER1DSHADOW");
			var SAMPLER2DSHADOW = ToTerm ("sampler2dshadow", "SAMPLER2DSHADOW");
			var SAMPLERCUBESHADOW = ToTerm ("samplercubeshadow", "SAMPLERCUBESHADOW");
			var SAMPLER1DARRAY = ToTerm ("sampler1darray", "SAMPLER1DARRAY");
			var SAMPLER2DARRAY = ToTerm ("sampler2darray", "SAMPLER2DARRAY");
			var SAMPLER1DARRAYSHADOW = ToTerm ("sampler1darrayshadow", "SAMPLER1DARRAYSHADOW");
			var SAMPLER2DARRAYSHADOW = ToTerm ("sampler2darrayshadow", "SAMPLER2DARRAYSHADOW");
			var SAMPLERCUBEARRAY = ToTerm ("samplercubearray", "SAMPLERCUBEARRAY");
			var SAMPLERCUBEARRAYSHADOW = ToTerm ("samplercubearrayshadow", "SAMPLERCUBEARRAYSHADOW");
			var ISAMPLER1D = ToTerm ("isampler1d", "ISAMPLER1D");
			var ISAMPLER2D = ToTerm ("isampler2d", "ISAMPLER2D");
			var ISAMPLER3D = ToTerm ("isampler3d", "ISAMPLER3D");
			var ISAMPLERCUBE = ToTerm ("isamplercube", "ISAMPLERCUBE");
			var ISAMPLER1DARRAY = ToTerm ("isampler1darray", "ISAMPLER1DARRAY");
			var ISAMPLER2DARRAY = ToTerm ("isampler2darray", "ISAMPLER2DARRAY");
			var ISAMPLERCUBEARRAY = ToTerm ("isamplercubearray", "ISAMPLERCUBEARRAY");
			var USAMPLER1D = ToTerm ("usampler1d", "USAMPLER1D");
			var USAMPLER2D = ToTerm ("usampler2d", "USAMPLER2D");
			var USAMPLER3D = ToTerm ("usampler3d", "USAMPLER3D");
			var USAMPLERCUBE = ToTerm ("usamplercube", "USAMPLERCUBE");
			var USAMPLER1DARRAY = ToTerm ("usampler1darray", "USAMPLER1DARRAY");
			var USAMPLER2DARRAY = ToTerm ("usampler2darray", "USAMPLER2DARRAY");
			var USAMPLERCUBEARRAY = ToTerm ("usamplercubearray", "USAMPLERCUBEARRAY");
			var SAMPLER2DRECT = ToTerm ("sampler2drect", "SAMPLER2DRECT");
			var SAMPLER2DRECTSHADOW = ToTerm ("sampler2drectshadow", "SAMPLER2DRECTSHADOW");
			var ISAMPLER2DRECT = ToTerm ("isampler2drect", "ISAMPLER2DRECT");
			var USAMPLER2DRECT = ToTerm ("usampler2drect", "USAMPLER2DRECT");
			var SAMPLERBUFFER = ToTerm ("samplerbuffer", "SAMPLERBUFFER");
			var ISAMPLERBUFFER = ToTerm ("isamplerbuffer", "ISAMPLERBUFFER");
			var USAMPLERBUFFER = ToTerm ("usamplerbuffer", "USAMPLERBUFFER");
			var SAMPLER2DMS = ToTerm ("sampler2dms", "SAMPLER2DMS");
			var ISAMPLER2DMS = ToTerm ("isampler2dms", "ISAMPLER2DMS");
			var USAMPLER2DMS = ToTerm ("usampler2dms", "USAMPLER2DMS");
			var SAMPLER2DMSARRAY = ToTerm ("sampler2dmsarray", "SAMPLER2DMSARRAY");
			var ISAMPLER2DMSARRAY = ToTerm ("isampler2dmsarray", "ISAMPLER2DMSARRAY");
			var USAMPLER2DMSARRAY = ToTerm ("usampler2dmsarray", "USAMPLER2DMSARRAY");
			var IMAGE1D = ToTerm ("image1d", "IMAGE1D");
			var IIMAGE1D = ToTerm ("iimage1d", "IIMAGE1D");
			var UIMAGE1D = ToTerm ("uimage1d", "UIMAGE1D");
			var IMAGE2D = ToTerm ("image2d", "IMAGE2D");
			var IIMAGE2D = ToTerm ("iimage2d", "IIMAGE2D");
			var UIMAGE2D = ToTerm ("uimage2d", "UIMAGE2D");
			var IMAGE3D = ToTerm ("image3d", "IMAGE3D");
			var IIMAGE3D = ToTerm ("iimage3d", "IIMAGE3D");
			var UIMAGE3D = ToTerm ("uimage3d", "UIMAGE3D");
			var IMAGE2DRECT = ToTerm ("image2drect", "IMAGE2DRECT");
			var IIMAGE2DRECT = ToTerm ("iimage2drect", "IIMAGE2DRECT");
			var UIMAGE2DRECT = ToTerm ("uimage2drect", "UIMAGE2DRECT");
			var IMAGECUBE = ToTerm ("imagecube", "IMAGECUBE");
			var IIMAGECUBE = ToTerm ("iimagecube", "IIMAGECUBE");
			var UIMAGECUBE = ToTerm ("uimagecube", "UIMAGECUBE");
			var IMAGEBUFFER = ToTerm ("imagebuffer", "IMAGEBUFFER");
			var IIMAGEBUFFER = ToTerm ("iimagebuffer", "IIMAGEBUFFER");
			var UIMAGEBUFFER = ToTerm ("uimagebuffer", "UIMAGEBUFFER");
			var IMAGE1DARRAY  = ToTerm ("image1darray", "IMAGE1DARRAY"); 
			var IIMAGE1DARRAY = ToTerm ("iimage1darray", "IIMAGE1DARRAY");
			var UIMAGE1DARRAY = ToTerm ("uimage1darray", "UIMAGE1DARRAY");
			var IMAGE2DARRAY = ToTerm ("image2darray", "IMAGE2DARRAY");
			var IIMAGE2DARRAY = ToTerm ("iimage2darray", "IIMAGE2DARRAY");
			var UIMAGE2DARRAY = ToTerm ("uimage2darray", "UIMAGE2DARRAY");
			var IMAGECUBEARRAY = ToTerm ("imagecubearray", "IMAGECUBEARRAY");
			var IIMAGECUBEARRAY = ToTerm ("iimagecubearray", "IIMAGECUBEARRAY");
			var UIMAGECUBEARRAY = ToTerm ("uimagecubearray", "UIMAGECUBEARRAY");
			var IMAGE2DMS = ToTerm ("image2dms", "IMAGE2DMS");
			var IIMAGE2DMS = ToTerm ("iimage2dms", "IIMAGE2DMS");
			var UIMAGE2DMS = ToTerm ("uimage2dms", "UIMAGE2DMS");
			var IMAGE2DMSARRAY = ToTerm ("image2dmsarray", "IMAGE2DMSARRAY");
			var IIMAGE2DMSARRAY = ToTerm ("iimage2dmsarray", "IIMAGE2DMSARRAY");
			var UIMAGE2DMSARRAY = ToTerm ("uimage2dmsarray", "UIMAGE2DMSARRAY");
			var SAMPLEREXTERNALOES = ToTerm ("samplerexternaloes", "SAMPLEREXTERNALOES");

			TypesTerms = new HashSet<KeyTerm> (
				new []{
				VOID_STM
				,FLOAT_TKN
				,DOUBLE_TKN
				,INT_TKN
				,UINT_TKN
				,BOOL_TKN
				,VEC2
				,VEC3
				,VEC4
				,DVEC2
				,DVEC3
				,DVEC4
				,BVEC2
				,BVEC3
				,BVEC4
				,IVEC2
				,IVEC3
				,IVEC4
				,UVEC2
				,UVEC3
				,UVEC4
				,MAT2
				,MAT3
				,MAT4
				,MAT2X2
				,MAT2X3
				,MAT2X4
				,MAT3X2
				,MAT3X3
				,MAT3X4
				,MAT4X2
				,MAT4X3
				,MAT4X4
				,DMAT2
				,DMAT3
				,DMAT4
				,DMAT2X2
				,DMAT2X3
				,DMAT2X4
				,DMAT3X2
				,DMAT3X3
				,DMAT3X4
				,DMAT4X2
				,DMAT4X3
				,DMAT4X4
				,ATOMIC_UINT
				,SAMPLER1D
				,SAMPLER2D
				,SAMPLER3D
				,SAMPLERCUBE
				,SAMPLER1DSHADOW
				,SAMPLER2DSHADOW
				,SAMPLERCUBESHADOW
				,SAMPLER1DARRAY
				,SAMPLER2DARRAY
				,SAMPLER1DARRAYSHADOW
				,SAMPLER2DARRAYSHADOW
				,SAMPLERCUBEARRAY
				,SAMPLERCUBEARRAYSHADOW
				,ISAMPLER1D
				,ISAMPLER2D
				,ISAMPLER3D
				,ISAMPLERCUBE
				,ISAMPLER1DARRAY
				,ISAMPLER2DARRAY
				,ISAMPLERCUBEARRAY
				,USAMPLER1D
				,USAMPLER2D
				,USAMPLER3D
				,USAMPLERCUBE
				,USAMPLER1DARRAY
				,USAMPLER2DARRAY
				,USAMPLERCUBEARRAY
				,SAMPLER2DRECT
				,SAMPLER2DRECTSHADOW
				,ISAMPLER2DRECT
				,USAMPLER2DRECT
				,SAMPLERBUFFER
				,ISAMPLERBUFFER
				,USAMPLERBUFFER
				,SAMPLER2DMS
				,ISAMPLER2DMS
				,USAMPLER2DMS
				,SAMPLER2DMSARRAY
				,ISAMPLER2DMSARRAY
				,USAMPLER2DMSARRAY
				,IMAGE1D
				,IIMAGE1D
				,UIMAGE1D
				,IMAGE2D
				,IIMAGE2D
				,UIMAGE2D
				,IMAGE3D
				,IIMAGE3D
				,UIMAGE3D
				,IMAGE2DRECT
				,IIMAGE2DRECT
				,UIMAGE2DRECT
				,IMAGECUBE
				,IIMAGECUBE
				,UIMAGECUBE
				,IMAGEBUFFER
				,IIMAGEBUFFER
				,UIMAGEBUFFER
				,IMAGE1DARRAY
				,IIMAGE1DARRAY
				,UIMAGE1DARRAY
				,IMAGE2DARRAY
				,IIMAGE2DARRAY
				,UIMAGE2DARRAY
				,IMAGECUBEARRAY
				,IIMAGECUBEARRAY
				,UIMAGECUBEARRAY
				,IMAGE2DMS
				,IIMAGE2DMS
				,UIMAGE2DMS
				,IMAGE2DMSARRAY
				,IIMAGE2DMSARRAY
				,UIMAGE2DMSARRAY
				,SAMPLEREXTERNALOES
				});

			var TYPE_NAME = TerminalFactory.CreateCSharpNumber ("TYPE_NAME");
			TYPE_NAME.Precedence = 1;
			string LEFT_BRACKET = "[";
			string RIGHT_BRACKET = "]";
			var STRUCT_STM = ToTerm ("struct", "STRUCT");
			var CONTINUE = ToTerm ("continue", "CONTINUE");
			var BREAK_STM = ToTerm ("break", "BREAK");
			var RETURN_STM = ToTerm ("return", "RETURN");
			var DISCARD = ToTerm ("discard", "DISCARD");
			var PRECISION = ToTerm ("precision", "PRECISION");
			var HIGH_PRECISION = ToTerm ("highp", "HIGH_PRECISION");
			var MEDIUM_PRECISION = ToTerm ("mediump", "MEDIUM_PRECISION");
			var LOW_PRECISION = ToTerm ("lowp", "LOW_PRECISION");
			FIELD_SELECTION = TerminalFactory.CreateCSharpIdentifier ("FIELD_SELECTION");
			INTCONSTANT = new NumberLiteral ("INTCONSTANT", NumberOptions.AllowSign | NumberOptions.IntOnly);
			INTCONSTANT.Priority = 4;
			var DOT = new NonTerminal("DOT");
			DOT.Rule = ".";
			DOT.Precedence = 1;
			var UINTCONSTANT = new NumberLiteral ("UINTCONSTANT", NumberOptions.IntOnly);
			UINTCONSTANT.Priority = 3;
			FLOATCONSTANT = new NumberLiteral ("FLOATCONSTANT", NumberOptions.AllowSign | NumberOptions.AllowLetterAfter);
			FLOATCONSTANT.Priority = 2;
			FLOATCONSTANT.AddSuffix("f", TypeCode.Single);
			REMAINDER = new NumberLiteral ("REMAINDER", NumberOptions.NoDotAfterInt | NumberOptions.IntOnly | NumberOptions.AllowLetterAfter);
			REMAINDER.AddSuffix("f", TypeCode.Single);
			var DOUBLECONSTANT = new NumberLiteral ("DOUBLECONSTANT", NumberOptions.AllowSign);
			var FALSE_STM = ToTerm ("false", "FALSE_STM");
			var TRUE_STM = ToTerm ("true", "TRUE_STM");
			var ATTRIBUTE = ToTerm ("attribute", "ATTRIBUTE");
			var VARYING = ToTerm ("varying", "VARYING");
			InOutTerm = ToTerm ("inout", "INOUT");
			InTerm = ToTerm ("in", "IN");
			OutTerm = ToTerm ("out", "OUT");
			var CONST_STM = ToTerm ("CONST", "CONST");
			UNIFORM = ToTerm ("uniform", "UNIFORM");
			var CENTROID = ToTerm ("centroid", "CENTROID");
			var SAMPLE = ToTerm ("sample", "SAMPLE");
			var READONLY = ToTerm ("readonly", "READONLY");
			var WRITEONLY = ToTerm ("writeonly", "WRITEONLY");
			var RESTRICT = ToTerm ("restrict", "RESTRICT");
			var VOLATILE = ToTerm ("volatile", "VOLATILE");
			var COHERENT = ToTerm ("coherent", "COHERENT");
			var BUFFER = ToTerm ("buffer", "BUFFER");
			var SHARED = ToTerm ("shared", "SHARED");
			var PATCH = ToTerm ("patch", "PATCH");
			var SUBROUTINE = ToTerm ("subroutine", "SUBROUTINE");
			var LAYOUT = ToTerm ("layout", "LAYOUT");
			var SMOOTH = ToTerm ("smooth", "SMOOTH");
			var FLAT = ToTerm ("flat", "FLAT");
			var NOPERSPECTIVE = ToTerm ("noperspective", "NOPERSPECTIVE");
			var INVARIANT = ToTerm ("invariant", "INVARIANT");
			var PRECISE = ToTerm ("precise", "PRECISE");

			var translation_unit = new NonTerminal("translation_unit"); // CHECKED
			var external_declaration = new NonTerminal("external_declaration"); // CHECKED
			var function_definition = new NonTerminal ("function_definition"); // CHECKED
			Declaration = new NonTerminal ("declaration"); // CHECKED
			var function_prototype = new NonTerminal ("function_prototype"); // CHECKED
			var compound_statement_no_new_scope = new NonTerminal ("compound_statement_no_new_scope"); // CHECKED
			var function_declarator = new NonTerminal ("function_declarator"); // CHECKED
			var statement_list = new NonTerminal("statement_list"); // CHECKED
			var statement = new NonTerminal ("statement"); // CHECKED
			var compound_statement = new NonTerminal ("compound_statement"); // CHECKED
			var simple_statement = new NonTerminal ("simple_statement"); // CHECKED
			var declaration_statement = new NonTerminal ("declaration_statement"); // CHECKED
			var expression_statement = new NonTerminal ("expression_statement"); // CHECKED
			var expression = new NonTerminal ("expression"); // CHECKED
			var assignment_expression = new NonTerminal ("assignment_expression"); // CHECKED
			var conditional_expression = new NonTerminal ("conditional_expression"); // CHECKED
			var logical_or_expression = new NonTerminal ("logical_or_expression"); // CHECKED
			var logical_xor_expression = new NonTerminal ("logical_xor_expression"); // CHECKED
			var logical_and_expression = new NonTerminal ("logical_and_expression"); // CHECKED
			var inclusive_or_expression = new NonTerminal ("inclusive_or_expression"); // CHECKED
			var exclusive_or_expression = new NonTerminal ("exclusive_or_expression"); // CHECKED
			var and_expression = new NonTerminal ("and_expression"); // CHECKED
			var equality_expression = new NonTerminal ("equality_expression"); // CHECKED
			var relational_expression = new NonTerminal ("relational_expression"); // CHECKED
			var shift_expression = new NonTerminal ("shift_expression"); // CHECKED
			var additive_expression = new NonTerminal ("additive_expression"); // CHECKED
			var multiplicative_expression = new NonTerminal ("multiplicative_expression"); // CHECKED
			var unary_expression = new NonTerminal ("unary_expression"); // CHECKED
			var postfix_expression = new NonTerminal ("postfix_expression"); // CHECKED
			var unary_operator = new NonTerminal ("unary_operator"); // CHECKED
			var assignment_operator = new NonTerminal ("assignment_operator"); // CHECKED
			var selection_statement = new NonTerminal ("selection_statement"); // CHECKED
			var selection_rest_statement = new NonTerminal ("selection_rest_statement"); // CHECKED
			var statement_scoped = new NonTerminal ("statement_scoped"); // CHECKED
			var switch_statement = new NonTerminal ("switch_statement"); // CHECKED
			var switch_statement_list = new NonTerminal ("switch_statement_list"); // CHECKED
			var case_label = new NonTerminal ("case_label"); // CHECKED
			var iteration_statement = new NonTerminal ("iteration_statement"); // CHECKED
			var condition = new NonTerminal ("condition"); // CHECKED
			var statement_no_new_scope = new NonTerminal ("statement_no_new_scope"); // CHECKED
			var for_init_statement = new NonTerminal ("for_init_statement"); // CHECKED
			var for_rest_statement = new NonTerminal ("for_rest_statement"); // CHECKED
			var conditionopt = new NonTerminal ("conditionopt"); // CHECKED
			FullySpecifiedType = new NonTerminal ("fully_specified_type"); // CHECKED
			var initializer = new NonTerminal ("initializer"); // CHECKED 
			var initializer_list = new NonTerminal ("initializer_list"); // CHECKED
			var type_specifier = new NonTerminal ("type_specifier"); // CHECKED
			var single_type_qualifier = new NonTerminal ("single_type_qualifier"); // CHECKED
			var type_specifier_nonarray = new NonTerminal ("type_specifier_nonarray"); // CHECKED
			TypeQualifier = new NonTerminal ("type_qualifier"); // CHECKED
			ArraySpecifier = new NonTerminal ("array_specifier"); // CHECKED
			ConstantExpression = new NonTerminal ("constant_expression"); // CHECKED
			StructSpecifier = new NonTerminal ("struct_specifier"); // CHECKED 
			var struct_declaration_list = new NonTerminal ("struct_declaration_list"); // CHECKED 
			var struct_declaration = new NonTerminal ("struct_declaration"); // CHECKED 
			var struct_declarator_list = new NonTerminal ("struct_declarator_list"); // CHECKED 
			var struct_declarator = new NonTerminal ("struct_declarator"); // CHECKED 
			var jump_statement = new NonTerminal ("jump_statement"); // CHECKED 
			var init_declarator_list = new NonTerminal ("init_declarator_list"); // CHECKED 
			var precision_qualifier = new NonTerminal ("precision_qualifier"); // CHECKED 
			BlockStructure = new NonTerminal ("block_structure"); // CHECKED 
			SingleDeclaration = new NonTerminal ("single_declaration"); // CHECKED 
			var identifier_list = new NonTerminal ("identifier_list"); // CHECKED 
			var function_header = new NonTerminal ("function_header"); // CHECKED
			var function_header_with_parameters = new NonTerminal ("function_header_with_parameters"); // CHECKED 
			var parameter_declaration = new NonTerminal ("parameter_declaration"); // CHECKED 
			var parameter_declarator = new NonTerminal ("parameter_declarator"); // CHECKED 
			var parameter_type_specifier = new NonTerminal ("parameter_type_specifier"); // CHECKED 
			PrimaryExpression = new NonTerminal ("primary_expression"); // CHECKED 
			var integer_expression = new NonTerminal ("integer_expression"); // CHECKED 
			var function_call = new NonTerminal ("function_call"); // CHECKED 
			var function_call_or_method = new NonTerminal ("function_call_or_method"); // CHECKED 
			var function_call_generic = new NonTerminal ("function_call_generic"); // CHECKED 
			var function_call_header_no_parameters = new NonTerminal ("function_call_header_no_parameters"); // CHECKED
			var function_call_header_with_parameters = new NonTerminal ("function_call_header_with_parameters"); // CHECKED 
			var function_call_header = new NonTerminal ("function_call_header"); // CHECKED 
			var function_identifier = new NonTerminal ("function_identifier"); // CHECKED 
			var variable_identifier = new NonTerminal ("variable_identifier"); // CHECKED 
			var boolconstant = new NonTerminal ("boolconstant"); // CHECKED
			StorageQualifier = new NonTerminal ("storage_qualifier"); // CHECKED
			var type_name_list = new NonTerminal ("type_name_list"); // CHECKED 
			LayoutQualifier = new NonTerminal ("layout_qualifier"); // CHECKED
			LayoutQualifierIdList = new NonTerminal ("layout_qualifier_id_list"); // CHECKED
			LayoutQualifierId = new NonTerminal ("layout_qualifier_id"); // CHECKED
			var interpolation_qualifier = new NonTerminal ("interpolation_qualifier"); // CHECKED 
			var invariant_qualifier = new NonTerminal ("invariant_qualifier"); // CHECKED
			var precise_qualifier = new NonTerminal ("precise_qualifier"); // CHECKED

			var array_empty_bracket = new NonTerminal ("array_empty_bracket");
			var constant_inside_bracket = new NonTerminal ("constant_inside_bracket");
			var floating_number_value = new NonTerminal ("floating_number_value");

			// Place Rules Here
			this.Root = translation_unit;

			//translation_unit.Rule = external_declaration | translation_unit + external_declaration;
			translation_unit.Rule = MakeStarRule(translation_unit, external_declaration);

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

			identifier_list.Rule = COMMA + IDENTIFIER
				| identifier_list + COMMA + IDENTIFIER;

//			identifier_list.Rule = MakePlusRule (identifier_list, COMMA, IDENTIFIER);

			precision_qualifier.Rule = HIGH_PRECISION 
							| MEDIUM_PRECISION 
							| LOW_PRECISION;

			BlockStructure.Rule = TypeQualifier + IDENTIFIER + LEFT_BRACE + struct_declaration_list + RIGHT_BRACE;

			init_declarator_list.Rule = SingleDeclaration
				| init_declarator_list + COMMA + IDENTIFIER
				| init_declarator_list + COMMA + IDENTIFIER + ArraySpecifier
				| init_declarator_list + COMMA + IDENTIFIER + ArraySpecifier + EQUAL + initializer
				| init_declarator_list + COMMA + IDENTIFIER + EQUAL + initializer;

			SingleDeclaration.Rule = FullySpecifiedType
				| FullySpecifiedType + IDENTIFIER
				| FullySpecifiedType + IDENTIFIER + ArraySpecifier
				| FullySpecifiedType + IDENTIFIER + ArraySpecifier + EQUAL + initializer
				| FullySpecifiedType + IDENTIFIER + EQUAL + initializer;

			function_definition.Rule = function_prototype + compound_statement_no_new_scope;

			function_prototype.Rule = function_declarator + RIGHT_PAREN;

			function_declarator.Rule = function_header 	
				| function_header_with_parameters;

			function_header.Rule = FullySpecifiedType + IDENTIFIER + LEFT_PAREN;

			function_header_with_parameters.Rule = function_header + parameter_declaration
				| function_header_with_parameters + COMMA + parameter_declaration;

//			function_header_with_parameters.Rule = MakePlusRule (function_header_with_parameters, COMMA, parameter_declaration);

			parameter_declaration.Rule = TypeQualifier + parameter_declarator 	
				| parameter_declarator
				| TypeQualifier + parameter_type_specifier
				| parameter_type_specifier;

			parameter_declarator.Rule = type_specifier + IDENTIFIER
				| type_specifier + IDENTIFIER + ArraySpecifier;

			parameter_type_specifier.Rule = type_specifier;

			compound_statement_no_new_scope.Rule = LEFT_BRACE + RIGHT_BRACE | LEFT_BRACE + statement_list + RIGHT_BRACE;

			statement_list.Rule = MakeStarRule (statement_list, statement);
//				statement | statement_list + statement;
//			statement_list.Rule = MakePlusRule(statement_list, statement);

			statement.Rule = compound_statement | simple_statement;

			compound_statement.Rule = LEFT_BRACE + RIGHT_BRACE | LEFT_BRACE + statement_list + RIGHT_BRACE;

			simple_statement.Rule = declaration_statement
										| expression_statement
										| selection_statement
										| switch_statement
										| case_label
										| iteration_statement
										| jump_statement;

			declaration_statement.Rule = Declaration;

			expression_statement.Rule = SEMICOLON | expression + SEMICOLON;

			expression.Rule = assignment_expression | expression  + COMMA + assignment_expression;
			//expression.Rule = MakePlusRule(expression, COMMA, assignment_expression);

			assignment_expression.Rule = conditional_expression	| unary_expression  + assignment_operator + assignment_expression;

			conditional_expression.Rule = logical_or_expression | logical_or_expression  + QUESTION + expression + COLON + assignment_expression;

			logical_or_expression.Rule = logical_xor_expression | logical_or_expression + OR_OP + logical_xor_expression;

			logical_xor_expression.Rule = logical_and_expression | logical_xor_expression + XOR_OP + logical_and_expression;

			logical_and_expression.Rule = inclusive_or_expression | logical_and_expression + AND_OP + inclusive_or_expression;

			inclusive_or_expression.Rule = exclusive_or_expression | inclusive_or_expression + VERTICAL_BAR + exclusive_or_expression;

			exclusive_or_expression.Rule = and_expression | exclusive_or_expression + CARET + and_expression;

			and_expression.Rule = equality_expression | and_expression + AMPERSAND +  equality_expression;
			//and_expression = MakePlusRule(and_expression, AMPERSAND, equality_expression);

			equality_expression.Rule = relational_expression
						| equality_expression + EQ_OP + relational_expression
						| equality_expression + NE_OP + relational_expression;

			relational_expression.Rule = shift_expression 
				| relational_expression + LEFT_ANGLE + shift_expression 
				| relational_expression + RIGHT_ANGLE + shift_expression  
				| relational_expression + LE_OP + shift_expression  
				| relational_expression + GE_OP + shift_expression;

			shift_expression.Rule =  additive_expression 
					| shift_expression + LEFT_OP + additive_expression 
						| shift_expression + RIGHT_OP + additive_expression;

		//	shift_expression.Rule = additive_expression
		//		| MakePlusRule (shift_expression, LEFT_OP, additive_expression)
	//			| MakePlusRule (shift_expression, RIGHT_OP, additive_expression);

			additive_expression.Rule = multiplicative_expression 
						| additive_expression + PLUS + multiplicative_expression 
						| additive_expression + DASH  + multiplicative_expression;

			multiplicative_expression.Rule = unary_expression
				| multiplicative_expression + STAR + unary_expression
				| multiplicative_expression + SLASH + unary_expression
				| multiplicative_expression + PERCENT + unary_expression;

			unary_expression.Rule = 
				 floating_number_value
				| postfix_expression				
				| INC_OP + unary_expression
				| DEC_OP + unary_expression
				| unary_operator + unary_expression;

			floating_number_value.Rule = INTCONSTANT + DOT + REMAINDER;

			postfix_expression.Rule = PrimaryExpression
				| postfix_expression + LEFT_BRACKET + integer_expression + RIGHT_BRACKET
				| FLOATCONSTANT
				| postfix_expression + DOT + INTCONSTANT // for floating values
				| function_call
				| postfix_expression + DOT + FIELD_SELECTION
				| postfix_expression + INC_OP
				| postfix_expression + DEC_OP;

			integer_expression.Rule = expression;

			function_call.Rule = function_call_or_method;

			function_call_or_method.Rule = function_identifier + LEFT_PAREN + function_call_generic + RIGHT_PAREN;

			function_call_generic.Rule = VOID_STM
				| function_call_header_with_parameters;

//			function_call_header_with_parameters.Rule = function_call_header + assignment_expression
//				| function_call_header_with_parameters + COMMA + assignment_expression;
			function_call_header_with_parameters.Rule = MakeStarRule(function_call_header_with_parameters, COMMA, assignment_expression);

			function_call_header.Rule = function_identifier + LEFT_PAREN;

			function_identifier.Rule = type_specifier
				| postfix_expression;

//			function_call_header_no_parameters.Rule = function_call_header + VOID_STM
//				| function_call_header;

			PrimaryExpression.Rule = variable_identifier
				| INTCONSTANT
				| UINTCONSTANT
				| FLOATCONSTANT
				| DOUBLECONSTANT
				| boolconstant
				| LEFT_PAREN + expression + RIGHT_PAREN;

			boolconstant.Rule = TRUE_STM | FALSE_STM;

			variable_identifier.Rule = IDENTIFIER;

			unary_operator.Rule = PLUS
					| DASH
					| BANG
					| TILDE;

			assignment_operator.Rule = EQUAL
					| MUL_ASSIGN 
					| DIV_ASSIGN
					| MOD_ASSIGN 
					| ADD_ASSIGN 
					| SUB_ASSIGN 
					| LEFT_ASSIGN 
					| RIGHT_ASSIGN 
					| AND_ASSIGN 
					| XOR_ASSIGN 
					| OR_ASSIGN;

			selection_statement.Rule = IF_STM + LEFT_PAREN + expression + RIGHT_PAREN  + selection_rest_statement;

			selection_rest_statement.Rule = statement_scoped + ELSE + statement_scoped	
						| statement_scoped;

			statement_scoped.Rule =  compound_statement	| simple_statement;

			switch_statement.Rule = SWITCH_STM  + LEFT_PAREN + expression + RIGHT_PAREN + LEFT_BRACE + switch_statement_list + RIGHT_BRACE;

			switch_statement_list.Rule = Empty | statement_list;				

			case_label.Rule = CASE_STM + expression + COLON 
				| DEFAULT_STM + COLON;

			iteration_statement.Rule = WHILE + LEFT_PAREN + condition + RIGHT_PAREN + statement_no_new_scope	
				| DO_STM + statement + WHILE + LEFT_PAREN + expression + RIGHT_PAREN + SEMICOLON
				| FOR_STM +  LEFT_PAREN + for_init_statement + for_rest_statement + RIGHT_PAREN + statement_no_new_scope;

			condition.Rule = expression 
					| FullySpecifiedType + IDENTIFIER + EQUAL + initializer;

			statement_no_new_scope.Rule = compound_statement_no_new_scope
						| simple_statement;

			for_init_statement.Rule = expression_statement
				| declaration_statement;

			for_rest_statement.Rule = conditionopt  + SEMICOLON
					| conditionopt + SEMICOLON + expression;

			conditionopt.Rule = condition 
					| Empty;

			initializer.Rule = assignment_expression 
				| LEFT_BRACE + initializer_list + RIGHT_BRACE
				| LEFT_BRACE + initializer_list + COMMA + RIGHT_BRACE;

			initializer_list.Rule =  initializer 
				| initializer_list + COMMA + initializer;

			FullySpecifiedType.Rule = type_specifier 
				| TypeQualifier + type_specifier;

			type_specifier.Rule = type_specifier_nonarray
				| type_specifier_nonarray + ArraySpecifier;

//			array_specifier.Rule = LEFT_BRACKET + RIGHT_BRACKET
//				| LEFT_BRACKET + constant_expression + RIGHT_BRACKET
//				| array_specifier + LEFT_BRACKET + RIGHT_BRACKET
//				| array_specifier + LEFT_BRACKET + constant_expression + RIGHT_BRACKET;

			ArraySpecifier.Rule = 
				  MakePlusRule (ArraySpecifier, array_empty_bracket)
				| MakePlusRule (ArraySpecifier, constant_inside_bracket);

			// two additional rules
			array_empty_bracket.Rule = LEFT_BRACKET + RIGHT_BRACKET;
			constant_inside_bracket.Rule = LEFT_BRACKET + ConstantExpression + RIGHT_BRACKET;

			ConstantExpression.Rule = conditional_expression;

//			type_qualifier.Rule = single_type_qualifier
//				| type_qualifier + single_type_qualifier;

			TypeQualifier.Rule = MakePlusRule(TypeQualifier, single_type_qualifier);

			single_type_qualifier.Rule = StorageQualifier
					| LayoutQualifier
					| precision_qualifier
					| interpolation_qualifier
					| invariant_qualifier
					| precise_qualifier;

			LayoutQualifier.Rule = LAYOUT + LEFT_PAREN + LayoutQualifierIdList + RIGHT_PAREN;

//			layout_qualifier_id_list.Rule =  layout_qualifier_id
//				| layout_qualifier_id_list + COMMA + layout_qualifier_id;

			LayoutQualifierIdList.Rule = MakePlusRule (LayoutQualifierIdList, COMMA, LayoutQualifierId);

			LayoutQualifierId.Rule = IDENTIFIER 
					| IDENTIFIER + EQUAL  + ConstantExpression
					| SHARED;

			StorageQualifier.Rule = CONST_STM 
				| ATTRIBUTE 
				| VARYING 
				| InOutTerm 
				| InTerm
				| OutTerm
				| CENTROID
				| PATCH
				| SAMPLE
				| UNIFORM
				| BUFFER
				| SHARED
				| COHERENT
				| VOLATILE
				| RESTRICT
				| READONLY
				| WRITEONLY
				| SUBROUTINE
				| SUBROUTINE + LEFT_PAREN + type_name_list + RIGHT_PAREN;

			interpolation_qualifier.Rule = SMOOTH
				| FLAT 
				| NOPERSPECTIVE;

			invariant_qualifier.Rule = INVARIANT;

			precise_qualifier.Rule = PRECISE;

			type_name_list.Rule = MakePlusRule (type_name_list, COMMA, TYPE_NAME);

			type_specifier_nonarray.Rule = VOID_STM 
				| FLOAT_TKN 
				| DOUBLE_TKN 
				| INT_TKN 
				| UINT_TKN 
				| BOOL_TKN 
				| VEC2 
				| VEC3 
				| VEC4 
				| DVEC2 
				| DVEC3 
				| DVEC4 
				| BVEC2 
				| BVEC3 
				| BVEC4
				| IVEC2
				| IVEC3
				| IVEC4
				| UVEC2
				| UVEC3
				| UVEC4
				| MAT2
				| MAT3
				| MAT4
				| MAT2X2
				| MAT2X3
				| MAT2X4
				| MAT3X2
				| MAT3X3
				| MAT3X4
				| MAT4X2
				| MAT4X3
				| MAT4X4
				| DMAT2
				| DMAT3
				| DMAT4
				| DMAT2X2
				| DMAT2X3
				| DMAT2X4
				| DMAT3X2
				| DMAT3X3
				| DMAT3X4
				| DMAT4X2
				| DMAT4X3
				| DMAT4X4
				| ATOMIC_UINT
				| SAMPLER1D
				| SAMPLER2D
				| SAMPLER3D
				| SAMPLERCUBE
				| SAMPLER1DSHADOW
				| SAMPLER2DSHADOW
				| SAMPLERCUBESHADOW
				| SAMPLER1DARRAY
				| SAMPLER2DARRAY
				| SAMPLER1DARRAYSHADOW
				| SAMPLER2DARRAYSHADOW
				| SAMPLERCUBEARRAY
				| SAMPLERCUBEARRAYSHADOW
				| ISAMPLER1D
				| ISAMPLER2D
				| ISAMPLER3D
				| ISAMPLERCUBE
				| ISAMPLER1DARRAY
				| ISAMPLER2DARRAY
				| ISAMPLERCUBEARRAY
				| USAMPLER1D
				| USAMPLER2D
				| USAMPLER3D
				| USAMPLERCUBE
				| USAMPLER1DARRAY
				| USAMPLER2DARRAY
				| USAMPLERCUBEARRAY
				| SAMPLER2DRECT
				| SAMPLER2DRECTSHADOW
				| ISAMPLER2DRECT
				| USAMPLER2DRECT
				| SAMPLERBUFFER
				| ISAMPLERBUFFER
				| USAMPLERBUFFER
				| SAMPLER2DMS
				| ISAMPLER2DMS
				| USAMPLER2DMS
				| SAMPLER2DMSARRAY
				| ISAMPLER2DMSARRAY
				| USAMPLER2DMSARRAY
				| IMAGE1D
				| IIMAGE1D
				| UIMAGE1D
				| IMAGE2D
				| IIMAGE2D
				| UIMAGE2D
				| IMAGE3D
				| IIMAGE3D
				| UIMAGE3D
				| IMAGE2DRECT
				| IIMAGE2DRECT
				| UIMAGE2DRECT
				| IMAGECUBE
				| IIMAGECUBE
				| UIMAGECUBE
				| IMAGEBUFFER
				| IIMAGEBUFFER
				| UIMAGEBUFFER
				| IMAGE1DARRAY
				| IIMAGE1DARRAY
				| UIMAGE1DARRAY
				| IMAGE2DARRAY
				| IIMAGE2DARRAY
				| UIMAGE2DARRAY
				| IMAGECUBEARRAY
				| IIMAGECUBEARRAY
				| UIMAGECUBEARRAY
				| IMAGE2DMS
				| IIMAGE2DMS
				| UIMAGE2DMS
				| IMAGE2DMSARRAY
				| IIMAGE2DMSARRAY
				| UIMAGE2DMSARRAY
				| SAMPLEREXTERNALOES
				| StructSpecifier
				| TYPE_NAME;

			StructSpecifier.Rule = STRUCT_STM + IDENTIFIER + LEFT_BRACE + struct_declaration_list + RIGHT_BRACE
				| STRUCT_STM + LEFT_BRACE + struct_declaration_list + RIGHT_BRACE;

//			struct_declaration_list.Rule = struct_declaration
//				| struct_declaration_list + struct_declaration;

			struct_declaration_list.Rule = MakePlusRule (struct_declaration_list, struct_declaration);

			struct_declaration.Rule = type_specifier + struct_declarator_list + SEMICOLON 
				| TypeQualifier + type_specifier + struct_declarator_list  + SEMICOLON;

//			struct_declarator_list.Rule = struct_declarator
//				| struct_declarator_list + COMMA + struct_declarator;

			struct_declarator_list.Rule = MakePlusRule (struct_declarator_list, COMMA, struct_declarator);

			struct_declarator.Rule = IDENTIFIER 
				| IDENTIFIER + ArraySpecifier;

			jump_statement.Rule = CONTINUE + SEMICOLON
				| BREAK_STM + SEMICOLON
				| RETURN_STM + SEMICOLON
				| RETURN_STM + expression + SEMICOLON
				| DISCARD + SEMICOLON;

			this.MarkPunctuation (LEFT_BRACKET, RIGHT_BRACKET, LEFT_BRACE, RIGHT_BRACE, LEFT_PAREN, RIGHT_PAREN, SEMICOLON);

			// This removes certain nodes from appearing in the AST tree later
			this.MarkTransient(
//				translation_unit,
//				external_declaration,
//				//function_definition,
//				//declaration,
//				//function_prototype,
//				compound_statement_no_new_scope,
//				function_declarator,
//				statement_list,
//				statement,
//				compound_statement,
//				simple_statement,
//				//declaration_statement,
//				expression_statement,
//				expression,
//				assignment_expression,
				conditional_expression,
				logical_or_expression,
				logical_xor_expression,
				logical_and_expression,
				inclusive_or_expression,
				exclusive_or_expression,
				and_expression,
				equality_expression,
				relational_expression,
				shift_expression,
				additive_expression,
				multiplicative_expression,
				unary_expression,
				postfix_expression,
				unary_operator,
//				assignment_operator,
//				selection_statement,
//				selection_rest_statement,
//				statement_scoped,
//				switch_statement,
//				switch_statement_list,
//				case_label,
//				iteration_statement,
//				//condition,
//				statement_no_new_scope,
//				for_init_statement,
//				for_rest_statement,
//				conditionopt,
//				fully_specified_type,
//				initializer,
//				initializer_list,
				type_specifier,
				single_type_qualifier,
				type_specifier_nonarray,
//				//type_qualifier,
				//array_specifier,
//				constant_expression,
//				struct_specifier,
//				struct_declaration_list, 
//				//struct_declaration, 
				struct_declarator_list,
//				struct_declarator, 
//				jump_statement, 
				init_declarator_list, 
//				precision_qualifier, 
//				block_structure, 
//				single_declaration, 
//				identifier_list,
//				//function_header,
//				function_header_with_parameters,
//				parameter_declaration,
//				parameter_declarator,
//				parameter_type_specifier,
				PrimaryExpression,
//				integer_expression,
////				function_call,
				function_call_generic,
//				function_call_header_with_parameters,
//				function_call_header_no_parameters
////				function_call_header,
////				function_identifier,
//				//variable_identifier,
//				boolconstant,
//				type_name_list,
//				storage_qualifier,
//				layout_qualifier_id,
//				invariant_qualifier
//				array_empty_bracket
				constant_inside_bracket,
				//floating_number_value,
				function_call_or_method,
				DOT
			);

		}

		public NonTerminal Declaration {
			get;
			private set;
		}

		public NonTerminal StorageQualifier {
			get;
			set;
		}

		NonTerminal PrimaryExpression {
			get;
			set;
		}

		NumberLiteral FLOATCONSTANT {
			get;
			set;
		}

		public NumberLiteral INTCONSTANT {
			get;
			private set;
		}

		public KeyTerm UNIFORM {
			get;
			private set;
		}

		public NonTerminal ArraySpecifier {
			get;
			private set;
		}

		public NumberLiteral REMAINDER {
			get;
			private set;
		}

		IdentifierTerminal FIELD_SELECTION {
			get;
			set;
		}

		public NonTerminal BlockStructure { get; private set; }

		public KeyTerm InOutTerm { get; private set;}
		public KeyTerm InTerm {	get; private set; }
		public KeyTerm OutTerm { get; private set; }
		public NonTerminal SingleDeclaration { get; private set; }
		public HashSet<KeyTerm> TypesTerms {get; private set; }
		public NonTerminal ConstantExpression { get; private set; }
		public NonTerminal LayoutQualifier { get; private set; }
		public NonTerminal LayoutQualifierIdList {	get; private set; }
		public NonTerminal LayoutQualifierId { get; private set; }
		public NonTerminal FullySpecifiedType {	get; private set; }
		public NonTerminal StructSpecifier { get; private set; }
		public NonTerminal TypeQualifier { get; private set;}
		public IdentifierTerminal IDENTIFIER { get; private set; }
	}
}

