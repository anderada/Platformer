using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundDetector : MonoBehaviour
{
    public bool grounded = true;

    void OnTriggerEnter2D(){
        grounded = true;
    }

    void OnTriggerExit2D(){
        grounded = false;
    }
}
