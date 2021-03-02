using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UserInteractionControl : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainMenu, credits, logo, tutorialImage, transition;
    [SerializeField]
    private Vector2 logoStartPoint, logoEndPoint, mainMenuStartPoint, mainMenuEndPoint, creditsStartPoint, creditsEndPoint,
                    tutorialStartPoint, tutorialEndPoint, transitionStarPoint, transitionEndPoint;
    [SerializeField]
    private Ease movementType;
    [SerializeField]
    private float movementDelay;
    [SerializeField]
    private float timeBeforeTutorialDisplay;
    [SerializeField]
    private CameraController workingCameraController;

    public void Start()
    {
        MoveToView(logo, logoStartPoint, movementType);
        DisplayMainMenu();
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
    }

    IEnumerator WaitAndShowTutorial(float secondsToWaitFor)
    {
        yield return new WaitForSeconds(secondsToWaitFor);
        DisplayTutorial();
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

        // my line - transition playerstate from ui to pinecone_standingby
        workingCameraController.GetCharacterStateMachine().SetUiState(false);
    }

    public void DisplayTransition()
    {
        MoveOneDirection(transition, transitionStarPoint, movementType);
    }

    private void MoveToView(RectTransform objectPositionToMove, Vector2 inViewCoordinates, Ease movementType)
    {
        objectPositionToMove.DOAnchorPos(inViewCoordinates, movementDelay).SetEase(movementType);
    }

    private void HideFromView(RectTransform objectPositionToMove, Vector2 outOfViewCoordinates, Ease movementType)
    {
        objectPositionToMove.DOAnchorPos(outOfViewCoordinates, movementDelay).SetEase(movementType);
    }

    private void ScaleToSize(RectTransform objectPositionToScale, Vector3 scaleCoordinates)
    {
        //work in progress
        objectPositionToScale.DOPunchScale(scaleCoordinates, movementDelay);
    }
    private void MoveOneDirection(RectTransform objectPositionToMove, Vector2 oppositeEndCoordinates, Ease movementType)
    {
        //work in progress
        objectPositionToMove.DOAnchorPos(oppositeEndCoordinates, movementDelay).SetEase(movementType).OnComplete(() => objectPositionToMove.DORewind());
        objectPositionToMove.DORestart();
    }
}
