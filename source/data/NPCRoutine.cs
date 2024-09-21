using Godot;

[GlobalClass]
public partial class NPCRoutine : Resource {
    [Export] public NPCRoutineCondition[] Conditions;
    [Export] public int StartTime;
    [Export] public NPCRoutineActivity Activity;
    /*
    Routines
        - Conditions
        - Activity
    */
    /*
    Written examples of activities
    ==============================
    Character goes to market at 8am.
    On Sundays, Character goes to library at 10am. Executes action sit.
    If mountain is open, on Saturdays, Character goes to the mountain at 5pm. Executes action "collect mushrooms".
    Character goes back home at 7pm.
    */
    public bool CheckIfAvailable() {
        foreach (var condition in Conditions) {
            if (!condition.Check()) return false;
        }
        return true;
    }
}