using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class SceneManager : MonoBehaviour
{
    WebSocket ws;
    public GameObject prefab;
    GameObject theTestRobot;
    GameObject mainCamera;
    Vector3 CalculatedPosition;
    Vector3 CalculatedRotation;
    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://127.0.0.1:8080/tag");
        ws.OnMessage += Ws_OnMessage;
        ws.Connect();
        mainCamera = Camera.main.gameObject;
        theTestRobot = Instantiate(prefab);
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
       
        var axis = JsonConvert.DeserializeObject<float[,]>(e.Data);
        
        CalculatedPosition = new Vector3(axis[2, 3], -axis[2, 5], -axis[2, 4]);
        CalculatedRotation = new Vector3(-axis[2, 0], axis[2, 2], axis[2, 1]);
    }

    // Update is called once per frame
    void Update()
    {
        theTestRobot.transform.position = CalculatedPosition;
        theTestRobot.transform.rotation = Quaternion.AngleAxis(CalculatedRotation.magnitude*57.3248f, CalculatedRotation);
    }
    private void OnDestroy()
    {
        if (ws != null) {
            ws.Close();
        }
    }
}
