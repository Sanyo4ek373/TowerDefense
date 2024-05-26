using UnityEngine;

[CreateAssetMenu(fileName = "LevelResources", menuName = "Level/New LevelResources")]
public class LevelResources : ScriptableObject {
    [field:SerializeField] public int Level { get; private set; }
    [field:SerializeField] public int Money { get; private set; }
    [field:SerializeField] public int Food { get; private set; }
}