using static Kq1Pathing.RoomPathing.GameState;
using static Kq1Pathing.RoomPathing.MapTraversal;

namespace Kq1Pathing.RoomPathing
{
    class GlobalTraverser
    {
        private GameState currentGameState = null;
        private GameState nextGameState;
        private StateTraversal traverser;
        private int startRoomNr;
        private int startX;
        private int startY;
        public int pathLength = 0;
        public string pathDescription = "";

        public GlobalTraverser(GameState gameState, int startRoomNr, int startX, int startY)
        {
            this.nextGameState = gameState;
            traverser = new();
            this.startRoomNr = startRoomNr;
            this.startX = startX;
            this.startY = startY;
        }

        public GameState GetNextGameState()
        {
            return nextGameState;
        }

        public void SetGoalDagger()
        {
            pathDescription += "DAG ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            nextGameState.SetBoulderMoved();

            GoalState goalState = new GoalState();
            goalState.SetGoalRegion(goalRoomNr: 3, DAGGER_PICKUP_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalClover()
        {
            pathDescription += "CLV ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            nextGameState.SetCloverAcquired();

            GoalState goalState = new GoalState();
            goalState.SetGoalAquireObject(goalRoomNr: 24, CLOVER_PICKUP_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalNut()
        {
            pathDescription += "NUT ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();

            GoalState goalState = new GoalState();
            goalState.SetGoalRegion(goalRoomNr: 30, WALNUT_PICKUP_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalBag()
        {
            pathDescription += "BAG ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();

            GoalState goalState = new GoalState();
            goalState.SetGoalRegion(goalRoomNr: 6, BAG_PICKUP_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalGnome()
        {
            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            nextGameState.SetTrollBridgesOpen();

            GoalState goalState = new GoalState();
            goalState.SetGoalRegion(goalRoomNr: 40, GNOME_INTERACTION_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        // Either the key is picked up when in the room *and we don't need to update the state with a dropped key*
        public void SetGoalKey1()
        {
            pathDescription += "KEY1 ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            GoalState goalState = new GoalState();
            goalState.AddDisallowedRoom(33);
            goalState.AddDisallowedRoom(41);
            goalState.AddDisallowedRoom(39);
            goalState.AddDisallowedRoom(25);
            goalState.SetGoalDistance(40, KEY_INTERACTIVE_DISTANCE, KEY_POS.x, KEY_POS.y);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        // Or we leave the gnome room and come back for the key with an acquire goal

        public void SetGoalLeaveGnomeRoom()
        {
            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            nextGameState.SetKeyDropped();
            GoalState goalState = new GoalState();
            goalState.SetGoalNotInRoom(40);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        // MUST be preceeded by GoalLeaveGnomeRoom
        public void SetGoalKey2()
        {
            pathDescription += "KEY2 ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            nextGameState.SetKeyAcquired();
            GoalState goalState = new GoalState();
            goalState.SetGoalAquireObjectByDistance(40, KEY_INTERACTIVE_DISTANCE, KEY_POS.x, KEY_POS.y);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalMushroom()
        {
            pathDescription += "MSH ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            nextGameState.SetMushroomAcquired();
            GoalState goalState = new GoalState();
            goalState.SetGoalAquireObject(goalRoomNr: 47, MUSHROOM_PICKUP_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalShieldEntrance()
        {
            pathDescription += "SHD ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            GoalState goalState = new GoalState();
            goalState.SetGoalRoom(73);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalChest()
        {
            pathDescription += "CHS ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            nextGameState.SetStairwayDoorOpen();
            GoalState goalState = new GoalState();
            goalState.SetGoalRegion(goalRoomNr: 19, STAIRCASE_INTERACTION_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalMirror()
        {
            pathDescription += "MRR ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            GoalState goalState = new GoalState();
            goalState.SetGoalRegion(goalRoomNr: 12, WELL_INTERACTION_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void SetGoalKing()
        {
            pathDescription += "KNG ";

            currentGameState = nextGameState;
            nextGameState = currentGameState.CloneState();
            GoalState goalState = new GoalState();
            goalState.SetGoalRegion(goalRoomNr: 16, KING_JUMP_AREA);
            traverser.RegisterPathingComponents(currentGameState, goalState);
        }

        public void DeterminePathLength()
        {
            pathDescription = pathDescription.Trim();
            pathLength = traverser.FindPaths(startRoomNr, startX, startY).Last().pathLength;
        }

        public List<Traversal> FindPathSequence()
        {
            return traverser.FindPaths(startRoomNr, startX, startY);
        }
    }
}
