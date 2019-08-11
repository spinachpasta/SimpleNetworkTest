using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Script.Serialization;

public class Client : MonoBehaviour
{
    public DataManager manager;
    //public List<int> participants;
    public bool isHost=false;
    public Server server;//get instance
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        //participants.Add(manager.myid);
        if (isHost)
        {
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    public byte[] ObjectToByteArray(System.Object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }*/
    //for testing
    void GetData()
    {
        if (isHost)
        {
            return;
        }
        WebRequest request = WebRequest.Create("http://localhost:5000");
        // If required by the server, set the credentials.  
        request.Credentials = CredentialCache.DefaultCredentials;

        request.ContentType = "text/json";

        BattleData bdata = new BattleData();
        bdata.participants = manager.participants;
        bdata.units = manager.units;
        request.Method = "POST";
        byte[] binary = Encoding.UTF8.GetBytes(JsonUtility.ToJson(bdata));
        using (Stream postStream = request.GetRequestStream())
        {
            // Send the data.
            postStream.Write(binary, 0, binary.Length);
            postStream.Close();
        }


        // Get the response.  
        WebResponse response = request.GetResponse();
        // Display the status.  
        Debug.Log(((HttpWebResponse)response).StatusDescription);
        

        // Get the stream containing content returned by the server. 
        // The using block ensures the stream is automatically closed. 
        using (Stream dataStream = response.GetResponseStream())
        {
            /*
            //BinaryFormatter binaryFormatter=new BinaryFormatter();
            // Open the stream using a StreamReader for easy access.  
            //StreamReader reader = new StreamReader(dataStream);
            Debug.Log(dataStream.Length);
            dataStream.re
            //MemoryStream ms = new MemoryStream();
            //dataStream.CopyTo(ms);
            //BattleData data = (BattleData)binaryFormatter.Deserialize(ms);//specified cast is invalid
            // Read the content.  
            //string responseFromServer = reader.ReadToEnd();
            // Display the content.  
            //Debug.Log(responseFromServer);*/

            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Debug.Log(responseFromServer);
            BattleData data = JsonUtility.FromJson<BattleData>(responseFromServer);
            if (data == null)
            {
                Debug.Log("null");
            }
            else
            {
                List<Unit> units = data.units;
                for (int i = 0; i < units.Count; i++)
                {
                    Debug.Log(String.Format("x:{0},y:{1},uuid:{2}", units[i].x, units[i].z,units[i].uuid));
                }
                manager.participants = data.participants;
                for(int i = 0; i < manager.participants.Count; i++)
                {
                    if (manager.participants[i] == manager.myid)
                    {
                        manager.order = i;
                        break;
                    }
                }
                manager.started = data.started;
                manager.UpdateData(units);
            }
        }

        // Close the response.  
        response.Close();
    }
    private void OnGUI()
    {
        if(GUILayout.Button("send request"))
        {
            //SendFirstRequest();
            GetData();
        }
    }

    void SendRequest()
    {

    }
    /*
    void ResponseCallack(IAsyncResult asynchronousResult)
    {
        try
        {
            // Set the State of request to asynchronous.
            RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
            WebRequest myWebRequest1 = myRequestState.request;
            // End the Asynchronous response.
            myRequestState.response = myWebRequest1.EndGetResponse(asynchronousResult);
            // Read the response into a 'Stream' object.
            Stream responseStream = myRequestState.response.GetResponseStream();
            myRequestState.responseStream = responseStream;
            // Begin the reading of the contents of the HTML page and print it to the console.
            IAsyncResult asynchronousResultRead = responseStream.BeginRead(myRequestState.bufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);

        }
    }*/
}
