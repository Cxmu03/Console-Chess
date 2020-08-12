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

	string s = getNextMove(argv[1]);

	tempFile << s;

	tempFile.close();

	return 0;
}