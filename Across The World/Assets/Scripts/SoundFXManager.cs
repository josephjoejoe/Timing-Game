using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    //this will make the script a singltin so that its accessed easily
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXOnject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // spawns in gameObject
        AudioSource audioSource = Instantiate(soundFXOnject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get legngth of sound FX clip
        float clipLength = audioSource.clip.length;

        //Destroy the clip afterit is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
