using UnityEngine;
using System.Collections.Generic;

public class CharacterResetManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public GameObject character;
        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Quaternion startRot;
    }

    public List<CharacterData> charactersToReset = new List<CharacterData>();

    void Start()
    {
        foreach (var data in charactersToReset)
        {
            if (data.character != null)
            {
                data.startPos = data.character.transform.position;
                data.startRot = data.character.transform.rotation;
            }
        }
    }

    public void ResetAll()
    {
        foreach (var data in charactersToReset)
        {
            if (data.character == null) continue;

            data.character.transform.position = data.startPos;
            data.character.transform.rotation = data.startRot;

            Animator anim = data.character.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
            }
        }
    }
}