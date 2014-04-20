#include "DependencyGraph.h"
#include <string>
#include <vector>
#include <set>

#ifndef CIRCULARDEPENDENCY
#define CIRCULARDEPENDENCY
class CircularDependency {
public:
	CircularDependency(DependencyGraph* DG);
	~CircularDependency(void);
	std::set<std::string>* GetDirectDependents(std::string name);
	std::vector<std::string>* GetCellsToRecalculate(std::set<std::string>* names);
	void Visit(std::string start, std::string name, std::set<std::string>* visited, std::vector<std::string>* changed);
	std::set<std::string> CellsToRecalculate(std::string name);


private:
	DependencyGraph *dg;
};
#endif
