/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#include "DependencyGraph.h"
#include <string>
#include <cstdlib>
#include <iostream>
#include <map>
#include <set>

//A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
/// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
/// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
/// set, and the element is already in the set, the set remains unchanged.)
/// 
/// Given a DependencyGraph DG:
/// 
///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///        
///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
//
// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
//     dependents("a") = {"b", "c"}
//     dependents("b") = {"d"}
//     dependents("c") = {}
//     dependents("d") = {"d"}
//     dependees("a") = {}
//     dependees("b") = {"a"}
//     dependees("c") = {"a"}
//     dependees("d") = {"b", "d"}

DependencyGraph::DependencyGraph() {
	DG = new std::map<std::string, std::set<std::string> >();
}

DependencyGraph::~DependencyGraph() {
	//delete DG;
}

/*
* The number of ordered pairs iin the DependencyGraph.
*/
int DependencyGraph::Size() {
	int size = 0;

	//Count the amount of values stored in each key.
	// Need an iterator.
	//for(std::map<std::string, std::hash_set<std::string> >::iterator it = DG->begin();
	for(std::map<std::string, std::set<std::string> >::iterator it = DG->begin(); it != DG->end(); ++it) {
		size += it->second.size();
	}
	return size;
}


/// <summary>
/// The size of dependees(s).
/// This property is an example of an indexer.  If dg is a DependencyGraph, you would
/// invoke it like this:
/// dg["a"]
/// It should return the size of dependees("a")
/// </summary>
int DependencyGraph::operator[](const std::string& s) {
		//Start the count at 0
		int count = 0;
		//loops through all of the values in the dependency graph and if it contains
		// the element s than it will increment the count by one.
		for(std::map<std::string, std::set<std::string> >::iterator dg=DG->begin(); dg != DG->end(); ++dg) {
			if(dg->second.find(s) != dg->second.end()) {
				count++;
			}
		}
		return count;
}

/// <summary>
/// Reports whether dependents(s) is non-empty.
/// </summary>
bool DependencyGraph::HasDependents(std::string s){

	//The order pair must actually contain s and then it checks
	// that the xount is not zero, if so returns true, otherwise it 
	// means it is empty so returns false.
	for(std::map<std::string, std::set<std::string> >::iterator it = DG->begin(); it != DG->end(); ++it) {
		if((it->first) == s && (it->second.size()) != 0) {
			return true;
		}
	}
	return false;
}

/// <summary>
/// Reports whether dependees(s) is non-empty.
/// </summary>
bool DependencyGraph::HasDependees(std::string s) {

	//The dependees is the key and s in this case is the dependent
	//
	//Checks if every value in the DG contains atleast one instance of s
	// if so it returns true, otherwise it will finish the loop and we will 
	// know that there must not be any dependee.
	for(std::map<std::string, std::set<std::string> >::iterator it = DG->begin(); it != DG->end(); ++it){

		//If the DG's value at position i contains s then it will return true
		// because that means that the dependee is not empty.
		if(it->second.find(s) != it->second.end()) {
			return true;
		}
	}
	return false;
}

/// <summary>
/// Enumerates dependents(s).
/// </summary>
std::set<std::string>& DependencyGraph::GetDependents(std::string s) {

	//Make sure that the key actually exists.

	//Goes through every item that is stored in dependee s
	// and returns its dependents.
	for(std::map<std::string, std::set<std::string> >::iterator it = DG->begin(); it != DG->end(); ++it) {
		if(it->first == s) {
			return (it->second);
		}
	}
	return *(new std::set<std::string>);
}

/// <summary>
/// Enumerates dependees(s).
/// </summary>
std::set<std::string>& DependencyGraph::GetDependees(std::string s) {
	std::set<std::string> *result = new std::set<std::string>; 

	//Goes through every value in the value ArrayList checking
	// for instances of s and if it finds one it will return it.
	for(std::map<std::string, std::set<std::string> >::iterator dg = DG->begin(); dg != DG->end(); ++dg) {
		//if the value at position i contains s then get the key
		// at that position and return it.
		if(dg->second.find(s) != dg->second.end())
			//if (DG.Values.ElementAt(i).Contains(s))
		{
			result->insert(dg->first);
			//yield return DG.Keys.ElementAt(i);
		}
	}
	return *result;

}

/// <summary>
/// Adds the ordered pair (s,t), if it doesn't exist
/// </summary>
/// <param name="s"></param>
/// <param name="t"></param>
void DependencyGraph::AddDependency(std::string s, std::string t) {
	//Makes sure that the key doesn't exists already if it already
	// does then it creates a brand new order pair.
	for(std::map<std::string, std::set<std::string> >::iterator dg = DG->begin(); dg != DG->end(); ++dg) {
		if(dg->first == s) {
			dg->second.insert(t);
			return;
		}
	}
	//You have to make a new array because the values in the DG have to be in array form
	//HashSet<string> arrL = new HashSet<string>();
	std::set<std::string> *hs = new std::set<std::string>;
	hs->insert(t); //Add the value to the hashset

	(*DG)[s] = *hs;//Now adds the new order pair.
}

/// <summary>
/// Removes the ordered pair (s,t), if it exists
/// </summary>
/// <param name="s"></param>
/// <param name="t"></param>
void DependencyGraph::RemoveDependency(std::string s, std::string t) {
	//Make sure it contains the s key, which is the dependee
	if(DG->find(s) != DG->end()) {
		//First scenario if there is only one item in the Dictionary
		// then it gets rid of the key all together.
		if(	(*DG)[s].size() == 1) {
			if((*DG)[s].find(t) != (*DG)[s].end()) {
				DG->erase(s);
				return;
			}
		}

		//Second scenario if there is more than one item in the DG then simply
		//remove the dependee.
		if((*DG)[s].find(t) != (*DG)[s].end()) {
			(*DG)[s].erase(t);
			return;
		}

	}
}

/// <summary>
/// Removes all existing ordered pairs of the form (s,r).  Then, for each
/// t in newDependents, adds the ordered pair (s,t).
/// </summary>
void DependencyGraph::ReplaceDependents(std::string s, std::set<std::string>* newDependents) {
	//Creates a new ArrayList in, which the new dependents are going to be stored in.
	//std::hash_set<std::string> *hs = new std::hash_set<std::string>;

	//If if contains the key already it first removes everything the dependee and 
	// everything in there 
	if(DG->find(s) != DG->end()) {
		DG->erase(s);
	}
	//Replaces the dependents of s with all of the new dependents.
	(*DG)[s] = *newDependents;

}


/// <summary>
/// Removes all existing ordered pairs of the form (r,s).  Then, for each 
/// t in newDependees, adds the ordered pair (t,s).
/// </summary>
void DependencyGraph::ReplaceDependees(std::string s, std::set<std::string> *newDependees) {
	//This loop removes all of the dependees that contain the s dependent.
	//
	//Loops through every key and its value, and if the value set contains s
	// which is the dependee then it removes that dependee.
	for(std::map<std::string, std::set<std::string> >::iterator dg=DG->begin(); dg!=DG->end(); ++dg) {
		if(dg->second.find(s) != dg->second.end()) {
			dg->second.erase(s);
		}

	}
	//For every new dependee in the set, it adds the dependent s.
	typedef std::set<std::string>::iterator it;

	it from (newDependees->begin());      

	it until (newDependees->end());                      

	std::reverse_iterator<it> rev_until (from);    

	std::reverse_iterator<it> rev_from (until);     

	while (rev_from != rev_until) {
		AddDependency(*rev_from, s);
		rev_from++;
	}
}
