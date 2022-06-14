using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip StartSound; //開始遊戲音效
    public AudioClip[] ComboSound; //Combo音效
    private AudioSource audioSource; //宣告撥放器

     void Awake()
    {
        if(Instance == null)
        { 
            Instance = this;
            audioSource = GetComponent<AudioSource>(); //獲得在在 Unity 中此物件底下的 audioSource 組件
        }
    }

    public void PlayStartSound()
    {
        audioSource.clip = StartSound;
        audioSource.Play();
    }

    public void PlayComboSound(int combo)
    {
        if (combo < 8)
            audioSource.clip = ComboSound[combo - 1];
        else
            audioSource.clip = ComboSound[7];

        audioSource.Play();
    }

}
