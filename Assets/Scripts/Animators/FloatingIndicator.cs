using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloatingIndicator : MonoBehaviour
{
    public bool randomRotation;
    public bool randomFloating;
    public float floatSpeed = 1;
    public float floatDistance = 1;

    protected float rotationX = 0;
    protected float rotationY = 0;
    protected float rotationZ = 0;

    protected float originalY;
    protected int direction = 1;
    protected Vector3 xAxis = new Vector3(1, 0, 0);
    protected Vector3 yAxis = new Vector3(0, 1, 0);
    protected Vector3 zAxis = new Vector3(0, 0, 1);

    void Start()
    {
        if (this.randomRotation)
        {
            this.rotationX = UnityEngine.Random.Range(-1.5f, 1.5f);
            this.rotationX = UnityEngine.Random.Range(-1.5f, 1.5f);
            this.rotationX = UnityEngine.Random.Range(-1.5f, 1.5f);
        }
        if (this.randomFloating)
        {
            this.floatSpeed = UnityEngine.Random.Range(1.0f, 3.0f);
            this.floatDistance = UnityEngine.Random.Range(0.5f, 1.25f);
        }
        this.originalY = this.gameObject.transform.position.y;
    }

    void Update()
    {
        this.gameObject.transform.Rotate(this.xAxis, this.rotationX);
        this.gameObject.transform.Rotate(this.yAxis, this.rotationY);
        this.gameObject.transform.Rotate(this.zAxis, this.rotationZ);

        this.transform.position = new Vector3(
            transform.position.x,
            originalY + ((float)Math.Sin(Time.time * floatSpeed) * floatDistance),
            transform.position.z);
    }
}
