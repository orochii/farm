using Godot;
using System;
using System.Data.Common;
[Tool]

[GlobalClass]
public partial class CharSprite : Sprite2D
{
	static int[] DIR4_REMAP = {2,0,1,3};
	[Export] public bool Animating = false;
	[Export] public float FrameRate = 16;
	[Export] public Vector2I FrameCounts = new Vector2I(4,4);
	[Export] public float Direction = 90f;
	[Export] public int FrameIdx = 0;
	private float frameTimer;
	public override void _Ready()
	{
		UpdateRegion();
	}
	public override void _Process(double delta)
	{
		// Update current frame doesn't happen in editor
		if (!Engine.IsEditorHint() && Animating) {
			UpdateFrame(delta);
		}
		UpdateRegion();
	}
	private int AngleToIndex(float angle) {
		var maxDir = FrameCounts.Y;
		//
		if (angle < 0) angle += 360;
		//
		var d = angle / (360 / maxDir);
		var dr = (int)Math.Round(d);
		if (dr >= maxDir) dr = 0;
		return DIR4_REMAP[dr];
	}
	private void UpdateFrame(double delta) {
		int prevFrame = FrameIdx;
		frameTimer += (float)delta * FrameRate;
		if (frameTimer > 1) {
			var wholes = (int)frameTimer;
			FrameIdx += wholes;
			frameTimer -= wholes;
		}
		UpdateFrameEvent(prevFrame);
	}
	private void UpdateRegion() {
		var dirIdx = AngleToIndex(Direction);
		// Clamp dir/frame
		if (FrameIdx < 0) FrameIdx = 0;
		if (dirIdx < 0) dirIdx = 0;
		FrameIdx = FrameIdx % FrameCounts.X;
		dirIdx = dirIdx % FrameCounts.Y;
		//
		if (Texture == null) return;
		// Set current region
		var w = Texture.GetWidth();
		var h = Texture.GetHeight();
		RegionEnabled = true;
		var rect = new Rect2();
		rect.Size = new Vector2(w / FrameCounts.X, h / FrameCounts.Y);
		rect.Position = new Vector2(rect.Size.X * FrameIdx, rect.Size.Y * dirIdx);
		RegionRect = rect;
	}
	private void UpdateFrameEvent(int prevFrame) {
		if (prevFrame != FrameIdx) {
			// TODO? Step sounds and uhh not much more for now lol...
		}
	}
}
