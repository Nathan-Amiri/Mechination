using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject indicatorTextBox;
    [SerializeField] private GameObject indicatorArrow;
    [SerializeField] private GameObject unmaskHole;

    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private TMP_Text indicatorText;

    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    [NonSerialized] public bool tutorialMode;

    private int currentTutorialPage;

    public void SelectEnterExitTutorial(bool enter)
    {
        // turn on tutorial mode
        // cache and set tickSpeed to 0
        // clear layout but don't save
        // cache zoom and zoom in to -6
        // turn on page

        // revert tickspeed and layout
        // reset currenttutorialpage
        // revert zoom
        // turn off page
        // turn off tutorial mode
    }

    public void SelectChangeTutorialPage(bool nextPage)
    {
        currentTutorialPage += nextPage ? 1 : -1;

        switch (currentTutorialPage)
        {
            case 0:
                tutorialText.text = "";

                indicatorTextBox.SetActive(true);
                indicatorTextBox.transform.position = Vector2.zero;
                indicatorText.text = "";

                indicatorArrow.SetActive(true);
                indicatorArrow.transform.position = Vector2.zero;

                unmaskHole.SetActive(true);
                unmaskHole.transform.position = Vector2.zero;

                backButton.interactable = true;
                nextButton.interactable = true;
                break;
        }
    }
}