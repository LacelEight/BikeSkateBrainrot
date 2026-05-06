using System;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "BrainrotConfig", menuName = "Configs/BrainrotConfig")]
public class BrainrotConfig : ScriptableObject
{
    public BrainrotType Type;
    public Rarity Rarity;
    public Range BaseTimeRange;
    public Range BaseValueRange;
    public SerializedDictionary<Modifier, Material> MaterialDic = new();
    public Mesh Mesh;
    [TextArea(3, 10)]
    public string Description;
}

[Serializable]
public class Range
{
    public float Min;
    public float Max;
}
