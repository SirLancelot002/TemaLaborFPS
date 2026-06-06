using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponData : EquipmentData
{
    public float damage;    //Egyértelmű, 100 a one-hit-kill
    public float fireRate;  //Hány lövést adjon le másodpercenként
    public float range;     //Szintén egyértelmű
    public float spray;     //Fokban mekkora legyen maximum szórás
    public int magazine;    //Ennyi lövés után újra kell tölteni
    public bool isAutomatic;    //Lehet-e folyamatosan lőni
    public float reloadTime;
}