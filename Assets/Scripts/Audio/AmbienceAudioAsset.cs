using UnityEngine;

[CreateAssetMenu(fileName ="New Ambience Audio", menuName ="Audio/Create New Ambience Audio")]
public class AmbienceAudioAsset : ScriptableObject
{
    public string ambienceName;
    public AudioClip clip;
}
