using NUnit.Framework;
using Kq1Pathing.RoomPathing;
using static Kq1Pathing.RoomPathing.MapTraversal;

// Ctrl R,T to run tests
// or play button in the test panel (which doesn't report 'Failed to load metadata')
namespace Kq1Tests
{
    public class MapTraversalTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCastleTraversals()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 53, 25, 115, RoomControl.DIR_E); // (119,115)(54)
            Assert.That((119, 115, 54), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 54, 119, 115, RoomControl.DIR_SW); // (34,116)(55)
            Assert.That((34, 116, 55), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 55, 117, 109, RoomControl.DIR_SW); // (97,135)(2)
            Assert.That((97, 135, 2), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestBunchOfTraversals()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 30, 48, 155, RoomControl.DIR_NE); // (106,97)(30)
            Assert.That((106, 97, 30), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 30, 48, 155, RoomControl.DIR_NW); // (90,42)(29)
            Assert.That((90, 42, 29), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 30, 48, 155, RoomControl.DIR_W); // (148,155)(26)
            Assert.That((148, 155, 26), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 10, 2, 157, RoomControl.DIR_S); // STOP(2,47)(7)
            Assert.That((2, 47, 7), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 10, 105, 132, RoomControl.DIR_SE); // (53,66)(7)
            Assert.That((53, 66, 7), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 48, 119, 167, RoomControl.DIR_NW); // STOP(90,139)(73)
            Assert.That((90, 139, 73), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 74, 50, 100, RoomControl.DIR_NE); // (107,135)(73)
            Assert.That((107, 135, 73), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 73, 107, 135, RoomControl.DIR_SW); // (50,100)(77)
            Assert.That((50, 100, 74), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 74, 62, 123, RoomControl.DIR_W); // (5,123)(75)
            Assert.That((5, 123, 75), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 74, 22, 110, RoomControl.DIR_SW); // (133,149)(75)
            Assert.That((133, 149, 75), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 75, 143, 124, RoomControl.DIR_E); // (125,124)(74)
            Assert.That((125, 124, 74), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            // these are good when I perform the script rewriting before the barrier clearing
            // (but otherwise are off by 1)
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 75, 154, 108, RoomControl.DIR_E);
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 75, 154, 109, RoomControl.DIR_NE);
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 75, 154, 109, RoomControl.DIR_E); // (6,109)(74)
            Assert.That((6, 109, 74), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 74, 80, 163, RoomControl.DIR_N); // (77,119)(73)
            Assert.That((77, 119, 73), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            // these two require the barrier clearance to be performed first to be correct
            // (but might operate on principles I haven't appreciated yet)
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 21, 0, 96, RoomControl.DIR_NW); // (153,96)(22)
            Assert.That((153, 96, 22), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 21, 0, 95, RoomControl.DIR_W); // WATER(136,96)(23)
            Assert.That((136, 96, 23), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            // this example shows that the tile entered in a room should also be evaluated for
            // water, deathtriggers or room triggers. The correct evaluation should be WATER
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 42, 154, 121, RoomControl.DIR_SE); // WATER(0,122)(43)
            Assert.That((0, 122, 43), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 79, 24, 113, RoomControl.DIR_W); // WATER(112,123)(43)
            Assert.That((112, 123, 43), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 79, 24, 113, RoomControl.DIR_NW); // (116,121)(44)
            Assert.That((116, 121, 44), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 79, 24, 113, RoomControl.DIR_SW); // (38,63)(27)
            Assert.That((38, 63, 27), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 44, 124, 124, RoomControl.DIR_NE); // (56,81)(79)
            Assert.That((56, 81, 79), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 44, 124, 124, RoomControl.DIR_E); // (39,113)(79)
            Assert.That((39, 113, 79), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 44, 124, 124, RoomControl.DIR_SE); // (69,158)(79)
            Assert.That((69, 158, 79), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 2, 139, 69, RoomControl.DIR_N); // (139,60)(15)
            Assert.That((139, 60, 15), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 15, 139, 60, RoomControl.DIR_SW); // (117,120)(3)
            Assert.That((117, 120, 3), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 21, 67, 68, RoomControl.DIR_NW); // (27,150)(28)
            Assert.That((27, 150, 28), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 21, 61, 62, RoomControl.DIR_N); // (63,128)(28)
            Assert.That((63, 128, 28), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            // this one runs into the dragon cave. But why is it invalid?
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 113, 72, RoomControl.DIR_E); // INVALID(147,72)(22)
            Assert.That((147, 72, 22), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 133, 72, RoomControl.DIR_SW); // (56,71)(11)
            Assert.That((56, 71, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 37, 167, RoomControl.DIR_SE); // (67,71)(11)
            Assert.That((67, 71, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 38, 167, RoomControl.DIR_SE); // (61,62)(11)
            Assert.That((61, 62, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 39, 167, RoomControl.DIR_SE); // (61,62)(11)
            Assert.That((61, 62, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 43, 167, RoomControl.DIR_SE); // (61,59)(11)
            Assert.That((61, 59, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 42, 167, RoomControl.DIR_SE); // (61,59)(11)
            Assert.That((61, 59, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 46, 167, RoomControl.DIR_SE); // (64,59)(11)
            Assert.That((64, 59, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 108, 88, RoomControl.DIR_SW); // (136,71)(10)
            Assert.That((136, 71, 10), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 33, 153, 80, RoomControl.DIR_SE); // (0,82)(40)
            Assert.That((0, 82, 40), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 33, 153, 81, RoomControl.DIR_SE); // (1,82)(40)
            Assert.That((1, 82, 40), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 33, 153, 82, RoomControl.DIR_SE); // (2,82)(40)
            Assert.That((2, 82, 40), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 9, 19, 76, RoomControl.DIR_NE); // (117,60)(26)
            Assert.That((117, 60, 26), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 9, 51, 68, RoomControl.DIR_N); // (46,63)(24)
            Assert.That((46, 63, 24), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 9, 54, 68, RoomControl.DIR_N); // (60,64)(24)
            Assert.That((60, 64, 24), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 21, 8, 144, RoomControl.DIR_SE); // WATER(105,71)(5)
            Assert.That((105, 71, 5), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 21, 34, 148, RoomControl.DIR_S); // (30,110)(12)
            Assert.That((30, 110, 12), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 6, 47, 41, RoomControl.DIR_NE); // (38,152)(11)
            Assert.That((38, 152, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 6, 48, 41, RoomControl.DIR_NE); // (71,157)(11)
            Assert.That((71, 157, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 27, 167, RoomControl.DIR_S); // (10,71)(11)
            Assert.That((10, 71, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 36, 88, 131, RoomControl.DIR_S); // (90,81)(29)
            Assert.That((90, 81, 29), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 17, 70, 165, RoomControl.DIR_SW);  // (67,47)(16)
            Assert.That((67, 47, 16), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 18, 74, 124, RoomControl.DIR_SE); // (126,55)(15)
            Assert.That((126, 55, 15), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 30, 74, 124, RoomControl.DIR_SE); // (135,59)(19)
            Assert.That((135, 59, 19), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 20, 106, 154, RoomControl.DIR_NE); // (116,144)(20)
            Assert.That((116, 144, 20), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 14, 4, 49, RoomControl.DIR_NE);  // (84,96)(19)
            Assert.That((84, 96, 19), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            // Giant area
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 61, 148, 62, RoomControl.DIR_SE); // DEATH(90,159)(62)
            Assert.That((90, 159, 62), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 61, 137, 70, RoomControl.DIR_SE); // DEATH(71,159)(62)
            Assert.That((71, 159, 62), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 59, 122, 167, RoomControl.DIR_NW); // (0,31)(58)
            Assert.That((0, 31, 58), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 64, 138, 109, RoomControl.DIR_E); // (9,109)(57)
            Assert.That((9, 109, 57), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 59, 40, 78, RoomControl.DIR_SW); // (25,141)(61)
            Assert.That((25, 141, 61), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        /*
         * Finding the water positions from the game data is not as straight-forward as normal traversals
         * because the player will be repositioned when entering the water.
         * The logic for the water repositioning hasn't been worked out or attempted. The asserted values
         * must be found by hand by analysing the bitmpas. In the traversal W(0,95)(21) the player will enter
         * room 23 along y = 96. It can be seen from the control data that the player's base line (or footprint)
         * will cover water tiles at the (136,96) position
         *
         *
         */
        [Test]
        public void TestTraveralsWater()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 21, 0, 95, RoomControl.DIR_W); // WATER(136,96)(23)
            Assert.That((136, 96, 23), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestBoulderMoved()
        {
            GameState gameState1 = new();
            MapTraversal mapTraversalBoulderPresent = new(gameState: gameState1);
            GameState gameState2 = new();
            gameState2.SetBoulderMoved();
            MapTraversal mapTraversalBoulderMoved = new(gameState: gameState2);

            Traversal traversal = null;
            traversal = mapTraversalBoulderPresent.EvaluateMapTraversal(roomNr: 3, 110, 100, RoomControl.DIR_SE);
            Assert.That((130, 120, 3), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
            traversal = mapTraversalBoulderMoved.EvaluateMapTraversal(roomNr: 3, 110, 100, RoomControl.DIR_SE);
            Assert.That((5, 150, 2), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestTraversalsCloverRewrite()
        {
            GameState gameState1 = new();
            MapTraversal mapTraversalCloverPresent = new(gameState: gameState1);
            GameState gameState2 = new();
            gameState2.SetCloverAcquired();
            MapTraversal mapTraversalCloverAcquired = new(gameState: gameState2);
            Traversal traversal = null;
            // (46,63)(24)
            traversal = mapTraversalCloverPresent.EvaluateMapTraversal(roomNr: 9, 51, 70, RoomControl.DIR_N);
            Assert.That((46, 63, 24), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
            // (51,64)(24)
            traversal = mapTraversalCloverAcquired.EvaluateMapTraversal(roomNr: 9, 51, 70, RoomControl.DIR_N);
            Assert.That((51, 64, 24), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestTraversalsGoatPen()
        {
            GameState gameStateGoatPresent = new();
            MapTraversal mapTraversal = new(gameState: gameStateGoatPresent);
            Traversal traversal = null;
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 11, 167, RoomControl.DIR_S); // (10,71)(11)
            Assert.That((10, 71, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 35, 167, RoomControl.DIR_S); // (10,71)(11)
            Assert.That((10, 71, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 36, 167, RoomControl.DIR_S); // (61,71)(11)
            Assert.That((61, 71, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 36, 167, RoomControl.DIR_SE); // (66,71)(11)
            Assert.That((66, 71, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 60, 167, RoomControl.DIR_S); // (61,59)(11)
            Assert.That((61, 59, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 22, 42, 167, RoomControl.DIR_SE); // (61,59)(11)
            Assert.That((61, 59, 11), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 23, 95, 167, RoomControl.DIR_S); // (88,67)(10)
            Assert.That((88, 67, 10), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestEdgeRewritingRoom1()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 16, 125, 167, RoomControl.DIR_S); // (135,119)(1)
            Assert.That((135, 119, 1), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 16, 136, 156, RoomControl.DIR_SW); // (59,106)(1)
            Assert.That((59, 106, 1), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestEdgeRewritingRoom2()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 15, 122, 167, RoomControl.DIR_SW); // (131,69)(2)
            Assert.That((131, 69, 2), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestEdgeRewritingRoom3()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 14, 121, 167, RoomControl.DIR_SW); // (67,91)(3)
            Assert.That((67, 91, 3), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestEdgeRewritingRoom19()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 30, 115, 167, RoomControl.DIR_SW);
            Assert.That((111, 59, 19), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }

        [Test]
        public void TestEdgeRewritingRoom31()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;

            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 18, 121, 47, RoomControl.DIR_NE);
            Assert.That((126, 158, 31), Is.EqualTo((traversal.X(), traversal.Y(), traversal.RNr())));
        }
    }
}
