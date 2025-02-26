using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCircle : MonoBehaviour
{
    [SerializeField]private float lifetime=0.2f;

    private float increaseRate;

    private bool setUp;
    private LineRenderer lineRenderer;
    private float radius;

    public void Setup(float maxRadius)
    {
        this.increaseRate = maxRadius / lifetime;
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
        const float angleInc = 2 * 3.14159f / 38;
        Vector3[] vecArr = new Vector3[40];
        for (int i = 0; i < 40; ++i)
        {
            vecArr[i] = new Vector3(Mathf.Cos(i * angleInc), Mathf.Sin(i * angleInc)) * radius;
        }
        lineRenderer.SetPositions(vecArr);
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 40;

        setUp = false;

    }


    // Update is called once per frame
    void Update()
    {
        if(setUp)
        {
            GenerateCircle();
            radius += Time.deltaTime * increaseRate;
        }
    }
}
