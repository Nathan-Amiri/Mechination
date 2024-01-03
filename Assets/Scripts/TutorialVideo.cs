using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideo : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private List<VideoClip> tutorialClips = new();

    public void SelectNewClip(int clipNumber)
    {
        videoPlayer.clip = tutorialClips[clipNumber];
    }
}