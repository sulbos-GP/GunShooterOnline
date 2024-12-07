using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testEffect : MonoBehaviour
{
    public Image hitImage;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        hitImage.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 2.0f)
        {
            StartCoroutine(HitEffect());
            timer = 0f;
        }
    }

    IEnumerator HitEffect()
    {
        hitImage.color = new Color(1f, 0f, 0f, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        hitImage.color = Color.clear;
    }

}
