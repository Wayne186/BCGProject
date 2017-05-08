using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Client : MonoBehaviour {
    public static bool multi = false;
	public string clientName;
	public string avatar;
	public int[] deck = new int[30];
	public bool host;
	private bool socketReady;
	private TcpClient socket;
	private NetworkStream stream;
	private StreamWriter writer;
	private StreamReader reader;
    public int[] edeck = new int[30];
    public string eavatar;
    public string eusername;

	public static List<GameClient> players = new List<GameClient>(); 

	private void Start() {
		DontDestroyOnLoad (gameObject);
	}

	public bool ConnectToServer(string host, int port) {
		if (socketReady) {
			return false;
		}

		try {
			socket = new TcpClient(host, port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
			socketReady = true;
		} catch(Exception e) {
			Debug.Log ("Socket error: " + e.Message);
		}

		return socketReady;
	}

	private void Update() {
		if (socketReady) {
			if (stream.DataAvailable) {
				string data = reader.ReadLine ();
				if (data != null) {
					OnIncomingDate (data);
				}
			}
		}
	}

	// sending message to server
	public void Send(string data) {
		//Debug.Log (data);
		if (!socketReady) {
			return;
		}
		writer.WriteLine (data);
		writer.Flush ();
	}

	// read messages from server
	private void OnIncomingDate(string data) {
		Debug.Log ("Client:"+data);;
		string[] aData = data.Split('|');
	
		switch(aData[0]) {
		case "SWHO":
			for (int i = 1; i < aData.Length - 1; i++) {
				UserConnected (aData [i], false);
			}
			Send ("CWHO|" + clientName + "|" + ((host)?1:0).ToString());
			break;
            case "SCNN":
                bool hh;
                if (aData[2] == "True")
                    hh = true;
                else
                    hh = false;
                UserConnected(aData[1], hh);
                break;
            case "SPLAY":
                if (aData[1] == "END")
                    crystal.instance.turnchange();
                else
                crystal.instance.enemyplay(int.Parse(aData[1]));
                break;
            case "SENEMY":
                players[1].name = aData[1];
                players[1].avatar = aData[2];
				int a = 0;
                foreach (char c in aData[3])
                {
                    players[1].deck[a++] = int.Parse(c+"");
                }
                break;
            case "SATK":
                crystal.instance.enemyatk(int.Parse(aData[1]),int.Parse(aData[2]));
                break;
		}

	}

	private void UserConnected(string name, bool host) {
		GameClient c = new GameClient ();
        c.isHost = host;
		c.name = name;
		players.Add (c);
		Debug.Log ("count: " + players.Count);
		if (players.Count == 2 ) {//&&(players[0].isHost != players[1].isHost)) {
            multi = true;
            string msg = "CENEMY|";
            msg += clientName;
            msg += "|";
            msg += avatar;
            msg += "|";
            string decks ="";
            for (int i = 0; i < 30; i++)
                decks += deck[i].ToString();
            msg += decks;
            Send(msg);

            StartCoroutine(DelayMethod(1.0f, () =>
            {
                GameManager.Instance.StartGame();

            }));        
            
		}
	}

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    private void OnApplicationQuit() {
		CloseSocket ();
	}

	private void OnDisable() {
		CloseSocket ();
	}

	private void CloseSocket() {
		if (!socketReady) {
			return;
		}
		writer.Close ();
		reader.Close ();
		socket.Close ();
		socketReady = false;
	}
}

public class GameClient {
	public string name;
	public bool isHost;
    public int[] deck = new int[30];
    public string avatar;

}
