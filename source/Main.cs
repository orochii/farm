using Godot;
using System;

public partial class Main : Node2D
{
	const int SCALE = 3;
	public static Main Instance;
	public static GameState State {
		get {
			if (Instance == null) return null;
			return Instance.state;
		}
	}
	[Export] public Node2D WorldRoot;
	[Export] public Loader Loader;
	GameState state;
	Window gameWindow;
	Vector2I originalSize;
	public override void _Ready()
	{
		foreach (var joyID in Input.GetConnectedJoypads()) {
			GD.Print(Input.GetJoyName(joyID));
		}
		//
		Instance = this;
		gameWindow = GetWindow();
		originalSize = gameWindow.Size;
		gameWindow.Size = originalSize * SCALE;
		gameWindow.MoveToCenter();
		//
		state = new GameState();
		//
		GD.Print("Loading game.");
		state.Load();
		//
		var _mapname = state.PopMapName();
		if (_mapname=="") _mapname = "res://maps/farm.tscn";
		state.ChangeMap(_mapname);
	}
}
