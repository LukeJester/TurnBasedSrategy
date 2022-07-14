using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{
    [SerializeField] CoverType coverType;

    [SerializeField] private Sprite fullCoverSprite;
    [SerializeField] private Sprite halfCoverSprite;
    [SerializeField] private SpriteRenderer[] spriteRendererArray;

    GridPosition thisGridPosition;
    GridPosition northGridPosition;
    GridPosition eastGridPosition;
    GridPosition sothGridPosition;
    GridPosition westGridPosition;

    private bool mouseOverCover;

    private void Awake()
    {
        if (coverType == CoverType.Half)
        {
            foreach(SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                spriteRenderer.sprite = halfCoverSprite;
                spriteRenderer.enabled = false;
            }
        }
        else if (coverType == CoverType.Full)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                spriteRenderer.sprite = fullCoverSprite;
                spriteRenderer.enabled = false;
            }
        }
        else 
        {
            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                spriteRenderer.enabled = false;
            }
        }

        //moved to Awake or race error
        Unit.OnAnyCoverStateChanged += unit_OnAnyCoverStateChanged;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

        thisGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        SetCoverGridPositions(thisGridPosition);
    }

    private void Start()
    {
        // Unit.OnAnyCoverStateChanged += unit_OnAnyCoverStateChanged;
        // Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

        // thisGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        // SetCoverGridPositions(thisGridPosition);
    }

    private void OnDestroy()
    {
        Unit.OnAnyCoverStateChanged -= unit_OnAnyCoverStateChanged;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }

    public void SetCoverGridPositions(GridPosition gridPosition)
    {
        northGridPosition = new GridPosition(gridPosition.x + 0, gridPosition.z + 1);
        eastGridPosition = new GridPosition(gridPosition.x + 1, gridPosition.z + 0);
        sothGridPosition = new GridPosition(gridPosition.x + 0, gridPosition.z - 1);
        westGridPosition = new GridPosition(gridPosition.x - 1, gridPosition.z + 0);

    }

    private void UpdateShieldSprites()
    {
        if (northGridPosition.z <= LevelGrid.Instance.GetHeight() && LevelGrid.Instance.HasAnyUnitOnGridPosition(northGridPosition))
            spriteRendererArray[0].enabled = true;
        else
        {
            spriteRendererArray[0].enabled = false;
        }

        if (eastGridPosition.x <= LevelGrid.Instance.GetWidth() && LevelGrid.Instance.HasAnyUnitOnGridPosition(eastGridPosition))
            spriteRendererArray[1].enabled = true;
        else
        {
            spriteRendererArray[1].enabled = false;
        }

        if (sothGridPosition.z >= 0 && LevelGrid.Instance.HasAnyUnitOnGridPosition(sothGridPosition))
            spriteRendererArray[2].enabled = true;
        else
        {
            spriteRendererArray[2].enabled = false;
        }

        if (westGridPosition.x >= 0 && LevelGrid.Instance.HasAnyUnitOnGridPosition(westGridPosition))
            spriteRendererArray[3].enabled = true;
        else
        {
            spriteRendererArray[3].enabled = false;
        }
    }

    private void unit_OnAnyCoverStateChanged(object sender, EventArgs e)
    {
        UpdateShieldSprites();
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateShieldSprites();
    }


    public CoverType GetCoverType()
    {
        return coverType;
    }

    public void SetCoverType(CoverType coverType) // could be used it full cover gets blownup and turned to half cover
    {
        this.coverType = coverType;
    }

    void OnMouseOver()
    {
        HighlightPossibleShileds();
    }

    void OnMouseExit()
    {
        UpdateShieldSprites();
    }

    public void HighlightPossibleShileds()
    {
        if (northGridPosition.z <= LevelGrid.Instance.GetHeight() && Pathfinding.Instance.IsWalkableGridPosition(northGridPosition))
            spriteRendererArray[0].enabled = true;
        else
        {
            spriteRendererArray[0].enabled = false;
        }

        if (eastGridPosition.x <= LevelGrid.Instance.GetWidth() && Pathfinding.Instance.IsWalkableGridPosition(eastGridPosition))
            spriteRendererArray[1].enabled = true;
        else
        {
            spriteRendererArray[1].enabled = false;
        }

        if (sothGridPosition.z >= 0 && Pathfinding.Instance.IsWalkableGridPosition(sothGridPosition))
            spriteRendererArray[2].enabled = true;
        else
        {
            spriteRendererArray[2].enabled = false;
        }

        if (westGridPosition.x >= 0 && Pathfinding.Instance.IsWalkableGridPosition(westGridPosition))
            spriteRendererArray[3].enabled = true;
        else
        {
            spriteRendererArray[3].enabled = false;
        }
    }
}

public enum CoverType
{
    None,
    Half,
    Full
}
