﻿using System.Collections.Generic;

namespace Chess.Pieces
{
	class Rook : Piece
	{
		public bool hasMoved = false;

		public Rook(bool isWhite, Position position)
		{
			this.isWhite = isWhite;
			this.position = position;
		}

		protected override void CopyPieceTo(Piece p)
		{
			base.CopyPieceTo(p);
		}

		public override void Move(Position pos)
		{
			if (!hasMoved) hasMoved = true;
			base.Move(pos);
		}

		public override List<Position> GenerateLegalMoves()
		{
			List<Position> moves = new List<Position>();
			Position currentPos = new Position(this.position.row, this.position.column);
			Position defaultPos = new Position(this.position.row, this.position.column);
			for (int direction = 0; direction <= 1; direction++)
			{
				for (int steps = 0; steps <= 1; steps++)
				{
					for(int i = 1; i < 8; i++)
					{
						currentPos = direction == 0 ? new Position(steps == 0 ? this.position.row + i : this.position.row - i, this.position.column) : new Position(this.position.row, steps == 0 ? this.position.column + i : this.position.column - i);
						if (OnBoardAndValid(currentPos))
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
						else
						{
							break;
						}
						if (Board.pieces.Find(x => x.position.Equals(currentPos)) != null)
						{
							break;
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

			for (int direction = 0; direction <= 1; direction++)
			{
				for (int steps = 0; steps <= 1; steps++)
				{
					for (int i = 1; i < 8; i++)
					{
						currentPos = direction == 0 ? new Position(steps == 0 ? this.position.row + i : this.position.row - i, this.position.column) : new Position(this.position.row, steps == 0 ? this.position.column + i : this.position.column - i);
						if (OnBoardAndValid(currentPos))
						{
							moves.Add(currentPos);
						}
						else
						{
							break;
						}
						if (Board.pieces.Find(x => x.position.Equals(currentPos)) != null)
						{
							break;
						}
					}
				}
			}

			return moves;
		}
	}
}
