using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{

    public static event EventHandler AfterAnyDestroyed;
    public static event EventHandler OnAnyPlacment;

    [SerializeField] CoverType coverType;

    [SerializeField] private Sprite fullCoverSprite;
    [SerializeField] private Sprite halfCoverSprite;
    [SerializeField] private SpriteRenderer[] spriteRendererArray;

    // have Awake loop through the transforms to fill their corosponding SpriteRendererArrays
    [SerializeField] private Transform northTransform;
    [SerializeField] private Transform southTransform;
    [SerializeField] private Transform eastTransform;
    [SerializeField] private Transform westTransform;
    private SpriteRenderer[] northSpriteRendererArray;
    private SpriteRenderer[] eastSpriteRendererArray;
    private SpriteRenderer[] southSpriteRendererArray;
    private SpriteRenderer[] westSpriteRendererArray;

    
    GridPosition thisGridPosition;
    GridPosition northGridPosition;
    GridPosition eastGridPosition;
    GridPosition sothGridPosition;
    GridPosition westGridPosition;

    //Need to make the cover grid positons dynamic to different sizes of covor
    GridPosition thisGridPositionAray;
    GridPosition northGridPositionAray;
    GridPosition eastGridPositionAray;
    GridPosition sothGridPositionAray;
    GridPosition westGridPositionAray;

    private bool mouseOverCover;

    private void Awake() // make this find all the renders in each directional spriteRendererArray by aranging the sprits in empty game gamgeOmcests
    {
        if (coverType == CoverType.Environment)
            return;

        if (spriteRendererArray == null) // if i forgot to fill the the sprite reneder on Environment
            return;

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

        //moved to Awake due to race error
        Unit.OnAnyCoverStateChanged += unit_OnAnyCoverStateChanged;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

        thisGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        SetCoverGridPositions(thisGridPosition); 
    }

    private void Start()
    {
        if (coverType == CoverType.Environment)
            return;

        if (spriteRendererArray == null) 
            return;

        OnAnyPlacment?.Invoke(this, EventArgs.Empty);

        // Unit.OnAnyCoverStateChanged += unit_OnAnyCoverStateChanged;
        // Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

        // thisGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        // SetCoverGridPositions(thisGridPosition);
    }

    private void OnDestroy()
    {
        AfterAnyDestroyed?.Invoke(this, EventArgs.Empty);

        Unit.OnAnyCoverStateChanged -= unit_OnAnyCoverStateChanged;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }

    public void SetCoverGridPositions(GridPosition gridPosition) // how to make this work for cover that takes uo mor that 1x1 grid position?
    {
        northGridPosition = new GridPosition(gridPosition.x + 0, gridPosition.z + 1);
        eastGridPosition = new GridPosition(gridPosition.x + 1, gridPosition.z + 0);
        sothGridPosition = new GridPosition(gridPosition.x + 0, gridPosition.z - 1);
        westGridPosition = new GridPosition(gridPosition.x - 1, gridPosition.z + 0);

    }

    private void UpdateShieldSprites()
    {
        if (coverType == CoverType.Environment)
            return;

        if (coverType == CoverType.None)
            return;

        if (spriteRendererArray == null)
            return;

        if (this == null)
            return;

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
        if (coverType == CoverType.Environment)
            return;

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

    public GridPosition GetGridPosition()
    {
        return thisGridPosition;
    }

}

public enum CoverType
{
    None,
    Half,
    Full,
    Environment
}

public enum CoverDirection
{
    North,
    East,
    South,
    West
}
