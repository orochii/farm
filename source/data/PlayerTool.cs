using Godot;

[GlobalClass]
public partial class PlayerTool : Resource {
    const string LOCATION = "res://data/tools/";
    const string EXTENSION = ".tres";
    [Export] Texture2D Icon;
    public string GetId() {
        var len = ResourcePath.Length - LOCATION.Length - EXTENSION.Length;
        return ResourcePath.Substring(LOCATION.Length, len);
    }
}