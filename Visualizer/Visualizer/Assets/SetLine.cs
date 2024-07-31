using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLine : MonoBehaviour
{
    public GameObject from;
    public GameObject to;
    public float displayTime;

    public Color colorBase;
    public Color colorHighlight;
    public float alpha = 1f;
    public float highlightPos;

    public void SetTarget(GameObject _from, GameObject _to)
    {
        from = _from;
        to = _to;
    }

    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        StartCoroutine(DestroyAfter());
    }

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(displayTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPositions(new Vector3[] { from.transform.position + new Vector3(0f,0.5f), to.transform.position + new Vector3(0f, 0.5f) });
        var gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(colorBase,0),
            new GradientColorKey(colorHighlight,highlightPos),
            new GradientColorKey(colorBase,1),
        };
        gradient.alphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(alpha,0),
            new GradientAlphaKey(alpha,1) 
        };
        gradient.mode = GradientMode.Fixed;
        lineRenderer.colorGradient = gradient;
    }
}
