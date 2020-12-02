using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFor2P : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public float MaxDistance = 20.0f;
    public float MinDistance = 3.0f;
    public float MaxZoomOut = 5.0f;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center = Vector3.Lerp(player1.transform.position, player2.transform.position, 0.5f);       

        float length = (player1.transform.position - player2.transform.position).sqrMagnitude;
        float powerMaxDist = MaxDistance * MaxDistance;
        float powerMinDist = MinDistance * MinDistance;

        length = Mathf.Clamp(length, powerMinDist, powerMaxDist);
        
        Vector3 LastPosition = startPosition + transform.forward * -1.0f * MaxZoomOut;        
        transform.position = Vector3.Lerp(startPosition, LastPosition, ( (length - powerMinDist) / (powerMaxDist - powerMinDist) )); 
        transform.rotation = Quaternion.LookRotation(center - transform.position);
    }
}
