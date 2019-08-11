using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading;
using System.Text;
//using System.Xml;
//using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

public class Server : MonoBehaviour
{
    public DataManager manager;
    //List<int> participants;

    HttpListener _httpListener = new HttpListener();
    // Start is called before the first frame update
    void Awake()
    {
        //participants = new List<int>();
        //participants.Add(manager.myid);

        Debug.Log("Starting server...");
        _httpListener.Prefixes.Add("http://localhost:5000/"); // add prefix "http://localhost:5000/"
        _httpListener.Start(); // start server (Run application as Administrator!)
        Debug.Log("Server started.");
        Thread _responseThread = new Thread(ResponseThread);
        _responseThread.Start(); // start the response thread
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ResponseThread()
    {
        while (true)
        {
            HttpListenerContext context = _httpListener.GetContext(); // get a context
                                                                      /*                                                         // Now, you'll find the request URL in context.Request.Url
                                                                     byte[] _responseArray = Encoding.UTF8.GetBytes("<html><head><title>Localhost server -- port 5000</title></head>" +
                                                                     "<body>Welcome to the <strong>Localhost server</strong> -- <em>port 5000!</em></body></html>"); // get the bytes to response
                                                                     //XmlSerializer ser = new XmlSerializer(typeof(List<Unit>));
                                                                     */
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(context.Request.InputStream);
            //BattleData data = (BattleData)binaryFormatter.Deserialize(context.Request.InputStream);
            string resposeFromClient = reader.ReadToEnd();
            Debug.Log(resposeFromClient);
            BattleData data = JsonUtility.FromJson<BattleData>(resposeFromClient);
            if (data == null)
            {
                Debug.Log("null");
            }
            else
            {
                List<Unit> units = data.units;
                for (int i = 0; i < units.Count; i++)
                {
                    Debug.Log(String.Format("x:{0},z:{1},uuid:{2},mine:{3}", units[i].x, units[i].z, units[i].uuid, units[i].owner==manager.myid));
                }
                ApplyParticipants(data.participants);
                manager.UpdateData(units);
                Debug.Log(data.participants.Count);
                Debug.Log(manager.participants.Count);
            }

            //binaryFormatter=new BinaryFormatter();

            data = new BattleData();
            data.units = manager.units;
            data.participants = manager.participants;
            data.started = manager.started;
            context.Response.ContentType = "text/json";
            context.Response.KeepAlive = false; // set the KeepAlive bool to false
            byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            //context.Response.ContentLength64 = _responseArray.LongLength;
            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
            context.Response.Close(); // close the connection
            Debug.Log("Respone given to a request.");
        }
    }
    void ApplyParticipants(List<int> uuids)
    {
        for(int i = 0; i < uuids.Count; i++)
        {
            if(!manager.participants.Contains(uuids[i]))
            {
                manager.participants.Add(uuids[i]);
            }
        }
    }
    public byte[] ObjectToByteArray(System.Object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new System.IO.MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}

[System.Serializable]
public class BattleData
{
    public List<Unit> units;
    public List<int> participants;
    public bool started = false;
}