using Godot;
using NoGo;
using System;
using System.Collections.Generic;
using static NoGo.StoneColor;

public partial class Main : Node
{
	private Board board;
	private GameBoard gameBoard;
	private Agent agent = new();
	private StoneColor playerColor;
	private Stack<int> moves = new();
	private void OnColorChose(int color) 
	{
		playerColor = (StoneColor)color;
		GetNode<Node2D>("ChooseColor").Hide();
		GetNode<Node2D>("InGame").Show();
		GameStart();
	}
	private void GameStart() 
	{
		if (playerColor == BLACK) 
		{
			board.SetButtonsDisable(false);
		}
		else 
		{
			AiMove();
		}
	}
	private void AiMove() 
	{
		agent.SetBoard(new GameBoard(gameBoard));
		agent.StartThread();
	}
	private void AiMoved(int move) 
	{
		moves.Push(move);
		board.PlayStone(move, playerColor == WHITE ? BLACK : WHITE);
		gameBoard.ApplyMove(move);
		StoneColor winner = gameBoard.IsTerminal();
		if (winner != EMPTY) 
		{
			GameOver(playerColor);
			return;
		}
		board.SetButtonsDisable(false);
	}
	private void OnStonePlayed(int move) 
	{
		moves.Push(move);
		board.PlayStone(move, playerColor);
		gameBoard.ApplyMove(move);
		StoneColor winner = gameBoard.IsTerminal();
		if (winner != EMPTY)
		{
			GameOver(playerColor);
			return;
		}
		board.SetButtonsDisable(true);
		AiMove();
	}

	private void GameOver(StoneColor winner) 
	{
		//ToDo
	}
	private void Reset() 
	{
		board.Reset();
		GetNode<Node2D>("ChooseColor").Show();
		GetNode<Node2D>("InGame").Hide();
		gameBoard = new();
		moves.Clear();
	}
	public override void _Ready()
	{
		board = GetNode<Board>("Board");
		Reset();
		agent.AiSelectedMove += AiMoved;
	}

}
