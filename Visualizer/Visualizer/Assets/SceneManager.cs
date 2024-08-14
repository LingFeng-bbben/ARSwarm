using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor.VersionControl;
using System.Drawing;
using System.IO;
using System;
using System.Text;
public class SceneManager : MonoBehaviour
{
    WebSocket ws;
    WebSocket ws2;
    public GameObject prefab;
    public GameObject linePrefab;
    public GameObject theBall;

    List<GameObject> Robots = new List<GameObject>(30);
    
    Vector3[] CalculatedPositions = new Vector3[30];
    Vector3[] CalculatedRotations = new Vector3[30];
    List<DeviceInfo> DeviceInfos = new List<DeviceInfo>();
    List<DeviceSensor> VirtualSensors = new List<DeviceSensor>();
    List<DeviceSensor> PhysicalSensors = new List<DeviceSensor>();

    FileStream fileStream;

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
        var path = Application.temporaryCachePath +"/"+ Guid.NewGuid().ToString() + ".csv";
        fileStream = File.Create(path);
        print(path);
    }

    //From Robot Message
    private void Ws2_OnMessage(object sender, MessageEventArgs e)
    {
        var physicalSensorMessage = JsonConvert.DeserializeObject<DeviceSensor>(e.Data);
        if (PhysicalSensors.Any(o => o.givenTag == physicalSensorMessage.givenTag))
        {
            PhysicalSensors.Find(o => o.givenTag == physicalSensorMessage.givenTag).message = physicalSensorMessage.message;
        }
        else
        {
            PhysicalSensors.Add(physicalSensorMessage);
        }
        var msg = physicalSensorMessage.message;
        if (msg.StartsWith("IRRCV"))
        {
            var to = physicalSensorMessage.givenTag;
            var from = int.Parse(msg.Split(' ')[2]);
            print("IR from " + from + " to " + to);
            DwkUnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var line = Instantiate(linePrefab);
                line.GetComponent<SetLine>().SetTarget(Robots[from], Robots[to]);
            });
            
        }
        if (msg.StartsWith("SeekBall"))
        {
            DwkUnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Robots[physicalSensorMessage.givenTag].GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Color.yellow);
            });
        }
        if (msg.StartsWith("Bump!Edge!"))
        {
            DwkUnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Robots[physicalSensorMessage.givenTag].GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Color.red);
            });
        }
        if (msg.StartsWith("Walk.."))
        {
            DwkUnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Robots[physicalSensorMessage.givenTag].GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Color.green);
            });
        }
    }

    void wsCallMainThread()
    {
        
    }

    //From Camera Message
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
            rbtobj.transform.position += 0.3f * posdelt;
            //smooth rotation
            var rawrotation = Quaternion.AngleAxis(CalculatedRotations[i].magnitude * 57.3248f, CalculatedRotations[i]).eulerAngles.y;
            var rotdelt = Mathf.DeltaAngle(rbtobj.transform.rotation.eulerAngles.y, rawrotation);
            var robotroty = rbtobj.transform.rotation.eulerAngles.y;
            rbtobj.transform.rotation = Quaternion.Euler(0, robotroty + (0.6f * rotdelt), 0);
            //display message
            var pysens = PhysicalSensors.Find(o => o.givenTag == i);
            var sensormessage = "";
            if (pysens != null)
            {
                sensormessage = pysens.message;
                var text = string.Format("ID({0}) {1}", pysens.givenTag,pysens.message);
                rbtobj.transform.Find("Canvas").Find("TextMessage").GetComponent<Text>().text = text;
            }

            //virtual sensor
            var pointer = rbtobj.transform.position - theBall.transform.position;
            var distance = (int)(pointer.magnitude*100);
            var angle = (int)Vector3.SignedAngle(rbtobj.transform.forward,pointer,rbtobj.transform.up);
            var message = distance + "," + angle;
            if (VirtualSensors.Any(o => o.givenTag == i)) { 
                VirtualSensors.Find(o => o.givenTag == i).message = message;
            }
            else
            {
                VirtualSensors.Add(new DeviceSensor(i, message));
            }

            //logging
            var log = string.Format("{0},{1},{2},{3},{4}\n",
                i,
                Time.fixedUnscaledTime,
                rbtobj.transform.position.x,
                rbtobj.transform.position.z,
                rbtobj.transform.rotation.eulerAngles.y);
            var buf = Encoding.UTF8.GetBytes(log);
            fileStream.Write(buf, 0, buf.Length);

        }
        var sensstr = JsonConvert.SerializeObject(VirtualSensors);
        ws2.Send(sensstr);
    }
    private void OnDestroy()
    {
        if (fileStream!=null)
        {
            fileStream.Close();
        }
        if (ws != null) {
            ws.Close();
        }
        if (ws2 != null)
        {
            ws2.Close();
        }
    }
}
