using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public AudioClip testClip;

    void Start()
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.clip = testClip;
        src.volume = 1f;
        src.spatialBlend = 0; // 2D
        src.loop = true;
        src.Play();
        Debug.Log("Music should be playing now!");
    }
}
