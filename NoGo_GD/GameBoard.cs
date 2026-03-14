using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using static NoGo.StoneColor;
using static TorchSharp.torch;

namespace NoGo
{
	public class GameBoard
	{
		public StoneColor[,] board = new StoneColor[9,9];
		public HashSet<GoString> goStrings = [];
		public StoneColor turn;
		public List<int> LegalMoves() 
		{
			List<int> moves = [];
			List<int> illegalMoves = [];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					if (board[i, j] == EMPTY)
					{
						int move = i * 9 + j;
						if (IsLegal(move))
						{
							moves.Add(move);
						}
						else
						{
							illegalMoves.Add(move);
						}
					}
				}
			}
			if (moves.Count != 0)
			{
				return moves;
			}
			else
			{
				return illegalMoves;
			}
		}
		public void ApplyMove(int move) 
		{
			int row = move / 9;
			int col = move % 9;
			board[row, col] = turn;
			HashSet<GoString> sameColor = [];
			HashSet<GoString> oppositeColor = [];
			List<Point> liberties = [];
			for (int i = 0; i < 4; i++)
			{
				int newRow = row + Directions.RowDirection[i];
				int newCol = col + Directions.ColDirection[i];
				if (newRow < 0 || newRow > 8 || newCol < 0 || newCol > 8)
				{
					continue;
				}
				if (board[newRow, newCol] == EMPTY)
				{
					liberties.Add(new Point(newRow, newCol));
				}
				else if (board[newRow, newCol] == turn)
				{
					sameColor.Add(FindString(new Point(newRow, newCol)));
				}
				else
				{
					oppositeColor.Add(FindString(new Point(newRow, newCol)));
				}
			}
			GoString newString = new(turn, [new Point(row, col)], [.. liberties]);
			foreach (GoString same in sameColor)
			{
				newString = newString.Merge(same);
				goStrings.Remove(same);
			}
			foreach (GoString opp in oppositeColor)
			{
				GoString newOpp = opp;
				goStrings.Remove(opp);
				newOpp.RemoveLiberty(new Point(row, col));
				goStrings.Add(newOpp);
			}
			goStrings.Add(newString);
			if (turn == BLACK)
			{
				turn = WHITE;
			}
			else
			{
				turn = BLACK;
			}
		}
		public StoneColor IsTerminal() 
		{
			foreach (var goString in goStrings)
			{
				if (goString.CountLiberties() == 0)
				{
					return turn;
				}
			}
			return EMPTY;
		}
		public Tensor NetWorkInput() 
		{
			Tensor input = zeros([1, 2, 9, 9]);
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					if (board[i, j] == turn)
					{
						input[0, 0, i, j] = 1;
					}
					else if (board[i, j] != EMPTY)
					{
						input[0, 1, i, j] = 1;
					}
				}
			}
			return input;
		}
		private bool IsLegal(int move) 
		{
			GameBoard newBoard = new(this);
			newBoard.ApplyMove(move);
			return newBoard.IsTerminal() == EMPTY;
		}
		private GoString FindString(Point point)
		{
			foreach (GoString goString in goStrings)
			{
				if (goString.HasPoint(point))
				{
					return goString;
				}
			}
			return null;
		}
		public GameBoard() 
		{
			turn = BLACK;
			for (int i = 0; i < 9; i++) 
			{
				for (int j = 0; j < 9; j++) 
				{
					board[i, j] = EMPTY;
				}
			}
		}
		public GameBoard(GameBoard aBoard) 
		{
			turn = aBoard.turn;
			foreach (GoString goString in aBoard.goStrings) 
			{
				goStrings.Add(goString.Clone());
			}
			Array.Copy(aBoard.board, board, board.Length);
		}
	}
}
