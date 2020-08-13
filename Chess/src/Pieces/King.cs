using System.Collections.Generic;

namespace Chess.Pieces
{
	class King : Piece
	{
		public bool hasMoved = false;
		public static bool castlingL;
		public static bool castlingS;

		public King(bool isWhite, Position position)
		{
			this.isWhite = isWhite;
			this.position = position;
		}

		public override void Move(Position pos)
		{
			if (!hasMoved) hasMoved = true;
			base.Move(pos);
		}

		/// <summary>
		/// Checks if the king is in check
		/// </summary>
		/// <returns>boolean value</returns>
		public bool InCheck()
		{
			bool inCheck = false;
			//Going over all possible moves
			foreach (Piece p in Board.pieces)
				if (p.GetType().ToString() != "Chess.Pieces.King" && p.isWhite != this.isWhite)
					if (p.GenerateSimplifiedMoves().Find(x => x.Equals(this.position)) != null)
						inCheck = true;
			//Preventing a recursive call of the two Functions "InCheck" and "GenerateLegalMoves"
			if (this.isWhite)
				if(Board.BlackKing.GenerateSimplifiedMoves().Find(x => x.Equals(this.position)) != null)
					inCheck = true;
			else if(!isWhite)
				if(Board.WhiteKing.GenerateSimplifiedMoves().Find(x => x.Equals(this.position)) != null)
					inCheck = true;

			return inCheck;
		}

		public override List<Position> GenerateLegalMoves()
		{
			List<Position> moves = new List<Position>();
			Position defaultPos = new Position(this.position.row, this.position.column);
			Position currentPos = new Position(this.position.row, this.position.column);

			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					currentPos = new Position(this.position.row + i, this.position.column + j);
					if (!currentPos.Equals(this.position))
					{
						if (OnBoardAndValid(currentPos))
						{
							this.position.CopyPositionFrom(currentPos);
							if (!InCheck())
							{
								moves.Add(currentPos);
							}
							this.position.CopyPositionFrom(defaultPos);
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
			for(int i = -1; i <= 1; i++)
			{
				for(int j = -1; j <= 1; j++)
				{
					currentPos = new Position(this.position.row + i, this.position.column + j);
					if(OnBoardAndValid(currentPos))
					{ 
							moves.Add(currentPos);
					}
				}
			}
			return moves;
		}

		public bool Castle(bool isShort)
		{
			
			if (isShort)
			{
				castlingS = true;
				Move(new Position(this.position.row, this.position.column + 2));
				if (isWhite)
				{
					Board.WhiteRookS.Move(new Position(this.position.row, this.position.column - 1));
				}
				else
				{
					Board.BlackRookS.Move(new Position(this.position.row, this.position.column - 1));
				}
				castlingS = false;
			}
			else
			{
				castlingL = true;
				Move(new Position(this.position.row, this.position.column - 2));
				if (isWhite)
				{
						Board.WhiteRookL.Move(new Position(this.position.row, this.position.column + 1));
				}
				else
				{
						Board.BlackRookL.Move(new Position(this.position.row, this.position.column + 1));
				}
				castlingL = false;
			}
			return true;
		}

		public bool CanCastleShort()
		{
			List<Position> castleSquares = new List<Position>();
			Position defaultPos = new Position(this.position);

			if (this.hasMoved)
				return false;

			if (this.InCheck())
				return false;

			for (int i = 1; i <= 2; i++)
			{
				castleSquares.Add(new Position(this.position.row, this.position.column + i));
			}

			return CastleCheckRoutine(castleSquares, defaultPos, false);

			
		}

		public bool CanCastleLong()
		{
			List<Position> castleSquares = new List<Position>();
			Position defaultPos = new Position(this.position);
		
			if (this.hasMoved)
				return false;

			if (this.InCheck())
				return false;

			for(int i = -1; i <= -2; i--)
			{
				castleSquares.Add(new Position(this.position.row, this.position.column + i));
			}

			if (Board.pieces.Find(x => x.position.Equals(new Position(this.position.row, this.position.column - 3))) != null)
				return false;

			return CastleCheckRoutine(castleSquares, defaultPos, true);
		}

		private bool CastleCheckRoutine(List<Position> castleSquares, Position defaultPos, bool isLong)
		{
			foreach (Position pos in castleSquares)
			{
				if (Board.pieces.Find(x => x.position.Equals(pos)) != null)
					return false;
				this.position.CopyPositionFrom(pos);
				if (InCheck())
				{
					this.position.CopyPositionFrom(defaultPos);
					return false;
				}
				this.position.CopyPositionFrom(defaultPos);
			}

			
			switch (isWhite)
			{
				case true:
					if (isLong)
						if (Board.WhiteRookL.hasMoved)
							return false;
					else
						if (Board.WhiteRookS.hasMoved)
							return false;
						break;
				case false:
					if (isLong)
						if (Board.WhiteRookL.hasMoved)
							return false;
					else
						if (Board.WhiteRookS.hasMoved)
							return false;
					break;
			}

			return true;
		}
	}
}
