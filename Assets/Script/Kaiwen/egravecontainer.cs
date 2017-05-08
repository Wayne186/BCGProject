using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class egravecontainer : MonoBehaviour {

    public List<GameObject> cards = new List<GameObject>();
    public void Addcard(GameObject go)
    {
        cards.Add(go);
    }
    public void Removecard(GameObject go)
    {
        cards.Remove(go);
    }
    public GameObject revive()
    {
        System.Random random = new System.Random();
        int n = random.Next(0, cards.Count - 1);
        GameObject go = cards[n];
        Removecard(go);
        go.GetComponent<carda>().currenthp = go.GetComponent<carda>().hp;
        go.GetComponent<carda>().currentatk = go.GetComponent<carda>().atk;
        go.GetComponent<carda>().state = carda.Currentstate.field;
        return go;
    }
}
