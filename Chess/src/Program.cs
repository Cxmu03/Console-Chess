using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chess.Pieces;

namespace Chess
{
	class Program
	{
		public static int move = 1;
		public static bool currentPlayerIsWhite = true;


		static void Main(string[] args)
		{
			Random r = new Random();
			int currentPlayer = r.Next(0, 2);
			string firstPlayerString = currentPlayer == 0 ? "White" : "Black";
			string move;
			if(currentPlayer == 1)
			{
				Board.BoardIsRotated = true;
			}
			bool moveValid = false;
			bool? checkmate = false;
			MoveInformation currentMove;
			Piece currentPiece = null;

			Console.SetWindowSize(98, 47);
			

			Notator.Initialize();

			Board.Initialize();
			Console.Clear();
			Console.Write($"    {firstPlayerString} to move\n\n");
			Board.DrawBoard();
			//Main game loop
			while (checkmate == false)
			{
				do
				{

					Console.SetWindowSize(98, 47);
					currentPlayerIsWhite = currentPlayer % 2 == 0;
					Console.SetCursorPosition(4, 0);
					Console.WriteLine(currentPlayerIsWhite ? "White" : "Black");
					//Console.SetCursorPosition(0, 1);
					if (currentPlayerIsWhite)
					{
						if (Board.WhiteKing.InCheck())
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.SetCursorPosition(Board.WhiteKing.position.column * 11 + 9, Board.WhiteKing.position.row * 5 + 4);
							Console.BackgroundColor = Board.WhiteKing.position.row % 2 == 0 ? (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
							Console.Write(" ");
							Console.SetCursorPosition(Board.WhiteKing.position.column * 11 + 9, Board.WhiteKing.position.row * 5 + 4);
							Console.Write($"K");
							Console.BackgroundColor = ConsoleColor.Black;
							Console.ForegroundColor = ConsoleColor.White;
						}

					}
					else
					{
						if (Board.BlackKing.InCheck())
						{
							Console.SetCursorPosition(Board.BlackKing.position.column * 11 + 9, Board.BlackKing.position.row * 5 + 4);
							Console.BackgroundColor = Board.BlackKing.position.row % 2 == 0 ? (Board.BlackKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
							Console.Write(" ");
							Console.SetCursorPosition(Board.BlackKing.position.column * 11 + 9, Board.BlackKing.position.row * 5 + 4);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write($"K");
							Console.BackgroundColor = ConsoleColor.Black;
							Console.ForegroundColor = ConsoleColor.White;
						}
					}
					
					Console.SetCursorPosition(4, 45);
					Console.Write("Your Move: ");
					move = Console.ReadLine();
					if (move.GetHashCode() != "rnb".GetHashCode() && move.GetHashCode() != "flb".GetHashCode())
					{
						currentMove = Position.parseInputToPosition(move, currentPlayerIsWhite);
						if (currentMove == null)
						{
							moveValid = false;
						}
						else if (currentMove.castled)
						{
							moveValid = true;
						}
						else
						{
							currentPiece = Board.pieces.Find(x => x.GetType().ToString() == currentMove.pieceName && x.position.Equals(currentMove.currentPosition));
							if (currentPiece != null && currentPiece.isWhite == currentPlayerIsWhite)
							{
								if (currentPiece.GenerateLegalMoves().Find(x => x.Equals(currentMove.desiredPosition)) != null)
								{
									moveValid = true;
									currentPiece.Move(currentMove.desiredPosition);
								}
							}
						}
					}
					else if(move.GetHashCode() == "rnb".GetHashCode())
					{
						Console.BackgroundColor = ConsoleColor.Black;
						Console.Clear();
						Console.Write("    xxxxx to move\n\n\n");
						Console.SetCursorPosition(4, 0);
						Console.WriteLine(currentPlayerIsWhite ? "White" : "Black");
						Console.SetCursorPosition(0, 2);
						Board.DrawBoard();
						moveValid = false;
					}
					else if(move.GetHashCode() == "flb".GetHashCode())
					{
						Board.BoardIsRotated = Board.BoardIsRotated ? false : true;
						Console.Clear();
						Console.BackgroundColor = ConsoleColor.Black;
						Console.Clear();
						Console.Write("    xxxxx to move\n\n\n");
						Console.SetCursorPosition(4, 0);
						Console.WriteLine(currentPlayerIsWhite ? "White" : "Black");
						Console.SetCursorPosition(0, 2);
						Board.DrawBoard();
						moveValid = false;
					}

					Console.SetCursorPosition(14, 45);
					for (int i = 0; i <= move.Length; i++)
					{
						Console.Write(" ");
					}
				} while (!moveValid);



				checkmate = Checkmate();

				if (checkmate == false)
				{
					currentPlayer++;
					moveValid = false;
					//Clearing the en passant status of all pawns that had it last move
					/*foreach(Pawn p in Board.pieces.Where(x => x.GetType().ToString() == "Chess.Pieces.Pawn"))
					{
						if (p.enPassant != null && p.enPassant != moveCount)
						{
							p.enPassant = null;
						}
					}*/
				}

				if (!currentPlayerIsWhite)
					Program.move++;

			}

			Console.Clear();

			int result = 0;

			if (checkmate == true)
			{
				if (currentPlayerIsWhite)
				{
					Console.WriteLine("White won by checkmate");
					result = 1;
				}
				else
				{
					Console.WriteLine("Black won by checkmate");
					result = 2;
				}
			}
			else if (checkmate == null)
			{
				Console.WriteLine("Draw by stalemate");
			}
			Notator.FinishNotation(result);

			Console.ReadKey();
		}

		public static bool? Checkmate()
		{
			int numOfMoves = 0;
			
			foreach(Piece p in Board.pieces.Where(x => x.isWhite != currentPlayerIsWhite))
				numOfMoves += p.GenerateLegalMoves().Count();

			if(numOfMoves == 0)
				if(!currentPlayerIsWhite)
					if (Board.WhiteKing.InCheck())
						return true;
					else
						return null;
				else
					if (Board.BlackKing.InCheck())
						return true;
					else
						return null;

			return false;
		}

		
		/*public void checkForWindowResize()
		{
			while(true)
			{
				if(windowHeight != Console.WindowHeight || windowWidth != Console.WindowWidth)
				{
					BoardRoutine();
					windowHeight = Console.WindowHeight;
					windowWidth = Console.WindowWidth;
				}
			}
		}*/
	}
}
