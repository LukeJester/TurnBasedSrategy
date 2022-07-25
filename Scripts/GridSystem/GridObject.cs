using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;
    private IInteractable interactable;
    private CoverType coverType;
    //this would be used to have gird positions effected by SE like lighting it on fire
    //donw want to have the GO incarge of this/ have a pointer from the level change event to keep track when the effect wears off.
    private StatusEffect statusEffect;
    private int statusEffectTime;

    private bool canBeInteravtedWith; // bool I can chege when making hte level? make it auto if a 3d ojcet is in its area

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach(Unit unit in unitList)
        {
            unitString += unit + "\n";
        }

        return gridPosition.ToString() + "\n" + unitString;;
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if(HasAnyUnit())
        {
            return unitList[0];
        }
        else
        {
            return null;
        }
    }

    public IInteractable GetInteractable()
    {
        return interactable;
    }

    public void SetInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }

    public void SetCoverType(CoverType coverType)
    {
        this.coverType = coverType;
    }

    public CoverType GetCoverType()
    {
        return coverType;
    }

    //might want to make StatusEffect a list, b/c right now only one SE can be active on a tile at a time
    // and if you place a new one it destroys the old one.
    public void SetStatusEffect(StatusEffect statusEffect) 
    {
        this.statusEffect = statusEffect;
    }

    public StatusEffect GetStatusEffect()
    {
        return statusEffect;
    }

}
