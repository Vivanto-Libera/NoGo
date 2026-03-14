using Godot;
using NoGo;
using System;

public partial class Board : Node2D
{
	[Signal]
	public delegate void StonePlayedEventHandler(int num);
	private Stone[] stones = new Stone[81];
	public void Reset() 
	{
		for (int i = 0; i < 81; i++) 
		{
			stones[i].Reset();
		}
	}
	public void PlayStone(int num, StoneColor color) 
	{
		stones[num].PlayStone(color);
		ShowCross(num);
	}
	public void UndoStone(int num, int lastStone) 
	{
		stones[num].Reset();
		ShowCross(lastStone);
	}
	public void SetButtonsDisable(bool disable) 
	{
		for (int i = 0; i < 81; i++)
		{
			stones[i].SetButtonDisable(disable);
		}
	}
	private void OnStonePlayed(int num)
	{
		EmitSignal(SignalName.StonePlayed, num);
	}
	private void ShowCross(int num) 
	{
		for (int i = 0; i < 81; i++)
		{
			stones[i].SetCrossVisible(false);
		}
		if (num != -1) 
		{
			stones[num].SetCrossVisible(true);
		}
	}
	public override void _Ready()
	{
		char[] colChar = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'];

		for (int row = 0; row < 9; row++)
		{
			for (int col = 0; col < 9; col++) 
			{
				string index = colChar[col] + (row + 1).ToString();
				int num = row * 9 + col;
				stones[num] = GetNode<Stone>(index);
				stones[num].number = num;
				stones[num].StonePlayed += OnStonePlayed;
			}
		}
		Reset();
	}
}
