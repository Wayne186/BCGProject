using MiniJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {


	private string user_jsonString;
	string Matched = "localhost/BCG/Matched.php";
	string matchStart = "localhost/BCG/matching.php";
	public static string updateStatus = "localhost/BCG/updateStatus.php";
	bool match;
	public static string username;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Awake() {

	}

	public void goChangeSetting() {
		OptionShow._instance.Show();
		//Application.LoadLevel(1);
	}

	public void goPracticeMode() {
		BlackMask._instance.Show ();
		ChooseNPC._instance.Show ();
	}
	public void goEditMode() {
		SceneManager.LoadScene("selectdeck", LoadSceneMode.Single);
	}

	public void goOnlineMode() {
		
		BlackMask._instance.Show ();
		showLoading._instance.Show();

		//Debug.Log ("wait done");

		user_jsonString = File.ReadAllText (Application.dataPath + "/Resources/User.json");
		Debug.Log (user_jsonString);
		username = "";
		IList userInfo = (IList)Json.Deserialize (user_jsonString);
		foreach (IDictionary person in userInfo) {
			username = (string)person ["username"];
			Debug.Log ("username:" + username);
		}

		StartCoroutine (exe(username));



	}

	public void profile() {
		profileShow._instance.Show();
	}

	IEnumerator exe(String username) {
		
			// StartCoroutine(statusUpdate (username, 2));
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("statusPost", 2);

		WWW www2 = new WWW (updateStatus, form);
		yield return www2;
		Debug.Log (www2.text);

		match = false;
			//MatchingStart ();
		while (true) {
			WWW www = new WWW (matchStart);
			yield return www;
			//Debug.Log("")
			if (www.text == null) {
				yield return null;
				//match = false;
			} else {
				//match = true;
				break;
			}
		}
			//StartCoroutine (wait ());
				Debug.Log ("End Loop");
				//matchedUpdate (username);
				//Debug.Log ("matchedUpdate");
				WWWForm form3 = new WWWForm ();
				form3.AddField ("usernamePost", username);
				Debug.Log (username);
				WWW www3 = new WWW (Matched, form3);
				yield return www3;
				Debug.Log (www3.text);
				//Debug.Log ("kokodayo");
				if (string.IsNullOrEmpty (www3.error)) {
					Debug.Log ("Success");
					//checkLogin = true;
					IList jsonList = (IList)Json.Deserialize (www3.text);
					int[] arr = new int[30];
					int n = 0;
					string name;
					string avatar;
					foreach (IDictionary param in jsonList) {
						name = (string)param ["username"];
						avatar = (string)param ["avatar"];
						IList deck = (IList)param ["deck"];
						Debug.Log ("username:" + name);
						Debug.Log ("avatar:" + avatar);
						foreach (string card in deck) {
							Debug.Log ("card:" + card);
							arr [n++] = Int32.Parse (card);
						}
						User user = new User (name, avatar, arr);
						JsonData userJson = JsonMapper.ToJson (user);
						File.WriteAllText (Application.dataPath + "/Resources/opponent.json", "[" + userJson.ToString () + "]");
					} 
					//Application.LoadLevel (3);
				} else {
					//Debug.Log ("Waiting For Matching");
					//matchedUpdate (username);
					Debug.Log ("Can't Match");
					//statusUpdate (username, 1);
				}

		}


	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

	IEnumerator loop(string username) {
		while (true) {
			yield return null;
			matchedUpdate(username);

			if (showLoading.back == 1) {
				Debug.Log ("here");
				StartCoroutine(statusUpdate (username, 1));
				break;
			}
		}
		Debug.Log ("Break Loop Success");
	}

	IEnumerator wait() {
		yield return new WaitForSeconds (10);
	}

	IEnumerator MatchingStart() {
		while (true) {
			WWW www = new WWW (matchStart);
			yield return www;
			//Debug.Log("")
			if (www.text == null) {
				yield return null;
			} else {
				//match = true;
				break;
			}
		}
		//yield return new WaitForSeconds (2);
	}

	IEnumerator statusUpdate(string username, int status) {
		Debug.Log ("status: " + status);
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("statusPost", status);

		WWW www2 = new WWW(updateStatus, form);
		yield return www2;
		Debug.Log(www2.text);

	}

	IEnumerator matchedUpdate(string username) {
		Debug.Log ("matchedUpdate");
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);

		WWW www = new WWW (Matched, form);
		yield return www;
		Debug.Log (www.text);
		Debug.Log ("kokodayo");
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log ("Success");
			//checkLogin = true;
			IList jsonList = (IList)Json.Deserialize (www.text);
			int[] arr = new int[30];
			int n = 0;
			string name;
			string avatar;
			foreach (IDictionary param in jsonList) {
				name = (string)param ["username"];
				avatar = (string)param ["avatar"];
				IList deck = (IList)param ["deck"];
				Debug.Log ("username:" + name);
				Debug.Log ("avatar:" + avatar);
				foreach (string card in deck) {
					Debug.Log ("card:" + card);
					arr [n++] = Int32.Parse (card);
				}
				User user = new User (name, avatar, arr);
				JsonData userJson = JsonMapper.ToJson (user);
				File.WriteAllText (Application.dataPath + "/Resources/opponent.json", "[" + userJson.ToString () + "]");
			} 
			Application.LoadLevel (3);
		} else {
			//Debug.Log ("Waiting For Matching");
			//matchedUpdate (username);
			Debug.Log("Can't Match");
			statusUpdate (username, 1);

		}

	}




	public class User {
		public string username;
		public string avatar;
		public int[] deck;

		public User(string username, string avatar, int[] deck) {
			this.username = username;
			this.avatar = avatar;
			this.deck = deck;
		}
	}

}
