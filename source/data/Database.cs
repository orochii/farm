using Godot;

[GlobalClass]
public partial class Database : Resource {
    static Database mInstance;
    public static Database Get() {
        if (mInstance == null) {
            mInstance = OZResourceLoader.Load<Database>("res://data/database.tres");
        }
        return mInstance;
    }
    [Export] public NPCData[] NPCs;
    [Export] public MapGraph MapGraph;
}