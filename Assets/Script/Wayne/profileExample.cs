using MiniJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using LitJson;
using System.IO;

public class profileExample : MonoBehaviour {

	public static profileExample _instance;
	private string username;
	private int[] array = new int[30];
	private string avatar;
	string user_jsonString;
	string update = "localhost/BCG/update.php";

	// Use this for initialization
	void Start () {

	}

	void Awake() {

		try {
			user_jsonString = File.ReadAllText (Application.persistentDataPath + "/User.json");
		} catch {
			user_jsonString = (Resources.Load ("User") as TextAsset).ToString ();
		}
		Debug.Log (user_jsonString);
		IList userInfo = (IList)Json.Deserialize (user_jsonString);
		foreach (IDictionary person in userInfo) {
			avatar = (string)person ["avatar"];
			Debug.Log ("avatar:" + avatar);
		}
		this.GetComponent<UISprite> ().spriteName = avatar;
	}

}



