using UnityEngine;
using Leap;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Net;
using System.Threading;
using System.Linq;

public class MyFinger : MonoBehaviour { 
	const int DIMENSION = 63;
	const int SPHERE_NUM = 21;
	const int CYLINER_NUM = 15;
	private Controller handController;
	private GameObject[] spheres = new GameObject[SPHERE_NUM];
	private GameObject[] cylinders = new GameObject[CYLINER_NUM];
	public GameObject spherePrefab;
	public GameObject cylinderPrefab;
	public static object FingerType { get; internal set; }


	//para
	private bool taskIsCollect = false;
	private string collectName = "lixue";
	private string collectType = "5";

	private string ip = "http://183.172.137.106:8000/";
	//collect
	private bool collect = false;
	private int collect_cnt = 0;
	//http and predict
    static HttpListener httpListener = null;
	static string[] sign = {"飞机","电话","你","坏","拳","七"};
	string predictSign = "";


	void Start () {
		handController = new Controller();
	}
	
	void Update () {
		
		if (httpListener == null) {
            httpListener = new HttpListener();

            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListener.Prefixes.Add(ip);
            httpListener.Start();

            new Thread(new ThreadStart(delegate {
                while (true) {
                    HttpListenerContext httpListenerContext = httpListener.GetContext();
                    string query = httpListenerContext.Request.Url.ToString();
                    string str = query.Split('/')[3];

                    httpListenerContext.Response.StatusCode = 200;
                    if (str.Length == 0) {
                        using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream)) {
							float[] inp_data = getFingerAbsoluteData ();
							if (inp_data == null) {
								predictSign = "";
								continue;
							}
							else {
								String sendData = string.Join(" ", new List<float>(inp_data).ConvertAll(i => i.ToString()).ToArray());
                            	writer.WriteLine(sendData);
							}
                        }
                    } else {
						predictSign = sign[int.Parse(str)];
						// Debug.LogError(predictSign);
                        using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream)) {
                            writer.WriteLine("I've got your predict model");
                        }
                    }
                    Thread.Sleep(10);
                }
            })).Start();
        }
		//collect
		if (!collect) {
			return;
		}
		float[] data = getFingerAbsoluteData ();
		if (data == null) {
			return;
		}
		try {
			string folder = "../data/" + collectName;
			FileStream file = new FileStream(folder + collectType + ".txt", FileMode.Append);
			StreamWriter sw = new StreamWriter(file);
			for (int i = 0; i < data.Length; i++) {
				sw.Write(data[i] + ",");
			}
			sw.Write(collectType + "\n");
			sw.Close();
			++ collect_cnt;
		}
		catch(IOException ex) {		
			Console.WriteLine(ex.Message);
			Console.ReadLine();
			return ;
		}
	}

	void OnGUI()  
	{  
		if (taskIsCollect) 
		{
			//开始按钮  
			if (GUI.Button (new Rect (0, 10, 100, 30), "start")) { 
					collect = true;
			}  
			//结束按钮  
			if (GUI.Button (new Rect (0, 50, 100, 30), "finish")) {  
					collect = false;
			} 
			//显示采集数据的数量
			if(GUI.Button(new Rect(0, 90, 100, 30), collect_cnt.ToString())) {

			}
		}
		else {
			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fontSize = 25;
			if (GUI.Button (new Rect (620, 10, 150, 45), predictSign, buttonStyle)) {  

			}  
		}
		
	}  

	
	private float[] getFingerData() {
		float[] raw = getFingerAbsoluteData();
		if (raw == null) {
			return null;
		}
		float[] data = new float[raw.Length - 3];
		for (int i = 0; i < data.Length; i++) {
			data[i] = (raw[i + 3] - raw[i % 3]) / 2.0f;
		}
		return data;
	}
	
	public float[] getFingerAbsoluteData() {
		float[] ret = null;
		
		foreach (Hand hand in handController.Frame().Hands) {
			if (hand.IsRight) {
				if (ret == null) {
					ret = new float[DIMENSION];
				}
				ret[0] = hand.PalmPosition.x;
				ret[1] = hand.PalmPosition.y;
				ret[2] = hand.PalmPosition.z;
				for (int i = 0; i < hand.Fingers.Count; i++) {
					Leap.Finger finger = hand.Fingers[i];
					Bone bone1 = finger.Bone(Bone.BoneType.TYPE_DISTAL);
					Bone bone2 = finger.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
					Bone bone3 = finger.Bone(Bone.BoneType.TYPE_PROXIMAL);
					Bone bone4 = finger.Bone(Bone.BoneType.TYPE_METACARPAL);
					ret[i * 12 + 3] = bone1.PrevJoint.x;
					ret[i * 12 + 4] = bone1.PrevJoint.y;
					ret[i * 12 + 5] = bone1.PrevJoint.z;
					ret[i * 12 + 6] = bone2.PrevJoint.x;
					ret[i * 12 + 7] = bone2.PrevJoint.y;
					ret[i * 12 + 8] = bone2.PrevJoint.z;
					ret[i * 12 + 9] = bone3.PrevJoint.x;
					ret[i * 12 + 10] = bone3.PrevJoint.y;
					ret[i * 12 + 11] = bone3.PrevJoint.z;
					ret[i * 12 + 12] = bone4.PrevJoint.x;
					ret[i * 12 + 13] = bone4.PrevJoint.y;
					ret[i * 12 + 14] = bone4.PrevJoint.z;
				}
			}
		}
		
		return ret;
	}
	
}
