using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterInfo : MonoBehaviour
{
    // Monster Damage
    public int Damage;

    // Animation
    public bool ActivatedAttackedAnim;
    public bool FinishedAttackAnim;

    public Animation MonsterAnim;

    // References
    private GameManager gameManagerRef;
    private InputManager inputManagerRef;
    private EnemyAi enemyAiRef;

    void Start()
    {
        // Reference to scripts
        gameManagerRef = FindObjectOfType<GameManager>();
        inputManagerRef = FindObjectOfType<InputManager>();
        enemyAiRef = FindObjectOfType<EnemyAi>();

        // Reference to Monster Animations
        MonsterAnim = GetComponent<Animation>();
        if (MonsterAnim != null && MonsterAnim["idle"] != null)
        {
            MonsterAnim["idle"].wrapMode = WrapMode.Loop;
            MonsterAnim.Play("idle");
        }
        else
        {
            Debug.LogError("Animation component or 'idle' animation not found.");
        }
    }

    void Update()
    {
        // Reset enable attack for monster for Use/Enemy when battlephase is done
        if (gameManagerRef.currentTurnPhase != GameManager.TurnPhases.battlePhase && gameManagerRef.currentTurnPhase != GameManager.TurnPhases.opponentBattlePhase)
        {
            ActivatedAttackedAnim = false;
        }

        // Only Activatable during battle phase
        if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.battlePhase || gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentBattlePhase)
        {
         



            //if (inputManagerRef.AttackTriggered && !ActivatedAttackedAnim)
            //{
            //    MonsterAttack();

            //    inputManagerRef.AttackTriggered = false;

            //    //if (gameObject.name == "Pheonix(Clone)") // pheonix special effect
            //    //{
            //    //    // Attack Special Effect
            //    //    GameObject AttackFX = Instantiate(gameManagerRef.SpecialEffects[2], gameObject.transform.position, gameObject.transform.rotation);

            //    //    //Destroy Effect After 2.5 s
            //    //    StartCoroutine(DestroySpecialEffectAfterDelay(2.5f, AttackFX));
            //    //}
            //    //if (gameObject.name == "RockMonster(Clone)") // RockMonster special effect
            //    //{
            //    //    // Summon Attack Effect
            //    //    GameObject AttackFX = Instantiate(gameManagerRef.SpecialEffects[3], gameObject.transform.position, gameObject.transform.rotation);

            //    //    //Destroy Effect After 2.5 s
            //    //    StartCoroutine(DestroySpecialEffectAfterDelay(2.5f, AttackFX));
            //    //}
            //}


        }
    }

    public void MonsterAttack()
    {
        // As long as it is battle phase and monster hasn't already attacked
        if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.battlePhase && ActivatedAttackedAnim == false)
        {
            Transform closestEnemyCard = null; // To store the closest Monster Transform
            float shortestDistance = Mathf.Infinity; // Initialize with a very large number
            // As long as there's monster on the board
            if (enemyAiRef.EnemyBoardCards.Count > 0)
            {


                // Find the closest enemy card
                foreach (GameObject enemyCard in enemyAiRef.EnemyBoardCards)
                {
                    if (enemyCard != null)
                    {
                        // Calculate distance to the current enemy card
                        float distance = Vector3.Distance(this.transform.position, enemyCard.transform.position);

                        // Update closest card if the distance is shorter
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            closestEnemyCard = enemyCard.transform;
                        }
                    }
                }

                

                // Handle the closest card if found
                if (closestEnemyCard != null)
                {
                    // Contain Attacking and Defending monster damage
                    float attackingMonsterDmg = this.Damage;
                    MonsterInfo defendMonsterInfo = closestEnemyCard.gameObject.GetComponent<MonsterInfo>();
                    float defendingMonsterDmg = defendMonsterInfo.Damage;

                    // Make this monster look at the Enemy monster
                    this.transform.LookAt(closestEnemyCard.transform.position);

                    // Play attack Animation
                    if (MonsterAnim.GetClip("attack") != null)
                    {
                        MonsterAnim.Play("attack");

                        Debug.Log("Attack animation has finished.");
                    }

                    // If attacking monster is stronger than destroy defending monster
                    if (attackingMonsterDmg > defendingMonsterDmg)
                    {

                        // Attack Special Effect
                        GameObject AttackFX = Instantiate(gameManagerRef.SpecialEffects[2], closestEnemyCard.gameObject.transform.position, closestEnemyCard.gameObject.transform.rotation);

                        //Destroy Effect After 2.5 s
                        StartCoroutine(DestroySpecialEffectAfterDelay(1.2f, AttackFX));

                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1f, closestEnemyCard.gameObject));

                        // Remove the destroyed card from the EnemyBoardCards list
                        enemyAiRef.EnemyBoardCards.Remove(closestEnemyCard.gameObject);

                        MonsterAnim.PlayQueued("idle");
                    }
                    // if damage is equal destroy both monsters
                    else if (attackingMonsterDmg == defendingMonsterDmg)
                    {
                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1f, this.gameObject));

                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1f, closestEnemyCard.gameObject));
                    }
                    // If attacking monster is weaker, destroy attacking monster
                    else 
                    {
                        // Attack Special Effect
                        GameObject AttackFX2 = Instantiate(gameManagerRef.SpecialEffects[2], this.gameObject.transform.position, this.gameObject.transform.rotation);
                        //Destroy Effect After 2.5 s
                        StartCoroutine(DestroySpecialEffectAfterDelay(1.2f, AttackFX2));

                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1.3f, this.gameObject));

                        gameManagerRef.PlayerBoardCards.Remove(this.gameObject);
                    }
                }
                else
                {
                    Debug.LogWarning("No closest card found, even though EnemyBoardCards is not empty.");
                }

            }
            else
            {
                Debug.LogWarning("No monsters on the enemy board.");
            }
        }

        if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentBattlePhase && ActivatedAttackedAnim == false)
        {
            Transform closestPlayerCard = null; // To store the closest Monster Transform
            float shortestDistance = Mathf.Infinity; // Initialize with a very large number
                                                     // As long as there's monster on the board
            if (gameManagerRef.PlayerBoardCards.Count > 0)
            {
                // Contain Attacking and Defending monster damage
            

                // Find the closest player card
                foreach (GameObject playercard in gameManagerRef.PlayerBoardCards)
                {
                    if (playercard != null)
                    {
                        // Calculate distance to the current enemy card
                        float distance = Vector3.Distance(this.transform.position, playercard.transform.position);

                        // Update closest card if the distance is shorter
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            closestPlayerCard = playercard.transform;
                        }
                    }
                }

                // Handle the closest card if found
                if (closestPlayerCard != null)
                {
                    float attackingMonsterDmg = this.Damage;
                    MonsterInfo defendMonsterInfo = closestPlayerCard.gameObject.GetComponent<MonsterInfo>();
                    float defendingMonsterDmg = defendMonsterInfo.Damage;

                    // Make this monster look at the Enemy monster
                    this.transform.LookAt(closestPlayerCard.transform.position);

                    // Play attack Animation
                    if (MonsterAnim.GetClip("attack") != null)
                    {
                        MonsterAnim.Play("attack");

                        Debug.Log("Attack animation has finished.");
                    }

                    // If attacking monster is stronger than destroy defending monster
                    if (attackingMonsterDmg > defendingMonsterDmg)
                    {
                        // Attack Special Effect
                        GameObject AttackFX = Instantiate(gameManagerRef.SpecialEffects[2], closestPlayerCard.gameObject.transform.position, closestPlayerCard.gameObject.transform.rotation);

                        //Destroy Effect After 2.5 s
                        StartCoroutine(DestroySpecialEffectAfterDelay(1.5f, AttackFX));

                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1f, closestPlayerCard.gameObject));

                        // Remove the destroyed card from the EnemyBoardCards list
                        gameManagerRef.PlayerBoardCards.Remove(closestPlayerCard.gameObject);

                        MonsterAnim.PlayQueued("idle");
                    }
                    // if damage is equal destroy both monsters
                    else if (attackingMonsterDmg == defendingMonsterDmg)
                    {
                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1f, this.gameObject));

                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1f, closestPlayerCard.gameObject));
                    }
                    else
                    {
                        // Attack Special Effect
                        GameObject AttackFX2 = Instantiate(gameManagerRef.SpecialEffects[2], this.gameObject.transform.position, this.gameObject.transform.rotation);
                        //Destroy Effect After 2.5 s
                        StartCoroutine(DestroySpecialEffectAfterDelay(1.2f, AttackFX2));

                        // Destroy Monster after delay
                        StartCoroutine(DestroyMonsterAfterDelay(1.3f, this.gameObject));

                        gameManagerRef.PlayerBoardCards.Remove(this.gameObject);
                    }
                }
                else
                {
                    Debug.LogWarning("No closest card found, even though EnemyBoardCards is not empty.");
                }

            }
            else
            {
                Debug.LogWarning("No monsters on the enemy board.");
            }
        }
     
                ActivatedAttackedAnim = true;


                //// Check if the attack animation has finished
                //gameManagerRef.Attacked = true;

                //FinishedAttackAnim = true;

        
    }

    private IEnumerator DestroyMonsterAfterDelay(float delay, GameObject Monster)
    {
        // Wait for the specified amount of time (1 seconds in this case)
        yield return new WaitForSeconds(delay);

        // Now destroy the child after the delay
        Destroy(Monster);
    }

    private IEnumerator DestroySpecialEffectAfterDelay(float delay, GameObject Fx)
    {
        // Wait for the specified amount of time (3 seconds in this case)
        yield return new WaitForSeconds(delay);

        // Now destroy the child after the delay
        Destroy(Fx);
    }
}
