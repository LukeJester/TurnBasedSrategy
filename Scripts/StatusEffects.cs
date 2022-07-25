using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    //these will allow diffrent units/eneimes/perks to effect if / how they are effected by status effects.
    [SerializeField] private List<StatusEffect> immuneStatusEffectList;
    [SerializeField] private List<StatusEffect> resistantStatusEffectList;
    [SerializeField] private List<StatusEffect> vulnerableStatusEffectList;

    [SerializeField] private List<Transform> statusEffectVisualsList;
    [SerializeField] private List<Transform> activeStatusEffectVisualsList;

    private List<StatusEffect> currentStatusEffectList;
    private List<int> currentStatusEffectRoundsRemaningList;
    private int j;


    private Unit unit;
    private HealthSystem healthSystem;

    private void Awake()
    {
        currentStatusEffectList = new List<StatusEffect>();
        currentStatusEffectRoundsRemaningList = new List<int>();

        unit = GetComponent<Unit>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;

        healthSystem.OnDead += healthSystem_OnDead;
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        currentStatusEffectList.Clear();
        currentStatusEffectRoundsRemaningList.Clear();
    }

    public void ApplyStatusEffect(StatusEffect statusEffect, int duration)
    {
        //List<StatusEffect> currentStatusEffectList1 = currentStatusEffectList;
        if (currentStatusEffectList.Contains(statusEffect))
        {
            bool statusEffectfound = false;
            int i = 0;
            j=0;
            
            while (!statusEffectfound)
            {
                if (currentStatusEffectList[i] == statusEffect)
                {
                    statusEffectfound = true;
                    j = i;
                }
                else{i += 1;}
            }

            int currentDuration = currentStatusEffectRoundsRemaningList[j];
            currentDuration += duration;
            currentStatusEffectRoundsRemaningList[j] = currentDuration;
        }

        else
        {
            currentStatusEffectList.Add(statusEffect);
            currentStatusEffectRoundsRemaningList.Add(duration);
            VisualStatusEffectEnabled(statusEffect, true);
        }
        

        //ActivateStatusEffect();
    }

    private void ActivateStatusEffect()
    {
        int i = 0;
        foreach (StatusEffect statusEffect in currentStatusEffectList)
        {
            // if (currentStatusEffectRoundsRemaningList[i] <= 0)
            // {
            //     VisualStatusEffectEnabled(statusEffect, false);
            //     currentStatusEffectList.Remove(statusEffect);
            //     currentStatusEffectRoundsRemaningList.RemoveAt(i);
            //     return;
            // }

            int roundsremaning = currentStatusEffectRoundsRemaningList[i];
            roundsremaning = Mathf.Max(roundsremaning - 1, 0);
            DoStatusEffect(statusEffect);
            currentStatusEffectRoundsRemaningList[i] = roundsremaning;
            
            if (currentStatusEffectRoundsRemaningList[i] <= 0)
            {
                VisualStatusEffectEnabled(statusEffect, false);
                currentStatusEffectList.Remove(statusEffect);
                currentStatusEffectRoundsRemaningList.RemoveAt(i);
                return;
            }
            i += 1;
        }

    }

    private void VisualStatusEffectEnabled(StatusEffect statusEffect, bool isEnabled)
    {
        switch (statusEffect)
        {
            case StatusEffect.Burning:
                if (isEnabled)
                {
                    Transform fireStatusEffectVisual = Instantiate(statusEffectVisualsList[0], unit.transform.position, Quaternion.identity);
                    Transform unitVisuals = unit.GetUnitVisual();
                    fireStatusEffectVisual.SetParent(unitVisuals);
                    activeStatusEffectVisualsList.Add(fireStatusEffectVisual);
                }
                else
                {
                    Destroy(activeStatusEffectVisualsList[0].gameObject);
                    activeStatusEffectVisualsList.RemoveAt(0);
                }
                
                break;
        }
    }

    private void DoStatusEffect(StatusEffect statusEffect)
    {
        if (currentStatusEffectList.Contains(statusEffect))
        {
            switch(statusEffect)
            {
                case StatusEffect.Burning:
                unit.Damage(20);
                //Debug.Log(unit.gameObject + " Is Burning for 20 damage bringing it health to " + unit.GetHealthNormilized()*100);
                break;

                case StatusEffect.Poisoned:
                    Debug.Log(unit.gameObject + " Is Poisoned");
                    break;

                case StatusEffect.Electrified:
                    Debug.Log(unit.gameObject + " Is Electrified");
                    break;

                case StatusEffect.Blinded:
                    Debug.Log(unit.gameObject + " Is Blinded");
                    break;

                case StatusEffect.Winded:
                    Debug.Log(unit.gameObject + " Is Winded");
                    break;

                case StatusEffect.Asleep:
                    Debug.Log(unit.gameObject + " Is Asleep");
                    break;
                case StatusEffect.Bleeding:
                    Debug.Log(unit.gameObject + " Is Bleeding");
                    break;
            }
        }

    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn() == !unit.IsEnemy() &&  !unit.IsEnemy())
            ActivateStatusEffect();
        if (!TurnSystem.Instance.IsPlayerTurn() == unit.IsEnemy() && unit.IsEnemy())
            ActivateStatusEffect();
    }
}
public enum StatusEffect
{
    None,
    Burning,
    Poisoned,
    Electrified,
    Blinded,
    Winded,
    Asleep,
    Bleeding
}
