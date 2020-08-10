using System.Collections.Generic;

namespace Chess.Pieces
{
	class Knight : Piece
	{
		private static readonly int[][] offsets = new int[][]
		{
			new int[]{-2, -1},
			new int[]{-2, 1},
			new int[]{-1, 2},
			new int[]{1, 2},
			new int[]{2, 1},
			new int[]{2, -1},
			new int[]{1, -2},
			new int[]{-1, -2}
			
		};

		public Knight(bool isWhite, Position position)
		{
			this.isWhite = isWhite;
			this.position = position;
		}

		public override List<Position> GenerateLegalMoves()
		{
			Position currentPos;
			Position defaultPos = new Position(this.position.row, this.position.column);
			List<Position> moves = new List<Position>();

			foreach(int[] offset in offsets)
			{
				currentPos = new Position(this.position.row + offset[0], this.position.column + offset[1]);
				if(OnBoardAndValid(currentPos))
				{
					this.position.CopyPositionFrom(currentPos);
					{
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
			}
			
			return moves;
		}

		public override List<Position> GenerateSimplifiedMoves()
		{
			List<Position> moves = new List<Position>();
			Position currentPos;
			foreach(int[] offset in offsets)
			{
				currentPos = new Position(this.position.row + offset[0], this.position.column + offset[1]);
				if(OnBoardAndValid(currentPos))
				{
					moves.Add(currentPos);
				}
			}
			return moves;
		}
	}
}
