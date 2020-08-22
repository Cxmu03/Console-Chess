using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Chess.Pieces;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
	static class Notator
	{
		public static string outputFile = string.Empty;
		public static string pgnString = string.Empty;
		public static string outPutString = "[Result \"";
		public static string whitePlayer = "Anonymous";
		public static string blackPlayer = "Anonymous";
		public static bool WhiteLongCastleRight = true;
		public static bool WhiteShortCastleRight = true;
		public static bool BlackLongCastleRigtht = true;
		public static bool BlackShortCastleRight = true;
		private static string utcDate;
		private static string utcTime;

		public static void Initialize()
		{
			bool fileExists = true;
			string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}";
			string date = DateTime.Now.ToString("yyyy.MM.dd");
			string site = "https://github.com/Cxmu03/ConsoleChess";
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
				string temp = $"[Event \"Standard Game\"]\n[Site \"{site}\"]\n[Date \"{date}\"]\n[White \"{whitePlayer}\"]\n[Black \"{blackPlayer}\"]";
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

			outPutString += $"[UTCDate \"{utcDate}\"]\n[UTCTime \"{utcTime}\"]\n[Start Position \"{Program.startFen}\"]\n[End Position \"{CreateFen()}\"]\n";

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
			string fen = string.Empty;
			List<Piece> temp;
			List<int> columns;
			for(int i = 0; i < 8; i++)
			{
				temp = Board.pieces.Where(x => x.position.row == i).ToList();
				columns = new List<int>();
				foreach(Piece p in temp)
				{
					columns.Add(p.position.column);
				}
				columns.Sort();
				if (columns.Count == 0)
				{
					fen += "8";
				}
				else
				{
					for (int j = 0; j < columns.Count; j++)
					{
						if (j == 0)
						{
							if (columns[j] > 0)
							{
								fen += columns[j];
								string current = Board.ChooseLetter(temp.Find(x => x.position.column == columns[j]).GetType().ToString()).ToString();
								fen += temp.Find(x => x.position.column == columns[j]).isWhite ? current : current.ToLower();
							}
							else
							{
								string current = Board.ChooseLetter(temp.Find(x => x.position.column == columns[j]).GetType().ToString()).ToString();
								fen += temp.Find(x => x.position.column == columns[j]).isWhite ? current : current.ToLower();
							}
						}
						else
						{
							if(columns[j] - columns[j - 1] > 1)
							{
								fen += (columns[j] - columns[j - 1]) - 1;
							}
							string current = Board.ChooseLetter(temp.Find(x => x.position.column == columns[j]).GetType().ToString()).ToString();
							fen += temp.Find(x => x.position.column == columns[j]).isWhite ? current : current.ToLower();
						}
					}
					if(columns.Last() < 7)
					{
						fen += 7 - columns.Last();
					}
				}
				if(i < 7)
					fen += "/";
			}

			fen += Program.currentPlayerIsWhite ? " w " : " b ";

			if (!WhiteShortCastleRight && !WhiteLongCastleRight && !BlackLongCastleRigtht && !BlackShortCastleRight)
				fen += "- ";
			else
			{
				if (WhiteShortCastleRight)
					fen += "K";
				if (WhiteLongCastleRight)
					fen += "Q";
				if (BlackShortCastleRight)
					fen += "k";
				if (BlackLongCastleRigtht)
					fen += "q";
				fen += " ";
			}

			bool enPassantExists = false;

			foreach(Pawn p in Board.pieces.Where(x => x.GetType().ToString().GetHashCode() == "Chess.Pieces.Pawn".GetHashCode()))
			{
				if(p.enPassant == Program.currentPlayer - 1)
				{
					Position pos = new Position(p.position.row + (p.isWhite ? 1 : -1), p.position.column);
					fen += $"{Position.PositionToNotation(pos)} ";
					enPassantExists = true;
				}
			}

			if (!enPassantExists)
				fen += "- ";
			
			fen += $"{Program.halfMoves} ";
			fen += $"{Program.move}";

			return fen;
		}
	}
}
