using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    [SerializeField] string directorId;

    public string GetDirectorId()
    {
        return directorId;
    }
    public char GetDirectionCode()
    {
        return directorId[0];
    }
}
