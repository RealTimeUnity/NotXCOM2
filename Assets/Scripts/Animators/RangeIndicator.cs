using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour {

    public float speed;

    protected int size = 0;

    public void Initialize(int size)
    {
        if (!this.isActiveAndEnabled)
        {
            this.gameObject.SetActive(true);
            this.transform.localScale = new Vector3(1, 1, 1);
            this.size = size;
        }
    }

    public void Update()
    {
        if (this.isActiveAndEnabled && this.transform.localScale.x < this.size)
        {
            this.transform.localScale += new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime);
        }
    }
}
