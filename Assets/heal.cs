using UnityEngine;
using System.Collections;

public class heal : MonoBehaviour {

    // Use this for initialization
    public void click()
    {
        if (GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect == false && GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().effect == "heal")
        {
            GameObject.Find("UI Root").GetComponent<crystal>().heal(GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().num, true);
            GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect = true;
        }
        else if (GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect == false && GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().effect == "damage")
        {
            GameObject.Find("UI Root").GetComponent<crystal>().damage(GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().num, true);
            GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect = true;
        }
    }
}
