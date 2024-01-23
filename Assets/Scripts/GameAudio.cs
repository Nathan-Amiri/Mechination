using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip shortcutClip;
    [SerializeField] private AudioClip cellPlaceEraseClip;
    [SerializeField] private AudioClip cellRotateClip;

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

        audioSource.PlayOneShot(uiClickClip, .5f);

        yield return new WaitForSeconds(duration);

        UIClickPlaying = false;
    }

    public IEnumerator PlayShortcut()
    {
        if (shortcutPlaying) yield break;

        shortcutPlaying = true;

        audioSource.PlayOneShot(shortcutClip, .5f);

        yield return new WaitForSeconds(shortcutClip.length);

        shortcutPlaying = false;
    }

    public IEnumerator PlayCellPlaceErase()
    {
        if (cellPlaceErasePlaying) yield break;

        cellPlaceErasePlaying = true;

        audioSource.PlayOneShot(cellPlaceEraseClip, .5f);

        yield return new WaitForSeconds(cellPlaceEraseClip.length);

        cellPlaceErasePlaying = false;
    }

    public IEnumerator PlayCellRotate()
    {
        if (cellRotatePlaying) yield break;

        cellRotatePlaying = true;

        audioSource.PlayOneShot(cellRotateClip, .5f);

        yield return new WaitForSeconds(cellRotateClip.length);

        cellRotatePlaying = false;
    }






    [SerializeField] private AudioClip pulserPushClip;
    [SerializeField] private AudioClip magnetPullClip;
    [SerializeField] private AudioClip nodeReverseClip;

    static public float pulserPushVolume;
    static public float magnetPullVolume;
    static public float nodeReverseVolume;

    public void PlayNodeSounds()
    {
        if (nodeReverseVolume > 0)
        {
            audioSource.PlayOneShot(nodeReverseClip, nodeReverseVolume);

            // Reset
            nodeReverseVolume = 0;
        }
    }

    public void PlayGadgetSounds()
    {
        if (pulserPushVolume > 0)
        {
            audioSource.PlayOneShot(pulserPushClip, pulserPushVolume);

            // Reset
            pulserPushVolume = 0;
        }

        if (magnetPullVolume > 0)
        {
            audioSource.PlayOneShot(magnetPullClip, magnetPullVolume);

            // Reset
            magnetPullVolume = 0;
        }
    }
}