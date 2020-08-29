using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
	class Menu
	{
		public static void MainMenu()
		{
			List<string> options = new List<string>()
			{
				"Play",
				"Settings",
				"Quit"
			};
			Console.Clear();
			DisplayHeader("Main Menu");
			DisplayOptions(options);
			Console.ReadKey();
		}

		private static void DisplayHeader(string s)
		{
			Console.SetCursorPosition(1, 0);
			for(int i = 2; i <= Console.WindowWidth - 1; i++)
			{
				Console.Write("_");
			}
			Console.SetCursorPosition(1, Console.WindowHeight - 2);
			for (int i = 2; i <= Console.WindowWidth - 1; i++)
			{
				Console.Write("_");
			}
			for(int i = 1; i < Console.WindowHeight - 1; i++)
			{
				for (int j = 0; j <= 1; j++)
				{
					if (j == 1)
					{
						Console.SetCursorPosition(1, i);
					}
					else
					{
						Console.SetCursorPosition(Console.WindowWidth - 2, i);
					}
					Console.Write("|");
				}
			}
			for (int i = 0; i < 4; i++)
			{
				Console.SetCursorPosition(Console.WindowWidth / 2 - (s.Length + 8) / 2, i + 2);
				switch(i)
				{
					case 0:
						for (int j = 0; j < s.Length + 8; j++)
							Console.Write("_");
						break;
					case 1:
						Console.Write("|");
						for (int j = 0; j < s.Length + 6; j++)
							Console.Write(" ");
						Console.Write("|");
						break;
					case 2:
						Console.Write($"|   {s}   |");
						break;
					case 3:
						Console.Write("|");
						for (int j = 0; j < s.Length + 6; j++)
							Console.Write("_");
						Console.Write("|");
						break;
				}
			}
		}

		private static void DisplayOptions(List<string> options)
		{
			for(int i = 0; i < options.Count; i++)
			{
				;
			}
		}
	}
}
