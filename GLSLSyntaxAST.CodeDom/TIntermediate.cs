using System;
using System.Collections.Specialized;

namespace GLSLSyntaxAST.CodeDom
{
	public class TIntermediate
	{
		private readonly StringCollection requestedExtensions;		
		public TIntermediate ()
		{
			requestedExtensions = new StringCollection ();
		}

		public void addRequestedExtension(string extension)
		{
			requestedExtensions.Add(extension); 
		}

		private int version;
		public void setVersion(int v)
		{
			version = v; 
		}

		private Profile profile;
		public void setProfile(Profile p)
		{
			profile = p; 
		}
	}
}

//
//	//
//	// Set of helper functions to help parse and build the tree.
//	//
//	class TIntermediate {
//		public:
//		explicit TIntermediate(EShLanguage l, int v = 0, EProfile p = ENoProfile) : language(l), treeRoot(0), profile(p), version(v), 
//		numMains(0), numErrors(0), recursive(false),
//		invocations(0), vertices(0), inputPrimitive(ElgNone), outputPrimitive(ElgNone), pixelCenterInteger(false), originUpperLeft(false),
//		vertexSpacing(EvsNone), vertexOrder(EvoNone), pointMode(false), earlyFragmentTests(false), depthLayout(EldNone), xfbMode(false)
//		{
//			localSize[0] = 1;
//			localSize[1] = 1;
//			localSize[2] = 1;
//			xfbBuffers.resize(TQualifier::layoutXfbBufferEnd);
//		}
//
//
//		bool postProcess(TIntermNode*, EShLanguage);
//		void output(TInfoSink&, bool tree);
//		void removeTree();
//
//		void setVersion(int v) { version = v; }
//		int getVersion() const { return version; }
//		void setProfile(EProfile p) { profile = p; }
//		EProfile getProfile() const { return profile; }
//		EShLanguage getStage() const { return language; }

//		const std::set<std::string>& getRequestedExtensions() const { return requestedExtensions; }
//
//		void setTreeRoot(TIntermNode* r) { treeRoot = r; }
//		TIntermNode* getTreeRoot() const { return treeRoot; }
//		void addMainCount() { ++numMains; }
//		int getNumMains() const { return numMains; }
//		int getNumErrors() const { return numErrors; }
//		bool isRecursive() const { return recursive; }
//
//		TIntermSymbol* addSymbol(int Id, const TString&, const TType&, const TSourceLoc&);
//		TIntermSymbol* addSymbol(const TVariable&, const TSourceLoc&);
//		TIntermTyped* addConversion(TOperator, const TType&, TIntermTyped*) const;
//		TIntermTyped* addBinaryMath(TOperator, TIntermTyped* left, TIntermTyped* right, TSourceLoc);
//		TIntermTyped* addAssign(TOperator op, TIntermTyped* left, TIntermTyped* right, TSourceLoc);
//		TIntermTyped* addIndex(TOperator op, TIntermTyped* base, TIntermTyped* index, TSourceLoc);
//		TIntermTyped* addUnaryMath(TOperator, TIntermTyped* child, TSourceLoc);
//		TIntermTyped* addBuiltInFunctionCall(const TSourceLoc& line, TOperator, bool unary, TIntermNode*, const TType& returnType);
//		bool canImplicitlyPromote(TBasicType from, TBasicType to) const;
//		TIntermAggregate* growAggregate(TIntermNode* left, TIntermNode* right);
//		TIntermAggregate* growAggregate(TIntermNode* left, TIntermNode* right, const TSourceLoc&);
//		TIntermAggregate* makeAggregate(TIntermNode* node);
//		TIntermAggregate* makeAggregate(TIntermNode* node, const TSourceLoc&);
//		TIntermTyped* setAggregateOperator(TIntermNode*, TOperator, const TType& type, TSourceLoc);
//		bool areAllChildConst(TIntermAggregate* aggrNode);
//		TIntermNode*  addSelection(TIntermTyped* cond, TIntermNodePair code, const TSourceLoc&);
//		TIntermTyped* addSelection(TIntermTyped* cond, TIntermTyped* trueBlock, TIntermTyped* falseBlock, const TSourceLoc&);
//		TIntermTyped* addComma(TIntermTyped* left, TIntermTyped* right, const TSourceLoc&);
//		TIntermTyped* addMethod(TIntermTyped*, const TType&, const TString*, const TSourceLoc&);
//		TIntermConstantUnion* addConstantUnion(const TConstUnionArray&, const TType&, const TSourceLoc&, bool literal = false) const;
//		TIntermConstantUnion* addConstantUnion(int, const TSourceLoc&, bool literal = false) const;
//		TIntermConstantUnion* addConstantUnion(unsigned int, const TSourceLoc&, bool literal = false) const;
//		TIntermConstantUnion* addConstantUnion(bool, const TSourceLoc&, bool literal = false) const;
//		TIntermConstantUnion* addConstantUnion(double, TBasicType, const TSourceLoc&, bool literal = false) const;
//		TIntermTyped* promoteConstantUnion(TBasicType, TIntermConstantUnion*) const;
//		bool parseConstTree(TIntermNode*, TConstUnionArray, TOperator, const TType&, bool singleConstantParam = false);
//		TIntermLoop* addLoop(TIntermNode*, TIntermTyped*, TIntermTyped*, bool testFirst, const TSourceLoc&);
//		TIntermBranch* addBranch(TOperator, const TSourceLoc&);
//		TIntermBranch* addBranch(TOperator, TIntermTyped*, const TSourceLoc&);
//		TIntermTyped* addSwizzle(TVectorFields&, const TSourceLoc&);
//
//		// Constant folding (in Constant.cpp)
//		TIntermTyped* fold(TIntermAggregate* aggrNode);
//		TIntermTyped* foldConstructor(TIntermAggregate* aggrNode);
//		TIntermTyped* foldDereference(TIntermTyped* node, int index, const TSourceLoc&);
//		TIntermTyped* foldSwizzle(TIntermTyped* node, TVectorFields& fields, const TSourceLoc&);
//
//		// Linkage related
//		void addSymbolLinkageNodes(TIntermAggregate*& linkage, EShLanguage, TSymbolTable&);
//		void addSymbolLinkageNode(TIntermAggregate*& linkage, TSymbolTable&, const TString&);
//		void addSymbolLinkageNode(TIntermAggregate*& linkage, const TSymbol&);
//
//		bool setInvocations(int i) 
//		{
//			if (invocations > 0)
//				return invocations == i;
//			invocations = i;
//			return true;
//		}
//		int getInvocations() const { return invocations; }
//		bool setVertices(int m)
//		{
//			if (vertices > 0)
//				return vertices == m;
//			vertices = m;
//			return true;
//		}
//		int getVertices() const { return vertices; }
//		bool setInputPrimitive(TLayoutGeometry p)
//		{
//			if (inputPrimitive != ElgNone)
//				return inputPrimitive == p;
//			inputPrimitive = p;
//			return true;
//		}
//		TLayoutGeometry getInputPrimitive() const { return inputPrimitive; }
//		bool setVertexSpacing(TVertexSpacing s)
//		{
//			if (vertexSpacing != EvsNone)
//				return vertexSpacing == s;
//			vertexSpacing = s;
//			return true;
//		}
//		TVertexSpacing getVertexSpacing() const { return vertexSpacing; }
//		bool setVertexOrder(TVertexOrder o)
//		{
//			if (vertexOrder != EvoNone)
//				return vertexOrder == o;
//			vertexOrder = o;
//			return true;
//		}
//		TVertexOrder getVertexOrder() const { return vertexOrder; }
//		void setPointMode() { pointMode = true; }
//		bool getPointMode() const { return pointMode; }
//
//		bool setLocalSize(int dim, int size)
//		{
//			if (localSize[dim] > 1)
//				return size == localSize[dim];
//			localSize[dim] = size;
//			return true;
//		}
//		unsigned int getLocalSize(int dim) const { return localSize[dim]; }
//
//		void setXfbMode() { xfbMode = true; }
//		bool getXfbMode() const { return xfbMode; }
//		bool setOutputPrimitive(TLayoutGeometry p)
//		{
//			if (outputPrimitive != ElgNone)
//				return outputPrimitive == p;
//			outputPrimitive = p;
//			return true;
//		}
//		TLayoutGeometry getOutputPrimitive() const { return outputPrimitive; }
//		void setOriginUpperLeft() { originUpperLeft = true; }
//		bool getOriginUpperLeft() const { return originUpperLeft; }
//		void setPixelCenterInteger() { pixelCenterInteger = true; }
//		bool getPixelCenterInteger() const { return pixelCenterInteger; }
//		void setEarlyFragmentTests() { earlyFragmentTests = true; }
//		bool getEarlyFragmentTests() const { return earlyFragmentTests; }
//		bool setDepth(TLayoutDepth d)
//		{
//			if (depthLayout != EldNone)
//				return depthLayout == d;
//			depthLayout = d;
//			return true;
//		}
//		TLayoutDepth getDepth() const { return depthLayout; }
//
//		void addToCallGraph(TInfoSink&, const TString& caller, const TString& callee);
//		void merge(TInfoSink&, TIntermediate&);
//		void finalCheck(TInfoSink&);
//
//		void addIoAccessed(const TString& name) { ioAccessed.insert(name); }
//		bool inIoAccessed(const TString& name) const { return ioAccessed.find(name) != ioAccessed.end(); }
//
//		int addUsedLocation(const TQualifier&, const TType&, bool& typeCollision);
//		int addUsedOffsets(int binding, int offset, int numOffsets);
//		int computeTypeLocationSize(const TType&) const;
//
//		bool setXfbBufferStride(int buffer, unsigned stride)
//		{
//			if (xfbBuffers[buffer].stride != TQualifier::layoutXfbStrideEnd)
//				return xfbBuffers[buffer].stride == stride;
//			xfbBuffers[buffer].stride = stride;
//			return true;
//		}
//		int addXfbBufferOffset(const TType&);
//		unsigned int computeTypeXfbSize(const TType&, bool& containsDouble) const;
//		static int getBaseAlignment(const TType&, int& size, bool std140);
//
//		protected:
//		void error(TInfoSink& infoSink, const char*);
//		void mergeBodies(TInfoSink&, TIntermSequence& globals, const TIntermSequence& unitGlobals);
//		void mergeLinkerObjects(TInfoSink&, TIntermSequence& linkerObjects, const TIntermSequence& unitLinkerObjects);
//		void mergeImplicitArraySizes(TType&, const TType&);
//		void mergeErrorCheck(TInfoSink&, const TIntermSymbol&, const TIntermSymbol&, bool crossStage);
//		void checkCallGraphCycles(TInfoSink&);
//		void inOutLocationCheck(TInfoSink&);
//		TIntermSequence& findLinkerObjects() const;
//		bool userOutputUsed() const;
//		static int getBaseAlignmentScalar(const TType&, int& size);
//
//		const EShLanguage language;
//		TIntermNode* treeRoot;
//		EProfile profile;
//		int version;
//		std::set<std::string> requestedExtensions;  // cumulation of all enabled or required extensions; not connected to what subset of the shader used them
//
//		int numMains;
//		int numErrors;
//		bool recursive;
//		int invocations;
//		int vertices;
//		TLayoutGeometry inputPrimitive;
//		TLayoutGeometry outputPrimitive;
//		bool pixelCenterInteger;
//		bool originUpperLeft;
//		TVertexSpacing vertexSpacing;
//		TVertexOrder vertexOrder;
//		bool pointMode;
//		int localSize[3];
//		bool earlyFragmentTests;
//		TLayoutDepth depthLayout;
//		bool xfbMode;
//
//		typedef std::list<TCall> TGraph;
//		TGraph callGraph;
//
//		std::set<TString> ioAccessed;           // set of names of statically read/written I/O that might need extra checking
//		std::vector<TIoRange> usedIo[4];        // sets of used locations, one for each of in, out, uniform, and buffers
//		std::vector<TOffsetRange> usedAtomics;  // sets of bindings used by atomic counters
//		std::vector<TXfbBuffer> xfbBuffers;     // all the data we need to track per xfb buffer
//
//		private:
//		void operator=(TIntermediate&); // prevent assignments
//	};
//}
//
