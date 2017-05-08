using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using LitJson;
using System;
using MiniJSON;
using System.Collections;
using UnityEngine.SceneManagement;

public class CardList : MonoBehaviour {
	public static CardList card_list;
	public static int deckNum;
	public static bool makeNew;

	private string jsonDeck;
	List<CardObject> cards = new List<CardObject>();
	List<CardObject> result_cards = new List<CardObject>();
	public GameObject[] panels;
	
	public GameObject sorttxt;
	public GameObject debugtxt;
	public InputField deck_name_input;
	private int sortMode = 0;
	int page = 0;
	string searchKey = null;
	string tempSearchKey = null;
	bool refresh = true;
	JsonData deckJson;
	readonly int MAX_CARDS = 30;
	
	List<CardObject> edit = new List<CardObject>();
	public Button[] sidepanels;
	int total = 0;
	int offset = 0;

	private string jsonString;
	private string user_jsonString;
	private string card_jsonString;
	private JsonData userData;
	private JsonData cardData;
	string update = "localhost/BCG/update.php";//ServerAddress.URL;
	private string username;
	private string avatar;
	private string nickname;

	int[][] array = new int[4][];
	string[] deck_name = new string[4];
	// Use this for initialization
	void Start() {
		array[0] = new int[30];
		array[1] = new int[30];
		array[2] = new int[30];
		array[3] = new int[30];
		card_list = this;
		// read deck here
		try {
			user_jsonString = File.ReadAllText(Application.persistentDataPath + "/User.json");
		} catch {
			user_jsonString = (Resources.Load("User") as TextAsset).ToString();
		}
		if (user_jsonString != null) {
			
			IList userInfo = (IList)Json.Deserialize(user_jsonString);
			foreach (IDictionary person in userInfo) {
				username = (string)person["username"];
				avatar = (string)person["avatar"];
				nickname = (string)person["nickname"];
				IList[] deck = new IList [4];
				for (int j = 0; j < 4; j++) {
					deck[j] = (IList)person["deck"+j];
					deck_name[j] = (string)person["deck"+j+"_name"];
				}
				int i = 0;
				if (deckNum != -1) {
					foreach (long card in deck[deckNum]) {
						array[deckNum][i++] = unchecked((int)card);
					}
				} else {
					array[deckNum][i++] = 0;
				}
			}
			for (int i = 0; i < 30; i++) {
				InsertCard(new CardObject(array[deckNum][i]), 1);
			}
		}
		for (int i = 1; i <= 3; i++) 
			for (int j = 0; j < 10; j++)
				cards.Add(new CardObject(i));
		deck_name_input.text = deck_name[deckNum];
		RemoveEmptyCards();
		StackCards();
		Sort();
	}
	// Update is called once per frame
	void Update () {
		if(refresh)
			UpdateResult();
		debugtxt.GetComponent<Text>().text = ToString();
	}
	void UpdateResult() {
		refresh = false;
		result_cards.Clear();
		if(searchKey == null)
			for(int i = 0; i < cards.Count; i++)
				result_cards.Add(cards[i]);
		else {
			for(int i = 0; i < cards.Count; i++)
				if(cards[i].card_name.Trim().ToLower().Contains(searchKey.Trim().ToLower()))
					result_cards.Add(cards[i]);
		}
		for (int i = 0; i < 10; i++)
			panels[i].GetComponent<CardPanel>().card = null;
		for (int i = 0; i < (result_cards.Count < 10 ? result_cards.Count : 10); i++) {
			if (cards[page * 10 + i] != null)
				panels[i].GetComponent<CardPanel>().card = result_cards[page * 10 + i];
		}
		for (int i = 0; i < 12; i++)
			sidepanels[i].GetComponent<CardPanelSide>().card = null;
		for (int i = 0; i < (result_cards.Count < 12 ? edit.Count : 12); i++) {
			if (cards[offset + i] != null)
				sidepanels[i].GetComponent<CardPanelSide>().card = edit[offset + i];
		}
	}
	public void Search(string searchKey) {
		this.searchKey = searchKey;
		refresh = true;
	}
	public void Search() {
		searchKey = tempSearchKey;
		refresh = true;
	}
	public void UpdateSearchKey(string searchKey) {
		this.tempSearchKey = searchKey;
	}

	public void PrevPage() {
		if (page > 0)
			page--;
		refresh = true;
	}
	public void NextPage() {
		if((page+1) * 10 < cards.Count)
			page++;
		refresh = true;
	}
	void StackCards() {
		List<CardObject> temp = new List<CardObject>();
		while(cards.Count > 0) {
			temp.Add(cards[0]);
			cards.RemoveAt(0);
			CardObject stacking = temp[temp.Count - 1];
			while (cards.Exists(x => x.card_name == stacking.card_name)) {
				cards.Remove(cards.Find(x => x.card_name == stacking.card_name));
				stacking.stack++;
			}
		}
		cards = temp;
	}
	void RemoveEmptyCards() {
		while (edit.Exists(x => x.id == 0)) {
			edit.Remove(edit.Find(x => x.id == 0));
			Debug.Log("Removed Card");
		}
	}
	public void InsertCard(CardObject card, int stack) {
		refresh = true;
		if (total >= MAX_CARDS)
			return;
		if (total + stack > MAX_CARDS)
			stack = MAX_CARDS - total;
		CardObject c = Find(edit,card.card_name);
		CardObject d = Find(cards, card.card_name);
		if (c == null) {
			edit.Add(new CardObject(card, stack));
			edit.Sort(SortCost);
		} else 
			c.AddStack(stack);
		if(d != null) {
			if (d.stack >= stack)
				d.DecStack(stack);
		}
		total += stack;
	}
	public void RemoveCard(CardObject card, int stack) {
		refresh = true;
		if (total <= 0)
			return;
		CardObject c = Find(edit,card.card_name);
		CardObject d = Find(cards, card.card_name);
		if (c == null) {
			return;
		} else {
			if (c.stack > stack)
				c.DecStack(stack);
			else
			edit.Remove(c);
		}
		if (d != null) {
			d.AddStack(stack);
		}
		total -= stack;
	}
	public void Sort() {
		sortMode = (sortMode + 1) % 4;
		refresh = true;
		switch (sortMode) {
			case 0:     // Alphabetation
				cards.Sort(SortAlpha);
				sorttxt.GetComponent<Text>().text = "Sort by Alphabetation";
				break;
			case 1:     // Cost
				sorttxt.GetComponent<Text>().text = "Sort by Cost";
				cards.Sort(SortCost);
				break;
			case 2:     // Health
				sorttxt.GetComponent<Text>().text = "Sort by Health";
				cards.Sort(SortHealth);
				break;
			case 3:     // Attack
				sorttxt.GetComponent<Text>().text = "Sort by Attack";
				cards.Sort(SortAttack);
				break;
		}
	}
	private static int SortAlpha(CardObject a, CardObject b) {
		if (a == null && b == null)
			return 0;
		if (a == null && b != null)
			return -1;
		if (a != null && b == null)
			return 1;
		return a.card_name.CompareTo(b.card_name);
	}
	private static int SortCost(CardObject a, CardObject b) {
		if (a == null && b == null)
			return 0;
		if (a == null && b != null)
			return -1;
		if (a != null && b == null)
			return 1;
		if (a.cost < b.cost)
			return -1;
		if (a.cost > b.cost)
			return 1;
		return a.card_name.CompareTo(b.card_name);
	}
	private static int SortHealth(CardObject a, CardObject b) {
		if (a == null && b == null)
			return 0;
		if (a == null && b != null)
			return -1;
		if (a != null && b == null)
			return 1;
		if (a.hp < b.hp)
			return -1;
		if (a.hp > b.hp)
			return 1;
		return a.card_name.CompareTo(b.card_name);
	}
	private static int SortAttack(CardObject a, CardObject b) {
		if (a == null && b == null)
			return 0;
		if (a == null && b != null)
			return -1;
		if (a != null && b == null)
			return 1;
		if (a.atk < b.atk)
			return -1;
		if (a.atk > b.atk)
			return 1;
		return a.card_name.CompareTo(b.card_name);
	}
	public override string ToString() {
		string rev = "\n";
		for (int i = 0; i < cards.Count; i++) {
			if (cards[i] != null && cards[i].ToString() != null)
				rev = rev + cards[i].ToString() + "   " + cards[i].stack + "\n";
		}
		return rev;
	}
	public CardObject Find(List<CardObject> list, string card) {
		for (int i = 0; i < list.Count; i++) {
			if (list[i].ToString() == card) {
				return list[i];
			}
		}
		return null;
	}
	public void ChangeDeckName(string newname) {
		deck_name[deckNum] = newname;
	}
	public void Flush() {
		refresh = true;
		while (edit.Count > 0) {
			RemoveCard(edit[0], edit[0].stack);
		}
		total = 0;
		offset = 0;
	}
	public void IncOffset() {
		refresh = true;
		if (total - offset > 12)
			offset++;
	}
	public void DecOffset() {
		refresh = true;
		if (offset > 0)
			offset--;
	}

	IEnumerator LoginToDB(string username, string deck0, string deck1, string deck2, string deck3, string deck0_name, string deck1_name, string deck2_name, string deck3_name) {
		WWWForm form = new WWWForm();
		form.AddField("usernamePost", username);
		//form.AddField("avatarPost", avatar);
		form.AddField("deck0Post", deck0);
		form.AddField("deck1Post", deck1);
		form.AddField("deck2Post", deck2);
		form.AddField("deck3Post", deck3);
		form.AddField("deck0_namePost", deck0_name);
		form.AddField("deck1_namePost", deck1_name);
		form.AddField("deck2_namePost", deck2_name);
		form.AddField("deck3_namePost", deck3_name);


		WWW www = new WWW(update, form);
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
	public struct Deck {
		public int[] cards;
		public string name;
	}
	public void Done() {
		if (total != 30)
			return;
		int count = 0;
		for (int i = 0; i < edit.Count; i++)
			for (int j = 0; j < edit [i].stack; j++)
				array [deckNum][count++] = edit [i].id;
		//Debug.Log (deck_name [deckNum]);
		User user = new User(username, avatar, nickname, array[0], array[1], array[2], array[3], deck_name[0], deck_name[1], deck_name[2], deck_name[3]);
		JsonData userJson = JsonMapper.ToJson(user);





		// Should be updated

		File.WriteAllText(Application.persistentDataPath + "/User.json", "[" + userJson.ToString() + "]");

		string arr0 = "";
		string arr1 = "";
		string arr2 = "";
		string arr3 = "";
		for (int i = 0; i < 30; i++) {
			arr0 = arr0 + array[0][i].ToString();
			arr1 = arr1 + array [1] [i].ToString ();
			arr2 = arr2 + array [2] [i].ToString ();
			arr3 = arr3 + array [3] [i].ToString ();
			if (i != 29) {
				arr0 = arr0 + ",";
				arr1 = arr1 + ",";
				arr2 = arr2 + ",";
				arr3 = arr3 + ",";
			}
		}
		StartCoroutine(LoginToDB(username, arr0, arr1, arr2, arr3, deck_name[0], deck_name[1], deck_name[2], deck_name[3]));
	}
	public void Exit() {
		SceneManager.LoadScene("selectdeck", LoadSceneMode.Single);
	}
}
