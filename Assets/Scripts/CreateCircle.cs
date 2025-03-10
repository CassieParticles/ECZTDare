using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCircle : MonoBehaviour
{
    [SerializeField]private float innerSpeed=1f;
    [SerializeField] private float outerSpeed = 15f;

    private bool setUp;
    private float lifetime;

    private LineRenderer lineRendererInner;
    private float innerRadius;

    private LineRenderer lineRendererOuter;
    private float outerRadius;


    public void Setup(float maxRadius)
    {
        lifetime = maxRadius / outerSpeed;
        setUp = true;
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void GenerateCircle()
    {
        const float angleInc = 2 * 3.14159f / 79;
        Vector3[] vecArrIn = new Vector3[80];
        Vector3[] vecArrOut = new Vector3[80];
        for (int i = 0; i < 80; ++i)
        {
            vecArrIn[i] = new Vector3(Mathf.Cos(i * angleInc), Mathf.Sin(i * angleInc)) * innerRadius;
            vecArrOut[i] = new Vector3(Mathf.Cos(i * angleInc), Mathf.Sin(i * angleInc)) * outerRadius;
        }
        lineRendererInner.SetPositions(vecArrIn);
        lineRendererOuter.SetPositions(vecArrOut);
    }

    private void Awake()
    {
        lineRendererInner = transform.GetChild(0).GetComponent<LineRenderer>();
        lineRendererInner.positionCount = 80;
        lineRendererOuter = transform.GetChild(1).GetComponent<LineRenderer>();
        lineRendererOuter.positionCount = 80;

        setUp = false;

    }


    // Update is called once per frame
    void Update()
    {
        if(setUp)
        {
            GenerateCircle();
            innerRadius += Time.deltaTime * innerSpeed;
            outerRadius += Time.deltaTime * outerSpeed;
        }
    }
}
