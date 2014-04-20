/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPH_H

#include <string>
#include <map>
#include <set>


class DependencyGraph {
	public:
		DependencyGraph();
		virtual ~DependencyGraph(void);
		int Size();
		int operator[](const std::string& b);
		bool HasDependents(std::string s);
		bool HasDependees(std::string s);
		std::set<std::string>& GetDependents(std::string s);
		std::set<std::string>& GetDependees(std::string s);
		void AddDependency(std::string s, std::string t);
		void RemoveDependency(std::string s, std::string t);
		void ReplaceDependents(std::string s, std::set<std::string> *newDependents);
		void ReplaceDependees(std::string s, std::set<std::string> *newDependees);

	private:
		std::map<std::string, std::set<std::string> > *DG;

};

#endif
