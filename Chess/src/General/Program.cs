using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Chess
{
	class Program
	{
		public static int move = 1;
		public static bool currentPlayerIsWhite = true;
		public static string uciMoves = string.Empty;
		public static string engineDepth = "10";


		static void Main(string[] args)
		{
			Random r = new Random();
			int currentPlayer = r.Next(0, 2);

			string move;
			string playerName = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Split('\\').Last();

			bool engineIsWhite = false;
			bool moveValid = true;
			bool? checkmate = false;

			MoveInformation currentMove;
			Piece currentPiece = null;

			Console.SetWindowSize(98, 47);

			if (currentPlayer == 1)
			{
				Board.BoardIsRotated = true;
				engineIsWhite = true;
				Notator.whitePlayer = $"Stockfish level {engineDepth}";
				Notator.blackPlayer = playerName;
			}
			else
			{
				Notator.blackPlayer = $"Stockfish level {engineDepth}";
				Notator.whitePlayer = playerName;
			}

			currentPlayer = 0;
			Notator.Initialize();

			Board.Initialize();
			Console.Clear();
			Console.Write($"    White to move\n\n");
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
					 
					if (currentPlayerIsWhite == true && engineIsWhite == true || currentPlayerIsWhite == false && engineIsWhite == false)
					{
						Console.WriteLine("Engine is thinking");
						Process stockfish = new Process();
						stockfish.StartInfo.CreateNoWindow = true;
						stockfish.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						stockfish.StartInfo.FileName = "stockfish_connector.exe";
						stockfish.StartInfo.Arguments = uciMoves == string.Empty ? $"noMove {engineDepth}" : uciMoves + $" {engineDepth}";
						Debug.WriteLine($"Uci Moves: {uciMoves}");
						stockfish.Start();

						while (!File.Exists("temp.txt"))
						{
							;
						}

						while(IsFileLocked(new FileInfo("temp.txt")))
						{
							;
						}

						System.Threading.Thread.Sleep(100);

						using (var reader = new StreamReader("temp.txt"))
						{
							move = reader.ReadLine();
						}

						
						File.Delete("temp.txt");
						stockfish.Close();
						Position pos1 = Position.NotationToPosition(move.Substring(0, 2));
						Position pos2 = Position.NotationToPosition(move.Substring(2, 2));
						Debug.WriteLine($"Move: {move}\nMoving from {pos1.row}, {pos1.column} to {pos1.row}, {pos1.column}");
						Board.pieces.Find(x => x.position.Equals(Position.NotationToPosition(move.Substring(0, 2)))).Move(Position.NotationToPosition(move.Substring(2, 2)));

						Console.SetCursorPosition(14, 45);

						for (int i = 0; i <= "Engine is thinking".Length; i++)
						{
							Console.Write(" ");
						}

						moveValid = true;
					}
					else
					{
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
						else if (move.GetHashCode() == "rnb".GetHashCode())
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
						else if (move.GetHashCode() == "flb".GetHashCode())
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

					}
				} while (!moveValid);
				

				checkmate = Checkmate();

				if (checkmate == false)
				{
					
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

				currentPlayer++;

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

		private static bool IsFileLocked(FileInfo file)
		{
			try
			{
				using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
				{
					stream.Close();
				}
			}
			catch (IOException)
			{
				return true;
			}
			
			return false;
		}
	}
}
