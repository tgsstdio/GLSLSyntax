using System;

namespace GLSLSyntaxAST.CodeDom
{
	public enum EvalPrecedence : int
	{
		MIN_PRECEDENCE = 0,
		COND, LOGOR, LOGAND, OR, XOR, AND, EQUAL, RELATION, SHIFT, ADD, MUL, UNARY,
		MAX_PRECEDENCE		
	}
}
