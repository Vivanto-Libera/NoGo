using Godot;
using NoGo;
using System;
using static NoGo.StoneColor;

public partial class Stone : Node2D
{
	[Signal]
	public delegate void StonePlayedEventHandler(int num);
	public int number;
	public bool stonePlayed;
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
		SetButtonDisable(true);
		stonePlayed = true;
	}
	public void SetCrossVisible(bool visible) 
	{
		GetNode<TextureRect>("Cross").SetDeferred(TextureRect.PropertyName.Visible, visible);
	}
	public void SetButtonDisable(bool disable) 
	{
		if (stonePlayed) 
		{
			GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, true);
			return;
		}
		GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, disable);
	}
	public void Reset() 
	{
		GetNode<TextureRect>("Black").SetDeferred(TextureRect.PropertyName.Visible, false);
		GetNode<TextureRect>("White").SetDeferred(TextureRect.PropertyName.Visible, false);
		GetNode<TextureRect>("Cross").SetDeferred(TextureRect.PropertyName.Visible, false);
		SetButtonDisable(true);
		stonePlayed = false;
	}
	public void OnButtonPressed() 
	{
		EmitSignal(SignalName.StonePlayed, number);
	}
}
