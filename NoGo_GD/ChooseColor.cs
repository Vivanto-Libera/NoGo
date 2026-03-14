using Godot;
using NoGo;
using System;

public partial class ChooseColor : Node2D
{
	[Signal]
	public delegate void ColorChoseEventHandler(int color);
	private void OnBlackPressed() 
	{
		EmitSignal(SignalName.ColorChose, (int)StoneColor.BLACK);
	}
	private void OnWhitePressed()
	{
		EmitSignal(SignalName.ColorChose, (int)StoneColor.WHITE);
	}
}
