using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieTest : MonoBehaviour {

    [SerializeField]
    private SkinnedMeshRenderer renderer;


    public bool dead;
    private float die = 0;
    private float dDie;

    public void Update()
    {
        if (dead)
        {
            die = Mathf.SmoothDamp(die, 1.0f, ref dDie, 1.5f);
        }
        else
        {
            die = 0;
        }

        renderer.material.SetFloat("_Die", die);
    }
}
