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
	private Timer aiTimer;
	private Timer playerTimer;
	private int aiTime;
	private int playerTime;
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
			aiTimer.Stop();
			playerTimer.Start();
			board.SetButtonsDisable(false);
		}
		else 
		{
			aiTimer.Start();
			playerTimer.Stop();
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
		aiTimer.Stop();
		playerTimer.Start();
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
		aiTimer.Start();
		playerTimer.Stop();
		AiMove();
	}

	private void GameOver(StoneColor winner) 
	{
		//ToDo
	}
	private void SetLabel() 
	{
		int playerMin = playerTime / 60;
		int playerSecond = playerTime % 60;
		int aiMin = aiTime / 60;
		int aiSecond = aiTime % 60;
		if (playerColor == BLACK) 
		{
			GetNode<Node2D>("InGame").GetNode<Label>("BlackPlayer").Text = "玩家\n" + playerMin.ToString("00") + ":" + playerSecond.ToString("00");
			GetNode<Node2D>("InGame").GetNode<Label>("WhitePlayer").Text = "Baduk模型\n" + aiMin.ToString("00") + ":" + aiSecond.ToString("00");
		}
		else 
		{
			GetNode<Node2D>("InGame").GetNode<Label>("WhitePlayer").Text = "玩家\n" + playerMin.ToString("00") + ":" + playerSecond.ToString("00");
			GetNode<Node2D>("InGame").GetNode<Label>("BlackPlayer").Text = "Baduk模型\n" + aiMin.ToString("00") + ":" + aiSecond.ToString("00");
		}
	}
	private void OnPlayerTimerTimeout() 
	{
		playerTime++;
		SetLabel();
	}
	private void OnAiTimerTimeout() 
	{
		aiTime++;
		SetLabel();
	}


	private void Reset() 
	{
		board.Reset();
		GetNode<Node2D>("ChooseColor").Show();
		GetNode<Node2D>("InGame").Hide();
		gameBoard = new();
		moves.Clear();
		aiTimer.Stop();
		playerTimer.Stop();
		aiTime = 0;
		playerTime = 0;
	}
	public override void _Ready()
	{
		board = GetNode<Board>("Board");
		aiTimer = GetNode<Timer>("AiTimer");
		playerTimer = GetNode<Timer>("PlayerTimer");
		Reset();
		agent.AiSelectedMove += AiMoved;
	}

}
