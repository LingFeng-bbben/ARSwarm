using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Linq;
public class SceneManager : MonoBehaviour
{
    WebSocket ws;
    WebSocket ws2;
    public GameObject prefab;
    public GameObject theBall;

    List<GameObject> Robots = new List<GameObject>(30);
    
    Vector3[] CalculatedPositions = new Vector3[30];
    Vector3[] CalculatedRotations = new Vector3[30];
    List<DeviceInfo> DeviceInfos = new List<DeviceInfo>();
    List<DeviceSensor> Sensors = new List<DeviceSensor>();
    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://127.0.0.1:8080/tag");
        ws.OnMessage += Ws_OnMessage;
        ws.Connect();
        ws2 = new WebSocket("ws://127.0.0.1/vs");
        ws2.OnMessage += Ws2_OnMessage;
        ws2.Connect();

        for (int i = 0; i < 30; i++)
        {
            var robot = Instantiate(prefab);
            robot.SetActive(false);
            Robots.Add(robot);
        }
    }

    private void Ws2_OnMessage(object sender, MessageEventArgs e)
    {
        DeviceInfos = JsonConvert.DeserializeObject<List<DeviceInfo>>(e.Data);
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
       
        var axis = JsonConvert.DeserializeObject<float[,]>(e.Data);
        for (int i = 0; i < 30; i++)
        {
            CalculatedPositions[i] = new Vector3(axis[i, 3], 0, -axis[i, 4]);
            CalculatedRotations[i] = new Vector3(-axis[i, 0], axis[i, 2], axis[i, 1]);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < 30; i++)
        {
            //skip
            if (CalculatedPositions[i] == null) continue;
            if (CalculatedPositions[i].magnitude == 0) continue;

            var rbtobj = Robots[i];
            //smooth position
            if (CalculatedPositions[i].magnitude != 0 && !rbtobj.activeSelf)
            {
                rbtobj.SetActive(true);
                rbtobj.transform.position = CalculatedPositions[i];
            }
            var posdelt = CalculatedPositions[i] - rbtobj.transform.position;
            rbtobj.transform.position += 1f * posdelt;
            //smooth rotation
            var rawrotation = Quaternion.AngleAxis(CalculatedRotations[i].magnitude * 57.3248f, CalculatedRotations[i]).eulerAngles.y;
            var rotdelt = Mathf.DeltaAngle(rbtobj.transform.rotation.eulerAngles.y, rawrotation);
            var robotroty = rbtobj.transform.rotation.eulerAngles.y;
            rbtobj.transform.rotation = Quaternion.Euler(0, robotroty + (1f * rotdelt), 0);
            //display message
            var dvinfo = DeviceInfos.Find(o => o.givenTag == i);
            if (dvinfo != null)
            {
                var text = string.Format("ID({2}) {0},{1}", dvinfo.rBump[0], dvinfo.rBump[1],i);
                rbtobj.transform.Find("Canvas").Find("TextMessage").GetComponent<Text>().text = text;
            }

            //virtual sensor
            var pointer = rbtobj.transform.position - theBall.transform.position;
            var distance = (int)(pointer.magnitude*100);
            var angle = (int)Vector3.SignedAngle(rbtobj.transform.forward,pointer,rbtobj.transform.up);
            var message = distance + "," + angle;
            if (Sensors.Any(o => o.givenTag == i)) { 
                Sensors.Find(o => o.givenTag == i).message = message;
            }
            else
            {
                Sensors.Add(new DeviceSensor(i, message));
            }

        }
        var sensstr = JsonConvert.SerializeObject(Sensors);
        ws2.Send(sensstr);
    }
    private void OnDestroy()
    {
        if (ws != null) {
            ws.Close();
        }
    }
}
