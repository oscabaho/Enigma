using UnityEngine;

[CreateAssetMenu(fileName = "NewPerk", menuName = "Inventory/Perk")]
public class PerkData : ScriptableObject
{
    public string id;
    public string perkName;
    public string description;
    public int precio;
    public float modAtaque;
    public float modSalud;
    public float modVelocidad;
    public float modDefensa;
    public Sprite icono;
}
