﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
	public class Program
	{
		public class Node
		{
			public string BoardRepresentation { get; set; }

			public int BestRow { get; set; }
			public int BestCol { get; set; }

			public Node BestChild { get; set; }

			public bool BoardEquals(char[][] board)
			{
				var representation = CreateRepresentation(board);

				return this.BoardRepresentation.Equals(representation);
			}

			public static string CreateRepresentation(char[][] board)
			{
				var representation = string.Join(string.Empty, board
					.SelectMany(row => row)
					.Select(cell => cell));

				return representation;
			}
		}

		public const int BoardSize = 3;

		public const char Person = 'X';
		public const char Computer = 'O';
		public const char EmptyCell = '-';

		public static void Main(string[] args)
		{
			var board = InitBoard();

			Console.WriteLine("You are X, play first?");
			var player = Console.ReadLine() == "y" ? Person : Computer;

			var node = (Node)null;

			while (!IsGameOver(board))
			{
				var row = -1;
				var col = -1;

				if (player == Person)
				{
					var input = Console.ReadLine()
						.Split()
						.Select(int.Parse)
						.ToList();

					row = input[0];
					col = input[1];
				}
				else
				{
					if (node == null || node.BestChild == null || !node.BestChild.BoardEquals(board))
					{
						(_, node) = Minimax(board, false, int.MinValue, int.MaxValue);
					}

					if (node.BestChild.BoardEquals(board))
					{
						node = node.BestChild;
					}

					row = node.BestRow;
					col = node.BestCol;

					node = node.BestChild;
				}

				MakeMove(board, player, row, col);

				Console.WriteLine($"After {player} plays {row}-{col}");
				PrintBoard(board);
				player = GetNextPlayer(player);
			}

			var message = "DRAW";

			if (!IsDraw(board))
			{
				var isMeWinner = IsWin(board, Person);

				if (isMeWinner)
				{
					message = "YOU WIN";
				}
				else
				{
					message = "YOU LOSE";
				}
			}

			Console.WriteLine(message);
		}

		public static (int score, Node node) Minimax(char[][] board, bool isMaxPlayer, int alpha, int beta)
		{
			var bestRow = -1;
			var bestCol = -1;

			if (IsGameOver(board))
			{
				var points = GetPoints(board);

				var node = new Node {
					BestRow = bestRow,
					BestCol = bestCol,
					BoardRepresentation = Node.CreateRepresentation(board)
				};

				return (points, node);
			}

			var newNode = (Node)null;

			var nextMoves = GetNextMoves(board);

			foreach (var (row, col) in nextMoves)
			{
				board = MakeMove(board, isMaxPlayer ? Person : Computer, row, col);

				var (score, nextNode) = Minimax(board, !isMaxPlayer, alpha, beta);

				board = ReverseMove(board, row, col);

				if (isMaxPlayer)
				{
					if (score > alpha)
					{
						alpha = score;
						bestRow = row;
						bestCol = col;

						newNode = new Node {
							BestRow = bestRow,
							BestCol = bestCol,
							BoardRepresentation = Node.CreateRepresentation(board),
							BestChild = nextNode
						};
					}

					if (alpha >= beta)
					{
						var node = new Node {
							BestRow = row,
							BestCol = col,
							BoardRepresentation = Node.CreateRepresentation(board),
							BestChild = nextNode
						};

						return (alpha, node);
					}
				}
				else
				{
					if (score < beta)
					{
						beta = score;
						bestRow = row;
						bestCol = col;

						newNode = new Node {
							BestRow = bestRow,
							BestCol = bestCol,
							BoardRepresentation = Node.CreateRepresentation(board),
							BestChild = nextNode
						};
					}

					if (alpha >= beta)
					{
						var node = new Node {
							BestRow = row,
							BestCol = col,
							BoardRepresentation = Node.CreateRepresentation(board),
							BestChild = nextNode
						};

						return (beta, node);
					}
				}
			}

			var bestScore = isMaxPlayer ? alpha : beta;

			return (bestScore, newNode);
		}

		public static List<(int row, int col)> GetNextMoves(char[][] board)
		{
			var moves = new List<(int row, int col)>();

			for (int i = 0; i < BoardSize; i++)
			{
				for (int j = 0; j < BoardSize; j++)
				{
					if (board[i][j] == EmptyCell)
					{
						moves.Add((i, j));
					}
				}
			}

			return moves;
		}

		public static int GetPoints(char[][] board)
		{
			if (IsDraw(board))
			{
				return 0;
			}

			var emptyCellsCount = board
				.SelectMany(cell => cell)
				.Where(cell => cell == EmptyCell)
				.Count();

			if (IsWin(board, Person))
			{
				return 1 + emptyCellsCount;
			}

			return -1 - emptyCellsCount;
		}

		public static bool IsDraw(char[][] board)
		{
			if (!IsWin(board, Person) && !IsWin(board, Computer) && IsBoardFull(board))
			{
				return true;
			}

			return false;
		}

		public static char GetNextPlayer(char player)
		{
			return player == Person ? Computer : Person;
		}

		public static char[][] MakeMove(char[][] board, char player, int row, int col)
		{
			board[row][col] = player;

			return board;
		}

		public static char[][] ReverseMove(char[][] board, int row, int col)
		{
			board[row][col] = EmptyCell;

			return board;
		}

		public static char[][] GetPredefinedBoard()
		{
			var board = new char[BoardSize][] {
				new char[3]{ EmptyCell, EmptyCell,Computer },
				new char[3]{ Computer, Computer, Person },
				new char[3]{ Person, EmptyCell, EmptyCell }
			};

			return board;
		}

		public static void PrintBoard(char[][] board)
		{
			for (int i = 0; i < BoardSize; i++)
			{
				Console.WriteLine(string.Join(' ', board[i]));
			}

			Console.WriteLine();
		}

		public static char[][] InitBoard()
		{
			var board = new char[BoardSize][];
			for (int i = 0; i < BoardSize; i++)
			{
				board[i] = Enumerable.Repeat(EmptyCell, BoardSize)
					.ToArray();
			}

			return board;
		}

		public static bool IsBoardFull(char[][] board)
		{
			var isFull = !board
				.SelectMany(b => b)
				.Any(c => c == EmptyCell);

			return isFull;
		}

		public static bool IsGameOver(char[][] board)
		{
			if (IsWin(board, Person) || IsWin(board, Computer))
			{
				return true;
			}

			return IsDraw(board);
		}

		public static bool IsWin(char[][] board, char player)
		{
			var isLeftDiagonalSolution = true;
			var isRightDiagonalSolution = true;

			for (int j = 0; j < BoardSize; j++)
			{
				if (!isLeftDiagonalSolution && !isRightDiagonalSolution)
				{
					break;
				}

				if (isLeftDiagonalSolution && board[j][j] != player)
				{
					isLeftDiagonalSolution = false;
				}

				if (isRightDiagonalSolution && board[BoardSize - 1 - j][j] != player)
				{
					isRightDiagonalSolution = false;
				}
			}

			if (isLeftDiagonalSolution || isRightDiagonalSolution)
			{
				return true;
			}

			for (int i = 0; i < BoardSize; i++)
			{
				var isRowSolution = true;
				var isColSolution = true;

				for (int j = 0; j < BoardSize; j++)
				{
					if (!isRowSolution && !isColSolution)
					{
						break;
					}

					if (isRowSolution && board[i][j] != player)
					{
						isRowSolution = false;
					}

					if (isColSolution && board[j][i] != player)
					{
						isColSolution = false;
					}
				}

				if (isRowSolution || isColSolution)
				{
					return true;
				}
			}

			return false;
		}
	}
}
