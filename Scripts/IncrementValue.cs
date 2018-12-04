using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementValue : MonoBehaviour
{
    public float counter = 0f;
    public float target = 99f;

    private void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update ()
    {
        Increment();
	}

    void Increment()
    {
        if (counter <= Time.time) {
            Reset();
        }
    }

    private void Reset()
    {
        counter = Time.time + target;
    }
}
