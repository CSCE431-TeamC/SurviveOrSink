using System;
using System.Collections;
using UnityEngine;

namespace Battleship
{
    class Ship_Destroyer : Ship
    {
        private GameObject mModel = null;

        public Ship_Destroyer()
            : base(ShipType.DESTROYER)
        {
        }

        public override bool LoadModel()
        {
            if (mModel != null) UnityEngine.Object.Destroy(mModel);
            mModel = GameObject.Instantiate(Resources.Load("Destroyer")) as GameObject;
            if (mModel == null) return false;

            if (mOrientation == ShipOrientation.Horizontal)
            {
                mModel.transform.RotateAround(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f);
            }

            mModel.transform.position = new Vector3(mLocation.X, -0.75f, 9 - mLocation.Y);
            Init_board.messages = Init_board.messages += "\nDestroyer loaded.";

            return true;
        }
    }
}
