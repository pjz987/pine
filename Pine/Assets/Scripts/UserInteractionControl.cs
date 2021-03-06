﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UserInteractionControl : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainMenu, levelFinishedMenu, credits, logo, tutorialImage, transition;
    [SerializeField]
    private Vector2 logoStartPoint, logoEndPoint, mainMenuStartPoint, mainMenuEndPoint, levelFinishedStartPoint,
                    creditsStartPoint, creditsEndPoint, tutorialStartPoint, tutorialEndPoint, transitionStartPoint;
    [SerializeField]
    private RectTransform transitionEndPoint;
    [SerializeField]
    private Ease movementType, oneDirectionMovementType;
    [SerializeField]
    private float movementDelay, moveOneDirectionDelay, scaleDelay;
    [SerializeField]
    private float timeBeforeTutorialDisplay, timeBeforeTransitionDisplay;
    [SerializeField]
    private CameraController workingCameraController;
    [SerializeField]
    private Vector3 workingCameraArialViewPosition;

    // Pete audio
    AudioManager audioManager;
    public void Start()
    {
        MoveToView(logo, logoStartPoint, movementType);
        DisplayMainMenu();
        
        // Pete audio
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void DisplayScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void MoveCameraToStart()
    {
        HideFromView(logo, logoEndPoint, movementType);
        HideFromView(mainMenu, mainMenuEndPoint, movementType);
        workingCameraController.OnGamePlay();
        StartCoroutine(WaitAndShowTutorial(timeBeforeTutorialDisplay));
        
        // Pete audio
        audioManager.MusicInAmbienceOut();
    }
    
    public void MoveCameraToArialView()
    {
        if (!workingCameraController.IsInGame)
        {
            workingCameraController.transform.parent.position = new Vector3(0,0,0);
            workingCameraController.transform.localPosition = workingCameraArialViewPosition;
        }
    }
    
    IEnumerator WaitAndShowTutorial(float secondsToWaitFor)
    {
        yield return new WaitForSeconds(secondsToWaitFor);
        DisplayTutorial();
    }
    
    public void MoveTransition()
    {
        StartCoroutine(WaitAndShowTransition(timeBeforeTransitionDisplay));
    }

    IEnumerator WaitAndShowTransition(float secondsToWaitFor)
    {
        yield return new WaitForSeconds(secondsToWaitFor);
        DisplayTransition();
    }

    public void DisplayCredits()
    {
        HideFromView(mainMenu, mainMenuEndPoint, movementType);
        MoveToView(credits, creditsStartPoint, movementType);
    }

    public void DisplayMainMenu()
    {
        HideFromView(credits, creditsEndPoint, movementType);
        MoveToView(mainMenu, mainMenuStartPoint, movementType);
    }

    private void DisplayTutorial()
    {
        MoveToView(tutorialImage, tutorialStartPoint, movementType);
    }

    public void HideTutorial()
    {
        HideFromView(tutorialImage, tutorialEndPoint, movementType);
        workingCameraController.GetCharacterStateMachine().SetUiState(false);
    }

    private void DisplayTransition()
    {
        MoveOneDirection(transition, transitionStartPoint, oneDirectionMovementType);
    }

    public void DisplayFinishedMenu()
    {
        MoveToView(levelFinishedMenu, levelFinishedStartPoint, movementType);
    }

    private void MoveToView(RectTransform objectPositionToMove, Vector2 inViewCoordinates, Ease movementType)
    {
        objectPositionToMove.DOAnchorPos(inViewCoordinates, movementDelay).SetEase(movementType);
    }

    private void HideFromView(RectTransform objectPositionToMove, Vector2 outOfViewCoordinates, Ease movementType)
    {
        objectPositionToMove.DOAnchorPos(outOfViewCoordinates, movementDelay).SetEase(movementType);
    }

    public void ScaleToSize(RectTransform objectPositionToScale, Vector3 scaleToCoordinates)
    {
        objectPositionToScale.DOBlendableScaleBy(scaleToCoordinates, scaleDelay);
    }

    private void MoveOneDirection(RectTransform objectPositionToMove, Vector2 oppositeEndCoordinates, Ease movementType)
    {
        objectPositionToMove.DOAnchorPos(oppositeEndCoordinates, moveOneDirectionDelay).SetEase(movementType).OnComplete(() => objectPositionToMove.position = transitionEndPoint.position);
    }
}
