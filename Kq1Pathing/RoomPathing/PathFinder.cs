using System.Diagnostics;
using static Kq1Pathing.RoomPathing.MapTraversal;

namespace Kq1Pathing.RoomPathing
{
    class PathFinder
    {
        readonly MapTraversal mapTraversal;

        public PathFinder(GameState? gameState = null, int footprintWidth = 6)
        {
            if (gameState == null)
            {
                gameState = new GameState(footprintWidth);
            }
            mapTraversal = new MapTraversal(gameState: gameState);
        }

        int goal = GOAL_UNDEFINED;
        const int GOAL_UNDEFINED = 0;
        const int GOAL_REGION = 1;
        const int GOAL_ROOM = 2;
        const int GOAL_AQUIRE_OBJECT = 3;
        const int GOAL_DISTANCE = 4;
        const int GOAL_NOT_IN_ROOM = 5;
        // int[] goalRegion = new int[4];
        List<int[]> goalRegions = new();
        Dictionary<int, bool> goalRooms = new();
        int goalRoom = -1;
        int goalX;
        int goalY;
        int goalDistance;
        // corresponding to the room that contains the object
        // (the object region is defined in RoomDefinition)

        bool[] disallowFirstDirection = new bool[9];
        List<int[]> disallowedRegions = new();

        public void AddDisallowPoint(int roomNr, int x, int y)
        {
            disallowedRegions.Add(new int[] { roomNr, x, y, x, y });
        }
        public void AddDisallowRegion(int roomNr, int x0, int y0, int x1, int y1)
        {
            disallowedRegions.Add(new int[] { roomNr, x0, y0, x1, y1 });
        }

        public void SetGoalTargetPosition(int roomNr, int targetX, int targetY)
        {
            goal = GOAL_REGION;
            SetGoalRegion(roomNr, targetX, targetY, targetX, targetY);
        }
        // assumes same room number
        public void AddTargetPosition(int x0, int y0)
        {
            AddGoalRegion(x0, y0, x0, y0);
        }

        public void SetGoalRegion(int roomNr, int[] rect)
        {
            goal = GOAL_REGION;
            this.goalRegions.Add(rect);
            this.goalRoom = roomNr;
        }

        public void SetGoalRegion(int roomNr, int x0, int y0, int x1, int y1)
        {
            goal = GOAL_REGION;
            this.goalRegions.Add(new int[] { x0, y0, x1, y1 });
            this.goalRoom = roomNr;
        }
        // assumes same room
        public void AddGoalRegion(int x0, int y0, int x1, int y1)
        {
            this.goalRegions.Add(new int[] { x0, y0, x1, y1 });
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

        public void SetGoalAquireObject(int goalObjectRoomNumber)
        {
            goal = GOAL_AQUIRE_OBJECT;
            this.goalRoom = goalObjectRoomNumber;
        }
        public void SetGoalDistance(int goalRoomNumber, int distance, int objX, int objY)
        {
            goal = GOAL_DISTANCE;
            this.goalRoom = goalRoomNumber;
            this.goalDistance = distance;
            this.goalX = objX;
            this.goalY = objY;
        }

        public void DisallowFirstDirection(int DIR)
        {
            disallowFirstDirection[DIR] = true;
        }

        Dictionary<(int, int, int), int> coordinateSeen = new();
        private bool CoordinateSeen((int, int, int) P)
        {
            return coordinateSeen.ContainsKey(P) && coordinateSeen[P] > 1;
        }

        private void AddCoordinate((int, int, int) P)
        {
            if (coordinateSeen.ContainsKey(P))
            {
                coordinateSeen[P]++;
            }
            else
            {
                coordinateSeen.Add(P, 1);
            }
        }

        private void RemoveCoordinate((int, int, int) P)
        {
            coordinateSeen[P]--;
        }

        private Traversal GetNexTraversalAction()
        {
            Traversal newTraversal = availableActions.Peek().Pop();
            AddCoordinate((newTraversal.RNr(), newTraversal.X(), newTraversal.Y()));
            return newTraversal;
        }

        private void RemoveLastPathAction()
        {
            Traversal lastPath = currentPath.Last();
            RemoveCoordinate((lastPath.RNr(), lastPath.X(), lastPath.Y()));
            currentPath.RemoveAt(currentPath.Count - 1);
        }

        // int MAXDEPTH;
        private Stack<Stack<Traversal>> availableActions;
        private List<Traversal> currentPath;

        public void FindPathsFrom(int roomNr, int x, int y, int MAXDEPTH = 10, bool stopPathOnSolution = true)
        {
            // this.MAXDEPTH = MAXDEPTH;
            if (goal == GOAL_UNDEFINED)
            {
                throw new Exception("path finding goal has not been set");
            }

            availableActions = new();
            // represent the starting point (first action) as an empty stack
            availableActions.Push(new Stack<Traversal>());
            int pathLength = 0;
            currentPath = new();
            currentPath.Add(new Traversal(-1, roomNr, x, y, RoomControl.STOP));
            bool pathValid = true;
            bool isSolution = false;

            AddCoordinate((roomNr, x, y));

            while (availableActions.Count > 0)
            {
                // Debug.WriteLine($"{pathValid && stopPathOnSolution && !isSolution}");

                // we shouldn't necessarily stop searching even if the goal is reached
                // (because extra steps for a positional advantage may be justified)
                Stack<Traversal>? pathActions =
                    pathValid && (!stopPathOnSolution || !isSolution) &&
                    pathLength < MAXDEPTH ? mapTraversal.GetPathActions(currentPath.Last()) : null;

                // Debug.WriteLine($"{pathActions.Count}");

                if (pathActions != null && pathActions.Count > 0)
                {
                    availableActions.Push(pathActions);
                    Traversal newTraversal = GetNexTraversalAction();
                    currentPath.Add(newTraversal);
                    pathLength++;
                }
                else
                {
                    // remove explored path element (depleted stacks) and increment path
                    while (pathLength >= 0 && availableActions.Peek().Count == 0)
                    {
                        availableActions.Pop();
                        RemoveLastPathAction();
                        pathLength--;
                    }
                    if (pathLength >= 0)
                    {
                        RemoveLastPathAction();
                        currentPath.Add(GetNexTraversalAction());
                    }
                }

                if (pathLength > 0)
                {
                    Traversal t = currentPath.Last();
                    pathValid = !CoordinateSeen((t.RNr(), t.X(), t.Y()));
                    isSolution = CheckSolution(t.RNr(), t.X(), t.Y());

                    if (pathLength == 1 && disallowFirstDirection[currentPath.Last().direction])
                    {
                        pathValid = false;
                        isSolution = false;
                        // Debug.WriteLine($"disallow {GetPathString(currentPath)}");
                    }

                    if (PositionDisallowed(t.RNr(), t.X(), t.Y()))
                    {
                        // Debug.WriteLine($"position disallowed {t.RNr()} {t.X()},{t.Y()}");
                        pathValid = false;
                        isSolution = false;
                    }

                    if (isSolution)
                    {
                        // Debug.WriteLine($"LEN({pathLength}) {GetPathString(currentPath)}");

                        SaveSolution(currentPath);
                    }

                    // if (pathValid || !pathValid)
                    if (pathValid)
                    {
                        pathCount++;
                    }
                    else
                    {
                        // Debug.WriteLine($"INVALID {GetPathString(currentPath)}");
                        // Debug.WriteLine($"{GetPathString(currentPath)}");
                    }
                }

                // pathways are pruned on (pathValid == false), set it on to see all pathways generated
                // pathValid = true;
            }
            // Debug.WriteLine($"{pathCount}");
            // PrintSolutions(omitPositionsSeen: true);
        }
        int pathCount = 0;

        public void ShowSolutions(bool omitPositionsSeen)
        {
            PrintSolutions(omitPositionsSeen);
        }

        private bool PositionDisallowed(int roomNr, int x, int y)
        {
            foreach (var r in disallowedRegions)
            {
                if (r[0] == roomNr && RoomControl.CheckPixelContained(x, y, r[1], r[2], r[3], r[4]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckSolution(int roomNr, int x, int y)
        {
            switch (goal)
            {
                case GOAL_REGION:
                    if (roomNr != goalRoom)
                    {
                        return false;
                    }
                    foreach (var rect in goalRegions)
                    {
                        if (RoomControl.CheckPixelContained(x, y, rect))
                        {
                            return true;
                        }
                    }
                    return false;

                case GOAL_ROOM:
                    return goalRooms.ContainsKey(roomNr);

                case GOAL_NOT_IN_ROOM:
                    return !goalRooms.ContainsKey(roomNr);

                case GOAL_DISTANCE:
                    return roomNr == goalRoom && RoomControl.IsWithinDistance(goalDistance, goalX, goalY, x, y);

                default:
                    throw new Exception("unknown goal");
            }
        }

        List<SolutionPath> solutions = new();
        // List<Traversal[]> solutions = new();

        private void SaveSolution(List<Traversal> pathway)
        {
            solutions.Add(new SolutionPath(pathway));
        }

        private void PrintSolutions(bool omitPositionsSeen)
        {
            solutions.Sort();
            if (solutions.Count == 0)
            {
                Debug.WriteLine($"no paths were found");
            }
            coordinateSeen = new();
            foreach (var solutionPath in solutions)
            {
                (int, int, int) P = (solutionPath.RNr(), solutionPath.X(), solutionPath.Y());
                AddCoordinate(P);
                if (!omitPositionsSeen || coordinateSeen[P] == 1)
                {
                    List<Traversal> path = solutionPath.path;
                    String lengthToken = $"LEN({path.Count - 1})";
                    Debug.WriteLine($"{lengthToken,7} {GetPathString(path)}");
                }
            }
        }

        public static bool HasDuplicates(List<Traversal> path)
        {
            Dictionary<(int, int, int), bool> coordinateSeen = new();
            foreach (var t in path)
            {
                if (coordinateSeen.ContainsKey((t.RNr(), t.X(), t.Y())))
                {
                    return false;
                }
                coordinateSeen.Add((t.RNr(), t.X(), t.Y()), true);
            }
            return true;
        }

        private String GetPathString(List<Traversal> path)
        {
            Traversal first = path[0];
            String positionToken = $"({first.sourceX},{first.sourceY})({first.sourceRoomNr})";
            ;
            String reportString = $"{positionToken,13}";
            for (int i = 1; i < path.Count; i++)
            {
                Traversal t = path[i];
                // String dirToken = $"({RoomControl.directionNames[t.DIR]})";
                // positionToken = $"{t.X(),3},{t.Y(),-1}({t.RNr()})";
                // reportString += $" {dirToken,4}{positionToken,13}";
                // reportString += $" {RoomControl.directionNames[t.DIR],2}{positionToken,12}";

                String roomToken = $"({t.RNr()})";
                reportString += $" {RoomControl.directionNames[t.direction],2} {t.X(),3},{t.Y(),-3}{roomToken,4}";
            }

            for (int i = 1; i < path.Count; i++)
            {
                Traversal t = path[i];
                reportString += $" {RoomControl.directionNames[t.direction],2}";
            }

            return reportString;
        }

        class SolutionPath : IComparable
        {
            public List<Traversal> path = new();
            public SolutionPath(List<Traversal> path)
            {
                this.path.AddRange(path.GetRange(0, path.Count()));
            }

            public int RNr()
            {
                return path.Last().RNr();
            }
            public int X()
            {
                return path.Last().X();
            }
            public int Y()
            {
                return path.Last().Y();
            }

            public int CompareTo(object? obj)
            {
                SolutionPath o2 = (SolutionPath) obj;
                if (this.path.Count != o2.path.Count)
                {
                    return this.path.Count - o2.path.Count;
                }
                for (int i = 1; i < path.Count; i++)
                {
                    if (this.path[i].direction != o2.path[i].direction)
                    {
                        return this.path[i].direction - o2.path[i].direction;
                    }
                }
                return 0;
            }
        }
    }
}
