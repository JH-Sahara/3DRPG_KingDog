using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPortal : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScene
    }
    [Header("Transition Info")]
    public string sceneName; //传送的场景名称
    public TransitionType transitionType;
    public TransitionDesitiation.DesitiationTag desitiationTag;

    private bool canTrans;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            //传送
            SceneController.Instance.TransitionToDesitiation(this);
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player"))
            canTrans = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}
