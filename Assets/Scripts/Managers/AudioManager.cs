using UnityEngine;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    public enum Sounds
    {
        Button,
        Charge,
        MetalImpact,
        EnemyProjectile,
        EnemyDeath
    }

    public enum Songs
    {
        MainMenu,
        Gameplay
    }

    [SerializeField] float soundWavePitchDivider = 0.2f;

    [SerializeField] AudioClip soundWaveClip;

    [SerializeField] AudioSource mainAudioSource;
    [SerializeField] AudioSource soundWaveAudioSource;

    [SerializeField] AudioClip[] sfxs;
    [SerializeField] AudioClip[] music;

    [Header("Sound Options")]
    [SerializeField] bool soundOn = true;
    [SerializeField] bool musicOn = true;

    public override void Awake()
    {
        base.Awake();

        soundWaveAudioSource.clip = soundWaveClip;
    }

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += PlayMusicOnNewScene;
    //}
    //
    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= PlayMusicOnNewScene;
    //}
    //
    //void PlayMusicOnNewScene(Scene scene, LoadSceneMode mode)
    //{
    //    Songs song;
    //
    //    switch (scene.name)
    //    {
    //        case "Main Menu":
    //            song = Songs.MainMenu;
    //            break;
    //        case "Gameplay":
    //            song = Songs.Gameplay;
    //            break;
    //        default:
    //            song = 0;
    //            break;
    //    }
    //
    //    PlayMusic(song);
    //}

    public void PlaySoundWave(int charge)
    {
        if (!soundOn) return;

        soundWaveAudioSource.pitch = 1.0f / (soundWavePitchDivider * charge);
        soundWaveAudioSource.Play();
    }

    public void PlaySound(Sounds sound)
    {
        if (soundOn) mainAudioSource.PlayOneShot(sfxs[(int)sound]);
    }

    public void PlayMusic(Songs song)
    {
        mainAudioSource.clip = music[(int)song];
    
        if (musicOn) mainAudioSource.Play();
    }
    
    public void ToggleSound()
    {
        soundOn = !soundOn;
    }
    
    public void ToggleMusic()
    {
        musicOn = !musicOn;
    
        if (musicOn) mainAudioSource.Play();
        else mainAudioSource.Stop();
    }
}