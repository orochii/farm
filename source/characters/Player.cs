using Godot;
using System;
using System.Collections.Generic;

public partial class Player : BaseCharacter
{
	
	[Export] Node2D HoldSlot;
	[Export] Node2D InteractPivot;
	[Export] Node2D PlacePivot;
	private PlaceableObject heldItem = null;
	public override void _Ready()
	{
		InteractPivot.RotationDegrees = Sprite.Direction;
		SpawnHeldItem();
		Main.Player = this;
	}
	public override void _PhysicsProcess(double delta)
	{
		var input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		ProcessMove(delta, input);
		// TEST
		/*var x = GlobalPosition.X;
		var y = GlobalPosition.Y;
		var tid = Main.State.Map.GetTerrainIdAt(x, y);
		GD.Print("Terrain @",x,",",y,":", tid);*/
		//
		ProcessInput();
	}
    private void ProcessInput() {
		var interact = Input.IsActionJustPressed("interact");
		if (interact) DoInteract();
		UpdateTileGrid();
		if (Input.IsActionJustPressed("cancel")) {
			PutAwayHeldItem();
		}
		if (Input.IsActionJustPressed("cycle_item_l")) {
			CycleInventory(true);
		}
		if (Input.IsActionJustPressed("cycle_item_r")) {
			CycleInventory(false);
		}
		if (Input.IsActionJustPressed("pause")) {
			Main.State.Save();
		}
	}
    protected override void ProcessOnRotate(float angle)
    {
        InteractPivot.RotationDegrees = angle;
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
	public void SpawnHeldItem() {
		if (heldItem != null) heldItem.QueueFree();
		heldItem = null;
		var itm = Main.State.GetHeldItem();
		if (itm != null) {
			var data = MapObject.GetData(itm.ID);
			var obj = data.Create(itm);
			PickUp(obj);
		}
	}
	public void CycleInventory(bool backwards) {
		if (Main.State.CycleInventory(backwards)) {
			SpawnHeldItem();
		}
	}
	public void PutAwayHeldItem() {
		if (Main.State.PutAwayHeldItem()) {
			SpawnHeldItem();
		}
	}
	const int GRID_SIZE = 16;
	private Vector2 AlignToGrid(Vector2 inV) {
		var x = ((int)Math.Round((inV.X-GRID_SIZE/2) / GRID_SIZE)) * GRID_SIZE;
		var y = ((int)Math.Round((inV.Y-GRID_SIZE/2) / GRID_SIZE)) * GRID_SIZE;
		Vector2 outV = new Vector2(x+GRID_SIZE/2,y+GRID_SIZE/2);
		return outV;
	}
	#endregion
	#region Interaction
	private List<Interactable> _nearbyInteractables = new List<Interactable>();
	private void DoInteract() {
		var interactable = GetNearestInteractable();
		if(interactable != null && !(interactable is PlaceableObject && heldItem!=null)) interactable.Interact(this);
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
