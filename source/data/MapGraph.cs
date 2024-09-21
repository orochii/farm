using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class MapGraph : Resource {
    [Export] public MapConnection[] Connections;
    private Dictionary<string, List<MapConnection>> mapConnectionsDictionary;
    private List<MapConnection> _emptyList = new List<MapConnection>();
    public List<MapConnection> GetMapConnections(string mapID) {
        if (mapConnectionsDictionary==null) PreprocessConnections();
        if (mapConnectionsDictionary.TryGetValue(mapID, out var list)) {
            return list;
        }
        return _emptyList;
    }
    private void PreprocessConnections() {
        mapConnectionsDictionary = new Dictionary<string, List<MapConnection>>();
        foreach (var connection in Connections) {
            // Add connections from left side.
            if (!mapConnectionsDictionary.TryGetValue(connection.Left.MapID, out var leftList)) {
                leftList = new List<MapConnection>();
                mapConnectionsDictionary.Add(connection.Left.MapID, leftList);
            }
            leftList.Add(connection);
            // Add connections from right side.
            if (!mapConnectionsDictionary.TryGetValue(connection.Right.MapID, out var rightList)) {
                rightList = new List<MapConnection>();
                mapConnectionsDictionary.Add(connection.Right.MapID, rightList);
            }
            rightList.Add(connection);
        }
    }
}
