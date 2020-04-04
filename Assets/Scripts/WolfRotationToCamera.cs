using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfRotationToCamera : MonoBehaviour
{
    private void OnEnable()
    {
        /*Vector3 relativePos = Camera.main.transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePos, new Vector3(0,1,0));
        transform.rotation = rotation;*/
        //the most interesting variant now


        Vector3 targetPosition = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        transform.LookAt(targetPosition);
    }
}
