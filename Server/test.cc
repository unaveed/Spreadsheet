#include <iostream>
#include <string>

int main(){
	std::string messy = "File1file2File3";
	
	size_t found = messy.find("crap");
	if(found != std::string::npos)
		std::cout << "NOOOOO" << std::endl;
	return 0;
}
