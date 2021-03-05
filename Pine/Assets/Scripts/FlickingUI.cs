using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlickingUI : MonoBehaviour
{
     [SerializeField] Texture2D openHandTexture;
     [SerializeField] Texture2D closedHandTexture;
     [SerializeField] GameObject grabRingUIObject;
     [SerializeField] GameObject pullLineUIObject;
     [SerializeField] GameObject forwardLineUIObject;
     GameObject grabRing;
     GameObject pullLines;
     GameObject forwardLines;

     public float ringDepth = 0f;

     CharacterStateMachine _stateMachine;
     Camera _camera;
     Vector3 flickDirection;
     float flickMagnitude;



     private void Awake()
     {
          _stateMachine = GetComponent<CharacterStateMachine>();
          _camera = _stateMachine.GetCamera();

          // Create the objects used for UI
          pullLines = Instantiate(pullLineUIObject, this.transform);
          forwardLines = Instantiate(forwardLineUIObject, this.transform);

          Transform grabRingParent = GameObject.Find("Canvas").transform;
          grabRing = Instantiate(grabRingUIObject, grabRingParent);
          
     }

     private void Update()
     {
          // Get the flick direction and length from the state machine
          flickDirection = _stateMachine.GetCurrentFlickDirection();
          flickMagnitude = flickDirection.magnitude;

          // If the mouse button is pressed down, draw the flicking UI
          if (_stateMachine.AllowClick() == true)
          {
               // Set the mouse cursor
               Cursor.SetCursor(closedHandTexture, Vector2.zero, CursorMode.Auto);

               DrawUIGrabRing();
               DrawUILinePull(pullLines);
               DrawUILineDashed(forwardLines);
          }
          else // If the mouse button is NOT held down, then hide the flicking UI objects.
          {
               // Set the mouse cursor
               Cursor.SetCursor(openHandTexture, Vector2.zero, CursorMode.Auto);

               SetUILineVisibility(grabRing, false);
               SetUILineVisibility(pullLines, false);
               SetUILineVisibility(forwardLines, false);
          }
     }


     /// <summary>
     /// Sets the gameObject active status for the passed object.
     /// Used to show or hide the flicking UI objects.
     /// </summary>
     /// <param name="uiLine"></param>
     /// <param name="status"></param>
     void SetUILineVisibility(GameObject uiLine, bool status)
     {
          uiLine.SetActive(status);
     }


     /// <summary>
     /// 
     /// </summary>
     void DrawUIGrabRing()
     {
          Vector2 clickedPosition = _stateMachine.GetMouseClickPosition();

          // Move the ring to the screen space position the mouse clicked at
          RectTransform ringRectTransform = grabRing.GetComponent<RectTransform>();
          ringRectTransform.anchoredPosition = clickedPosition;

          // Show the ring
          SetUILineVisibility(grabRing, true);
     }


     /// <summary>
     /// Show and translates the passed object to match the direction and length of the flicking vector.
     /// </summary>
     /// <param name="lineObject"></param>
     void DrawUILinePull(GameObject lineObject)
     {
          // Show the line
          SetUILineVisibility(lineObject, true);

          // Get the translation vectors
          Vector3 additionalOffset = new Vector3(0f, 0.3f, 0f);
          Transform flickingObject = _stateMachine.GetFlickingObjectTransform();
               // This is the center of the flicking object regardless of its rotation.
          Vector3 from = flickingObject.position + (flickingObject.rotation * additionalOffset);
          Vector3 to = from + new Vector3(flickDirection.x, 0f, flickDirection.z);

          // Center the position of the line object
          lineObject.transform.position = from;

          // Rotate the object
          lineObject.transform.LookAt(to);

          // Scale the line object
          Vector3 lineLength = new Vector3(1f, 1f, (flickMagnitude*0.5f));
          lineObject.transform.localScale = lineLength;
     }


     /// <summary>
     /// 
     /// </summary>
     /// <param name="lineObject"></param>
     void DrawUILineDashed(GameObject lineObject)
     {
          // Show the line
          SetUILineVisibility(lineObject, true);

          // Get the translation vectors
          Vector3 additionalOffset = new Vector3(0f, 0.3f, 0f);
          Transform flickingObject = _stateMachine.GetFlickingObjectTransform();
               // This is the center of the flicking object regardless of its rotation.
          Vector3 from = flickingObject.position + (flickingObject.rotation * additionalOffset);
          Vector3 to = from - new Vector3(flickDirection.x, flickDirection.y, flickDirection.z);

          // Center the position of the line object
          lineObject.transform.position = from;

          // Rotate the object
          lineObject.transform.LookAt(to);

          // Lengthen the line object with spacing to simulate a dashed line
          int lineLength = (int)Mathf.Floor(flickMagnitude * 2f);

          // Make sure there are enough line segment children to iterate through.
          if (lineLength > lineObject.transform.childCount)
          {
               int diff = lineLength - lineObject.transform.childCount;

               for (int k = lineObject.transform.childCount - 1; k < lineLength; k++)
               {
                    // Set up the position for the new line
                    Transform lineTransform = lineObject.transform;
                    float segmentSeparation = lineTransform.GetChild(0).localPosition.z * 2f;
                    float segmentPosition = lineTransform.GetChild(k).localPosition.z + segmentSeparation;
                    Vector3 targetPosition = new Vector3(0, 0, segmentPosition);

                    // Create the new line
                    Transform newLineSegment = Instantiate(lineTransform.GetChild(0), lineObject.transform);

                    // Reposition and rename the new line
                    newLineSegment.localPosition = targetPosition;
                    newLineSegment.name = "LineSegment_" + lineObject.transform.childCount;
               }
          }
          
          // Show or hide the line segment based on it's child count position compared to line length.
          for (int i = 0; i < lineObject.transform.childCount; i++)
          {
               GameObject lineChild = lineObject.transform.GetChild(i).gameObject;

               // If the child position is an ODD number, show it. If EVEN, hide it.
               if ((i % 2) == 0)
                    lineChild.SetActive(true);
               else
                    lineChild.SetActive(false);

               // If the child position is GREATER than the lineLength, hide it
               if (i > lineLength)
                    lineChild.SetActive(false);
          }
     }

}
