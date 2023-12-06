using UnityEngine;

[CreateAssetMenu(fileName = "LevelResources", menuName = "Level/New LevelResources")]
public class LevelResources : ScriptableObject {
    [SerializeField] private int _level;
    [SerializeField] private int _money;
    [SerializeField] private int _food;

    public int Level => _level;
    public int Money => _money;
    public int Food => _food;
}