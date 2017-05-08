using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class Dragable : UIDragDropItem {
    public GameObject card;
    public GameObject myparent;
    public GameObject sword;
    public int cardlimit = 6;
    protected override void OnDragDropRelease(GameObject surface)
    {
        base.OnDragDropRelease(surface);
        if (GameObject.Find("Fight").GetComponent<Fightcard>().cardList.Count > 0 && GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect == false)
        {
            GameObject.Find("hand").GetComponent<hand>().UpdateShow();
        }
            
            else if (surface != null && GameObject.Find("Fight").GetComponent<Fightcard>().cardList.Count < cardlimit && this.GetComponentInParent<carda>().state == carda.Currentstate.hand && surface.tag == "Fight" && GameObject.Find("UI Root").GetComponent<crystal>().usableNum >= card.GetComponent<carda>().getmana() && GameObject.Find("UI Root").GetComponent<crystal>().turn == crystal.CurrentTurn.player)
            {
            string msg = "CPLAY|";
            msg += this.GetComponentInParent<carda>().cardnumber.ToString();
            msg += "|";

                GameObject.Find("UI Root").GetComponent<crystal>().consume(card.GetComponent<carda>().getmana());
                GameObject.Find("hand").GetComponent<hand>().Remove(this.gameObject);
                surface.GetComponent<Fightcard>().AddCard(this.gameObject);
                GameObject.Find("hand").GetComponent<hand>().UpdateShow();
                this.GetComponentInParent<carda>().state = carda.Currentstate.field;
                NGUITools.SetActive(sword, true);
                GameObject.Find("UI Root").GetComponent<crystal>().currentcard = this.gameObject;
                /*if (this.GetComponentInParent<carda>().effect == "revive")
            {
                //msg += " revive|";
                 if (GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect == false && GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().effect == "revive")
                {
                    if (this.GetComponentInParent<carda>().owner == carda.Currentowner.player)
                    {
                        if (GameObject.Find("grave").GetComponent<gravecontainer>().cards.Count > 0)
                        {
                            GameObject go = GameObject.Find("grave").GetComponent<gravecontainer>().revive();
                            GameObject.Find("Fight").GetComponent<Fightcard>().AddCard(go);
                            GameObject.Find("Fight").GetComponent<Fightcard>().UpdateShow();
                        }
                        GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect = true;

                    }
                    else
                    {
                        if (GameObject.Find("egrave").GetComponent<egravecontainer>().cards.Count > 0)
                        {
                            GameObject go = GameObject.Find("egrave").GetComponent<egravecontainer>().revive();
                            GameObject.Find("efight").GetComponent<Fightcard>().AddCard(go);
                            GameObject.Find("efight").GetComponent<Fightcard>().UpdateShow();
                        }
                        GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect = true;

                    }
                }
            }*/
            GameObject.Find("hand").GetComponent<hand>().UpdateShow();
            if (Client.multi == true)
            GameObject.Find("UI Root").GetComponent<crystal>().client.Send(msg);
            GameObject.Find("hand").GetComponent<hand>().Updatecardnum();
            GameObject.Find("ehand").GetComponent<ehand>().Updatecardnum();
        }
            else if (surface != null && this.GetComponentInParent<carda>().state == carda.Currentstate.field && surface.tag == "Card" && this.GetComponentInParent<carda>().couldatk() == true && GameObject.Find("UI Root").GetComponent<crystal>().turn == crystal.CurrentTurn.player)
            {
            string msg = "CATK|";
            msg += this.GetComponent<carda>().cardnumber;
            msg += '|';
            msg += surface.GetComponent<carda>().cardnumber;
                surface.GetComponentInParent<carda>().takedamage(this.GetComponentInParent<carda>().getatk());
                this.GetComponentInParent<carda>().takedamage(surface.GetComponentInParent<carda>().getatk());
                GameObject.Find("Fight").GetComponent<Fightcard>().UpdateShow();
                if (this.gameObject.GetComponent<carda>().couldatk())
                    this.gameObject.GetComponent<carda>().atkreset();
                if (Client.multi == true)
                GameObject.Find("UI Root").GetComponent<crystal>().client.Send(msg);
            GameObject.Find("efight").GetComponent<Efightcard>().Updatecardnum();
            GameObject.Find("Fight").GetComponent<Fightcard>().Updatecardnum();
            GameObject.Find("Fight").GetComponent<Fightcard>().UpdateShow();
            GameObject.Find("efight").GetComponent<Efightcard>().UpdateShow();
        }
            else if (this.GetComponentInParent<carda>().state == carda.Currentstate.field)
            {
                GameObject.Find("Fight").GetComponent<Fightcard>().UpdateShow();
            }
            else
            {
                GameObject.Find("hand").GetComponent<hand>().UpdateShow();
            }
        GameObject.Find("Fight").GetComponent<Fightcard>().UpdateShow();
    }
    

}
