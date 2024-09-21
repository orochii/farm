using Godot;

[GlobalClass]
public partial class NPCRoutineConditionWeekday : NPCRoutineCondition {
    [Export] EWeekday ValidWeekdays;
    public override bool Check()
    {
        if (Main.State == null) return false;
        var currWeekDay = 1 >> (Main.State.GetWeekday()+1);
        return (currWeekDay & (int)ValidWeekdays) != 0;
    }
}
