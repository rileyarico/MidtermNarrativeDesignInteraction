using UnityEngine;

[CreateAssetMenu(menuName = "Seeds/SeedType")]
public class SeedTypes : ScriptableObject
{
    public string seedName;
    public GameObject grownPrefab;
    public float growRate;
}
