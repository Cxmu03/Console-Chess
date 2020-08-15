using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace Chess.Pieces
{
	class Pawn : Piece
	{
		public bool hasMoved = false;
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
				if(pos.row == this.position.row + (this.isWhite ? -2 : 2))
					enPassant = Program.currentPlayer;
				hasMoved = true;
			}
			else
				enPassant = null;

			//Promoting
			if(this.isWhite ? pos.row == 0 : pos.row == 7)
			{
				promoting = true;
				string piece = string.Empty;
				if (promotePiece != null)
				{
					do
					{
						Console.SetCursorPosition(4, 45);
						Console.Write("Which Piece to promote to? (Q, R, B, N): ");
						piece = Console.ReadLine();

					} while (!(new string[] { "Q", "R", "B", "N" }.Contains(piece.ToUpper())));

					promotePiece = piece;
				}

				base.Move(pos);

				Board.pieces.Remove(this);

				switch(promotePiece.ToUpper())
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
				Console.Write(promotePiece.ToUpper());

				Console.ResetColor();
				
				Console.SetCursorPosition(0, 45);
				for(int i = 0; i < 47; i++)
				{
					Console.Write(" ");
				}

				promotePiece = null;
			}
			else
			{
				//En Passant
				if(Board.pieces.Find(x => x.position.Equals(pos)) == null && pos.column != this.position.column)
				{
					Piece toRemove = Board.pieces.Find(x => x.position.Equals(new Position(this.position.row, pos.column)));
					if (!Board.BoardIsRotated)
					{
						Console.SetCursorPosition(toRemove.position.column * 11 + 9, toRemove.position.row * 5 + 4);
						Console.BackgroundColor = toRemove.position.row % 2 == 0 ? (toRemove.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (toRemove.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
					}
					else
					{
						Console.SetCursorPosition((7 - toRemove.position.column) * 11 + 9, (7 - toRemove.position.row) * 5 + 4);
						Console.BackgroundColor = toRemove.position.row % 2 != 0 ? (toRemove.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (toRemove.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
					}
					Console.Write(" ");
					Board.pieces.RemoveAll(x => x.position.Equals(new Position(this.position.row, pos.column)));
					Program.halfMoves = 0;
				}

				base.Move(pos);
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

			//Looking at possible En Passant moves
			for(int i = -1; i <= 1; i += 2)
			{
				currentPos = new Position(this.position.row, this.position.column + i);
				//Looping through the list backwards so I can modify it in the loop without getting an error
				for(int j = Board.pieces.Count - 1; j >= 0; j--)
				{
					if(Board.pieces[j].isWhite != this.isWhite && Board.pieces[j].position.Equals(currentPos))
					{
						if (Board.pieces[j].enPassant == Program.currentPlayer - 1)
						{
							Pawn p = new Pawn(Board.pieces[j].isWhite, new Position(Board.pieces[j].position.row, Board.pieces[j].position.column));
							Board.pieces.RemoveAt(j);
							this.position.CopyPositionFrom(currentPos);
							switch (isWhite)
							{
								case true:
									if (!Board.WhiteKing.InCheck())
										moves.Add(new Position(this.isWhite ? defaultPos.row - 1 : defaultPos.row + 1, currentPos.column));
									break;
								case false:
									if (!Board.BlackKing.InCheck())
										moves.Add(new Position(this.isWhite ? defaultPos.row - 1 : defaultPos.row + 1, currentPos.column));
									break;
							}
							this.position.CopyPositionFrom(defaultPos);
							Board.pieces.Add(p);
						}
					}
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
