using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : HBoxContainer
{
	[Export] PackedScene SlotTemplate;
	[Export] Container SlotContainer;
	[Export] string ContainerId;
	private List<InvSlot> _currentSlots = new List<InvSlot>();
	private int lastSize = -1;
	private int ContainerSize => Main.State.GetContainerMax(ContainerId);
	private int ContainerCount => Main.State.GetContainerItemCount(ContainerId);
	public override void _Process(double delta)
	{
		if (Main.State == null) return;
		if (lastSize != ContainerSize) RefreshSlotsSize();
		RefreshSlots();
	}
	public void RefreshSlotsSize() {
		foreach(var s in _currentSlots) s.QueueFree();
		_currentSlots.Clear();
		for (int i = 0; i < ContainerSize; i++) {
			var inst = SlotTemplate.Instantiate<InvSlot>();
			SlotContainer.AddChild(inst);
			_currentSlots.Add(inst);
		}
	}
	public void RefreshSlots() {
		for (int i = 0; i < ContainerSize; i++) {
			_currentSlots[i].Setup(Main.State.GetContainerItem(ContainerId, i));
		}
	}
}
