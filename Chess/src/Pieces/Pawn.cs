using System.Collections.Generic;
using System;
using System.Linq;

namespace Chess.Pieces
{
	class Pawn : Piece
	{
		public bool hasMoved = false;
		public int? enPassant = null;
		public static bool promoting = false;
		public static string promotePiece;

		public Pawn(bool isWhite, Position position)
		{
			this.isWhite = isWhite;
			this.position = position;
		}

		public override void Move(Position pos)
		{
			if (!hasMoved)
			{
				if(pos.column == this.position.column + (this.isWhite ? -2 : 2))
					enPassant = Program.move;
				hasMoved = true;
			}
			else
				enPassant = null;

			

			if(this.isWhite ? pos.row == 0 : pos.row == 7)
			{
				promoting = true;
				string piece = string.Empty;
				do
				{
					Console.SetCursorPosition(4, 45);
					Console.Write("Which Piece to promote to? (Q, R, B, N): ");
					piece = Console.ReadLine();

				} while (!(new string[] { "Q", "R", "B", "N" }.Contains(piece.ToUpper())));

				promotePiece = piece;

				base.Move(pos);

				Board.pieces.Remove(this);

				switch(piece.ToUpper())
				{
					case "Q":
						Board.pieces.Add(new Queen(this.isWhite, this.position));
						break;
					case "R":
						Board.pieces.Add(new Rook(this.isWhite, this.position));
						break;
					case "B":
						Board.pieces.Add(new Bishop(this.isWhite, this.position));
						break;
					case "N":
						Board.pieces.Add(new Knight(this.isWhite, this.position));
						break;
				}

				if (!Board.BoardIsRotated)
				{
					Console.SetCursorPosition(pos.column * 11 + 9, pos.row * 5 + 4);
					Console.BackgroundColor = pos.row % 2 == 0 ? (pos.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (pos.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
				}
				else
				{
					Console.SetCursorPosition((7 - pos.column) * 11 + 9, (7 - pos.row) * 5 + 4);
					Console.BackgroundColor = pos.row % 2 != 0 ? (pos.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (pos.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
				}
				Console.ForegroundColor = this.isWhite ? ConsoleColor.White : ConsoleColor.Black;
				Console.Write(piece.ToUpper());

				Console.ResetColor();

				Console.SetCursorPosition(0, 45);
				for(int i = 0; i < 47; i++)
				{
					Console.Write(" ");
				}
			}
		}

		public override List<Position> GenerateLegalMoves()
		{
			List<Position> moves = new List<Position>();
			Position currentPos;
			Position defaultPos = new Position(this.position.row, this.position.column);
			int steps = hasMoved ? 1 : 2;

			for(int i = 1; i <= steps; i++)
			{
				currentPos = isWhite ? new Position(this.position.row - i, this.position.column) : new Position(this.position.row + i, this.position.column);
				if (Board.pieces.Find(x => x.position.Equals(currentPos)) != null)
					break;
				if(OnBoardAndValid(currentPos))
				{
					this.position.CopyPositionFrom(currentPos);
					switch(isWhite)
					{
						case true:
							if (!Board.WhiteKing.InCheck())
								moves.Add(currentPos);
							break;
						case false:
							if (!Board.BlackKing.InCheck())
								moves.Add(currentPos);
							break;
					}
					this.position.CopyPositionFrom(defaultPos);
				}
			}

			for(int i = -1; i <= 1; i += 2)
			{
				currentPos = new Position(isWhite ? this.position.row - 1 : this.position.row + 1, this.position.column + i);
				if (Board.pieces.Find(x => x.position.Equals(currentPos) && x.isWhite != this.isWhite) != null)
				{
					this.position.CopyPositionFrom(currentPos);
					switch (isWhite)
					{
						case true:
							if (!Board.WhiteKing.InCheck())
								moves.Add(currentPos);
							break;
						case false:
							if (!Board.BlackKing.InCheck())
								moves.Add(currentPos);
							break;
					}
					this.position.CopyPositionFrom(defaultPos);
				}
			}

			return moves;
		}

		public override List<Position> GenerateSimplifiedMoves()
		{
			List<Position> moves = new List<Position>();
			Position currentPos;
			int steps = hasMoved ? 1 : 2;

			for (int i = -1; i <= 1; i += 2)
			{
				currentPos = new Position(isWhite ? this.position.row - 1 : this.position.row + 1, this.position.column + i);
				if (Board.pieces.Find(x => x.position.Equals(currentPos) && x.isWhite != this.isWhite) != null)
				{
					moves.Add(currentPos);
				}
			}

			return moves;
		}
	}
}
