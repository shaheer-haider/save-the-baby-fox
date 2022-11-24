using UnityEngine;
using System.Collections;

public class GroundController : MonoBehaviour {

    public void OnAnimationEnd()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
