using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace Chess
{
	class StockfishPipe
	{
		public static string moves = string.Empty;

		static Process stockfish = new Process()
		{
			StartInfo =
			{
				FileName = "stockfish.exe",
				CreateNoWindow = true,
				UseShellExecute = false
			}
		};
		
		public static string GetNextMove()
		{
			using (var pipeRead = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
			{
				using (var pipeWrite = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
				{
					stockfish.StartInfo.Arguments = pipeRead.GetClientHandleAsString() + " " + pipeWrite.GetClientHandleAsString();
					stockfish.Start();

					pipeWrite.DisposeLocalCopyOfClientHandle();
					pipeRead.DisposeLocalCopyOfClientHandle();

					try
					{
						using (var sw = new StreamWriter(pipeWrite))
						{
							sw.Write($"position startpos moves {moves}");
							sw.Write("go depth 10");
						}

						using (var sr = new StreamReader(pipeRead))
						{
							string temp = null;

							do
							{
								temp = sr.ReadLine();
							} while (temp == null);

							while((temp = sr.ReadLine()) != null && !temp.StartsWith("bestmove"))
							{
								return temp.Split(' ')[1];
							}
						}


					}
					catch(Exception)
					{
						Console.Clear();
						Console.WriteLine("Fatal Error...\nAborting Game...");
					}
					finally
					{
						stockfish.WaitForExit();
						stockfish.Close();
					}
				}
			}

			return string.Empty;
		}
	}
}
