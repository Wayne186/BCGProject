using MiniJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using LitJson;
using System.IO;


public class generate_deck : MonoBehaviour {
    public UILabel number;

	private string jsonString;
	private string user_jsonString;
	private string card_jsonString;
	private JsonData userData;
	private JsonData cardData;
	public List<GameObject> cards = new List<GameObject>();
	public GameObject cardprefab;
    //System.Random random = new System.Random();
    public Transform deck;
    //string update = "localhost/BCG/update.php";

    // Use this for initialization
    void Awake () {

        /*********************************************
		 * read data from UserInfo json
		 * *******************************************/


        try
        {
            user_jsonString = File.ReadAllText(Application.persistentDataPath + "/User.json");
        }
        catch
        {
            user_jsonString = (Resources.Load("User") as TextAsset).ToString();
        }
        Debug.Log (user_jsonString);
		int[] array = new int[30];
		string avatar;
		IList userInfo = (IList)Json.Deserialize (user_jsonString);
		foreach (IDictionary person in userInfo) {
			string username = (string)person ["username"];
			Debug.Log ("username:" + username);
			avatar = (string)person ["avatar"];
			Debug.Log ("avatar:" + avatar);
			IList deck = (IList)person ["deck"];
			int i = 0;
			foreach (long card in deck) {
				Debug.Log ("card:" + card);
				array [i++] = unchecked((int)card);
			}
		}



        /************************************************
		 * Read data from CardsInfo by using UserInfo
		 * **********************************************/

        try
        {
            card_jsonString = File.ReadAllText(Application.persistentDataPath + "/cards.json");
        }
        catch
        {
            card_jsonString = (Resources.Load("cards") as TextAsset).ToString();
        }
        Debug.Log (card_jsonString);
		string cardname;
		int cost;
		int hp;
		int atk;
		string sprite;
		string description;
        string effect;
        int num;
        int id;
        string effectdes;

		IList cardInfo = (IList) Json.Deserialize (card_jsonString);

		foreach (IDictionary card in cardInfo) {
			long getID = (long)card ["id"];
			//Debug.Log ("card: " + getID);
			int count = 1;
            if (Client.multi == true)
                array = Client.players[1].deck;
			for (int i = 0; i < 30; i++) {
				if (unchecked((int)getID) == array [i]) {
                    //Debug.Log("Check Array：   "+ array[i]);
					cardname = (string)card ["name"];
					cost = unchecked((int)((long)card ["cost"]));
					hp = unchecked((int)((long)card ["hp"]));
                    id = unchecked((int)((long)card["id"]));
                    atk = unchecked((int)((long)card ["atk"]));
                    effect = (string)card["effect"];
                    description = (string)card ["description"];
					sprite = (string)card ["sprite"];
                    num = unchecked((int)((long)card["num"]));
                    effectdes = (string)card["effectdes"];
                    // call structure
                    GameObject go =  NGUITools.AddChild(this.gameObject, cardprefab);
					go.GetComponent<carda>().setowner(true);
					/*generate.spriteName = cardname[n]*/;
					cards.Add(go);
					go.GetComponent<carda>().state = carda.Currentstate.deck;
					go.GetComponent<carda> ().setstatus (atk, hp, cost, description,effect,num,effectdes,cardname,id);
                    iTween.MoveTo(go, deck.position, 0.5f);
					//Debug.Log (count + ": name: " + cardname + " cost: " + cost + " hp: " + hp + " atk: " + atk + " effect: " + effect + " sprite: " + sprite);
				}
				count++;
			}
        }
	}
    void Update()
    {
        number.text = cards.Count.ToString();
    }

	public GameObject getcard()
	{
        GameObject go = cards[0];
        cards.Remove(go);
        return go;
	}
	public void addcard(GameObject go)
	{
		cards.Add (go);
	}
}
