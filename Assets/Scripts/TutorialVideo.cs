using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideo : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private List<VideoClip> tutorialClips = new();

    [SerializeField] private GameObject clickMessage;

    public void SelectNewClip(int clipNumber)
    {
        clickMessage.SetActive(false);

        videoPlayer.Stop();
        videoPlayer.clip = tutorialClips[clipNumber];
        videoPlayer.Play();
    }
}