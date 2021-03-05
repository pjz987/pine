using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineconeVFX : MonoBehaviour
{

     Rigidbody _rb;
     float force = 0f;

     [HideInInspector] public bool playLargeVFX = false;
     [HideInInspector] public bool playSmallVFX = false;
     [SerializeField, Range(1f, 20f)] float largeForceCheck = 10f;
     [SerializeField] Vector2 smallForceCheck = new Vector2(0, 0);


     private void Awake()
     {
          _rb = GetComponent<Rigidbody>();
     }

     private void OnCollisionEnter(Collision collision)
     {
          force = collision.relativeVelocity.magnitude;

          if (force > largeForceCheck)   
               playLargeVFX = true;

          if (force >= smallForceCheck.x && force <= smallForceCheck.y)
               playSmallVFX = true;
     }

     private void OnCollisionStay(Collision collision)
     {
          playLargeVFX = false;
          playSmallVFX = false;
     }
}
