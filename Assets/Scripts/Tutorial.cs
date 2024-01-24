using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialScreen;

    [SerializeField] private GameObject indicatorArrow;

    [SerializeField] private RectTransform unmaskHoleTransform;

    [SerializeField] private TMP_Text tutorialText;

    [SerializeField] private Button nextButton;

    [NonSerialized] public bool tutorialMode;

    private int currentTutorialPage;

    public void SelectEnterExitTutorial(bool enter)
    {
        if (enter)
        {
            tutorialMode = true;

            // erase mode, no rotation on gadgets, purple node color
            // set tickSpeed to .5
            // clear layout but don't save
            // cache zoom and zoom in to -6

            //maybe reset scene entirely?

            currentTutorialPage = -1;
            NextTutorialPage();

            tutorialScreen.SetActive(true);
        }
        else
        {
            // revert tickspeed (using playerprefs) and saved layout
            // revert zoom

            tutorialScreen.SetActive(false);

            tutorialMode = false;
        }
    }

    public void NextTutorialPage()
    {
        currentTutorialPage++;

        switch (currentTutorialPage)
        {
            case 0: // Welcome

                tutorialText.text = "Welcome to Mechination!\n\n" +
                    "This tutorial will teach you the basic rules of the game.";

                nextButton.interactable = true;

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 1: // Cell Intro

                tutorialText.text = "In Mechination, you can create machines by placing blocks, called Cells, onto the grid.\n\n" +
                    "There are three types of Cells: Pulsers, Magnets, and Nodes.";

                break;
            case 2: // Select Pulser

                tutorialText.text = "Select the Pulser icon,\n\n" +
                    "then place a Pulser onto the grid as indicated";

                nextButton.interactable = false;

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.localPosition = new Vector2(165, -460);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 3: // Place Pulser

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;
            case 4: // Select Magnet

                tutorialText.text = "Next, place a Magnet onto the grid.";

                indicatorArrow.transform.localPosition = new Vector2(295, -320);

                unmaskHoleTransform.localPosition = new Vector2(295, -460);

                break;
            case 5: // Place Magnet

                indicatorArrow.transform.localPosition = new Vector2(-58, -58);

                unmaskHoleTransform.localPosition = new Vector2(-58, -172);

                break;
            case 6: // Select Node

                tutorialText.text = "Finally, place a Node onto the grid.";

                indicatorArrow.transform.localPosition = new Vector2(425, -320);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);

                break;
            case 7: // Place Node

                indicatorArrow.transform.localPosition = new Vector2(172, -58);

                unmaskHoleTransform.localPosition = new Vector2(172, -172);

                break;
            case 8: // Select Green

                tutorialText.text = "Nodes can be placed in six different colors.\n" +
                    "Try switching the Node to green, then back to purple.\n\n" +
                    "A Node's color doesn't affect its behavior.";

                indicatorArrow.transform.localPosition = new Vector2(570, -370);

                unmaskHoleTransform.sizeDelta = new Vector2(34.47f, 34.47f);
                unmaskHoleTransform.localPosition = new Vector2(570, -435);

                break;
            case 9: // Turn Node Green

                indicatorArrow.transform.localPosition = new Vector2(172, -58);

                unmaskHoleTransform.localPosition = new Vector2(172, -172);
                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);

                break;
            case 10: // Select Purple

                indicatorArrow.transform.localPosition = new Vector2(515, -370);

                unmaskHoleTransform.sizeDelta = new Vector2(34.47f, 34.47f);
                unmaskHoleTransform.localPosition = new Vector2(515, -435);

                break;
            case 11: // Turn Node Purple

                indicatorArrow.transform.localPosition = new Vector2(172, -58);

                unmaskHoleTransform.localPosition = new Vector2(172, -172);
                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);

                break;
            case 12: // Select Pulser

                tutorialText.text = "You can't change the color of a Pulser or a Magnet, but you can rotate them.";

                nextButton.interactable = true;

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 13:

                tutorialText.text = "To rotate the Pulser, select the Pulser icon, then left click it on the grid. Rotate it until it's facing up again.";

                nextButton.interactable = false;

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.localPosition = new Vector2(165, -460);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 14: // Rotate Pulser

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;

            // For cases 15-17, do nothing (continue rotating Pulser)

            case 18: // Fasten Intro

                tutorialText.text = "When Cells are placed adjacent to each other (but not diagonally), they Fasten together.";

                nextButton.interactable = true;

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 19: // Select Node

                tutorialText.text = "Place a second Node adjacent to the first.";

                nextButton.interactable = false;

                indicatorArrow.transform.localPosition = new Vector2(425, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 20: // Place and Fasten Node

                indicatorArrow.transform.localPosition = new Vector2(286, -58);

                unmaskHoleTransform.localPosition = new Vector2(286, -172);

                break;
            case 21: // Note Fastener Icon

                tutorialText.text = "Note the two black lines connecting them, indicating that they are Fastened.";

                nextButton.interactable = true;

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 22: // Place Node Above Pulser

                tutorialText.text = "Pulsers and Magnets can't be Fastened in the direction they're facing.\n\n" +
                    "To illustrate this, place Nodes around the Pulser.";

                indicatorArrow.transform.localPosition = new Vector2(-286, 58);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.localPosition = new Vector2(-286, -58);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 23: // Place Node Right of Pulser

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;
            case 24: // Place Node Below Pulser

                indicatorArrow.transform.localPosition = new Vector2(-172, -286);
                indicatorArrow.transform.rotation = Quaternion.Euler(0, 0, -90);

                unmaskHoleTransform.localPosition = new Vector2(-286, -286);

                break;
            case 25: // Place Node Left of Pulser

                indicatorArrow.transform.localPosition = new Vector2(-401, -58);
                indicatorArrow.transform.rotation = Quaternion.identity;

                unmaskHoleTransform.localPosition = new Vector2(-401, -172);

                break;
            case 26: // Select Pulser

                tutorialText.text = "Now, rotate the Pulser until it's facing up again.\n\n" +
                    "Note how the Pulser is never fastened in the direction it's facing";

                indicatorArrow.transform.localPosition = new Vector2(165, -320);

                unmaskHoleTransform.localPosition = new Vector2(165, -460);

                break;
            case 27: // Rotate Pulser

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;

            // For cases 28-30, do nothing (continue rotating Pulser)

            case 31: // Select Eraser

                tutorialText.text = "You can remove Cells from the grid by selecting the Eraser icon.";

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 32: // Erase Cell

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;
            case 33: // Select Clear

                tutorialText.text = "While the Eraser is selected, you can click the Trash icon to clear the whole grid.";

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 34: // Select Proceed

                tutorialText.text = "Be very careful when clearing the grid--you can lose a lot of work!\n\n" +
                    "Go ahead and click Proceed. (You're only clearing this temporary Tutorial grid)";

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.localPosition = new Vector2(300, -150);
                unmaskHoleTransform.sizeDelta = new Vector2(200, 100);

                break;
            case 35: // Pan and Zooom

                tutorialText.text = "When not in the Tutorial, you can pan around the grid using your right mouse button, and zoom in and out with your mouse wheel.";

                nextButton.interactable = true;

                unmaskHoleTransform.gameObject.SetActive(false);
                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);

                break;       
            case 36: // Exit

                tutorialText.text = "When not in the Tutorial, you can quit the game with the Exit button";

                nextButton.interactable = true;

                indicatorArrow.transform.localPosition = new Vector2(-865, -320);
                indicatorArrow.SetActive(true);

                break;
            case 37: // Save

                tutorialText.text = "When designing, be sure to save regularly using the Save button!";

                indicatorArrow.transform.localPosition = new Vector2(-715, -320);

                break;
            case 38: // Layouts

                tutorialText.text = "Up to five grids can be saved at a time. You can switch between them using the number buttons below.";

                indicatorArrow.transform.localPosition = new Vector2(-425, -320);

                break;
            case 39: // Start Play Mode

                tutorialText.text = "Press the Play button below to enter Play Mode.";

                nextButton.interactable = false;

                indicatorArrow.transform.localPosition = new Vector2(-116, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 40: // Tempo

                tutorialText.text = "While in Play Mode, Cells move autonomously according to several basic rules.\n\n" +
                    "When not in the Tutorial, you can speed up/slow down the Cells by pressing the Tempo button.";

                nextButton.interactable = true;

                indicatorArrow.transform.localPosition = new Vector2(15, -320);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 41: // Stop Play Mode

                tutorialText.text = "Press the Stop button to return to Edit Mode, then place Cells on the grid as indicated.";

                nextButton.interactable = false;

                indicatorArrow.transform.localPosition = new Vector2(-116, -320);

                // Unmask Hole is already in the correct position
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 42: // Select Pulser

                indicatorArrow.transform.localPosition = new Vector2(165, -320);

                unmaskHoleTransform.localPosition = new Vector2(165, -460);

                break;
            case 43: // Place Pulser 1

                indicatorArrow.transform.localPosition = new Vector2(-401, -172);

                unmaskHoleTransform.localPosition = new Vector2(-401, -286);

                break;
            case 44: // Place Pulser 2

                indicatorArrow.transform.localPosition = new Vector2(-515, 58);

                unmaskHoleTransform.localPosition = new Vector2(-515, -58);

                break;
            case 45: // Rotate Pulser 2

                indicatorArrow.transform.localPosition = new Vector2(-515, 58);

                unmaskHoleTransform.localPosition = new Vector2(-515, -58);

                break;
            case 46: // Place Pulser 3

                indicatorArrow.transform.localPosition = new Vector2(172, 58);

                unmaskHoleTransform.localPosition = new Vector2(172, -58);

                break;
            case 47: // Place Pulser 4

                indicatorArrow.transform.localPosition = new Vector2(-286, 172);

                unmaskHoleTransform.localPosition = new Vector2(-286, 58);

                break;
            case 48: // Rotate Pulser 4

                indicatorArrow.transform.localPosition = new Vector2(-286, 172);

                unmaskHoleTransform.localPosition = new Vector2(-286, 58);

                break;
            case 49: // Place Pulser 5

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;
            case 50: // Rotate Pulser 5

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;
            case 51: // Place Pulser 6

                indicatorArrow.transform.localPosition = new Vector2(515, 58);

                unmaskHoleTransform.localPosition = new Vector2(515, -58);

                break;
            case 52: // Select Node

                indicatorArrow.transform.localPosition = new Vector2(425, -320);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);

                break;
            case 53: // Place Node 1

                indicatorArrow.transform.localPosition = new Vector2(-401, -58);

                unmaskHoleTransform.localPosition = new Vector2(-401, -172);

                break;
            case 54: // Place Node 2

                indicatorArrow.transform.localPosition = new Vector2(172, -172);

                unmaskHoleTransform.localPosition = new Vector2(172, -286);

                break;
            case 55: // Place Node 3

                indicatorArrow.transform.localPosition = new Vector2(286, -172);

                unmaskHoleTransform.localPosition = new Vector2(286, -286);

                break;
            case 56: // Place Node 4

                indicatorArrow.transform.localPosition = new Vector2(401, -172);

                unmaskHoleTransform.localPosition = new Vector2(401, -286);

                break;
            case 57: // Place Node 5

                indicatorArrow.transform.localPosition = new Vector2(286, -58);

                unmaskHoleTransform.localPosition = new Vector2(286, -172);

                break;
            case 58: // Place Node 6

                indicatorArrow.transform.localPosition = new Vector2(286, 58);

                unmaskHoleTransform.localPosition = new Vector2(286, -58);

                break;
            case 59: // Start Play Mode for Pulser Demo

                tutorialText.text = "If a Pulser detects a Cell 1 space in front of it, it moves the Cell 1 space forward. (As well as any other Fastened Cells or Cells in the way)";
                
                indicatorArrow.transform.localPosition = new Vector2(-116, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);

                break;

            // For case 60, do nothing (player must stop play mode before progressing)

            case 61:

                Debug.Log("Ready for magnets!");

                break;
        }
    }
}