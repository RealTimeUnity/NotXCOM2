using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSeperation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Animator>().enabled = false;
        Seperation(transform);

        Collider[] cols = Physics.OverlapSphere(transform.position, 5);
        foreach(Collider col in cols)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(1000, transform.position, 5, 3);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Seperation(Transform t)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < t.childCount; i++)
        {
            children.Add(t.GetChild(i));
        }

        foreach(Transform child in children)
        {
            child.SetParent(null);
            child.gameObject.AddComponent<Rigidbody>();
            Seperation(child);
        }
    }
}
