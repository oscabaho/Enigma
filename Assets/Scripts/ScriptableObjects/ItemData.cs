using UnityEngine;
using Jugabilidad;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public string description;
    public int precio;
    public float modAtaque;
    public float modSalud;
    public float modVelocidad;
    public float modDefensa;
    public Sprite icono;
    public ItemTipo tipo;
}
