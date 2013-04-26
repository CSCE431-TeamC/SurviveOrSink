using System;
using System.Collections;
using UnityEngine;

namespace Battleship
{
    class Ship_Battleship : Ship
    {
        private GameObject mModel = null;

        public Ship_Battleship()
            : base(ShipType.BATTLESHIP)
        {
        }

        public override bool LoadModel()
        {
            if (mModel != null) UnityEngine.Object.Destroy(mModel);
            mModel = GameObject.Instantiate(Resources.Load("Battleship")) as GameObject;
            if (mModel == null) return false;

            mModel.SetActive(false);

            Init_board.messages = Init_board.messages += "\nBattleship loaded.";

            return true;
        }

        public override void HideModel()
        {
            mModel.SetActive(false);
        }
        public override void ShowModel()
        {
            if (mOrientation == ShipOrientation.Horizontal)
            {
                mModel.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
            else
            {
                mModel.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }


            mModel.transform.position = new Vector3(mLocation.X, -0.75f, mLocation.Y);

            mModel.SetActive(true);
        }
    }
}
