using Godot;
using System;
using System.Data;

public partial class PlaceableObject : StaticBody2D, Interactable
{
	[System.Serializable]
	public class State {
		public string InstanceName;
		public string ID;
		public int X;
		public int Y;
		public int CreationDate;
		public int Durability;
		public int AdvanceValue;
	}
	[Export] MapObject MapObjectData;
	private State state;
	private uint originalCollisionLayer;
	private uint originalCollisionMask;
	public override void _Ready()
	{
		originalCollisionLayer = CollisionLayer;
		originalCollisionMask = CollisionMask;
		if (MapObjectData != null) {
			// Add original objects to map.
			state = new State();
			Main.State.Map.RegisterObject(this);
		}
	}
	public void Setup(State state) {
		MapObjectData = MapObject.GetData(state.ID);
		GlobalPosition = new Vector2(state.X, state.Y);
		this.state = state;
	}
	public void Setup(MapObject data, State state) {
		MapObjectData = data;
		GlobalPosition = new Vector2(state.X, state.Y);
		this.state = state;
	}
	public State Serialize() {
		state.InstanceName = Name;
		state.ID = MapObjectData.GetId();
		state.X = (int)GlobalPosition.X;
		state.Y = (int)GlobalPosition.Y;
		return state;
	}
	//
    public void Interact(Player p)
    {
        p.PickUp(this);
    }
	public void SetCollision(bool v) {
		if (v) {
			CollisionLayer = originalCollisionLayer;
			CollisionMask = originalCollisionMask;
		} else {
			CollisionLayer = 0;
			CollisionMask = 0;
		}
	}
}
