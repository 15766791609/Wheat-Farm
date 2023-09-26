using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class AudioManager : MonoSingleton<AudioManager>
{
    [Header("“Ù¿÷ ˝æ›ø‚")]
    public SoundDetailsList_SO SoundDetailsData;
    public SceneSoundList_SO sceneSoundData;
    [Header("Audio Soure")]
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private float MusicStattSecond;

    private Coroutine soundRoutine;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;


    [Header("Snapshots(“ÙπÏ)")]
    public AudioMixerSnapshot normalSnapShot;
    public AudioMixerSnapshot ambientSnapShot;
    public AudioMixerSnapshot muteSnapShot;
    private float musicTransitionSecond = 5f;


    private void Start()
    {
        MusicStattSecond = UnityEngine.Random.Range(3, 8);
    }
    private void OnEnable()
    {
        EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEven;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEven;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
    }

    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails = SoundDetailsData.GetsoundDetails(soundName);
        if(soundDetails != null)
        {
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }

    private void OnAfterScenenUnloadEven()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
            return;
        SoundDetails ambient = SoundDetailsData.GetsoundDetails(sceneSound.ambient);
        SoundDetails music = SoundDetailsData.GetsoundDetails(sceneSound.music);

        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
        soundRoutine = StartCoroutine(PlaySoundRotine(music, ambient));
    }


    private IEnumerator PlaySoundRotine(SoundDetails music, SoundDetails ambient)
    {
        if(music!=null && ambient != null)
        {
            PlayeAmbientClip(ambient,1f);
            yield return new WaitForSeconds(MusicStattSecond);
            PlayeMusicClip(music, musicTransitionSecond);
        }
    }


    /// <summary>
    /// ≤•∑≈±≥æ∞“Ù¿÷
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayeMusicClip(SoundDetails soundDetails, float transitionTime)
    {

        audioMixer.SetFloat("MusicVolume", ConverSoundVolume(soundDetails.soundVolume));
        gameSource.clip = soundDetails.soundClip;
        if(gameSource.isActiveAndEnabled)
        {
            gameSource.Play();
        }
        normalSnapShot.TransitionTo(transitionTime);
    }



    /// <summary>
    /// ≤•∑≈ª∑æ≥“Ù–ß
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayeAmbientClip(SoundDetails soundDetails,float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConverSoundVolume(soundDetails.soundVolume));

        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
        {
            ambientSource.Play();
        }
        ambientSnapShot.TransitionTo(transitionTime);
    }

    /// <summary>
    /// Ω´ ˝æ›∑∂Œß∆•≈‰
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    private float ConverSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }
}
