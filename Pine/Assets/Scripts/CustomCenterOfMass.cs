using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To change the center of mass, this script requires a rigidbody for the gameobject this is attached to.
[RequireComponent(typeof(Rigidbody))]
public class CustomCenterOfMass : MonoBehaviour
{
     [SerializeField, Range(-2f,2f)] float centerOfMassHeight = 0f;
     [SerializeField, Range(0f, 0.5f)] float drawSize = 0.2f;

     Rigidbody _rb = null;
     Vector3 drawNewCenterOfMass = new Vector3(0, 0, 0);



     private void Awake()
     {
          _rb = GetComponent<Rigidbody>();
          SetCenterOfMass();
     }


     private void Update()
     {
          SetCenterOfMass();
     }



     /// <summary>
     /// Sets the center of mass for this game object.
     /// </summary>
     private void SetCenterOfMass()
     {
          // Create the new center of mass using a position ALREADY RELATIVE to this rigidbody.
          Vector3 newCenter = _rb.transform.rotation * new Vector3(0, centerOfMassHeight, 0);

          // Allow it to be drawn via vector for Gizmos
          drawNewCenterOfMass = newCenter;

          // And apply it to this rigidbody.
          _rb.centerOfMass = newCenter;
     }


     private void OnDrawGizmos()
     {
          // Draw the center of mass
          Gizmos.color = Color.blue;
          Gizmos.DrawSphere(drawNewCenterOfMass + this.transform.position, drawSize);
     }
}
