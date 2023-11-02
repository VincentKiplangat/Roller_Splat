using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiece : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isColored = false;

    public void ChangeColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
        isColored = true;
        
        GameManager.singleton.CheckComplete();
    }
}
