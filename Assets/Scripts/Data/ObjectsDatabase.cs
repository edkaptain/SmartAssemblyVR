using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SnapItemDatabase", menuName = "Assembly/Snap Item Database")]
public class ObjectsDataBase : ScriptableObject
{
    public List<Item> items = new List<Item>();

    /// <summary>
    /// Regresa el objeto entero, dependiendo del snapitem type seleccionado
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Item GetItem(SnapItemType type)
    {
        return items.Find(item => item.type == type);
    }
}