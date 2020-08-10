using System.Collections.Generic;
using System.Linq;

namespace Chess.Pieces
{
	class Pawn : Piece
	{
		public bool hasMoved = false;
		public int? enPassant = null;

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
			base.Move(pos);
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
