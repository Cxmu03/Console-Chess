using System;
using System.Collections.Generic;

namespace Chess
{
	class Position
	{
		public int row;
		public int column;

		private static readonly Dictionary<string, string> operatorToPiece = new Dictionary<string, string>()
		{
			{"K", "Chess.Pieces.King"},
			{"Q", "Chess.Pieces.Queen"},
			{"N", "Chess.Pieces.Knight"},
			{"B", "Chess.Pieces.Bishop"},
			{"R", "Chess.Pieces.Rook"},
			{"P", "Chess.Pieces.Pawn"},

		};

		private static readonly Dictionary<string, int> letterToColumn = new Dictionary<string, int>()
		{
			{"A", 0},
			{"B", 1},
			{"C", 2},
			{"D", 3},
			{"E", 4},
			{"F", 5},
			{"G", 6},
			{"H", 7},
		};

		private static readonly Dictionary<int, string> columnToLetter = new Dictionary<int, string>()
		{
			{0,"a"},
			{1,"b"},
			{2,"c"},
			{3,"d"},
			{4,"e"},
			{5,"f"},
			{6,"g"},
			{7,"h"},
		};


		public Position(int row, int column)
		{
			this.row = row;
			this.column = column;
		}

		public Position(Position pos)
		{
			this.row = pos.row;
			this.column = pos.column;
		}

		public void CopyPositionTo(Position pos2)
		{
			pos2.row = row;
			pos2.column = column;
		}

		public void CopyPositionFrom(Position pos2)
		{
			row = pos2.row;
			column = pos2.column;
		}

		public bool Equals(Position p) => this.row == p.row && this.column == p.column;

		public static Position NotationToPosition(string str) => new Position(8 - Convert.ToInt32(str.ToUpper().Substring(1, 1)), letterToColumn[str.ToUpper().Substring(0, 1)]);

		public static string PositionToNotation(Position pos) => $"{columnToLetter[pos.column]}{8 - pos.row}";

		public void ShowPosition(string prefix) => Console.WriteLine($"{prefix}{this.row}, {this.column}");


		public static MoveInformation parseInputToPosition(string input, bool isWhite)
		{
			input = input.Trim(' ');
			MoveInformation moveInformation = new MoveInformation();
			if(new List<string>() {"O-O-O", "O-O"}.Contains(input.ToUpper()))
			{
				if (input.ToUpper() == "O-O")
				{
					switch(isWhite)
					{
						case true:
							if (Board.WhiteKing.CanCastleShort())
							{
								Board.WhiteKing.Castle(true);
								moveInformation.castled = true;
							}
							else
								return null;
							break;
						case false:
							if (Board.BlackKing.CanCastleShort())
							{
								Board.BlackKing.Castle(true);
								moveInformation.castled = true;
							}
							else
								return null;
							break;
					}
				}
				else if (input.ToUpper() == "O-O-O")
				{
					switch (isWhite)
					{
						case true:
							if (Board.WhiteKing.CanCastleLong())
							{
								Board.WhiteKing.Castle(false);
								moveInformation.castled = true;
							}
							else
								return null;
							break;
						case false:
							if (Board.BlackKing.CanCastleLong())
							{
								Board.BlackKing.Castle(false);
								moveInformation.castled = true;
							}
							else
								return null;
							break;
					}
				}
			}
			else if (input.Length != 5)
				return null;
			else
			{
				if(operatorToPiece.ContainsKey(input.Substring(0, 1).ToUpper()))
					moveInformation.pieceName = operatorToPiece[(input.Substring(0, 1)).ToUpper()];
				try
				{
					moveInformation.currentPosition = NotationToPosition(input.Substring(1, 2));
					moveInformation.desiredPosition = NotationToPosition(input.Substring(3, 2));
				}
				catch(Exception)
				{
					return null;
				}
			}

			return moveInformation;
			
		}
	}
}
