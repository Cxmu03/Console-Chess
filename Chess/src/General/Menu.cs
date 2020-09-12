using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

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
			List<Action> commands = new List<Action>()
			{
				Program.Game,
				Settings,
				() => Environment.Exit(0)
			};
			DisplayHeader("Console Chess");
			DisplayOptions(options, commands, "Console Chess".Length, "Choice: ");
		}

		public static void Settings()
		{
			List<string> options = new List<string>()
			{
				"Stockfish Depth",
				"Pgn saving",
				"Sound",
				"Go Back"
			};
			List<Action> commands = new List<Action>()
			{
				StockfishDepth,
				PgnSaving,
				Sound,
				MainMenu,
			};
			DisplayHeader("Settings");
			DisplayOptions(options, commands, "Settings".Length, "Choice: ");
		}

		private static void StockfishDepth()
		{
			ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string headerString = "Stockfish Depth";
			DisplayHeader(headerString);
			Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 8);
			Console.Write($"Current stockfish depth is {Program.engineDepth}");
			Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 9);
			Console.Write("Enter new depth: ");
			string prompt = "16";
			while(!Enumerable.Range(1, 15).Contains(Convert.ToInt32(prompt)))
			{
				prompt = Console.ReadLine();
				Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 26);
				for (int i = 0; i < prompt.Length; i++)
					Console.Write(" ");
				Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 26);
			}
			Program.engineDepth = prompt;
			ConfigurationManager.AppSettings.Set("StockfishDepth", prompt);
			ConfigurationManager.RefreshSection("appSettings");
			Settings(); 
		}

		private static void PgnSaving()
		{
			ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string headerString = "Pgn saving";
			DisplayHeader(headerString);
			Console.SetCursorPosition(Console.WindowWidth / 2 - (int)(headerString.Length * 1.5), 8);
			Console.Write("Pgn saving is currently turned {0}", Notator.pgnSaving ? "on" : "off");
			Console.SetCursorPosition(Console.WindowWidth / 2 - (int)(headerString.Length * 1.5), 9);
			Console.Write("Do you want to turn pgn saving on(1) or off(2): ");
			string prompt = "15";
			while (!Enumerable.Range(1, 2).Contains(Convert.ToInt32(prompt)))
			{
				prompt = Console.ReadLine();
				Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 26);
				for (int i = 0; i < prompt.Length; i++)
					Console.Write(" ");
				Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 26);
			}
			if (Convert.ToInt32(prompt) == 1)
			{
				Notator.pgnSaving = true;
				ConfigurationManager.AppSettings.Set("savePgn", "true");
				ConfigurationManager.RefreshSection("appSettings");
			}
			else
			{
				Notator.pgnSaving = false;
				ConfigurationManager.AppSettings.Set("savePgn", "false");
				ConfigurationManager.RefreshSection("appSettings");
			}
			Settings();
		}

		private static void Sound()
		{
			ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string headerString = "Sound";
			DisplayHeader(headerString);
			Console.SetCursorPosition(Console.WindowWidth / 2 - (int)(headerString.Length * 1.5), 8);
			Console.Write("Sound is currently turned {0}", Program.playSound ? "on" : "off");
			Console.SetCursorPosition(Console.WindowWidth / 2 - (int)(headerString.Length * 1.5), 9);
			Console.Write("Do you want to turn sound on(1) or off(2): ");
			string prompt = "15";
			while (!Enumerable.Range(1, 2).Contains(Convert.ToInt32(prompt)))
			{
				prompt = Console.ReadLine();
				Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 26);
				for (int i = 0; i < prompt.Length; i++)
					Console.Write(" ");
				Console.SetCursorPosition(Console.WindowWidth / 2 - headerString.Length / 2, 26);
			}
			if (Convert.ToInt32(prompt) == 1)
			{
				ConfigurationManager.AppSettings.Set("playSound", "true");
				ConfigurationManager.RefreshSection("appSettings");
				Program.playSound = true;
			}
			else
			{
				ConfigurationManager.AppSettings.Set("playSound", "false");
				ConfigurationManager.RefreshSection("appSettings");
				Program.playSound = false;
			}
			Settings();
		}

		private static void DisplayHeader(string s)
		{
			Console.Clear();
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

		private static void DisplayOptions(List<string> options, List<Action> commands, int headerStringLength, string prompt)
		{
			int top = 8;
			for(int i = 0; i < options.Count; i++)
			{
				Console.SetCursorPosition(Console.WindowWidth / 2 - headerStringLength / 2, top);
				Console.Write($"{i + 1}: {options[i]}");
				top += 2;
			}
			Console.SetCursorPosition(Console.WindowWidth / 2 - headerStringLength / 2, top);
			bool cont = true;
			Console.Write(prompt);
			while (cont)
			{
				string choice = Console.ReadLine();
				try
				{
					int choiceInt = Convert.ToInt32(choice);
					if(choiceInt > 0 && choiceInt <= options.Count)
					{
						commands[choiceInt - 1].DynamicInvoke();
					}
				}
				catch (Exception)
				{
					cont = false;
					Console.SetCursorPosition(Console.WindowWidth / 2 - headerStringLength / 2, top);
					for (int i = 0; i < choice.Length; i++)
					{
						Console.Write(" ");
					}
					Console.SetCursorPosition(Console.WindowWidth / 2 - headerStringLength / 2, top);
				}
			}
		}
	}
}
