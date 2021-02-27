using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UserInteractionControl : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainMenu, credits, logo;
    [SerializeField]
    private Vector2 logoStartPoint, logoEndPoint, mainMenuStartPoint, mainMenuEndPoint, creditsStartPoint, creditsEndPoint;
    [SerializeField]
    private Ease movementType;
    [SerializeField]
    private float movementDelay;

    public CameraController workingCameraController;

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

    private void MoveToView(RectTransform objectPositionToMove, Vector2 inViewCoordinates, Ease movementType)
    {
        objectPositionToMove.DOAnchorPos(inViewCoordinates, movementDelay).SetEase(movementType);
    }

    private void HideFromView(RectTransform objectPositionToMove, Vector2 outOfViewCoordinates, Ease movementType)
    {
        objectPositionToMove.DOAnchorPos(outOfViewCoordinates, movementDelay).SetEase(movementType);
    }
}
