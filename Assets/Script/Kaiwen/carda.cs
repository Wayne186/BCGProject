using UnityEngine;
using System.Collections;

public class carda : MonoBehaviour {

    private UISprite sprite;
    public UILabel hpl;
    public UILabel atkl;
    public UILabel manal;
    public UILabel des;
    public int atk;
    public int hp;
    public int mana;
    public bool canatk = false;
    public Transform grave;
    public Transform egrave;
    public GameObject green;
    public GameObject myparent;
    public string description;
    public GameObject back;
    public string effect;
    public int num;
    public string cardname;
    public string effectdes;
    public bool iseffect = false;
    public UILabel namel;
    public int currentatk;
    public int id;
    public int currenthp;
    public int cardnumber;
    public enum Currentowner
    {
        player,
        enemy
    }
    public enum Currentstate
    {
        deck,
        hand,
        grave,
        field
    }
    public Currentowner owner;
    public Currentstate state;
    private string CardName
    {
        get
        {
            return sprite.spriteName;
        }
    }
    void Start()
    {
        iseffect = true;
    }
    void Update()
    {
        if (owner == Currentowner.player)
        {
            if (state == Currentstate.deck)
                back.SetActive(true);
            else
                back.SetActive(false);
        }
        else
        {
            if (state == Currentstate.field)
                back.SetActive(false);
            else
                back.SetActive(true);
        }
		atkl.text = currentatk.ToString ();
		hpl.text = currenthp.ToString ();
		manal.text = mana.ToString ();
        namel.text = cardname;
        if (state == Currentstate.deck)
        {
            this.GetComponent<Dragable>().enabled = false;
        }
        else if (owner == Currentowner.player)
        {
            this.GetComponent<Dragable>().enabled = true;
        }

    }
    public void setstate(string description)
    {
        if (string.Compare(description,"die") == 0)
        {
            state = Currentstate.grave;
        }
        if (string.Compare(description, "hand") == 0)
        {
            state = Currentstate.hand;
        }
        if (string.Compare(description, "field") == 0)
        {
            state = Currentstate.field;
        }

    }
    public void setowner(bool isplayer)
    {
        if (isplayer == true)
            owner = Currentowner.player;
        else
            owner = Currentowner.enemy;
    }
    public void atkreset()
    {
        if (canatk == true)
        {
            canatk = false;
            NGUITools.SetActive(green,false);
        }
        else
        {
            canatk = true;
            NGUITools.SetActive(green, true);
        }
        }
    public bool couldatk()
    {
        return canatk;
    }
    void Awake()
    {
        /*sprite = this.GetComponent<UISprite>();*/
        atk = int.Parse(atkl.text);
        hp = int.Parse(hpl.text);
        mana = int.Parse(manal.text);
        grave = GameObject.Find("grave").transform;
        egrave = GameObject.Find("egrave").transform;
        description = "I am description";

    }
    public int getmana()
    {
        return mana;
    }
    public int getatk()
    {
        return atk;
    }
    public int gethp()
    {
        return hp;
    }

    public void reset()
    {
        atkl.text = currentatk + "";
        hpl.text = currenthp + "";
    }
    public void die()
    {
        if (owner == Currentowner.player)
        {
            NGUITools.SetActive(green, false);
            iTween.MoveTo(this.gameObject, grave.position, 0.5f);
            GameObject.Find("grave").GetComponent<gravecontainer>().Addcard(this.gameObject);
            GameObject.Find("Fight").GetComponent<Fightcard>().RemoveCard(this.gameObject);
            state = Currentstate.grave;

        }
        else
        {
            iTween.MoveTo(this.gameObject, egrave.position, 0.5f);
            GameObject.Find("egrave").GetComponent<egravecontainer>().Addcard(this.gameObject);
            GameObject.Find("efight").GetComponent<Efightcard>().RemoveCard(this.gameObject);
            state = Currentstate.grave;
        }
    }
    public void takedamage(int n)
    {
        currenthp -= n;
        if (currenthp <= 0)
            die();
    }
    public void setstatus(int atk,int hp,int mana,string des,string effect,int num,string effectdes,string name,int id)
    {
        this.atk = atk;
        this.hp = hp;
        this.mana = mana;
        description = des;
		atkl.text = atk.ToString ();
		hpl.text = hp.ToString ();
		manal.text = mana.ToString ();
        this.effect = effect;
        this.num = num;
        this.cardname = name;
        this.effectdes = effectdes;
        currentatk = atk;
        currenthp = hp;
        this.id = id;
    }
    /*void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Card" && obj.gameObject.GetComponent<carda>().iseffect == false)
        {
            if (obj.gameObject.GetComponent<carda>().effect == "heal")
            {
                this.heal(obj.gameObject.GetComponent<carda>().num);
                obj.gameObject.GetComponent<carda>().iseffect = true;

            }
        }

    }*/
    public GameObject gettarget()
    {
        return this.gameObject;
    }
    void onMouseDown()
    {
        gettarget();
    }
    void OnPress(bool isPressed)
    {
        if (state == Currentstate.hand && owner == Currentowner.player)
        showcard._instance.show(atk,hp,mana,effectdes,description);
    }
    void OnClick()
    {
        if (GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect == false && GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().effect == "heal")
        {
            heal(GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().num);
            GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect = true;
        }
        else if (GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect == false && GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().effect == "damage")
        {
            takedamage(GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().num);
            GameObject.Find("UI Root").GetComponent<crystal>().currentcard.GetComponent<carda>().iseffect = true;
        }
      
    }
    public void heal (int n)
    {
        currenthp += n;
        if (currenthp > hp)
            currenthp = hp;
    }
}
