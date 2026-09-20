using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public bool Start = false;
    public AnimationCurve cur;
    public float duration = 1f;
    

    // Update is called once per frame
    void Update()
    {
        if(Start)
        {
            Start = false;
            StartCoroutine(Shaking());
        }
    }
    IEnumerator Shaking()
    {
       Vector3 startpos = transform.position;
       float elapsedTime = 0f;
       while (elapsedTime < duration) 
       {
        elapsedTime += Time.deltaTime;
        float strength = cur.Evaluate(elapsedTime/duration);
        transform.position = startpos + Random.insideUnitSphere;
        yield return null;
       }
       transform.position = startpos;
    }
}
