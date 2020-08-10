using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.IO;

namespace Chess
{
	/// <summary>
	/// Base class for all pieces
	/// </summary>
	abstract class Piece
	{
		public bool isWhite;
		public Position position;

		private Dictionary<string, char> PieceToLetter = new Dictionary<string, char>()
		{
			{"Chess.Pieces.King", 'K'},
			{"Chess.Pieces.Queen", 'Q'},
			{"Chess.Pieces.Knight", 'N'},
			{"Chess.Pieces.Bishop", 'B'},
			{"Chess.Pieces.Rook", 'R'},
			{"Chess.Pieces.Pawn", 'P'}
		};

		private Dictionary<int, string> columnToLetter = new Dictionary<int, string>()
		{
			{0, "A"},
			{1, "B"},
			{2, "C"},
			{3, "D"},
			{4, "E"},
			{5, "F"},
			{6, "G"},
			{7, "H"},
		};

		/// <summary>
		/// Generates all possible Legal moves for a given piece
		/// </summary>
		/// <returns>List of type Position</returns>
		public abstract List<Position> GenerateLegalMoves();

		/// <summary>
		/// Generates all possible moves of a piece without checking if the king is in check
		/// to prevent a stack overflow because of recursive function calling between Piece.GenerateLegalMoves and King.InCheck
		/// </summary>
		public abstract List<Position> GenerateSimplifiedMoves();

		/// <summary>
		/// Moves the piece to a desired location
		/// </summary>
		/// <param name="pos", type=Position></param>
		public virtual void Move(Position pos)
		{
			string fileOutput = string.Empty;
			string move = Program.currentPlayerIsWhite ? $"{Program.move.ToString()}. " : "";

			if(this.GetType().ToString().GetHashCode() != "Chess.Pieces.Pawn".GetHashCode())
				fileOutput += PieceToLetter[this.GetType().ToString()];

			List<Piece> temp = new List<Piece>();

			foreach(Piece p in Board.pieces.Where(x => x.GetType().ToString().GetHashCode() == this.GetType().ToString().GetHashCode() && x.isWhite == this.isWhite && x.GenerateLegalMoves().Count(y => y.Equals(pos)) > 0 && x != this))
			{
				temp.Add(p);
			}

			bool onSameRow = false;
			bool onSameCol = false;
			if (temp.Find(x => x.position.row == this.position.row) != null)
			{
				fileOutput += columnToLetter[this.position.column];
				onSameRow = true;
			}
			if (temp.Find(x => x.position.column == this.position.column) != null)
			{
				fileOutput += this.position.row.ToString();
				onSameCol = true;
			}
			if (onSameRow == false && onSameCol == false && temp.Count() > 0)
				fileOutput += columnToLetter[this.position.column];

			//capturing a piece
			if (Board.pieces.Find(piece => piece.position.Equals(pos)) != null)
			{
				if(this.GetType().ToString().GetHashCode() == "Chess.Pieces.Pawn".GetHashCode() && fileOutput == string.Empty)
				{
					fileOutput += columnToLetter[this.position.column];
				}
				Board.pieces.Remove((Piece)Board.pieces.Find(x => x.position.Equals(pos)));
				fileOutput += "x";
			}

			fileOutput += $"{columnToLetter[pos.column]}{8 - pos.row}";

			if (!Board.BoardIsRotated)
			{
				Console.SetCursorPosition(this.position.column * 11 + 9, this.position.row * 5 + 4);
				Console.BackgroundColor = pos.row % 2 == 0 ? (pos.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (pos.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
			}
			else
			{
				Console.SetCursorPosition((7 - this.position.column) * 11 + 9, (7 - this.position.row) * 5 + 4);
				Console.BackgroundColor = pos.row % 2 != 0 ? (pos.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (pos.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
			}
			
			//Console.BackgroundColor = this.position.row % 2 == 0 ? (this.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (this.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
			Console.Write(" ");

			this.position.row = pos.row;
			this.position.column = pos.column;
			
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
			Console.Write(PieceToLetter[this.GetType().ToString()]);

			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.White;

			if (Program.currentPlayerIsWhite)
			{
				if (Board.BlackKing.InCheck() && Program.Checkmate() == false)
					fileOutput += "+ ";
				else if (Board.BlackKing.InCheck() && Program.Checkmate() == true)
					fileOutput += "# ";
			}
			else
			{
				if (Board.WhiteKing.InCheck() && Program.Checkmate() == false)
					fileOutput += "+ ";
				else if (Board.WhiteKing.InCheck() && Program.Checkmate() == true)
					fileOutput += "# ";
			}

			fileOutput = move + fileOutput + " ";

			Notator.pgnString += fileOutput;
			if(Notator.pgnString.Length % 80 > 70 && Notator.pgnString.Length > 0)
			{
				Notator.pgnString += "\n";
			}
			//Program.AppendToPgnFile(fileOutput);
		}

		/// <summary>
		/// copies this piece to another piece p
		/// </summary>
		/// <param name="p", type=Piece></param>
		protected virtual void CopyPieceTo(Piece p)
		{
			p.isWhite = this.isWhite;
			p.position.CopyPositionFrom(this.position);
		}

		protected bool OnBoardAndValid(Position p) => (Enumerable.Range(0, 8).Contains(p.row) && Enumerable.Range(0, 8).Contains(p.column)) && (Board.pieces.Find(piece => piece.position.Equals(p)) == null || Board.pieces.Find(piece => piece.position.Equals(p) && piece.isWhite != this.isWhite) != null);

		public void ShowMoves()
		{
			List<Position> moves = GenerateLegalMoves();
			foreach (Position p in moves)
				p.ShowPosition(string.Empty);
		}

		/// <summary>
		/// Is used for the Queen and the Bishop to generate a new Position given one of the 4 direction values
		/// </summary>
		/// <param name="direction", type=int></param>
		/// <param name="i", type=int></param>
		/// <returns>Position</returns>
		protected Position PositionFromDirection(int direction, int i)
		{
			Position currentPos = null;
			switch (direction)
			{
				case 0:
					currentPos = new Position(this.position.row + i, this.position.column + i);
					break;
				case 1:
					currentPos = new Position(this.position.row + i, this.position.column - i);
					break;
				case 2:
					currentPos = new Position(this.position.row - i, this.position.column + i);
					break;
				case 3:
					currentPos = new Position(this.position.row - i, this.position.column - i);
					break;
			}
			return currentPos;
		}
	}
}
