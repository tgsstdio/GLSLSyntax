using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GLSLSyntaxAST.CodeDom
{
	public class TSymbolTableLevel
	{
		TSymbolTableLevel clone()
		{
			var symTableLevel = new TSymbolTableLevel();
			symTableLevel.anonId = anonId;
			std::vector<bool> containerCopied(anonId, false);
			tLevel::const_iterator iter;
			for (iter = level.begin(); iter != level.end(); ++iter) {
				const TAnonMember* anon = iter->second->getAsAnonMember();
				if (anon) {
					// Insert all the anonymous members of this same container at once,
					// avoid inserting the other members in the future, once this has been done,
					// allowing them to all be part of the same new container.
					if (! containerCopied[anon->getAnonId()]) {
						TVariable* container = anon->getAnonContainer().clone();
						container->changeName(NewPoolTString(""));
						// insert the whole container
						symTableLevel.insert(*container, false);
						containerCopied[anon->getAnonId()] = true;
					}
				} else
					symTableLevel->insert(*iter->second->clone(), false);
			}

			return symTableLevel;
		}

		bool insert(TSymbol symbol, bool separateNameSpaces)
		{
			//
			// returning true means symbol was added to the table with no semantic errors
			//
			tInsertResult result;
			const TString& name = symbol.getName();
			if (name == "") {
				// An empty name means an anonymous container, exposing its members to the external scope.
				// Give it a name and insert its members in the symbol table, pointing to the container.
				char buf[20];
				snprintf(buf, 20, "%s%d", AnonymousPrefix, anonId);
				symbol.changeName(NewPoolTString(buf));

				bool isOkay = true;
				const TTypeList& types = *symbol.getAsVariable()->getType().getStruct();
				for (unsigned int m = 0; m < types.size(); ++m) {
					TAnonMember* member = new TAnonMember(&types[m].type->getFieldName(), m, *symbol.getAsVariable(), anonId);
					result = level.insert(tLevelPair(member->getMangledName(), member));
					if (! result.second)
						isOkay = false;
				}

				++anonId;

				return isOkay;
			} else {
				// Check for redefinition errors:
				// - STL itself will tell us if there is a direct name collision, with name mangling, at this level
				// - additionally, check for function-redefining-variable name collisions
				const TString& insertName = symbol.getMangledName();
				if (symbol.getAsFunction()) {
					// make sure there isn't a variable of this name
					if (! separateNameSpaces && level.find(name) != level.end())
						return false;

					// insert, and whatever happens is okay
					level.insert(tLevelPair(insertName, &symbol));

					return true;
				} else {
					result = level.insert(tLevelPair(insertName, &symbol));

					return result.second;
				}
			}
		}
	}

	public class TSymbolTable
	{
		public uint uniqueId;      // For cross-scope comparing during code generation
		public uint adoptedLevels;
		public bool noBuiltInRedeclarations;
		public bool separateNameSpaces;
		public void adoptLevels (TSymbolTable symTable)
		{
			foreach (var level in symTable.table) {
				table.Push(level);
				++adoptedLevels;
			}

			uniqueId = symTable.uniqueId;
			noBuiltInRedeclarations = symTable.noBuiltInRedeclarations;
			separateNameSpaces = symTable.separateNameSpaces;
		}

		public void copyTable(TSymbolTable copyOf)
		{
			Debug.Assert(adoptedLevels == copyOf.adoptedLevels);

			uniqueId = copyOf.uniqueId;
			noBuiltInRedeclarations = copyOf.noBuiltInRedeclarations;
			separateNameSpaces = copyOf.separateNameSpaces;

			foreach (var level in copyOf.table) {
				table.Push(level.clone());
			}
		}

		public TSymbolTable ()
		{
			table = new Stack<TSymbolTableLevel> ();
		}

		private Stack<TSymbolTableLevel> table;
		public void push()
		{
			table.Push(new TSymbolTableLevel());
		}

		public bool isEmpty() 
		{
			return table.Count == 0; 
		}
	}
}

