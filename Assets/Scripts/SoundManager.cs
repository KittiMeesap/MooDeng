using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // สำหรับเพลงประกอบ (Background Music)
    [SerializeField] private AudioSource sfxSource; // สำหรับเสียงเอฟเฟกต์ (Sound Effects)

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
        // ทำให้ SoundManager เป็น Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // คงอยู่เมื่อเปลี่ยน Scene
        }
        else
        {
            Destroy(gameObject); // ทำลายตัวซ้ำ
        }
    }

    /// <summary>
    /// เล่นเสียง BGM
    /// </summary>
    /// <param name="clip">AudioClip ที่ต้องการเล่น</param>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// หยุดเสียง BGM
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// เล่นเสียงเอฟเฟกต์
    /// </summary>
    /// <param name="clip">AudioClip ที่ต้องการเล่น</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// ปรับระดับเสียงของ BGM
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// ปรับระดับเสียงของ SFX
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
    /// <summary>
    /// เล่นเสียงเดินทีละรอบ
    /// </summary>
    public void PlayPlayerWalk(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Player walk clip is null.");
            return;
        }

        // ตรวจสอบว่าเสียงยังไม่เล่นอยู่
        if (!sfxSource.isPlaying)
        {
            Debug.Log("Playing player walk clip.");
            sfxSource.clip = clip;
            sfxSource.loop = false; // ไม่วนเสียง
            sfxSource.Play();
        }
        else
        {
            Debug.Log("Walk sound is already playing.");
        }
    }


}
