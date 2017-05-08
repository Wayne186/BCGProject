﻿using MiniJSON;
using UnityEngine;
using System.Collections;

public class CardObject {
    public string card_name;
	public string description;
	public Sprite sprite;
	public int hp;
	public int cost;
	public int atk;
	public int id;

	public int stack;
	private string card_jsonString;

	public CardObject(string card_name){
        this.card_name = card_name;
        stack = 1;
    }
	
	public CardObject(int id) {
		if(id == 0) {
			this.id = 0;
			card_name = "";
			description = "";
			sprite = Resources.Load<Sprite>("/Sprites/blank") as Sprite;
			cost = 0;
			hp = 0;
			atk = 0;
			Debug.Log(this.id);
			stack = 1;
			return;
		}
		card_jsonString = (Resources.Load("cards") as TextAsset).ToString();
		IList cardInfo = (IList)Json.Deserialize(card_jsonString);

		foreach (IDictionary card in cardInfo) {
			if (id == unchecked((int)((long)card["id"]))) {
				this.id = unchecked((int)((long)card["id"]));
				card_name = (string)card["name"];
				description = (string)card["description"];
				sprite = Resources.Load<Sprite>((string)card["sprite"]) as Sprite;
				cost = unchecked((int)((long)card["cost"]));
				hp = unchecked((int)((long)card["hp"]));
				atk = unchecked((int)((long)card["atk"]));
			}
		}
		stack = 1;
	}
    public CardObject(CardObject card, int stack){
		id = card.id;
        card_name = card.card_name;
		description = card.description;
		sprite = card.sprite;
		cost = card.cost;
		hp = card.hp;
		atk = card.atk;
        this.stack = stack;
    }
    public void AddStack(int stack){
        this.stack+= stack;
    }
	public void DecStack(int stack) {
		this.stack -= stack;
	}
	public override string ToString(){
        return card_name;
    }
    public static bool operator < (CardObject A, CardObject B){
        return 0 < string.Compare(A.ToString(), B.ToString());
    }
    public static bool operator > (CardObject A, CardObject B){
        return 0 > string.Compare(A.ToString(), B.ToString());
    }
	public int compare(string x, string y) {
		return x.CompareTo(y);
	}
}
