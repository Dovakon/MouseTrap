using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MoveMouse : MonoBehaviour {

    public GameObject Footprints;

    int minAxis = 0;
    int maxAxis = 3;
    

    int minBound = 0;
    int maxBound = 3;

    
    void Awake () {

        this.gameObject.transform.position = new Vector2(0, 0);
        
    }
	
	


    public void Move(int Xaxis, int Yaxis)
    {
        this.gameObject.transform.position = new Vector2(Xaxis, Yaxis) ;

        
        float newPosX = Mathf.Clamp(transform.position.x, minBound, maxBound);
        float newPosY = Mathf.Clamp(transform.position.y, minBound, maxBound);
        this.gameObject.transform.position = new Vector2(newPosX, newPosY);

        
    }

    public void InstantiateFootprints(int Xaxis, int Yaxis, int Zrotation)
    {
        Instantiate(Footprints, new Vector3(Xaxis, Yaxis, 0), Quaternion.Euler(new Vector3(0, 0, Zrotation)));
    }


    public Vector3 GetPossision()
    {
        return this.gameObject.transform.position;
    }

    public void GoToStart()
    {
        this.gameObject.transform.position = new Vector2(0, 0);
    }
}
