using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // ����Ѻ�ŧ��Сͺ (Background Music)
    [SerializeField] private AudioSource sfxSource; // ����Ѻ���§�Ϳ࿡�� (Sound Effects)

    [Header("Audio Clips")]
    public AudioClip playerDeathClip;
    public AudioClip playerJumpClip;
    public AudioClip playerWalkClip;
    public AudioClip gameStartClip;
    public AudioClip gameWinClip;
    public AudioClip enemyDeathClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyNearClip;
    public AudioClip getCarrotClip;
    public AudioClip soundtrackClip;
    public AudioClip portalopenClip;
    public AudioClip portalgoinClip;

    private void Awake()
    {
        // ����� SoundManager �� Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���������������¹ Scene
        }
        else
        {
            Destroy(gameObject); // ����µ�ǫ��
        }
    }

    /// <summary>
    /// ������§ BGM
    /// </summary>
    /// <param name="clip">AudioClip ����ͧ������</param>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// ��ش���§ BGM
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// ������§�Ϳ࿡��
    /// </summary>
    /// <param name="clip">AudioClip ����ͧ������</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// ��Ѻ�дѺ���§�ͧ BGM
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// ��Ѻ�дѺ���§�ͧ SFX
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
    /// <summary>
    /// ������§�Թ�����ͺ
    /// </summary>
    public void PlayPlayerWalk(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Player walk clip is null.");
            return;
        }

        // ��Ǩ�ͺ������§�ѧ����������
        if (!sfxSource.isPlaying)
        {
            Debug.Log("Playing player walk clip.");
            sfxSource.clip = clip;
            sfxSource.loop = false; // ���ǹ���§
            sfxSource.Play();
        }
        else
        {
            Debug.Log("Walk sound is already playing.");
        }
    }


}
