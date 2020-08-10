using System;
using Chess.Pieces;
using System.Collections.Generic;

namespace Chess
{
	static class Board
	{
		public static bool BoardIsRotated = false;

		public static King WhiteKing = new King(true, new Position(7, 4));
		public static King BlackKing = new King(false, new Position(0, 4));
		public static Rook WhiteRookL = new Rook(true, new Position(7, 0));
		public static Rook WhiteRookS = new Rook(true, new Position(7, 7));
		public static Rook BlackRookL = new Rook(false, new Position(0, 0));
		public static Rook BlackRookS = new Rook(false, new Position(0, 7));

		public static List<Piece> pieces;

		/// <summary>
		/// Initializes the piece list
		/// </summary>
		public static void Initialize()
		{
			pieces = new List<Piece>()
			{
				WhiteKing,
				BlackKing,
				WhiteRookL,
				WhiteRookS,
				BlackRookL,
				BlackRookS,
				new Queen(true, new Position(7, 3)),
				new Knight(true, new Position(7, 1)),
				new Bishop(true, new Position(7, 2)),
				new Bishop(true, new Position(7, 5)),
				new Knight(true, new Position(7, 6)),
				new Queen(false, new Position(0, 3)),
				new Knight(false, new Position(0, 1)),
				new Bishop(false, new Position(0, 2)),
				new Bishop(false, new Position(0, 5)),
				new Knight(false, new Position(0, 6)),
			};

			for (int row = 1; row <= 6; row += 5)
			{
				for (int column = 0; column <= 7; column++)
				{
					pieces.Add(new Pawn(row == 1 ? false : true, new Position(row, column)));
				}
			}
		}

		private static void NewLine() => Console.Write("\n");

		/// <summary>
		/// Draws the whole Board
		/// </summary>
		public static void DrawBoard()
		{
			Piece currentPiece;

			for (int row = 0; row < 8; row++)
			{
				int columnStart = BoardIsRotated ? (row % 2 == 0 ? 1 : 0) : (row % 2 == 0 ? 0 : 1);

				for (int i = 0; i < 5; i++)
				{
					Console.Write("    ");

					for (int j = 0; j < 8; j++)
					{
						for (int x = 0; x < 11; x++)
						{
							if (columnStart % 2 == 0)
								Console.BackgroundColor = ConsoleColor.Gray;
							else
								Console.BackgroundColor = ConsoleColor.DarkGray;
							if (x == 5 && i == 2 && (Board.BoardIsRotated ? pieces.Find(piece => piece.position.Equals(new Position(7 - row, 7 - j))) : pieces.Find(piece => piece.position.Equals(new Position(row, j)))) != null)
							{
								currentPiece = Board.BoardIsRotated ? pieces.Find(piece => piece.position.Equals(new Position(7 - row, 7 - j))) : pieces.Find(piece => piece.position.Equals(new Position(row, j)));
								if (currentPiece.isWhite)
								{
									if (!WhiteKing.InCheck())
										Console.ForegroundColor = ConsoleColor.White;
									else
										Console.ForegroundColor = ConsoleColor.Red;
									Console.Write($"{ChooseLetter(currentPiece.GetType().ToString())}");
								}
								else
								{
									if (!BlackKing.InCheck())
										Console.ForegroundColor = ConsoleColor.Black;
									else
										Console.ForegroundColor = ConsoleColor.Red;
									Console.Write($"{ChooseLetter(currentPiece.GetType().ToString())}");
								}
							}
							else
								Console.Write(" ");
						}
						columnStart++;
					}
					Console.ResetColor();
					if (i == 2)
						if (!Board.BoardIsRotated)
							Console.Write($"   {8 - row}");
						else
							Console.Write($"   {1 + row}");
					Console.Write("\n");
				}
			}
			DrawLetters();
		}

		/// <summary>
		/// Draws the letters below the board
		/// </summary>
		private static void DrawLetters()
		{
			NewLine();
			for (int i = 0; i < 9; i++)
				Console.Write(" ");
			if (!Board.BoardIsRotated)
			{
				for (char c = 'A'; c <= 'H'; c++)
				{
					Console.Write(c);
					for (int i = 0; i < 10; i++)
						Console.Write(" ");
				}
			}
			else
			{
				for (char c = 'H'; c >= 'A'; c--)
				{
					Console.Write(c);
					for (int i = 0; i < 10; i++)
						Console.Write(" ");
				}
			}
		}

		/// <summary>
		/// Gets the corresponding character to the type name
		/// </summary>
		/// <param name="pieceName", type=string></param>
		public static char ChooseLetter(string pieceName)
		{
			Dictionary<string, char> PieceToLetter = new Dictionary<string, char>()
			{
				{"Chess.Pieces.King", 'K'},
				{"Chess.Pieces.Queen", 'Q'},
				{"Chess.Pieces.Knight", 'N'},
				{"Chess.Pieces.Bishop", 'B'},
				{"Chess.Pieces.Rook", 'R'},
				{"Chess.Pieces.Pawn", 'P'}
			};

			return PieceToLetter[pieceName];
		}


	}
}
