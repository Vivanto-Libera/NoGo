using Godot;
using NoGo;
using System;
using System.Collections.Generic;
using System.IO;

public partial class SaveRecord : Window
{
	private List<int> moves;
	private StoneColor winner;
	private StoneColor myColor;
	public void SetMovesAndWinner(Stack<int> newMoves, StoneColor newWinner, StoneColor mine) 
	{
		winner = newWinner;
		moves = [.. newMoves];
		moves.Reverse();
		myColor = mine;
	}
	private void OnSavePressed() 
	{
		DateTime now = DateTime.Now;
		string date1 = now.ToString("yyyy.MM.dd HH:mm");
		string date2 = now.ToString("yyyy.MM.dd HH-mm");
		string gameName = now.ToString("yyyy") + " CCGC";
		string win = winner == StoneColor.BLACK ? "先手胜" : "后手胜";
		string myTeam = "Baduk Zero";
		string otherTeam = GetNode<TextEdit>("TeamEdit").Text;
		string position = GetNode<TextEdit>("PositionEdit").Text;
		string datePos1 = date1 + " " + position;
		string datePos2 = date2 + " " + position;
		string filename = "NG-";
		if (myColor == StoneColor.BLACK) 
		{
			filename += myTeam + " B vs " + otherTeam + " W-";
		}
		else 
		{
			filename += otherTeam + " B vs " + myTeam + " W-";
		}
		filename += win + "-" + datePos2 + "-" + gameName + ".txt";
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
		string content = "([NG]";
		if (myColor == StoneColor.BLACK)
		{
			content += "[" + myTeam + " B ][" + otherTeam + " W]";
		}
		else
		{
			content += "[" + otherTeam + " B][" + myTeam + " W]";
		}
		content += "[" + win + "]" + "[" + datePos1 + "]" + "[" + gameName + "]";
		for (int i = 0; i < moves.Count; i++) 
		{
			string moveRecord = i % 2 == 0 ? ";B[" : "W[";
			char[] colChar = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'];
			int row = moves[i] / 9;
			int col = moves[i] % 9;
			moveRecord += colChar[col] + (row + 1).ToString() + "]";
			content += moveRecord;
		}
		content += ")";
		File.WriteAllText(path, content);
		Hide();
	}
	private void OnClosedClicked() 
	{
		Hide();
	}
}
