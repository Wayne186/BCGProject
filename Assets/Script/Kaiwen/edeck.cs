using MiniJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using LitJson;
using System.IO;

//assume the enemy information is variable avatar edeck username, when client.multi == false it is single mode, if not is multi
public class edeck : MonoBehaviour
{
    public UILabel number;
    private string jsonString;
    private string user_jsonString;
    private string card_jsonString;
    private JsonData userData;
    private JsonData cardData;
    public List<GameObject> cards = new List<GameObject>();
    public GameObject cardprefab;
    System.Random random = new System.Random();
    public Transform deck;
    //string update = "localhost/BCG/update.php";

    // Use this for initialization
    void Awake()
    {
        string cardname;
        int cost;
        int hp;
        int atk;
        string effect;
        string sprite;
        string description;
        string effectdes;
        int num;
        int id;
        
            
            int[] array = new int[30];
            /*********************************************
             * read data from UserInfo json
             * *******************************************/
            if (NPCselect.flag == -1)
            {

                if (NPCselect.numOfNPC == 1)
                {
                    array = NPCselect.passDeck1;
                }
                else if (NPCselect.numOfNPC == 2)
                {
                    array = NPCselect.passDeck2;
                }
                else
                {
                    array = NPCselect.passDeck3;
                }
            }
            else
            {


                try
                {
                    user_jsonString = File.ReadAllText(Application.persistentDataPath + "/User.json");
                }
                catch
                {
                    user_jsonString = (Resources.Load("User") as TextAsset).ToString();
                }
                Debug.Log(user_jsonString);
                
                string avatar;
                IList userInfo = (IList)Json.Deserialize(user_jsonString);
                foreach (IDictionary person in userInfo)
                {
                    string username = (string)person["username"];
                    //Debug.Log ("username:" + username);
                    avatar = (string)person["avatar"];
                    //Debug.Log ("avatar:" + avatar);
                    IList deck = (IList)person["deck"];
                    int i = 0;
                    foreach (long card in deck)
                    {
                        //Debug.Log ("card:" + card);
                        array[i++] = unchecked((int)card);
                    }
                }

            }
        /************************************************
         * Read data from CardsInfo by using UserInfo
         * **********************************************/
        if (Client.multi == true)
            array = Client.players[1].deck;
            try
            {
                card_jsonString = File.ReadAllText(Application.persistentDataPath + "/cards.json");
            }
            catch
            {
                card_jsonString = (Resources.Load("cards") as TextAsset).ToString();
            }
            Debug.Log(card_jsonString);
            IList cardInfo = (IList)Json.Deserialize(card_jsonString);
            foreach (IDictionary card in cardInfo)
            {
                long getID = (long)card["id"];
                //Debug.Log("card: " + getID);
                int count = 1;
                for (int i = 0; i < 30; i++)
                {
                    if (unchecked((int)getID) == array[i])
                    {
                        cardname = (string)card["name"];
                        cost = unchecked((int)((long)card["cost"]));
                        hp = unchecked((int)((long)card["hp"]));
                        id = unchecked((int)((long)card["id"]));
                        atk = unchecked((int)((long)card["atk"]));
                        effect = (string)card["effect"];
                        description = (string)card["description"];
                        sprite = (string)card["sprite"];
                        effectdes = (string)card["effectdes"];
                        num = unchecked((int)((long)card["num"]));
                        // call structure
                        GameObject go = NGUITools.AddChild(this.gameObject, cardprefab);
                        go.GetComponent<carda>().setowner(true);
                        /*generate.spriteName = cardname[n]*/
                        ;
                        cards.Add(go);
                        go.GetComponent<carda>().state = carda.Currentstate.deck;
                        go.GetComponent<carda>().setstatus(atk, hp, cost, description, effect, num, effectdes, cardname, id);
                        go.GetComponent<carda>().setowner(false);
                        iTween.MoveTo(go, deck.position, 0.5f);
                        //Debug.Log(count + ": name: " + cardname + " cost: " + cost + " hp: " + hp + " atk: " + atk + " effect: " + effect + " sprite: " + sprite);
                    }
                    count++;
                }

            }
            for (int i = 0; i < 4; i++)
                GameObject.Find("ehand").GetComponent<ehand>().getcard();

        
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
        cards.Add(go);
    }
}