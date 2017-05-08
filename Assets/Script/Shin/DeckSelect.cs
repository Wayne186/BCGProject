using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

using MiniJSON;
using System.Collections;

public class DeckSelect : MonoBehaviour {
	private string user_jsonString;
	public Button[] panels;
	// Use this for initialization
	void Start() {
		try {
			user_jsonString = File.ReadAllText(Application.persistentDataPath + "/User.json");
			//user_jsonString = (Resources.Load("User") as TextAsset).ToString();
		}
		catch {
			user_jsonString = (Resources.Load("User") as TextAsset).ToString();
		}
		IList userInfo = (IList)Json.Deserialize(user_jsonString);
		foreach (IDictionary person in userInfo) {
			for (int i = 0; i < 4; i++) {
				panels[i].GetComponentInChildren<Text>().text = (string)person["deck" + i + "_name"];
				Debug.Log((string)person["deck" + i + "_name"]);
			}
		}
	}
	void NewDeck() {
		CardList.makeNew = true;
		SceneManager.LoadScene("editdeck", LoadSceneMode.Single);
	}
	void DeleteDeck() {
		CardList.makeNew = true;
		SceneManager.LoadScene("editdeck", LoadSceneMode.Single);
	}
	void EditDeck() {

	}
	public void SelectDeck(int deck) {
		if(CardList.deckNum != deck)
			CardList.deckNum = deck;
		else {
			CardList.makeNew = false;
			SceneManager.LoadScene("editdeck", LoadSceneMode.Single);
		}
	}

	public void OnClick() {
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
		//Application.LoadLevel (2);
	}
	// Update is called once per frame
}
