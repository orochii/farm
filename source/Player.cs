using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public partial class Player : CharacterBody2D
{
	[ExportGroup("Statistics")]
	[Export] float WalkSpeed = 128f;
	[Export] float WalkAcceleration = 1200f;
	[Export] float RunSpeed = 200f;
	[Export] float RunAcceleration = 400f;
	[ExportGroup("Components")]
	[Export] CharSprite Sprite;
	[Export] Node2D HoldSlot;
	[Export] Node2D InteractPivot;
	[Export] Node2D PlacePivot;
	private PlaceableObject heldItem = null;
	public override void _Ready()
	{
		InteractPivot.RotationDegrees = Sprite.Direction;
		// Spawn held item (if any).
		var itm = Main.State.GetHeldItem();
		if (itm != null) {
			GD.Print("HeldID:",itm.ID);
			var data = MapObject.GetData(itm.ID);
			GD.Print("Data:",data);
			var obj = data.Create(itm);
			GD.Print("Inst:",obj);
			PickUp(obj);
		}
	}
	public override void _Process(double delta)
	{
		var input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		var interact = Input.IsActionJustPressed("interact");
		var moveDir = input.Normalized();
		var velocity = Velocity;
		var move = moveDir * GetMoveSpeed();
		Velocity = velocity.MoveToward(move, GetMoveAcceleration() * (float)delta);
		if (input != Vector2.Zero) {
			var angle = Mathf.RadToDeg(Velocity.Angle());
			Sprite.Animating = true;
			Sprite.Direction = angle;
			InteractPivot.RotationDegrees = angle;
		} else {
			Sprite.Animating = false;
			Sprite.FrameIdx = 0;
		}
		if (interact) DoInteract();
		MoveAndSlide();
		UpdateTileGrid();
		// TEST
		/*var x = GlobalPosition.X;
		var y = GlobalPosition.Y;
		var tid = Main.State.Map.GetTerrainIdAt(x, y);
		GD.Print("Terrain @",x,",",y,":", tid);*/
		//
		if (Input.IsActionJustPressed("pause")) {
			Main.State.Save();
		}
	}
	//
	private void UpdateTileGrid() {
		Main.State.Map.RepositionTileGrid(AlignToGrid(PlacePivot.GlobalPosition));
	}
	#region Pickup item
	public void PickUp(PlaceableObject obj) {
		if (heldItem != null) return;
		heldItem = obj;
		if(heldItem.GetParent() != null) heldItem.GetParent().RemoveChild(heldItem);
		HoldSlot.AddChild(heldItem);
		heldItem.Position = Vector2.Zero;
		heldItem.SetCollision(false);
		Main.State.SetHeldItem(obj.Serialize());
		Main.State.Map.UnregisterObject(heldItem);
	}
	public void Place() {
		if (heldItem == null) return;
		heldItem.GetParent().RemoveChild(heldItem);
		GetParent().AddChild(heldItem);
		heldItem.GlobalPosition = AlignToGrid(PlacePivot.GlobalPosition);
		Main.State.Map.RegisterObject(heldItem);
		heldItem.SetCollision(true);
		heldItem = null;
		Main.State.SetHeldItem(null);
	}
	const int GRID_SIZE = 16;
	private Vector2 AlignToGrid(Vector2 inV) {
		var x = ((int)Math.Round((inV.X-GRID_SIZE/2) / GRID_SIZE)) * GRID_SIZE;
		var y = ((int)Math.Round((inV.Y-GRID_SIZE/2) / GRID_SIZE)) * GRID_SIZE;
		Vector2 outV = new Vector2(x+GRID_SIZE/2,y+GRID_SIZE/2);
		return outV;
	}
	#endregion
	#region Movement Helpers
	private bool IsRunning() {
		return Input.IsActionPressed("run");
	}
	private float GetMoveSpeed() {
		return IsRunning() ? RunSpeed : WalkSpeed;
	}
	private float GetMoveAcceleration() {
		return IsRunning() ? RunAcceleration : WalkAcceleration;
	}
	#endregion
	#region Interaction
	private List<Interactable> _nearbyInteractables = new List<Interactable>();
	private void DoInteract() {
		var interactable = GetNearestInteractable();
		if(interactable != null) interactable.Interact(this);
		else Place();
	}
	private Interactable GetNearestInteractable() {
		Interactable nearest = null;
		float dst = 0;
		foreach (var i in _nearbyInteractables) {
			var iDst = GlobalPosition.DistanceSquaredTo(((Node2D)i).GlobalPosition);
			if (nearest == null || iDst < dst) {
				nearest = i;
				dst = iDst;
			}
		}
		return nearest;
	}
	public void OnBodyEntered(Node2D body) {
		if (body is Interactable) {
			var interactable = body as Interactable;
			if (!_nearbyInteractables.Contains(interactable)) _nearbyInteractables.Add(interactable);
		}
	}
	public void OnBodyExited(Node2D body) {
		if (body is Interactable) {
			var interactable = body as Interactable;
			if (_nearbyInteractables.Contains(interactable)) _nearbyInteractables.Remove(interactable);
		}
	}
	#endregion
}
