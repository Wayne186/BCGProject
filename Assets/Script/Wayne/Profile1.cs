using MiniJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using LitJson;
using System.IO;

public class Profile1 : MonoBehaviour {

	public static Profile1 _instance;
	private string username;
	private string avatar;
	private string nickname;
	string user_jsonString;
	string update = "localhost/BCG/update_avatar.php";

	int[][] array = new int[4][];
	string[] deck_name = new string[4];

	// Use this for initialization
	void Start () {
	
	}

	void Awake() {

		array[0] = new int[30];
		array[1] = new int[30];
		array[2] = new int[30];
		array[3] = new int[30];

		try {
			user_jsonString = File.ReadAllText(Application.persistentDataPath + "/User.json");
		} catch {
			user_jsonString = (Resources.Load("User") as TextAsset).ToString();
		}
		if (user_jsonString != null) {

			IList userInfo = (IList)Json.Deserialize (user_jsonString);
			foreach (IDictionary person in userInfo) {
				username = (string)person ["username"];
				avatar = (string)person ["avatar"];
				nickname = (string)person["nickname"];
				IList[] deck = new IList [4];
				for (int j = 0; j < 4; j++) {
					deck [j] = (IList)person ["deck" + j];
					deck_name [j] = (string)person ["deck" + j + "_name"];
				}
			}
		}


		_instance = this;
		this.GetComponent<UISprite> ().spriteName = avatar;
		//print (this.GetComponent<UISprite> ().spriteName);
	}

	// Update is called once per frame
	public void changeProfile(string name) {
		_instance.GetComponent<UISprite> ().spriteName = name;
		avatar = name;
		User user = new User(username, avatar, nickname, array[0], array[1], array[2], array[3], deck_name[0], deck_name[1], deck_name[2], deck_name[3]);
		JsonData userJson = JsonMapper.ToJson(user);
		File.WriteAllText(Application.persistentDataPath + "/User.json", "[" + userJson.ToString() + "]");
		StartCoroutine(LoginToDB (username, avatar));
	}

	IEnumerator LoginToDB(string username, string avatar) {
		WWWForm form = new WWWForm();
		form.AddField ("usernamePost", username);
		form.AddField ("avatarPost", avatar);
		Debug.Log ("avatar: " + avatar);
		WWW www = new WWW (update, form);
		yield return www;
		Debug.Log (www.text);
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


}
