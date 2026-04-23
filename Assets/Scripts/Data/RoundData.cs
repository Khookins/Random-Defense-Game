using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "RoundData")]
public class RoundData : ScriptableObject
{
    public List<WaveData> waves;
}
