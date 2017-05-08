using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

public class Server : MonoBehaviour {

	public int port = 1992;

	private List<serverClient> clients;
	private List<serverClient> disconnectList;

	private TcpListener server;
	private bool serverStarted;

	public void Init() {
		DontDestroyOnLoad (gameObject);
		clients = new List<serverClient> ();
		disconnectList = new List<serverClient> ();

		try {

			server = new TcpListener(IPAddress.Any, port);
			server.Start();

			StartListening();
			serverStarted = true;
			 
		} catch (Exception e) {
			Debug.Log ("Socket error: "+ e.Message);
		}
	}

	private void Update() {
		if (!serverStarted) {
			return;
		}

		foreach (serverClient c in clients) {
			// Is the clients still connect?
			if (!IsConnected (c.tcp)) {
				c.tcp.Close ();
				disconnectList.Add (c);
				continue;
			} else {
				NetworkStream s = c.tcp.GetStream ();
				if (s.DataAvailable) {
					StreamReader reader = new StreamReader (s, true);
					string data = reader.ReadLine ();

					if (data != null) {
						OnIncomingData (c, data);
					}
				}
			}
		}
	
		for (int i = 0; i < disconnectList.Count - 1; i++) {

			// Tell our player somebody has disconnected.
			clients.Remove (disconnectList[i]);
			disconnectList.RemoveAt (i);
		}
	}

	private void StartListening() {
		server.BeginAcceptTcpClient(AcceptTcpClient, server);
	}

	private void AcceptTcpClient(IAsyncResult ar) {

		TcpListener listener = (TcpListener)ar.AsyncState;

		string allUsers = "";
		foreach(serverClient i in clients) {
			allUsers += i.clientName + '|';
			//Debug.Log (i.clientName);
		}

		serverClient sc = new serverClient(listener.EndAcceptTcpClient(ar));
		clients.Add(sc);

		StartListening ();

		//Debug.Log("somebody has connected");

		Broadcast("SWHO|" + allUsers, clients[clients.Count - 1]);

		//Debug.Log ("hello");
	}
	private bool IsConnected(TcpClient c) {
		try {
			if( c != null && c.Client != null && c.Client.Connected) {
				if(c.Client.Poll(0, SelectMode.SelectRead)) {
					return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
				}
				return true;
			} else {
				return false;
			}


		} catch {
			return false;
		}
	}

	// server send
	private void Broadcast(string data, List<serverClient> cl) {
		foreach (serverClient sc in cl) {
			try {
				StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
				writer.WriteLine(data);
				writer.Flush();
			} catch (Exception e) {
				Debug.Log ("Write error: " + e.Message);
			}
		}	
	}

	private void Broadcast(string data, serverClient c) {
		List<serverClient> sc = new List<serverClient> {c};
		Broadcast (data, sc);

	}


	// server read
	private void OnIncomingData(serverClient c, string data) {
		Debug.Log ( "Server: " + data);
        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "CWHO":
                c.clientName = aData[1];
                c.isHost = (aData[2] == "0") ? false : true;
                Broadcast("SCNN|" + c.clientName + "|" + c.isHost.ToString(), clients);
                break;
            case "CPLAY":
                Broadcast("SPLAY|" + aData[1], clients);
                break;
            case "CENEMY":
                Broadcast("SENEMY|" + aData[1] + "|" + aData[2] + "|" + aData[3], clients);
                break;
            case"CATK":
                Broadcast("SATK|" + aData[1] +"|"+ aData[2],clients);
                break;
        }
	}

}

public class serverClient {
	
	public string clientName;
	public TcpClient tcp;
    public bool isHost;

	public serverClient(TcpClient tcp) {
		this.tcp = tcp;
	}
}
