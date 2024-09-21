using System;
using Godot;

public partial class BaseCharacter : CharacterBody2D {
    [ExportGroup("Statistics")]
	[Export] float WalkSpeed = 128f;
	[Export] float WalkAcceleration = 1200f;
	[Export] float RunSpeed = 200f;
	[Export] float RunAcceleration = 400f;
	[ExportGroup("Components")]
	[Export] protected CharSprite Sprite;
    //
    #region Movement Helpers
	public bool Running;
    protected void ProcessMove(double delta, Vector2 input)
    {
        var moveDir = input.Normalized();
		var velocity = Velocity;
		var move = moveDir * GetMoveSpeed();
		Running = Input.IsActionPressed("run");
		Velocity = velocity.MoveToward(move, GetMoveAcceleration() * (float)delta);
		if (input != Vector2.Zero) {
			var angle = Mathf.RadToDeg(Velocity.Angle());
			Sprite.Animating = true;
			Sprite.Direction = angle;
			ProcessOnRotate(angle);
		} else {
			Sprite.Animating = false;
			Sprite.FrameIdx = 0;
		}
		MoveAndSlide();
    }

    protected virtual void ProcessOnRotate(float angle)
    {
        
    }

    private float GetMoveSpeed() {
		return Running ? RunSpeed : WalkSpeed;
	}
	private float GetMoveAcceleration() {
		return Running ? RunAcceleration : WalkAcceleration;
	}
	#endregion
}