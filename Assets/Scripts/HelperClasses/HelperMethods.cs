using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    public static bool GetComponentsAtBoxLocation<T>(out List<T> componentsAtBoxLocation, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();


        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        for (int i= 0; i < collider2DArray.Length; i++)
        {
            T component = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (component != null)
            {
                found = true;
                componentList.Add(component);
            }
            else
            {
                component = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (component != null)
                {
                    found = true;
                    componentList.Add(component);
                }
            }
        }

        componentsAtBoxLocation = componentList;
        return found;
    }
}
