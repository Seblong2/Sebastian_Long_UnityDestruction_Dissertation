using UnityEngine;
using System;

public static class CharacterDefaulkts 
{
    #region - Player - 

    [Serializable]

    public class InputSettings
    {
        [Header("LookSettings")]
        public float lookXsens;
        public float lookYsens;

        [Header("MovementSettings")]
        public float Forwardspeed;
        public float StrafeSpeed;
        public float BackSpeed;


    }
    #endregion
}
