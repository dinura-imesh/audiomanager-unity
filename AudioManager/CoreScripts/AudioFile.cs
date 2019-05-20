using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AudioFile
{

    public EnumAudioId audioID;

    public AudioClip audioClip;

    [Range(0f,1f)]
    public float volume;

    [HideInInspector]
    public AudioSource source;

    public bool isLooping;

    public bool playOnAwake;

}
