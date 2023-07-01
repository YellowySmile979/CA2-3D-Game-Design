using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData/ScriptableCharacter")]
public class CharacterData : ScriptableObject
{
    public float moveSpeed;
    public float damage;
    public float health;
}
