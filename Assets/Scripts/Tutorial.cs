using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialScreen;

    [SerializeField] private GameObject nextArrow;
    [SerializeField] private GameObject indicatorArrow;
    [SerializeField] private GameObject ring;

    [SerializeField] private RectTransform unmaskHoleTransform;

    [SerializeField] private TMP_Text tutorialText;

    [SerializeField] private Button nextButton;

    [NonSerialized] public bool tutorialMode;

    private int currentTutorialPage;

    public void SelectEnterExitTutorial(bool enter)
    {




        //prevent tutorial playstop from overriding save layout





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
            // clear tutorial grid somehow

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
                nextArrow.SetActive(true);

                indicatorArrow.SetActive(false);
                indicatorArrow.transform.localRotation = Quaternion.identity;

                ring.SetActive(false);
                ring.transform.localScale = new Vector2(1, 1);

                unmaskHoleTransform.gameObject.SetActive(false);
                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);

                break;
            case 1: // Cell Intro

                tutorialText.text = "In Mechination, you can create machines by placing blocks, called Cells, onto the grid.\n\n" +
                    "There are 3 types of Cells: Pulsers, Magnets, and Nodes.";

                break;
            case 2: // Select Pulser

                tutorialText.text = "Select the Pulser icon,\n\n" +
                    "then place a Pulser onto the grid as indicated.";

                nextButton.interactable = false;
                nextArrow.SetActive(false);

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

                tutorialText.text = "Nodes can be placed in 6 different colors.\n" +
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

                tutorialText.text = "You can't change the color of a Pulser or Magnet, but you can rotate them.";

                nextButton.interactable = true;
                nextArrow.SetActive(true);

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 13:

                tutorialText.text = "To rotate the Pulser, select the Pulser icon, then left click it on the grid. Rotate it until it's facing up again.";

                nextButton.interactable = false;
                nextArrow.SetActive(false);

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.localPosition = new Vector2(165, -460);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 14: // Rotate Pulser Right

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;

            // Case 15: Rotate Pulser Down

            // Case 16: Rotate Pulser Left

            // Case 17: Rotate Pulser Up

            case 18: // Fasten Intro

                tutorialText.text = "When Cells are placed adjacent to each other (but not diagonally), they Fasten together.";

                nextButton.interactable = true;
                nextArrow.SetActive(true);

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 19: // Select Node

                tutorialText.text = "Place a second Node adjacent to the first.";

                nextButton.interactable = false;
                nextArrow.SetActive(false);

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
                nextArrow.SetActive(true);

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 22: // Place Node Above Pulser

                tutorialText.text = "Pulsers and Magnets can't be Fastened in the direction they're facing.\n\n" +
                    "To illustrate this, place Nodes around the Pulser.";

                nextButton.interactable = false;
                nextArrow.SetActive(false);

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
                    "Note how the Pulser is never fastened in the direction it's facing.";

                indicatorArrow.transform.localPosition = new Vector2(165, -320);

                unmaskHoleTransform.localPosition = new Vector2(165, -460);

                break;
            case 27: // Rotate Pulser Right

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;

            // Case 28: Rotate Pulser Down

            // Case 29: Rotate Pulser Left

            // Case 30: Rotate Pulser Up

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
                nextArrow.SetActive(true);

                unmaskHoleTransform.gameObject.SetActive(false);
                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);

                break;       
            case 36: // Exit

                tutorialText.text = "When not in the Tutorial, you can quit the game with the Exit button.";

                ring.transform.localPosition = new Vector2(-865, -460);
                ring.SetActive(true);

                break;
            case 37: // Save

                tutorialText.text = "When designing, be sure to save regularly using the Save button!";

                ring.transform.localPosition = new Vector2(-715, -460);

                break;
            case 38: // Layouts

                tutorialText.text = "Up to 5 grids can be saved at a time. You can switch between them using the number buttons below.";

                ring.transform.localPosition = new Vector2(-425, -460);
                ring.transform.localScale = new Vector2(3.5f, 1);

                break;
            case 39: // Start Play Mode

                tutorialText.text = "Press the Play button below to enter Play Mode.";

                nextButton.interactable = false;
                nextArrow.SetActive(false);

                indicatorArrow.transform.localPosition = new Vector2(-115, -320);
                indicatorArrow.SetActive(true);

                ring.SetActive(false);
                ring.transform.localScale = new Vector2(1, 1);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 40: // Tempo

                tutorialText.text = "While in Play Mode, Cells move autonomously according to several basic rules.\n\n" +
                    "When not in the Tutorial, you can speed up/slow down the Cells by pressing the Tempo button.";

                nextButton.interactable = true;
                nextArrow.SetActive(true);

                indicatorArrow.SetActive(false);

                ring.transform.localPosition = new Vector2(15, -460);
                ring.SetActive(true);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;
            case 41: // Stop Play Mode

                tutorialText.text = "Press the Stop button to return to Edit Mode, then place/rotate Cells on the grid as indicated.";

                nextButton.interactable = false;
                nextArrow.SetActive(false);

                indicatorArrow.transform.localPosition = new Vector2(-115, -320);
                indicatorArrow.SetActive(true);

                ring.SetActive(false);

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

            // Case 45: Rotate Pulser 2 Right

            case 46: // Place Pulser 3

                indicatorArrow.transform.localPosition = new Vector2(172, 58);

                unmaskHoleTransform.localPosition = new Vector2(172, -58);

                break;
            case 47: // Place Pulser 4

                indicatorArrow.transform.localPosition = new Vector2(-286, 172);

                unmaskHoleTransform.localPosition = new Vector2(-286, 58);

                break;

            // Case 48: Rotate Pulser 4 Down

            case 49: // Place Pulser 5

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;

            // Case 50: Rotate Pulser 4 Left

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

                tutorialText.text = "If a Pulser detects a Cell 1 space in front of it, it moves the Cell 1 space forward. " +
                    "(As well as any other Fastened Cells or Cells in the way)";
                
                indicatorArrow.transform.localPosition = new Vector2(-115, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);

                break;

            // Case 60: Stop Play Mode

            case 61: // Select Eraser

                tutorialText.text = "Clear the grid, then place/rotate Cells on the grid as indicated.";

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 62: // Select Clear

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 63: // Select Proceed

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.localPosition = new Vector2(300, -150);
                unmaskHoleTransform.sizeDelta = new Vector2(200, 100);

                break;
            case 64: // Select Pulser

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);
                unmaskHoleTransform.localPosition = new Vector2(165, -460);

                break;
            case 65: // Place Pusler 1

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;

            // Case 66: Rotate Pulser 1 Up

            // Case 67: Rotate Pulser 1 Right

            case 68: // Select Magnet

                indicatorArrow.transform.localPosition = new Vector2(295, -320);

                unmaskHoleTransform.localPosition = new Vector2(295, -460);

                break;
            case 69: // Place Magnet 1

                indicatorArrow.transform.localPosition = new Vector2(-630, -172);

                unmaskHoleTransform.localPosition = new Vector2(-630, -286);

                break;
            case 70: // Place Magnet 2

                indicatorArrow.transform.localPosition = new Vector2(-630, 172);

                unmaskHoleTransform.localPosition = new Vector2(-630, 58);

                break;

            // Case 71: Rotate Magnet 2 Right

            // Case 72: Rotate Magnet 2 Down

            case 73: // Place Magnet 3

                indicatorArrow.transform.localPosition = new Vector2(-58, -58);

                unmaskHoleTransform.localPosition = new Vector2(-58, -172);

                break;

            // Case 74: rotate Magnet 3 Left

            case 75: // Select Node

                indicatorArrow.transform.localPosition = new Vector2(425, -320);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);

                break;
            case 76: // Place Node 1

                indicatorArrow.transform.localPosition = new Vector2(-745, -58);
                indicatorArrow.transform.rotation = Quaternion.Euler(0, 0, 90);

                unmaskHoleTransform.localPosition = new Vector2(-630, -58);

                break;
            case 77: // Place Node 2

                indicatorArrow.transform.localPosition = new Vector2(-515, 58);
                indicatorArrow.transform.rotation = Quaternion.identity;

                unmaskHoleTransform.localPosition = new Vector2(-515, -58);

                break;
            case 78: // Place Node 3

                indicatorArrow.transform.localPosition = new Vector2(-401, -58);

                unmaskHoleTransform.localPosition = new Vector2(-401, -172);
                break;
            case 79: // Place Node 4

                indicatorArrow.transform.localPosition = new Vector2(-401, 58);

                unmaskHoleTransform.localPosition = new Vector2(-401, -58);
                break;
            case 80: // Place Node 5

                indicatorArrow.transform.localPosition = new Vector2(-401, 172);

                unmaskHoleTransform.localPosition = new Vector2(-401, 58);
                break;
            case 81: // Place Node 6

                indicatorArrow.transform.localPosition = new Vector2(-286, -286);
                indicatorArrow.transform.rotation = Quaternion.Euler(0, 0, 90);

                unmaskHoleTransform.localPosition = new Vector2(-172, -286);
                break;
            case 82: // Place Node 7

                indicatorArrow.transform.localPosition = new Vector2(-58, 58);
                indicatorArrow.transform.rotation = Quaternion.identity;

                unmaskHoleTransform.localPosition = new Vector2(-58, -58);

                break;
            case 83: // Start Play Mode for Magnet Demo

                tutorialText.text = "If a Magnet detects a Cell 2 spaces in front of it and an empty space 1 space in front of it, " +
                    "it moves the Cell 1 space backward. (As well as any other Fastened Cells or Cells in the way)";

                indicatorArrow.transform.localPosition = new Vector2(-115, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);

                break;

            // Case 84: Stop Play Mode

            case 85: // Select Eraser

                tutorialText.text = "Clear the grid, then place/rotate Cells on the grid as indicated.";

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 86: // Select Clear

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 87: // Select Proceed

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.localPosition = new Vector2(300, -150);
                unmaskHoleTransform.sizeDelta = new Vector2(200, 100);

                break;
            case 88: // Select Pulser

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);
                unmaskHoleTransform.localPosition = new Vector2(165, -460);

                break;
            case 89: // Place Pulser 1

                indicatorArrow.transform.localPosition = new Vector2(-286, 172);

                unmaskHoleTransform.localPosition = new Vector2(-286, 58);

                break;

            // Case 90: Rotate Pulser 1 Down

            case 91: // Place Pulser 2

                indicatorArrow.transform.localPosition = new Vector2(-58, 172);

                unmaskHoleTransform.localPosition = new Vector2(-58, 58);

                break;

            // Case 92: Rotate Pulser 2 Left

            // Case 93: Rotate Pulser 2 Up

            case 94: // Place Pulser 3

                indicatorArrow.transform.localPosition = new Vector2(-286, -172);

                unmaskHoleTransform.localPosition = new Vector2(-286, -286);

                break;
            case 95: // Select Magnet

                indicatorArrow.transform.localPosition = new Vector2(295, -320);

                unmaskHoleTransform.localPosition = new Vector2(295, -460);

                break;
            case 96: // Place Magnet 1

                indicatorArrow.transform.localPosition = new Vector2(172, 58);

                unmaskHoleTransform.localPosition = new Vector2(172, -58);

                break;

            // Case 97: Rotate Magnet 1 Up

            case 98: // Select Node

                indicatorArrow.transform.localPosition = new Vector2(425, -320);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);

                break;
            case 99: // Place Node 1

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;
            case 100: // Place Node 2

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;
            case 101: // Place Node 3

                indicatorArrow.transform.localPosition = new Vector2(-58, -58);

                unmaskHoleTransform.localPosition = new Vector2(-58, -172);

                break;
            case 102: // Place Node 4

                indicatorArrow.transform.localPosition = new Vector2(58, -58);

                unmaskHoleTransform.localPosition = new Vector2(58, -172);

                break;
            case 103: // Start Play Mode for Node Demo

                tutorialText.text = "If a Node moves onto a Space next to or behind a Pulser/Magnet, it transforms the Pulser into a Magnet or vice versa.";

                indicatorArrow.transform.localPosition = new Vector2(-115, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);

                break;

            // Case 104: Stop Play Mode

            case 105: // Select Eraser

                tutorialText.text = "Clear the grid.";

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 106: // Select Clear

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 107: // Select Proceed

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.localPosition = new Vector2(300, -150);
                unmaskHoleTransform.sizeDelta = new Vector2(200, 100);

                break;
            case 108: // Fail Condition Intro

                tutorialText.text = "Under certain conditions, a Pulser/Magnet will fail to move any Cells. There are 3 of these fail conditions.";

                nextButton.interactable = true;
                nextArrow.SetActive(true);

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.gameObject.SetActive(false);
                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);

                break;
            case 109: // Select Pulser

                tutorialText.text = "Place/rotate Cells on the grid as indicated.";

                nextButton.interactable = false;
                nextArrow.SetActive(false);

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.localPosition = new Vector2(165, -460);
                unmaskHoleTransform.gameObject.SetActive(true);

                break;
            case 110: // Place Pulser 1

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;
            case 111: // Select Magnet

                indicatorArrow.transform.localPosition = new Vector2(295, -320);

                unmaskHoleTransform.localPosition = new Vector2(295, -460);

                break;
            case 112: // Place Magnet 1

                indicatorArrow.transform.localPosition = new Vector2(172, -58);

                unmaskHoleTransform.localPosition = new Vector2(172, -172);

                break;
            case 113: // Select Node

                indicatorArrow.transform.localPosition = new Vector2(425, -320);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);

                break;
            case 114: // Place Node 1

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;
            case 115: // Place Node 2

                indicatorArrow.transform.localPosition = new Vector2(-172, 58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -58);

                break;
            case 116: // Place Node 3

                indicatorArrow.transform.localPosition = new Vector2(-286, 58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -58);

                break;
            case 117: // Place Node 4

                indicatorArrow.transform.localPosition = new Vector2(286, -58);

                unmaskHoleTransform.localPosition = new Vector2(286, -172);

                break;
            case 118: // Place Node 5

                indicatorArrow.transform.localPosition = new Vector2(286, 58);

                unmaskHoleTransform.localPosition = new Vector2(286, -58);

                break;
            case 119: // Place Node 6

                indicatorArrow.transform.localPosition = new Vector2(286, 172);

                unmaskHoleTransform.localPosition = new Vector2(286, 58);

                break;
            case 120: // Place Node 7

                indicatorArrow.transform.localPosition = new Vector2(172, 172);

                unmaskHoleTransform.localPosition = new Vector2(172, 58);

                break;
            case 121: // Start Play Mode for Fail 1 Demo

                tutorialText.text = "Fail Condition 1:\n" +
                    "A Pulser/Magnet will fail if it's trying to move itself.";

                indicatorArrow.transform.localPosition = new Vector2(-115, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);

                break;

            // Case 122: Stop Play Mode

            case 123: // Select Eraser

                tutorialText.text = "Clear the grid, then place/rotate Cells on the grid as indicated.";

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 124: // Select Clear

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 125: // Select Proceed

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.localPosition = new Vector2(300, -150);
                unmaskHoleTransform.sizeDelta = new Vector2(200, 100);

                break;
            case 126: // Select Pulser

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);
                unmaskHoleTransform.localPosition = new Vector2(165, -460);

                break;
            case 127: // Place Pulser 1

                indicatorArrow.transform.localPosition = new Vector2(-401, -172);

                unmaskHoleTransform.localPosition = new Vector2(-401, -286);

                break;
            case 128: // Place Pulser 2

                indicatorArrow.transform.localPosition = new Vector2(286, -172);

                unmaskHoleTransform.localPosition = new Vector2(286, -286);

                break;
            case 129: // Place Pulser 3

                indicatorArrow.transform.localPosition = new Vector2(-172, 58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -58);

                break;

            // Case 130: Rotate Pulser 3 Right

            // Case 131: Rotate Pulser 3 Down

            // Case 132: Rotate Pulser 3 Left

            case 133: // Select Magnet

                indicatorArrow.transform.localPosition = new Vector2(295, -320);

                unmaskHoleTransform.localPosition = new Vector2(295, -460);

                break;
            case 134: // Place Magnet 1

                indicatorArrow.transform.localPosition = new Vector2(401, 58);

                unmaskHoleTransform.localPosition = new Vector2(401, -58);

                break;

            // Case 135: Rotate Magnet 1 Right

            // Case 136: Rotate Magnet 1 Down

            // Case 137: Rotate Magnet 1 Left

            case 138: // Select Node

                indicatorArrow.transform.localPosition = new Vector2(425, -320);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);

                break;
            case 139: // Place Node 1

                indicatorArrow.transform.localPosition = new Vector2(-401, -58);

                unmaskHoleTransform.localPosition = new Vector2(-401, -172);

                break;
            case 140: // Place Node 2

                indicatorArrow.transform.localPosition = new Vector2(-286, 58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -58);

                break;
            case 141: // Place Node 3

                indicatorArrow.transform.localPosition = new Vector2(172, 58);

                unmaskHoleTransform.localPosition = new Vector2(172, -58);

                break;
            case 142: // Place Node 4

                indicatorArrow.transform.localPosition = new Vector2(286, -58);

                unmaskHoleTransform.localPosition = new Vector2(286, -172);

                break;
            case 143: // Start Play Mode for Fail 2 Demo

                tutorialText.text = "Fail Condition 2:\n" +
                    "A Pulser/Magnet will fail if it's trying to move a Cell into a space that another Cell is trying to move into.";

                indicatorArrow.transform.localPosition = new Vector2(-115, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);

                break;

            // Case 144: Stop Play Mode

            case 145: // Select Eraser

                tutorialText.text = "Clear the grid, then place/rotate Cells on the grid as indicated.";

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 146: // Select Clear

                indicatorArrow.transform.localPosition = new Vector2(725, -320);

                unmaskHoleTransform.localPosition = new Vector2(725, -460);

                break;
            case 147: // Select Proceed

                indicatorArrow.SetActive(false);

                unmaskHoleTransform.localPosition = new Vector2(300, -150);
                unmaskHoleTransform.sizeDelta = new Vector2(200, 100);

                break;
            case 148: // Select Pulser

                indicatorArrow.transform.localPosition = new Vector2(165, -320);
                indicatorArrow.SetActive(true);

                unmaskHoleTransform.sizeDelta = new Vector2(100, 100);
                unmaskHoleTransform.localPosition = new Vector2(165, -460);

                break;
            case 149: // Place Pulser 1

                indicatorArrow.transform.localPosition = new Vector2(-172, -58);

                unmaskHoleTransform.localPosition = new Vector2(-172, -172);

                break;
            case 150: // Place Pulser 2

                indicatorArrow.transform.localPosition = new Vector2(172, -58);

                unmaskHoleTransform.localPosition = new Vector2(172, -172);

                break;

            // Case 151: Rotate Pulser 2 Up

            case 152: // Place Pulser 3

                indicatorArrow.transform.localPosition = new Vector2(-401, 58);

                unmaskHoleTransform.localPosition = new Vector2(-401, -58);

                break;

            // Case 153: // Rotate Pulser 3 Right

            case 154: // Select Magnet

                indicatorArrow.transform.localPosition = new Vector2(295, -320);

                unmaskHoleTransform.localPosition = new Vector2(295, -460);

                break;
            case 155: // Place Magnet 1

                indicatorArrow.transform.localPosition = new Vector2(401, 58);

                unmaskHoleTransform.localPosition = new Vector2(401, -58);

                break;
            case 156: // Select Node

                indicatorArrow.transform.localPosition = new Vector2(425, -320);

                unmaskHoleTransform.localPosition = new Vector2(425, -460);

                break;
            case 157: // Place Node 1

                indicatorArrow.transform.localPosition = new Vector2(-286, -58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -172);

                break;
            case 158: // Place Node 2

                indicatorArrow.transform.localPosition = new Vector2(-286, 58);

                unmaskHoleTransform.localPosition = new Vector2(-286, -58);

                break;
            case 159: // Place Node 3

                indicatorArrow.transform.localPosition = new Vector2(172, 58);

                unmaskHoleTransform.localPosition = new Vector2(172, -58);

                break;
            case 160: // Start Play Mode for Fail 3 Demo

                tutorialText.text = "Fail Condition 3:\n" +
                    "A Pulser/Magnet will fail if it's trying to move a Cell that another Pulser/Magnet is trying to move in a <u>different</u> direction.";

                indicatorArrow.transform.localPosition = new Vector2(-115, -320);

                unmaskHoleTransform.localPosition = new Vector2(-115, -460);

                break;

            // Case 161: // Stop Play mode

            case 162: // Closing Message

                tutorialText.text = "You're ready to create on your own!\n\n" +
                    "Have fun designing :)";

                indicatorArrow.transform.localPosition = new Vector2(-730, 400);
                indicatorArrow.transform.rotation = Quaternion.Euler(0, 0, 90);

                unmaskHoleTransform.gameObject.SetActive(false);

                break;


                //choice:
                //Which would you like to do?
                //Play the tutorial(recommended for new players)
                //View the game rules


                //handle saving
                //finish first method
        }
    }
}