namespace Kq1Pathing.RoomPathing
{
    public class GoalState
    {
        private const int GOAL_UNDEFINED = 0;
        private const int GOAL_REGION = 1;
        private const int GOAL_ROOM = 2;
        private const int GOAL_ACQUIRE_OBJECT = 3;
        private const int GOAL_ACQUIRE_OBJECT_BY_DISTANCE = 4;
        private const int GOAL_DISTANCE = 5;
        private const int GOAL_NOT_IN_ROOM = 6;

        private int goal = GOAL_UNDEFINED;
        private List<int[]> goalRegions = new();
        private Dictionary<int, bool> goalRooms = new();
        public int goalRoomNr = -1;
        private int goalX;
        private int goalY;
        private int goalDistance;
        private bool[] disallowedRooms = new bool[80];

        public void SetGoalTargetPosition(int goalRoomNr, int targetX, int targetY)
        {
            goal = GOAL_REGION;
            SetGoalRegion(goalRoomNr, new int[] { targetX, targetY, targetX, targetY });
        }
        // assumes same room number
        public void AddTargetPosition(int x0, int y0)
        {
            AddGoalRegion(new int[] { x0, y0, x0, y0 });
        }

        public void SetGoalRegion(int goalRoomNr, int[] rect)
        {
            goal = GOAL_REGION;
            this.goalRegions.Add(rect);
            this.goalRoomNr = goalRoomNr;
        }

        // assumes same room
        public void AddGoalRegion(int[] rect)
        {
            this.goalRegions.Add(rect);
        }

        public void SetGoalRoom(int goalRoom)
        {
            goal = GOAL_ROOM;
            // this.goalRoom = goalRoom;
            goalRooms.Add(goalRoom, true);
        }
        public void SetGoalNotInRoom(int goalRoom)
        {
            goal = GOAL_NOT_IN_ROOM;
            goalRooms.Add(goalRoom, true);
        }

        public void AddGoalRoom(int goalRoom)
        {
            goalRooms.Add(goalRoom, true);
        }

        public void SetGoalAquireObject(int goalRoomNr, int[] objectRegion)
        {
            goal = GOAL_ACQUIRE_OBJECT;
            this.goalRoomNr = goalRoomNr;
            goalRegions.Add(objectRegion);
        }

        public void SetGoalAquireObjectByDistance(int goalRoomNr, int interactionDistance, int objX, int objY)
        {
            goal = GOAL_ACQUIRE_OBJECT_BY_DISTANCE;
            this.goalRoomNr = goalRoomNr;
            this.goalDistance = interactionDistance;
            this.goalX = objX;
            this.goalY = objY;
        }

        public void SetGoalDistance(int goalRoomNr, int interactionDistance, int objX, int objY)
        {
            goal = GOAL_DISTANCE;
            this.goalRoomNr = goalRoomNr;
            this.goalDistance = interactionDistance;
            this.goalX = objX;
            this.goalY = objY;
        }

        private bool IsInsideGoalRegion(int roomNr, int x, int y)
        {
            if (roomNr != goalRoomNr)
            {
                return false;
            }
            foreach (int[] rect in goalRegions)
            {
                if (RoomControl.CheckPixelContained(x, y, rect))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsGoalAcquireObject()
        {
            return goal == GOAL_ACQUIRE_OBJECT || goal == GOAL_ACQUIRE_OBJECT_BY_DISTANCE;
        }

        public void AddDisallowedRoom(int roomNr)
        {
            disallowedRooms[roomNr] = true;
        }

        public bool RoomIsDisallowed(int roomNr) {
            return disallowedRooms[roomNr];
        }

        public bool GoalReached(int roomNr, int x, int y)
        {
            switch (goal)
            {
                case GOAL_UNDEFINED:
                    return false;

                case GOAL_ACQUIRE_OBJECT:
                    return roomNr == goalRoomNr && IsInsideGoalRegion(roomNr, x, y);

                case GOAL_ACQUIRE_OBJECT_BY_DISTANCE:
                    return roomNr == goalRoomNr && RoomControl.IsWithinDistance(goalDistance, goalX, goalY, x, y);

                case GOAL_REGION:
                    return roomNr == goalRoomNr && IsInsideGoalRegion(roomNr, x, y);

                case GOAL_ROOM:
                    return goalRooms.ContainsKey(roomNr);

                case GOAL_NOT_IN_ROOM:
                    return !goalRooms.ContainsKey(roomNr);

                case GOAL_DISTANCE:
                    return roomNr == goalRoomNr && RoomControl.IsWithinDistance(goalDistance, goalX, goalY, x, y);

                default:
                    throw new Exception("Unknown goal");
            }
        }
    }
}
