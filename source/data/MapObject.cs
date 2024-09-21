using Godot;

[GlobalClass]
public partial class MapObject : Resource {
    const string LOCATION = "res://data/objects/";
    const string EXTENSION = ".tres";
    [Export] public Texture2D Icon;
    [Export] public bool CanPickup;
    [Export] public PackedScene Template;
    public PlaceableObject Create(PlaceableObject.State state) {
        PlaceableObject obj = Template.Instantiate<PlaceableObject>();
        if (obj != null) {
            obj.Setup(this, state);
        }
        //Main.State.Map.RegisterObject(this);
        return obj;
    }
    public string GetId() {
        var len = ResourcePath.Length - LOCATION.Length - EXTENSION.Length;
        return ResourcePath.Substring(LOCATION.Length, len);
    }
    public static MapObject GetData(string id) {
        if (id == "") return null;
        return OZResourceLoader.Load<MapObject>(LOCATION + id + EXTENSION);
    }
}