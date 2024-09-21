using Godot;

[GlobalClass]
public partial class NPCRoutineActivity : Resource {
    /*
    NPC ACTIVITY
        - Map
        - Position
        - Action
        - Interactable -> when applicable for action, if interactable isn't there or close, or if disabled, they'll skip this location after arriving.
    */
    [Export] public string MapId;
    [Export] public Vector2I Position;
    [Export] public string Action;
    [Export] public string InteractableId;
}