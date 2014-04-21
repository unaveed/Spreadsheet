#include "DependencyGraph.h"
#include "CircularDependency.h"
#include <set>
#include <string>
#include <vector>
#include <exception>
#include <iostream>

CircularDependency::CircularDependency(DependencyGraph* DG) {
	dg = DG;
}

CircularDependency::~CircularDependency(void) {
}

/*
class myexception: public std::exception {
	virtual const char* CircDep() const throw() {
		return "Circular Dependency Encountered";
	}
} CircularException;
*/

/// <summary>
/// If name is null, throws an ArgumentNullException.
/// 
/// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
/// 
/// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
/// values depend directly on the value of the named cell.  In other words, returns
/// an enumeration, without duplicates, of the names of all cells that contain
/// formulas containing name.
/// 
/// For example, suppose that
/// A1 contains 3
/// B1 contains the formula A1 * A1
/// C1 contains the formula B1 + A1
/// D1 contains the formula B1 - C1
/// The direct dependents of A1 are B1 and C1
/// </summary>        CircularDependency
std::set<std::string>* CircularDependency::GetDirectDependents(std::string name) {
	return &dg->GetDependees(name);
}

/// <summary>
/// Requires that names be non-null.  Also requires that if names contains s,
/// then s must be a valid non-null cell name.
/// 
/// If any of the named cells are involved in a circular dependency,
/// throws a CircularException.
/// 
/// Otherwise, returns an enumeration of the names of all cells whose values must
/// be recalculated, assuming that the contents of each cell named in names has changed.
/// The names are enumerated in the order in which the calculations should be done.  
/// 
/// For example, suppose that 
/// A1 contains 5
/// B1 contains 7
/// C1 contains the formula A1 + B1
/// D1 contains the formula A1 * C1
/// E1 contains 15
/// 
/// If A1 and B1 have changed, then A1, B1, and C1, and D1 must be recalculated,
/// and they must be recalculated in either the order A1,B1,C1,D1 or B1,A1,C1,D1.
/// The method will produce one of those enumerations.
/// 
/// Please note that this method depends on the abstract GetDirectDependents.
/// It won't work until GetDirectDependents is implemented correctly.
/// </summary>
std::vector<std::string>* CircularDependency::GetCellsToRecalculate(std::set<std::string>* names) {
	//LinkedList<String> changed = new LinkedList<String>();
	std::vector<std::string> *changed = new std::vector<std::string>;

	//HashSet<String> visited = new HashSet<String>();
	std::set<std::string> *visited = new std::set<std::string>;

	//foreach (String name in names)
	for(std::set<std::string>::iterator name = names->begin(); name != names->end(); ++name) {
		//if (!visited.Contains(name))
		if(visited->find(*name) == visited->end())
			Visit(*name, *name, visited, changed);
	}
	return changed;
}


/// <summary>
/// A helper for the GetCellsToRecalculate method.
/// </summary>
void CircularDependency::Visit(std::string start, std::string name, std::set<std::string>* visited, std::vector<std::string>* changed) {
	visited->insert(name);

	std::set<std::string>* DirectDependents = GetDirectDependents(name);

	for(std::set<std::string>::iterator n = DirectDependents->begin(); n != DirectDependents->end(); ++n) {
		if(*n == start) {
			//throw CircularException;
			std::cout << "CircularDependency.cc:Throwing exception" << std::endl;
			throw -1;
		}
		else if(visited->find(*n) == visited->end()) {
			Visit(start, *n, visited, changed);
		}
	}
	changed->insert(changed->begin(), name);

}
