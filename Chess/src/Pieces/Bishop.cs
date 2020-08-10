using System.Collections.Generic;

namespace Chess.Pieces
{
	class Bishop : Piece
	{
		public Bishop(bool isWhite, Position position)
		{
			this.isWhite = isWhite;
			this.position = position;
		}

		public override List<Position> GenerateLegalMoves()
		{
			List<Position> moves = new List<Position>();
			Position currentPos;
			Position defaultPos = new Position(this.position.row, this.position.column);

			for (int direction = 0; direction < 4; direction++)
			{
				for(int i = 1; i < 8; i++)
				{
					currentPos = PositionFromDirection(direction, i);
					if(OnBoardAndValid(currentPos))
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

			return moves;
		}

		public override List<Position> GenerateSimplifiedMoves()
		{
			List<Position> moves = new List<Position>();
			Position currentPos;
			for (int direction = 0; direction < 4; direction++)
			{
				for (int i = 1; i < 8; i++)
				{
					currentPos = PositionFromDirection(direction, i);
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

			return moves;
		}

	}
}
