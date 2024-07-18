using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTest : MonoBehaviour
{
    Rigidbody2D rigidbody;
    Vector2 dir = new Vector2(1, 0);
    Vector2 target = new Vector2(8, 0);
    float t;
    private void Start()
    {
        //t = Time.time;
        //Debug.Log(t);
        //StartCoroutine(co());
        StartCoroutine(co2());
        //rigidbody = GetComponent<Rigidbody2D>();
        //rigidbody.velocity = dir * 20;
        
    }




    private void Update()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, dir * 100, Time.deltaTime * 20);
    }

    IEnumerator co()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("0.2 : "+ (Time.time - t));
        //Debug.Log("0.2"+transform.position);
        yield return new WaitForSeconds(0.8f);
        Debug.Log("1_ : " + (Time.time - t));
        //Debug.Log("1_" + transform.position);

    }
    IEnumerator co2()
    {
        Vector3 _least = new Vector3();
        _least = transform.position;

        yield return new WaitForSeconds(1f);
        Debug.Log("1 : " + (Time.time - t));
        Debug.Log("1 : " + (transform.position - _least));

        _least = transform.position;

        yield return new WaitForSeconds(1f);
        Debug.Log("2 : " + (Time.time - t));
        Debug.Log("2 : " + (transform.position - _least));

        _least = transform.position;

        yield return new WaitForSeconds(1f);
        Debug.Log("3 : " + (Time.time - t));
        Debug.Log("3 : " + (transform.position - _least));

        _least = transform.position;

        yield return new WaitForSeconds(1f);
        Debug.Log("4 : " + (Time.time - t));
        Debug.Log("4 : " + (transform.position - _least));






        //Debug.Log("1" + transform.position);
    }
  

}
