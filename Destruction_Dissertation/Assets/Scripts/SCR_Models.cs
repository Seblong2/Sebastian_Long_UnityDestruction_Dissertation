using UnityEngine;
using System;

public static class SCR_Models
{
    #region - Player - 

    [Serializable]
    
    public class PlayerSettingsModel
    {

        [Header("View Settings")]
        public float ViewXsens;
        public float ViewYsens;

        [Header("Movement")]
        public float forwardspeed;
        public float strafespeed;
        public float backspeed;

        

    }





    #endregion

}

