using System;
using System.Collections.Generic;
using Godot;

public partial class WorldStateManager : Node2D {
    private Dictionary<string,NPC> instancedNPCs;
    public override void _Ready()
    {
        Main.State.onLoadFinished += RefreshDay;
        Main.State.onDayPassed += RefreshDay;
        Main.State.onMapLoaded += RefreshNPCObjects;
    }
    public override void _ExitTree()
    {
        base._ExitTree();
        Main.State.onLoadFinished -= RefreshDay;
        Main.State.onDayPassed -= RefreshDay;
        Main.State.onMapLoaded += RefreshNPCObjects;
    }
    public void RefreshDay() {
        if (Main.State == null) return;
        // Add non-existing NPCs to state from Database.
        foreach (var n in Database.Get().NPCs) {
            //n.GetId();
            NPCState state = Main.State.GetOrAddNPC(n);
            state.Refresh(n);
        }
    }
    public void RefreshNPCObjects() {
        // I should flush the instanced NPCs. This I think will only be called when map is loaded.
        // So assume all the previously instanced NPCs don't exist anymore. And/or shouldn't.
        foreach (var inst in instancedNPCs) {
            if (IsInstanceValid(inst.Value)) inst.Value.QueueFree();
        }
        instancedNPCs.Clear();
        //
        var currMapName = Main.State.GetCurrentMapName();
        foreach (var n in Database.Get().NPCs) {
            NPCState state = Main.State.GetOrAddNPC(n);
            // Create new set of NPCs.
            if (state.CurrentMap == currMapName) {
                CreateNPCInstance(n, state);
            }
        }
    }

    private void CreateNPCInstance(NPCData n, NPCState state)
    {
        var inst = n.CharacterTemplate.Instantiate<NPC>();
        var pos = new Vector2(state.CurrentPosX, state.CurrentPosY);
        inst.state = state;
        Main.State.Map.PlaceNPC(inst, pos);
        instancedNPCs.Add(state.ID, inst);
    }
    private void DestroyNPCInstance(string id) {
        instancedNPCs[id].QueueFree();
        instancedNPCs.Remove(id);
    }

    public override void _Process(double delta)
    {
        if (Main.State == null) return;
        var currMapName = Main.State.GetCurrentMapName();
        // Advance NPC states.
        foreach (var n in Database.Get().NPCs) {
            NPCState state = Main.State.GetOrAddNPC(n);
            bool _npcMapChange = state.Process(delta, n);
            if (_npcMapChange) {
                // Check if a new NPC must be instanced, or destroyed, based on if they're entering or leaving the current map
                if (state.CurrentMap == currMapName) {
                    // Create
                    CreateNPCInstance(n, state);
                } else {
                    // Destroy
                    DestroyNPCInstance(state.ID);
                }
            }
        }
        // Advance time.
        Main.State.AdvanceTime((float)delta);
    }
}
