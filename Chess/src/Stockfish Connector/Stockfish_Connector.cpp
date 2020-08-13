#include <fstream>
#include <string>
#include "Connector.hpp"

using namespace std;

int main(int argc, char *argv[])
{
	ShowWindow(GetConsoleWindow(), SW_HIDE);

	string engine = "stockfish.exe";
	ConnectToEngine(&engine[0]);

	ofstream tempFile("temp.txt");

	string s;

	string argument;

	for (int i = 1; i < argc - 1; i++)
	{
		argument += argv[i];
		argument += " ";
	}

	s = getNextMove(argument, argv[argc - 1]);

	tempFile << s;

	tempFile.close();

	CloseConnection();

	return 0;
}