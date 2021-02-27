using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
<<<<<<< Updated upstream

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 mountainCenter = new Vector3(0, 0, 0);
    [SerializeField, Range(1,10)] float distanceFromPlayer = 5f;
    [SerializeField, Range(0, 4)] float cameraHeight = 3f;
    [SerializeField, Range(0, 45)] float tiltAngle = 20f;
    [SerializeField]
    private float movementDelay;
    [SerializeField]
    Ease movementType;
    GameObject characterController = null;
    private bool isInGame;

    virtual public void OnGamePlay()
    {
        if (characterController == null)
            characterController = GameObject.FindWithTag("GameController");

        isInGame = true;

        SetPositionOffset(movementType, movementDelay);
    }

     private void Update()
     {
        if (isInGame)
        {
=======
public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 mountainCenter = new Vector3(0, 0, 0);
    [SerializeField, Range(1,20)] float distanceFromPlayer = 5f;
    [SerializeField, Range(0, 20)] float cameraHeight = 3f;
    [SerializeField, Range(0, 60)] float tiltAngle = 20f;
    [SerializeField]
    Ease movementType;
    [SerializeField]
    private float movementDelay;
    virtual public bool IsInGame { get; set; }
    GameObject characterController = null;
    public GameObject initialObjectToFocusOn;

    virtual public void OnGamePlay()
    {
        // Find the character controller object.
        if (characterController == null)
        {
            characterController = GameObject.FindWithTag("GameController");
            MoveCameraToPositionOffSet(movementType, movementDelay);
        }

        // If the character controller reference is still null, send debug message.
        if (characterController == null)
               Debug.Log("No character controller in scene. Must add to scene for camera to work properly.");
     }



     private void Update()
     {
        if (IsInGame)
        {
            SetPositionOffset();
>>>>>>> Stashed changes
            SetRotation();
        }
     }

<<<<<<< Updated upstream
     /// <summary>
     /// Offsets the position of the camera from the camera's parent position.
     /// Should only be called in Start or Awake since it overrides rotation from parent.
     /// </summary>
     void SetPositionOffset(Ease movementType, float movementDelay)
=======
    private void MoveCameraToPositionOffSet(Ease movementType, float movementDelay)
    {
        Vector3 offset = new Vector3(0, cameraHeight, -distanceFromPlayer);
        Quaternion targetTilt = Quaternion.Euler(tiltAngle, 0, 0);
        
        transform.DOLocalRotate(targetTilt.eulerAngles, movementDelay).SetEase(movementType);
        transform.DOMove(initialObjectToFocusOn.transform.position + offset, movementDelay).SetEase(movementType).OnComplete(() => IsInGame = true);
    }
    
    

    /// <summary>
    /// Offsets the position of the camera from its local position and rotation.
    /// </summary>
    void SetPositionOffset()
>>>>>>> Stashed changes
     {
        // Move the camera away from the its parent along the Y and Z axis
        Vector3 offset = new Vector3(0, cameraHeight, -distanceFromPlayer);
        transform.DOLocalMove(offset, movementDelay).SetEase(movementType);

        // Rotate the camera along its X axis.
        Quaternion targetTilt = Quaternion.Euler(tiltAngle, 0, 0);
        transform.DOLocalRotate(targetTilt.eulerAngles, movementDelay).SetEase(movementType);
     }



     /// <summary>
     /// Have the camera's parent's position follow the character controller's position.
     /// </summary>
     public void FollowTransform(Transform objectToFollow)
     {
          transform.parent.position = objectToFollow.position;
     }



     /// <summary>
     /// Sets the rotation of the child camera around its parent.
     /// This will have it looking at the pinecone while always facing the mountain.
     /// </summary>
     void SetRotation()
     {
          // Find the direction from the character controller to the mountain.
          Vector3 targetDirection = (characterController.transform.position - mountainCenter).normalized;

          // Find the position to look at, but only rotating along the camera's parent's y axis.
          Vector3 targetPosition = new Vector3(targetDirection.x,
                                                  this.transform.parent.position.y,
                                                  targetDirection.z);

          // Apply the rotation to the camera's parent.
          transform.parent.LookAt(targetPosition);
     }
}
