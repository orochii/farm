using Godot;

[GlobalClass]
public partial class NPCRoutineCondition : Resource {
    public virtual bool Check() {
        return true;
    }
}