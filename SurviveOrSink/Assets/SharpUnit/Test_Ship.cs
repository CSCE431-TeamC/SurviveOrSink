using System;
using Battleship;

namespace SharpUnit
{
	public class Test_Ship : TestCase
	{
		Ship testShipBattleShip = null;
		Ship testShipCustom = null;
		Ship testShipInvalid = null;
		
		
		public override void SetUp()
        {
			testShipBattleShip = new Ship(ShipType.BATTLESHIP);
			testShipCustom = new Ship(7);
			testShipInvalid = new Ship(4);
        }
		
		public override void TearDown()
		{
			testShipBattleShip = null;
			testShipCustom = null;
			testShipInvalid = null;
		}
		
		[UnitTest]
        public void BattleshipLength()
        {
			Assert.Equal(testShipBattleShip.Length, 4);
        }

        [UnitTest]
        public void CustomLength()
        {
            Assert.Equal(testShipCustom.Length, 7);
        }		
		
		[UnitTest]
		public void ValidShipPlace()
		{
			testShipBattleShip.Place(new Point(3,3),ShipOrientation.Horizontal, new Size(10,10));
			Assert.True(testShipBattleShip.IsPlaced);
			Assert.True(testShipBattleShip.IsAt(new Point(3,3)));
			Assert.True(testShipBattleShip.Location == new Point(3,3));
			Assert.True(testShipBattleShip.Orientation == ShipOrientation.Horizontal);
		}
		
		[UnitTest]
		public void InvalidShipPlace()
		{
			testShipBattleShip.Place(new Point(-2,-3),ShipOrientation.Horizontal, new Size(10,10));
			Assert.False(testShipBattleShip.IsPlaced);
			Assert.True(testShipBattleShip.IsAt(new Point(-1,-1)));
		}
		
		[UnitTest]
		public void TestHit()
		{
			testShipBattleShip.Place(new Point(3,3),ShipOrientation.Horizontal, new Size(10,10));
			Assert.True(testShipBattleShip.testHit(new Point(3,3)));
			Assert.False(testShipBattleShip.testHit(new Point(7,7)));
		}
		
		[UnitTest]
		public void TestConflictsWithInvalid()
		{
			testShipBattleShip.Place(new Point(1,1),ShipOrientation.Horizontal, new Size(10,10));
			testShipCustom.Place(new Point(1,1),ShipOrientation.Vertical, new Size(10,10));
			Assert.True(testShipBattleShip.ConflictsWith(testShipCustom));
		}
		
		[UnitTest]
		public void TestConflictsWithValid()
		{
			testShipBattleShip.Place(new Point(5,5),ShipOrientation.Horizontal, new Size(10,10));
			testShipCustom.Place(new Point(1,1),ShipOrientation.Vertical, new Size(10,10));
			Assert.False(testShipBattleShip.ConflictsWith(testShipCustom));
		}		
	}
}

