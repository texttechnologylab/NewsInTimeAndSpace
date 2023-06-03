using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class for an interactable arrow element in a menu.
/// </summary>
public class ArrowElement : UIButton
{
    private FilterMenu3D filterMenu3d;
    private FilterPoolScrollBar filterPoolScrollBar;
    private EventView eventView;

    public enum Type
    {
        Undefined,
        Group,
        Filter
    };
    public Type type = Type.Undefined;

    public enum Direction
    {
        Undefined,
        Up,
        Down
    };
    public Direction direction = Direction.Undefined;

    /// <summary>
    /// Calls method in one of the menus when interacted with.
    /// </summary>
    public override void onRaycastInteraction()
    {
        if (type != Type.Undefined && direction != Direction.Undefined)
        {
            if (filterMenu3d != null)
                filterMenu3d.moveIndexesByArrow(type, direction);
            else if (filterPoolScrollBar != null)
                filterPoolScrollBar.moveIndexesByArrow(type, direction);
            else if (eventView != null)
                eventView.moveIndexesByArrow(type, direction);
        }
    }

    /*public void OnMouseDown()
    {
        if (type != Type.Undefined && direction != Direction.Undefined)
        {
            if (filterMenu3d != null)
                filterMenu3d.moveIndexesByArrow(type, direction);
            else if (filterPoolScrollBar != null)
                filterPoolScrollBar.moveIndexesByArrow(type, direction);
            else if (eventView != null)
                eventView.moveIndexesByArrow(type, direction);
        }
    }*/
    
    /// <summary>
    /// Set own Menu.
    /// </summary>
    /// <param name="filterMenu3d"></param>
    public void setFilterMenu3d(FilterMenu3D filterMenu3d)
    {
        this.filterMenu3d = filterMenu3d;
    }

    /// <summary>
    /// Set own Menu.
    /// </summary>
    /// <param name="filterPoolScrollBar"></param>
    public void setFilterPoolScrollBar(FilterPoolScrollBar filterPoolScrollBar)
    {
        this.filterPoolScrollBar = filterPoolScrollBar;
    }

    /// <summary>
    /// Set own Menu.
    /// </summary>
    /// <param name="eventView"></param>
    public void setEventViewScrollBar(EventView eventView)
    {
        this.eventView = eventView;
    }
}
