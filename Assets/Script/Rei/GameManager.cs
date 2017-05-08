using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using LitJson;
using MiniJSON;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour {
    System.Random random = new System.Random();
    public static GameManager Instance { set; get; }

	public GameObject mainMenu;
	public GameObject serverMenu;
	public GameObject connectMenu;

	public GameObject serverPrefab;
	public GameObject clientPrefab;

	private string user_jsonString;

	public GameObject[] hostinfo;


	private void Start() {

		Instance = this;
		serverMenu.SetActive (false);
		connectMenu.SetActive (false);
		DontDestroyOnLoad (gameObject);
		//for (int i = 0; i < 10; i++)
		//	hostinfo [i].SetActive (false);

	}

	public void ConncetButton() {
		
		mainMenu.SetActive (false);
		connectMenu.SetActive (true);
	}

	public void HostButton() {
		try {
			Server s = Instantiate (serverPrefab).GetComponent<Server> ();
			s.Init();

			Client c = Instantiate(clientPrefab).GetComponent<Client>();


			// Read userInfo from Json
			try{
				user_jsonString = File.ReadAllText(Application.persistentDataPath + "/User.json");
			}
			catch{
				user_jsonString = (Resources.Load("User") as TextAsset).ToString();
			}
				//Debug.Log (user_jsonString);
			int[] array = new int[30];
			string avatar = "";
			string username = "";
			IList userInfo = (IList)Json.Deserialize (user_jsonString);
			foreach (IDictionary person in userInfo) {
				username = (string)person ["username"];
				//Debug.Log ("username:" + username);
				avatar = (string)person ["avatar"];
				//Debug.Log ("avatar:" + avatar);
				IList deck = (IList)person ["deck"];
				int i = 0;
				foreach (long card in deck) {
					//Debug.Log ("card:" + card);
					array [i++] = unchecked((int)card);
				}
			} 
            for (int i = array.Length - 1; i > 0; i--)
            {
                int index = random.Next(0, i);  
                int temp = array[i];
                array[i] = array[index];
                array[index] = temp;
            }


            //int[] Array = {1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,};
            c.clientName = username; // read json to give the nickname;
			c.deck = array;
			c.avatar = avatar;
            c.host = true;
			c.ConnectToServer("127.0.0.1", 1992);
		} catch (Exception e) {
			Debug.Log (e.Message);
		}

		mainMenu.SetActive (false);
		serverMenu.SetActive (true);
	}

	public void ConnectToServerButton() { 
		
		string hostAddress = GameObject.Find ("HostInput").GetComponent<InputField> ().text;
		if (hostAddress == "") {
			hostAddress = "127.0.0.1";
		}

		try {
			Client c = Instantiate(clientPrefab).GetComponent<Client>();

			// Read userInfo from Json
			try{
				user_jsonString = File.ReadAllText(Application.persistentDataPath + "/User.json");
			}
			catch{
				user_jsonString = (Resources.Load("User") as TextAsset).ToString();
			}
			//Debug.Log (user_jsonString);
			int[] array = new int[30];
			string avatar = "";
			string username = "";
			IList userInfo = (IList)Json.Deserialize (user_jsonString);
			foreach (IDictionary person in userInfo) {
				username = (string)person ["username"];
				//Debug.Log ("username:" + username);
				avatar = (string)person ["avatar"];
				//Debug.Log ("avatar:" + avatar);
				IList deck = (IList)person ["deck"];
				int i = 0;
				foreach (long card in deck) {
					//Debug.Log ("card:" + card);
					array [i++] = unchecked((int)card);
				}
			}
            for (int i = array.Length - 1; i > 0; i--)
            {
                int index = random.Next(0, i);
                int temp = array[i];
                array[i] = array[index];
                array[index] = temp;
            }
            Debug.Log(array);
            c.clientName = username; // read json to give the nickname;
			c.deck = array;//array;
			c.avatar = avatar;
			c.host = false;
			//c.clientName = "Connect";
			c.ConnectToServer(hostAddress, 1992);
			connectMenu.SetActive(false);
			//for(int i = 0 ; i < 10; i++)
			//	hostinfo[i].SetActive(true);
		} catch(Exception e) {
			Debug.Log (e.Message);
		}


	}

	public void BackButton() {
		mainMenu.SetActive (true);
		serverMenu.SetActive (false);
		connectMenu.SetActive (false);

		Server s = FindObjectOfType<Server> ();
		if (s != null) {
			Destroy (s.gameObject);
		}

		Client c = FindObjectOfType<Client> ();
		if (c != null) {
			Destroy (c.gameObject);
		}

	}
    public void StartGame()
    {
        SceneManager.LoadScene("battle");
    }

}
