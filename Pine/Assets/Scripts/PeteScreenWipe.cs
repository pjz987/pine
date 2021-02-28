using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PeteScreenWipe : MonoBehaviour
{   
    private RectTransform image;
    [SerializeField]
    private Ease movementType;
    [Range(0f, 5f)]
    public float transitionTime;
    bool effectActive = false;
    void Start()
    {
        image = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && !effectActive)
        {
            StartCoroutine(TransitionTimer());
        }
    }

    private void RaiseImage ()
    {
        image.DOAnchorPos(new Vector2(0f, 0f), transitionTime).SetEase(movementType);
    }

    private void LowerImage()
    {
        image.DOAnchorPos(new Vector2(0f, -900f), transitionTime).SetEase(movementType);
    }

    IEnumerator TransitionTimer ()
    {   
        effectActive = true;
        RaiseImage();
        yield return new WaitForSeconds(5f);
        effectActive = false;
        LowerImage();
    }
}
