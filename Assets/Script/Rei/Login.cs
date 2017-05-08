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

public static class ServerAddress{
	public static string URL = "localhost/BCG/Login.php";
}
public class Login : MonoBehaviour {
	public GameObject username;
	public GameObject password;
	private string Username;
	private string Password;
	bool checkLogin = false;

	string LoginURL = "localhost/BCG/Login.php";

	public void LoginButton() {
		bool UN = false;
		bool PW = false;
		if (Username != "") {
			UN = true;

		} else {
			Debug.LogWarning ("Username Field Empty");
		}
		if (Password != "") {
			PW = true;
		} else {
			Debug.LogWarning ("Password Field Empty");
		}
		if (UN == true && PW == true) {
			StartCoroutine(LoginToDB (Username, Password));

			StartCoroutine(DelayMethod(1.0f, () =>
			{
					Debug.Log("Delay call");
				
				print (checkLogin);
				if (checkLogin == true) {
					SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
					//Application.LoadLevel (2);
				}
			}));
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (username.GetComponent<InputField> ().isFocused) {
				password.GetComponent<InputField> ().Select ();
			}
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			if (Password != "" && Username != "") {
				LoginButton ();
			}
		}
		Username = username.GetComponent<InputField> ().text;
		Password = password.GetComponent<InputField> ().text;
	}

	IEnumerator LoginToDB(string username, string password) {
		WWWForm form = new WWWForm();
		form.AddField ("usernamePost", username);
		form.AddField ("passwordPost", password);

		WWW www = new WWW (LoginURL, form);
		yield return www;
		Debug.Log (www.text);
		if (!string.IsNullOrEmpty(www.error)) {
			Debug.Log ("Error!");
		} else {
			Debug.Log ("Success");
			//checkLogin = true;
			IList jsonList = (IList)Json.Deserialize (www.text);
			int[] deck0 = new int[30];
			int[] deck1 = new int[30];
			int[] deck2 = new int[30];
			int[] deck3 = new int[30];
			string deck0_name;
			string deck1_name;
			string deck2_name;
			string deck3_name;
			string nickname;
			int n = 0;
			string name;
			string avatar;
			foreach (IDictionary param in jsonList) {
				name = (string)param ["username"];
				avatar = (string)param["avatar"];
				nickname = (string)param ["nickname"];
				IList Deck0 = (IList)param ["deck0"];
				IList Deck1 = (IList)param["deck1"];
				IList Deck2 = (IList)param ["deck2"];
				IList Deck3 = (IList)param["deck3"];
				deck0_name = (string)param ["deck0_name"];
				deck1_name = (string)param ["deck1_name"];
				deck2_name = (string)param ["deck2_name"];
				deck3_name = (string)param ["deck3_name"];

				foreach(string card0 in Deck0){
					deck0 [n++] = Int32.Parse(card0);
				}
				n = 0;
				foreach(string card1 in Deck1){
					deck1 [n++] = Int32.Parse(card1);
				}
				n = 0;
				foreach(string card2 in Deck2){
					deck2 [n++] = Int32.Parse(card2);
				}
				n = 0;
				foreach(string card3 in Deck3){
					deck3 [n++] = Int32.Parse(card3);
				}
				User user = new User (name, avatar, nickname, deck0, deck1, deck2, deck3, deck0_name, deck1_name, deck2_name, deck3_name);
				JsonData userJson = JsonMapper.ToJson (user);
				File.WriteAllText(Application.persistentDataPath + "/User.json", "[" + userJson.ToString() + "]");
			}
			checkLogin = true;
		}
	}

	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

}

public class User {
	public string username;
	public string avatar;
	public string nickname;
	public int[] deck0;
	public int[] deck1;
	public int[] deck2;
	public int[] deck3;
	public string deck0_name;
	public string deck1_name;
	public string deck2_name;
	public string deck3_name;

	public User(string username, string avatar, string nickname, int[] deck1, int[] deck2, int[] deck3, int[] deck4, string deck_name1, string deck_name2, string deck_name3, string deck_name4) {
		this.username = username;
		this.avatar = avatar;
		this.nickname = nickname;
		this.deck0 = deck1;
		this.deck1 = deck2;
		this.deck2 = deck3;
		this.deck3 = deck4;
		this.deck0_name = deck_name1;
		this.deck1_name = deck_name2;
		this.deck2_name = deck_name3;
		this.deck3_name = deck_name4;


	}
}
