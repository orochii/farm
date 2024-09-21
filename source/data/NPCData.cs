using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class NPCData : Resource {
    const string LOCATION = "res://data/npc/";
    const string EXTENSION = ".tres";
    [Export] public PackedScene CharacterTemplate;
    [Export] public string StartingMapID;
    [Export] public Vector2I StartingPosition;
    // Define the daily routine somehow 8').
    // Also routine should be by day, and maybe add variations?
    /*
    NPC
        - Routines
    */
    [Export] NPCRoutine[] Routines;
    public List<NPCRoutine> GetAvailableRoutines() {
        List<NPCRoutine> availableRoutines = new List<NPCRoutine>();
        foreach (var routine in Routines) {
            if (routine.CheckIfAvailable()) availableRoutines.Add(routine);
        }
        return availableRoutines;
    }
    /*
    On deciding which to take:
    ==========================
    Check which is the highest option under the current hour mark.
    If there's none, take the highest overall (what the character was doing last day).
    */
    public static NPCRoutine GetCurrentRoutine(List<NPCRoutine> availableRoutines) {
        var currHour = Main.State.GetHour();
        NPCRoutine selected = null;
        NPCRoutine highest = null;
        foreach (var routine in availableRoutines) {
            if (routine.StartTime <= currHour) {
                if (selected == null || routine.StartTime >= selected.StartTime) 
                    selected = routine;
            }
            if (highest == null || routine.StartTime >= selected.StartTime)
                highest = routine;
        }
        if (selected != null) return selected;
        return highest;
    }
    public string GetId() {
        var len = ResourcePath.Length - LOCATION.Length - EXTENSION.Length;
        return ResourcePath.Substring(LOCATION.Length, len);
    }
    public static NPCData GetData(string id) {
        if (id == "") return null;
        return OZResourceLoader.Load<NPCData>(LOCATION + id + EXTENSION);
    }
}
