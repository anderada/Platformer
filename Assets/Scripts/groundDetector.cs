using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundDetector : MonoBehaviour
{
    public bool activated = true;

    void OnTriggerEnter2D(){
        activated = true;
    }

    void OnTriggerExit2D(){
        activated = false;
    }
}
