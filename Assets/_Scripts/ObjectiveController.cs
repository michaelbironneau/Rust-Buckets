using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour
{
    [SerializeField] Color completedColor;

    private Image _img;

    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<Image>();
    }

    public void Complete()
    {
        StartCoroutine(doComplete());
    }

    IEnumerator doComplete()
    {
        // TODO: Add some nicer animation
        _img.color = completedColor;
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    
}
