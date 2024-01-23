using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EditModeAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip shortcutClip;
    [SerializeField] private AudioClip cellPlaceEraseClip;
    [SerializeField] private AudioClip cellRotateClip;

    public float uiClickVolume;
    public float shortcutVolume;
    public float cellPlaceEraseVolume;
    public float cellRotateVolume;

    private bool UIClickPlaying;
    private bool shortcutPlaying;
    private bool cellPlaceErasePlaying;
    private bool cellRotatePlaying;

    // Wrapper method is needed since ui buttons can't start coroutines
    public void PlayUIClickWrapper()
    {
        StartCoroutine(PlayUIClick(uiClickClip.length));
    }
    private IEnumerator PlayUIClick(float duration)
    {
        if (UIClickPlaying) yield break;

        UIClickPlaying = true;

        audioSource.volume = uiClickVolume;
        audioSource.clip = uiClickClip;
        audioSource.Play();

        yield return new WaitForSeconds(duration);

        UIClickPlaying = false;
    }

    public IEnumerator PlayShortcut()
    {
        if (shortcutPlaying) yield break;

        shortcutPlaying = true;

        audioSource.volume = shortcutVolume;
        audioSource.clip = shortcutClip;
        audioSource.Play();

        yield return new WaitForSeconds(shortcutClip.length);

        shortcutPlaying = false;
    }

    public IEnumerator PlayCellPlaceErase()
    {
        if (cellPlaceErasePlaying) yield break;

        cellPlaceErasePlaying = true;

        audioSource.volume = cellPlaceEraseVolume;
        audioSource.clip = cellPlaceEraseClip;
        audioSource.Play();

        yield return new WaitForSeconds(cellPlaceEraseClip.length);

        cellPlaceErasePlaying = false;
    }

    public IEnumerator PlayCellRotate()
    {
        if (cellRotatePlaying) yield break;

        cellRotatePlaying = true;

        audioSource.volume = cellRotateVolume;
        audioSource.clip = cellRotateClip;
        audioSource.Play();

        yield return new WaitForSeconds(cellRotateClip.length);

        cellRotatePlaying = false;
    }
}