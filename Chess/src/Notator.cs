using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
	static class Notator
	{
		public static string outputFile = string.Empty;
		public static string pgnString = string.Empty;
		public static string outPutString = "[Result \"";
		public static bool WhiteLongCastleRight = true;
		public static bool WhiteShortCastleRight = true;
		public static bool BlackCastleRigtht = true;
		public static bool BlackShortCastleRight = true;
		private static string utcDate;
		private static string utcTime;

		public static void Initialize()
		{
			bool fileExists = true;
			string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}";
			string date = DateTime.Now.ToString("yyyy.MM.dd");
			string site = "https://github.com/Cxmu03/Chess-Client";
			utcDate = DateTime.UtcNow.ToString("yyyy.MM.dd");
			utcTime = DateTime.UtcNow.ToString("HH.mm.ss");

			int counter = 1;

			while (fileExists)
			{
				if (!File.Exists($"{fileName} Game {counter}.txt"))
				{
					fileExists = false;
					File.Create($"{fileName} Game {counter}.txt").Dispose();
					outputFile = $"{fileName} Game {counter}.txt";
				}
				else
					counter++;
			}

			using (StreamWriter sw = File.AppendText(outputFile))
			{
				string temp = $"[Event \"Standard Game\"]\n[Site \"{site}\"]\n[Date \"{date}\"]\n[White \"Anonymous\"]\n[Black \"Anonymous\"]";
				sw.WriteLine(temp);
			}
		}

		public static void FinishNotation(int result)
		{
			switch(result)
			{
				case 0:
					outPutString += "1/2-1/2\"]\n";
					break;
				case 1:
					outPutString += "1-0\"]\n";
					break;
				case 2:
					outPutString += "0-1\"]\n";
					break;
			}

			outPutString += $"[UTCDate \"{utcDate}\"]\n[UTCTime \"{utcTime}\"]\n[FEN \"{CreateFen()}\"]\n";

			AppendToPgnFile(outPutString, 0);
			AppendToPgnFile(pgnString, 1);
		}

		public static void AppendToPgnFile(string str, int numBlankLines)
		{

			using (StreamWriter sw = File.AppendText(outputFile))
			{
				for (int i = 0; i < numBlankLines; i++)
				{
					sw.WriteLine();
				}
				sw.Write(str);
			}
		}

		public static string CreateFen()
		{
			return string.Empty;
		}
	}
}
