using Godot;
using NoGo;
using System;
using static NoGo.StoneColor;

public partial class Stone : Node2D
{
	[Signal]
	public delegate void StonePlayedEventHandler(int num);
	public int number;
	public void PlayStone(StoneColor color) 
	{
		if (color == WHITE) 
		{
			GetNode<TextureRect>("Black").SetDeferred(TextureRect.PropertyName.Visible, false);
			GetNode<TextureRect>("White").SetDeferred(TextureRect.PropertyName.Visible, true);
		}
		else 
		{
			GetNode<TextureRect>("Black").SetDeferred(TextureRect.PropertyName.Visible, true);
			GetNode<TextureRect>("White").SetDeferred(TextureRect.PropertyName.Visible, false);
		}
		GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, true);
	}
	public void SetCrossVisible(bool visible) 
	{
		GetNode<TextureRect>("Cross").SetDeferred(TextureRect.PropertyName.Visible, visible);
	}
	public void StartGame() 
	{
		GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, false);
	}
	public void Reset() 
	{
		GetNode<TextureRect>("Black").SetDeferred(TextureRect.PropertyName.Visible, false);
		GetNode<TextureRect>("White").SetDeferred(TextureRect.PropertyName.Visible, false);
		GetNode<TextureRect>("Cross").SetDeferred(TextureRect.PropertyName.Visible, false);
		GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, true);
	}
	public void OnButtonPressed() 
	{
		EmitSignal(SignalName.StonePlayed, number);
	}
}
