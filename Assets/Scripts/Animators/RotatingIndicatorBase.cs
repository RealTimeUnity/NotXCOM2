using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class RotatingIndicatorBase : MonoBehaviour
{
    public int rotateSpeed = 1;

    public void Update()
    {
        this.gameObject.transform.Rotate(new Vector3(0, 0, 1), this.rotateSpeed * Time.deltaTime);
    }
}
