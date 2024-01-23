using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    // EditMode audio:
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip shortcutClip;
    [SerializeField] private AudioClip cellPlaceEraseClip;
    [SerializeField] private AudioClip cellRotateClip;

    private bool UIClickPlaying;
    private bool shortcutPlaying;
    private bool cellPlaceErasePlaying;
    private bool cellRotatePlaying;

    // PlayMode audio:
    [SerializeField] private AudioClip pulserPushClip;
    [SerializeField] private AudioClip magnetPullClip;
    [SerializeField] private AudioClip nodeReverseClip;

    static public int pulserPushVolumeLevel;
    static public int magnetPullVolumeLevel;
    static public int  nodeReverseVolumeLevel;

    // PlayMode audio:
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

    // PlayMode audio:
    public void PlayNodeSounds()
    {
        if (nodeReverseVolumeLevel > 0)
        {
            float volume = .02f + .02f * nodeReverseVolumeLevel;
            audioSource.PlayOneShot(nodeReverseClip, volume);

            // Reset
            nodeReverseVolumeLevel = 0;
        }
    }

    public void PlayGadgetSounds()
    {
        if (pulserPushVolumeLevel > 0)
        {
            float volume = .02f + .02f * pulserPushVolumeLevel;
            audioSource.PlayOneShot(pulserPushClip, volume);

            // Reset
            pulserPushVolumeLevel = 0;
        }

        if (magnetPullVolumeLevel > 0)
        {
            float volume = .02f + .02f * magnetPullVolumeLevel;
            audioSource.PlayOneShot(magnetPullClip, volume);

            // Reset
            magnetPullVolumeLevel = 0;
        }
    }
}