using UnityEngine;
using Jugabilidad;

[CreateAssetMenu(fileName = "NewPerkConTipo", menuName = "Inventory/PerkConTipo")]
public class PerkConTipoData : ScriptableObject
{
    public string id;
    public string perkName;
    public string description;
    public int precio;
    public PerkTipo tipo;
    public float modificador; // Valor del bonus (ej: 0.2f para +20%)
    public Sprite icono;
}
