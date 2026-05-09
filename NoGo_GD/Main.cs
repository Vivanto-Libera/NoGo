using Godot;
using NoGo;
using System;
using System.Collections.Generic;
using static NoGo.StoneColor;
using System.IO;

public partial class Main : Node
{
	private Board board;
	private GameBoard gameBoard;
	private Agent agent = new();
	private StoneColor playerColor;
	private Stack<int> moves = new();
	private StoneColor finalWinner;
	private Stack<GameBoard> lastBoards = new();
	private GameBoard lastBoard;
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
		GetNode<Node2D>("InGame").GetNode<Button>("Record").Hide();
		if (playerColor == BLACK) 
		{
			lastBoard = new(gameBoard);
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
		lastBoard = new(gameBoard);
	}
	private void OnStonePlayed(int move) 
	{
		lastBoards.Push(lastBoard);
		moves.Push(move);
		board.PlayStone(move, playerColor);
		gameBoard.ApplyMove(move);
		StoneColor winner = gameBoard.IsTerminal();
		if (winner != EMPTY)
		{
			GameOver(playerColor == BLACK ? WHITE : BLACK);
			return;
		}
		board.SetButtonsDisable(true);
		aiTimer.Start();
		playerTimer.Stop();
		AiMove();
	}
	private void OnUndoPressed() 
	{
		GetNode<Label>("WhoWin").Hide();
		GetNode<Node2D>("InGame").GetNode<Button>("Record").Hide();
		if (lastBoards.Count == 0) 
		{
			return;
		}
		agent.StopThread();
		if (gameBoard.turn == playerColor) 
		{
			board.UndoStone(moves.Pop(), moves.Peek());
		}
		if (moves.Count == 1)
		{
			board.UndoStone(moves.Pop(), -1);
		}
		else
		{
			board.UndoStone(moves.Pop(), moves.Peek());
		}
		gameBoard = lastBoards.Pop();
		lastBoard = new(gameBoard);
		board.SetButtonsDisable(false);
		aiTimer.Stop();
		playerTimer.Start();
	}
	private void OnOverPressed() 
	{
		agent.StopThread();
		Reset();
	}


	private void GameOver(StoneColor winner) 
	{
		GetNode<Node2D>("InGame").GetNode<Button>("Record").Show();
		aiTimer.Stop();
		playerTimer.Stop();
		board.SetButtonsDisable(true);
		if (winner == BLACK) 
		{
			GetNode<Label>("WhoWin").Text = "黑方获胜";
		}
		else 
		{
			GetNode<Label>("WhoWin").Text = "白方获胜";
		}
		finalWinner = winner;
		GetNode<Label>("WhoWin").Show();
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
	private void OnSimsChanged(float val) 
	{
		agent.sims = (int)(val);
		File.WriteAllText("sims.dat", agent.sims.ToString());
	}
	private void OnRecordPressed() 
	{
		GetNode<SaveRecord>("SaveRecord").SetMovesAndWinner(moves, finalWinner, playerColor == StoneColor.BLACK ? WHITE : StoneColor.BLACK);
		GetNode<SaveRecord>("SaveRecord").Show();

	}

	private void Reset() 
	{
		board.Reset();
		GetNode<Node2D>("ChooseColor").Show();
		GetNode<Node2D>("InGame").Hide();
		gameBoard = new();
		moves.Clear();
		lastBoards.Clear();
		aiTimer.Stop();
		playerTimer.Stop();
		aiTime = 0;
		playerTime = 0;
		GetNode<Label>("WhoWin").Hide();
	}

	private int ReadSims()
	{
		string sims = File.ReadAllText("sims.dat");
		return sims.ToInt();
	}
	public override void _Ready()
	{
		board = GetNode<Board>("Board");
		aiTimer = GetNode<Timer>("AiTimer");
		playerTimer = GetNode<Timer>("PlayerTimer");
		Reset();
		agent.AiSelectedMove += AiMoved;
		agent.sims = ReadSims();
		GetNode<Node2D>("InGame").GetNode<SpinBox>("SimsBox").SetValueNoSignal(agent.sims);
	}

}
