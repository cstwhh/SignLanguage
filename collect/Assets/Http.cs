using UnityEngine;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;

public class Predict : MonoBehaviour {
    static HttpListener httpListener = null;
    static float[] oup;
    static bool updated = false;
    static string sendData = "haha";

    // public int getFingerRock(int[] inp) {
    //     int inpLength = MainCamera.R * MainCamera.C * 2;
    //     if (inp == null || inp.Length != inpLength) {
    //         inp = new int[inpLength];
    //     }
    //     sendData = string.Join(" ", new List<int>(inp).ConvertAll(i => i.ToString()).ToArray());
    //     updated = false;
    //     for (int i = 0; i < 30; i++) {
    //         Thread.Sleep(1);
    //         if (updated) {
    //             break;
    //         }
    //     }
    //     if (oup != null && oup.Length > 0) {
    //         Debug.Log((int)oup[0]);
    //         return (int)oup[0];
    //     }
    //     return 0;
    // }

    // public float[] getFinderData(int[] inp) {
    //     int inpLength = MainCamera.R * MainCamera.C * 2;
    //     if (inp == null || inp.Length != inpLength) {
    //         inp = new int[inpLength];
    //     }
    //     sendData = string.Join(" ", new List<int>(inp).ConvertAll(i => i.ToString()).ToArray());
    //     updated = false;
    //     for (int i = 0; i < 30; i++) {
    //         Thread.Sleep(1);
    //         if (updated) {
    //             break;
    //         }
    //     }
    //     return oup;
    // }

    void Update() {
        if (httpListener == null) {
            httpListener = new HttpListener();

            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListener.Prefixes.Add("http://127.0.0.1:8000/");
            httpListener.Start();
            new Thread(new ThreadStart(delegate {
                while (true) {
                    HttpListenerContext httpListenerContext = httpListener.GetContext();
                    string query = httpListenerContext.Request.Url.ToString();
                    string str = query.Split('/')[3];

                    httpListenerContext.Response.StatusCode = 200;
                    //meide zhinengyongyuma!!!!!
                    if (str.Length == 0) {
                        using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream)) {
                            writer.WriteLine(sendData);
                        }
                    } else {
                        oup = new float[60];
                        string[] tags = str.Split('?');
                        for (int i = 0; i < 60; i++)
                            oup[i] = float.Parse(tags[i]);
                        updated = true;
                        using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream)) {
                            writer.WriteLine("");
                        }
                    }
                    Thread.Sleep(10);
                }
            })).Start();
        }
    }
}

/*string str = "-100.237 272.8936 5.675243 -172.7949 261.7891 13.90575 -148.7537 266.8069 32.57311 -113.3566 273.9937 59.68701 -113.3566 273.9937 59.68701 -144.1798 244.359 -66.92436 -137.3393 253.3001 -48.17607 -124.7974 267.4788 -14.2259 -109.8348 290.473 46.42104 -113.4354 250.9895 -82.45232 -110.5136 259.4765 -58.34054 -105.52 272.1716 -16.91907 -99.73083 292.9191 42.41583 -77.52336 258.556 -76.02538 -80.55856 266.1529 -52.33422 -85.56472 276.0233 -13.45415 -88.80891 293.3731 40.40178 -45.10191 262.6089 -49.96777 -52.68843 268.5644 -35.13096 -67.10824 276.0293 -7.566146 -76.97628 288.6944 42.3797";
float[] raw = new List<string>(str.Split(' ')).ConvertAll(i => float.Parse(i)).ToArray();
oup = new float[raw.Length - 3];
for (int i = 0; i < raw.Length - 3; i++) {
    oup[i] = raw[i + 3] - raw[i % 3];
}*/
