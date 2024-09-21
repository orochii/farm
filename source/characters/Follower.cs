using System;
using Godot;

public partial class Follower : BaseCharacter
{
	[Export] NavigationAgent2D Agent;
	bool agentReady = false;
	public override void _Ready()
	{
		//
		NavigationServer2D.MapChanged += OnMapChange;
	}

    private void OnMapChange(Rid map)
    {
        agentReady = true;
    }
	public override void _PhysicsProcess(double delta)
	{
		if (Main.Player != null) {
			Agent.TargetPosition = Main.Player.GlobalPosition;
			//
			var deltaPos = Vector2.Zero;
			if (agentReady && !Agent.IsNavigationFinished()) {
				var nextPos = Agent.GetNextPathPosition();
				deltaPos = (nextPos - GlobalPosition).Normalized();
			}
			ProcessMove(delta, deltaPos);
		}
	}
}
