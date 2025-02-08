using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Info : MonoBehaviour
{
    //References
    private GameManager GamemanagerRef;
    private EnemyAi enemyAi;

    private List<string> SpellCardsName;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize references
        GamemanagerRef = FindObjectOfType<GameManager>();
        enemyAi = FindObjectOfType<EnemyAi>();

        // Copy from Gamemanager
        SpellCardsName = new List<string>(GamemanagerRef.SpellCardsNames);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpellSpecifier(string spellName, Cards cardref)
    {
        print(spellName);
        switch (spellName)
        {
            case "SpellCard DoubleAttack":
                print("SpellCard");
                foreach (GameObject playerCard in GamemanagerRef.PlayerBoardCards)
                {
                    if (playerCard != null)
                    {
                        // Double the damage of the monster
                        MonsterInfo monsterInfo = playerCard.GetComponent<MonsterInfo>();
                        print("SpellCard" + monsterInfo.Damage);
                        monsterInfo.Damage *= 2;
                        print("SpellCard" + monsterInfo.Damage);

                        cardref.MonsterDamageInfo.text = monsterInfo.Damage.ToString();
                        print("SpellCard" + cardref.MonsterDamageInfo.text);
                    }
                }
                    break;
            case "SpellCard SwitchMonster":
                print("SpellCard ");
                break;
            case "SpellCard DestroyMonster":
                print("SpellCard");
                break;
            case "SpellCard DummyCard":
                print("SpellCard");
                break;
            case "SpellCard Draw2":
                print("SpellCard");
                break;
            default:
                print("Not a spell");
                break;

        }
    }
}
