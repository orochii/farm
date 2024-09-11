using Godot;
using System;

public partial class GameCamera : Camera2D
{
	[Export] Node2D Target;
	public override void _Ready()
	{
		if (Target != null) GlobalPosition = Target.GlobalPosition;
	}
	public override void _Process(double delta)
	{
		if (Target != null) GlobalPosition = Target.GlobalPosition;
	}
}
