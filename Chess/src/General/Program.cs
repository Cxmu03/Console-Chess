using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Media;
using System.Configuration;

namespace Chess
{
	class Program
	{
		public static int move = 1;
		public static int halfMoves = 0;
		public static int currentPlayer;
		public static bool currentPlayerIsWhite = true;
		public static bool hasFirstCaptured = false;
		public static bool playSound;
		public static string uciMoves = string.Empty;
		public static string engineDepth;
		public static string startFen;
		private static SoundPlayer moveSoundPlayer = new SoundPlayer(Properties.Resources.Move);
		
		[STAThread]
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
			initializeVars();
			Menu.MainMenu();
		}
		
		public static void Game()
		{
			if (File.Exists("temp.txt"))
				File.Delete("temp.txt");
			List<string> positions = new List<string>();
			List<string> declinedDraws = new List<string>();
			Random r = new Random();
			currentPlayer = r.Next(0, 2);

			string move;
			string playerName = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Split('\\').Last();
			string drawReason = string.Empty;

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
			if(Notator.pgnSaving)
				Notator.Initialize();

			Board.Initialize();
			startFen = Notator.CreateFen();
			Console.Clear();
			Console.Write($"    White to move\n\n");
			Board.DrawBoard();
			//Main game loop
			while (checkmate == false)
			{
				currentPlayerIsWhite = currentPlayer % 2 == 0;

				Console.SetWindowSize(98, 47);
				Console.SetCursorPosition(4, 0);
				Console.WriteLine(currentPlayerIsWhite ? "White" : "Black");

				//Draw after 50 moves without capture
				if (hasFirstCaptured)
					halfMoves++;
				//Debug.WriteLine(Notator.CreateFen());
				string currentFen = Notator.CreateFen();
				var currentFenArr = currentFen.Split(' ');
				Debug.WriteLine($"Adding {string.Join(" ", currentFenArr.Where((item, index) => index < currentFenArr.Length - 2).ToArray())}");
				if (currentPlayer > 0)
					positions.Add(string.Join(" ", currentFenArr.Where((item, index) => index < currentFenArr.Length - 2).ToArray()));

				var query = positions.GroupBy(x => x)
									 .ToDictionary(x => x.Key, y => y.Count());

				if (query.ContainsValue(5))
				{
					checkmate = null;
					drawReason = "Fivefold Repetition";
					goto loopEnding;
				}

				if (query.ContainsValue(3))
				{
					bool DrawOffer = false;
					Debug.WriteLine(declinedDraws.Count);
					foreach (KeyValuePair<string, int> kv in query.Where(x => x.Value == 3))
					{
						if (!declinedDraws.Contains(kv.Key))
						{
							DrawOffer = true;
						}

					}

					//Stockfish cant agree to draws
					if (currentPlayerIsWhite == engineIsWhite)
					{
						DrawOffer = false;
						foreach (KeyValuePair<string, int> kv in query)
						{
							if (kv.Value == 3 && !declinedDraws.Contains(kv.Key))
							{
								declinedDraws.Add(kv.Key);
							}
						}
					}

					if (DrawOffer)
					{
						Console.SetCursorPosition(4, 45);
						Console.Write("Do you want to claim a draw? y/n: ");
						if (Console.ReadLine() == "y")
						{
							checkmate = null;
							drawReason = "Threefold Repetition";
							goto loopEnding;
						}
						else
						{
							foreach (KeyValuePair<string, int> kv in query)
							{
								if (kv.Value == 3 && !declinedDraws.Contains(kv.Key))
								{
									declinedDraws.Add(kv.Key);
								}
							}

							Console.SetCursorPosition(4, 45);
							for (int i = 0; i < 40; i++)
							{
								Console.Write(" ");
							}
						}
					}
				}

				if (halfMoves == 50)
				{
					checkmate = null;
				}
				else
				{
					do
					{
						Console.SetWindowSize(98, 47);
						Console.SetCursorPosition(4, 0);
						Console.WriteLine(currentPlayerIsWhite ? "White" : "Black");
						if (currentPlayerIsWhite)
						{
							if (Board.WhiteKing.InCheck())
							{
								Console.ForegroundColor = ConsoleColor.Red;
								Console.SetCursorPosition(Board.WhiteKing.position.column * 11 + 9, Board.WhiteKing.position.row * 5 + 4);
								Console.BackgroundColor = Board.WhiteKing.position.row % 2 == 0 ? (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
								if (Board.BoardIsRotated)
								{
									Console.SetCursorPosition((7 - Board.WhiteKing.position.column) * 11 + 9, (7 - Board.WhiteKing.position.row) * 5 + 4);
									Console.BackgroundColor = Board.WhiteKing.position.row % 2 != 0 ? (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
								}
								Console.Write($"K");
								Console.ResetColor();
							}
						}
						else
						{
							if (Board.BlackKing.InCheck())
							{
								Console.SetCursorPosition(Board.BlackKing.position.column * 11 + 9, Board.BlackKing.position.row * 5 + 4);
								Console.BackgroundColor = Board.BlackKing.position.row % 2 == 0 ? (Board.BlackKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
								Console.ForegroundColor = ConsoleColor.Red;
								if (Board.BoardIsRotated)
								{
									Console.SetCursorPosition((7 - Board.BlackKing.position.column) * 11 + 9, (7 - Board.BlackKing.position.row) * 5 + 4);
									Console.BackgroundColor = Board.BlackKing.position.row % 2 != 0 ? (Board.BlackKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
								}
								Console.Write($"K");
								Console.ResetColor();
							}
						}

						Console.SetCursorPosition(4, 45);

						if (currentPlayerIsWhite == engineIsWhite)
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
								System.Threading.Thread.Sleep(200);
							}

							while (IsFileLocked(new FileInfo("temp.txt")))
							{
								System.Threading.Thread.Sleep(200);
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
							if(playSound)
								moveSoundPlayer.PlaySync();
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
							switch(move)
							{
								case "dnb":
									Console.BackgroundColor = ConsoleColor.Black;
									Console.Clear();
									Console.Write("    xxxxx to move\n\n\n");
									Console.SetCursorPosition(4, 0);
									Console.WriteLine(currentPlayerIsWhite ? "White" : "Black");
									Console.SetCursorPosition(0, 2);
									Board.DrawBoard();
									moveValid = false;
									break;
								case "rtb":
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
									break;
								case "fen":
									Clipboard.SetText(Notator.CreateFen());
									break;
								case "ext":
									Menu.MainMenu();
									break;
								default:
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
												if(playSound)
													moveSoundPlayer.PlaySync();
											}
										}
									}
									break;
							}
							
							Console.SetCursorPosition(14, 45);
							for (int i = 0; i <= move.Length; i++)
							{
								Console.Write(" ");
							}

						}
					} while (!moveValid);

					//Changing the colors of the Kings back if they were in check at the start of the move
					Console.ForegroundColor = ConsoleColor.White;
					Console.SetCursorPosition(Board.WhiteKing.position.column * 11 + 9, Board.WhiteKing.position.row * 5 + 4);
					if (!Board.BoardIsRotated)
						Console.BackgroundColor = Board.WhiteKing.position.row % 2 == 0 ? (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
					else
					{
						Console.SetCursorPosition((7 - Board.WhiteKing.position.column) * 11 + 9, (7 - Board.WhiteKing.position.row) * 5 + 4);
						Console.BackgroundColor = Board.WhiteKing.position.row % 2 != 0 ? (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
					}
					Console.Write($"K");
					Console.ResetColor();

					Console.SetCursorPosition(Board.BlackKing.position.column * 11 + 9, Board.BlackKing.position.row * 5 + 4);
					if (!Board.BoardIsRotated)
						Console.BackgroundColor = Board.BlackKing.position.row % 2 == 0 ? (Board.BlackKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.WhiteKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
					else
					{
						Console.SetCursorPosition((7 - Board.BlackKing.position.column) * 11 + 9, (7 - Board.BlackKing.position.row) * 5 + 4);
						Console.BackgroundColor = Board.BlackKing.position.row % 2 != 0 ? (Board.BlackKing.position.column % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray) : (Board.BlackKing.position.column % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray);
					}
					Console.ForegroundColor = ConsoleColor.Black;
					Console.Write($"K");
					Console.ResetColor();

					checkmate = Checkmate();

					if (checkmate == false)
					{
						moveValid = false;
					}

					if (Board.WhiteRookS.hasMoved)
						Notator.WhiteShortCastleRight = false;
					if (Board.WhiteRookL.hasMoved)
						Notator.WhiteLongCastleRight = false;
					if (Board.BlackRookS.hasMoved)
						Notator.BlackShortCastleRight = false;
					if (Board.BlackRookS.hasMoved)
						Notator.BlackLongCastleRigtht = false;


					if (!currentPlayerIsWhite)
						Program.move++;
					currentPlayer++;
				}
			loopEnding:;

			}

			halfMoves++;

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
				if (drawReason == string.Empty)
					drawReason = "Stalemate";
				Console.WriteLine($"Draw by {drawReason}");
			}
			if(Notator.pgnSaving)
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

		private static void initializeVars()
		{
			engineDepth = ConfigurationManager.AppSettings.Get("StockfishDepth");
			playSound = ConfigurationManager.AppSettings.Get("playSound") == "true" ? true : false;
			Notator.pgnSaving = ConfigurationManager.AppSettings.Get("savePgn") == "true" ? true : false;
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

		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			//still existing temp.txt can cause access problems on next run
			if(File.Exists("temp.txt"))
			{
				File.Delete("temp.txt");
			}
		}
	}
}
