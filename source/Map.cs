using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Node2D
{
	[System.Serializable]
	public class State {
		public List<PlaceableObject.State> objectStates = new List<PlaceableObject.State>();
	}
	private List<PlaceableObject> objects = new List<PlaceableObject>();
	[Export] public bool AllowPlacingObjects;
	[Export] Node2D ObjectParent;
	[Export] Node2D TileGridGraphic;
	public void RepositionTileGrid(Vector2 p) {
		if (TileGridGraphic != null) TileGridGraphic.GlobalPosition = p;
	}
    public override async void _Ready()
    {
        base._Ready();
		//
		await ToSignal(GetTree(), "process_frame");
		var s = Main.State.GetCurrentMapState();
		if (s == null) {
			// If there's no state, do nothing. That should be okay.
		} else {
			// First get all existing original objects.
			List<PlaceableObject> existingObjects = new List<PlaceableObject>(objects);
			foreach (var objState in s.objectStates) {
				// Find original instance.
				PlaceableObject found = null;
				foreach (var o in existingObjects) {
					if (o.Name == objState.InstanceName) {
						found = o;
						break;
					}
				}
				if (found != null) {
					// Update preexisting instance.
					found.Setup(objState);
					// Remove from array
					existingObjects.Remove(found);
				} else {
					// Create new objects placed during gameplay.
					var data = MapObject.GetData(objState.ID);
					var obj = data.Create(objState);
					ObjectParent.AddChild(obj);
				}
			}
			// Destroy original objects that have been destroyed before.
			foreach (var o in existingObjects) {
				// For now I'll just erase all remaining objects.
				// Should filter out new ones that game didn't know about before, so I can add new stuff to old maps.
				UnregisterObject(o);
				o.QueueFree();
			}
		}
    }
    public void RegisterObject(PlaceableObject obj) {
		if (!objects.Contains(obj)) objects.Add(obj);
	}
	public void UnregisterObject(PlaceableObject obj) {
		if (objects.Contains(obj)) objects.Remove(obj);
	}
	public State Serialize() {
		var s = new State();
		foreach (var o in objects) {
			GD.Print(o);
			if(o != null) s.objectStates.Add(o.Serialize());
		}
		return s;
	}
	public List<int> GetTerrainIdsAt(float x, float y) {
		var xx = (int)x / 16;
		var yy = (int)y / 16;
		var coord = new Vector2I(xx,yy);
		List<int> ids = new List<int>();
		foreach (var c in GetChildren()) {
			if (c is TileMapLayer) {
				var layer = c as TileMapLayer;
				var data = layer.GetCellTileData(coord);
				if (data != null) {
					var value = data.GetCustomData("TerrainID").AsInt32();
					ids.Add(value);
				}
			}
		}
		return ids;
	}
	public int GetTerrainIdAt(float x, float y) {
		var id = 0;
		var xx = (int)x / 16;
		var yy = (int)y / 16;
		var coord = new Vector2I(xx,yy);
		foreach (var c in GetChildren()) {
			if (c is TileMapLayer) {
				var layer = c as TileMapLayer;
				var data = layer.GetCellTileData(coord);
				if (data != null) {
					var value = data.GetCustomData("TerrainID").AsInt32();
					if (id < value) id = value;
				}
			}
		}
		return id;
	}
}
