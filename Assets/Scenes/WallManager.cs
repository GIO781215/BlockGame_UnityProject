using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public GameObject wall1;
    public GameObject wall2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.transform.position.y > wall1.transform.position.y )
        {
            GameObject w1 = Instantiate(wall1, wall1.transform.position + Vector3.up * 100, wall1.transform.rotation);
            GameObject w2 = Instantiate(wall2, wall2.transform.position + Vector3.up * 100, wall2.transform.rotation);

            wall1 = w1;
            wall2 = w2;
        }
    }
}
