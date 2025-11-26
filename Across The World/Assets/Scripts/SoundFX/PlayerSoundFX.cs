using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging; // for Animation Rigging

public class PlayerSoundFX : MonoBehaviour
{
    // for soundFX
    AudioSource audioSource;
    public AudioClip[] footStepsSounds;
    public AudioClip[] jumpingSounds;
    public AudioClip[] landingSounds;
    public AudioClip[] fallingSounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FootStep()
    {
        int random = Random.Range(0, footStepsSounds.Length);
        var clip = footStepsSounds[random];
        audioSource.PlayOneShot(clip);
    }
    public void Jumping()
    {
        int random = Random.Range(0, jumpingSounds.Length);
        var clip = jumpingSounds[random];
        audioSource.PlayOneShot(clip);
    }
    public void Landing()
    {
        int random = Random.Range(0, landingSounds.Length);
        var clip = landingSounds[random];
        audioSource.PlayOneShot(clip);
    }
    public void Falling()
    {
        int random = Random.Range(0, fallingSounds.Length);
        var clip = fallingSounds[random];
        audioSource.PlayOneShot(clip);
    }
}
