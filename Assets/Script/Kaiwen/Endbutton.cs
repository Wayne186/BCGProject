using UnityEngine;
using System.Collections;

public class Endbutton : MonoBehaviour {
    private UILabel label;
        void Awake()
    {
        label = transform.Find("Label").GetComponent<UILabel>();
    }
    public void onEndbuttonClick()
    {
        
        if (GameObject.Find("Fight").GetComponent<Fightcard>().cardList.Count > 0 &&  GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect == false)
            return;
        if (GameObject.Find("UI Root").GetComponent<crystal>().turn == crystal.CurrentTurn.player)
        {
            if (Client.multi == false)
                GameObject.Find("UI Root").GetComponent<crystal>().turnchange();
            else
            {
                string msg = "CPLAY|END";
                GameObject.Find("UI Root").GetComponent<crystal>().client.Send(msg);
            }
        }
    }

	// Use this for initialization
}
