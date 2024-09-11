using Godot;
using System;

public partial class InvSlot : NinePatchRect
{
	[Export] Sprite2D sprite;
	public void Setup(PlaceableObject.State obj) {
		if (obj==null) {
			sprite.Texture = null;
			return;
		}
		var data = MapObject.GetData(obj.ID);
		//
		if (data == null) sprite.Texture = null;
		else sprite.Texture = data.Icon;
	}
}
