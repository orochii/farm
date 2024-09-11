using Godot;
using System;

public partial class Status : VBoxContainer
{
	[Export] InvSlot HandsSlot;
    public override void _Process(double delta)
    {
		if (Main.State == null) return;
        HandsSlot.Setup(Main.State.GetHeldItem());
    }
}
