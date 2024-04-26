using static Kq1Pathing.RoomPathing.MapTraversal;

namespace Kq1Pathing.RoomPathing
{
    class StateTraversal
    {
        public StateTraversal() { }

        int finalGameState = 0;
        public void RegisterPathingComponents(GameState gameState, GoalState goalState)
        {
            int mapStateId = finalGameState++;
            MapTraversal mapTraversal = new MapTraversal(mapStateId, gameState, goalState);
            mapTraversers.Add(mapTraversal);
        }

        public Stack<Traversal> GetPathActions(Traversal t)
        {
            Stack<Traversal> nextPathComonents = mapTraversers[t.mapStateId].GetPathActions(t);
            foreach (Traversal tNext in nextPathComonents)
            {
                tNext.pathLength = tNext.action == MapTraversal.AQ ? t.pathLength : t.pathLength + 1;
            }
            return nextPathComonents;
        }

        private List<MapTraversal> mapTraversers = new();
        public SortedList<long, Traversal> pathComponents = new();

        private Dictionary<(int mapId, int roomNr, int x, int y), Traversal> pathLookup = new();

        private Traversal LookupPathComponent(int mapId, int roomNr, int x, int y)
        {
            if (pathLookup.ContainsKey((mapId, roomNr, x, y)))
            {
                return pathLookup[(mapId, roomNr, x, y)];
            }
            return null;
        }

        private void UpdateLookup(Traversal t)
        {
            Traversal t2 = LookupPathComponent(t.mapStateId, t.RNr(), t.X(), t.Y());
            if (t2 == null || t2.pathLength > t.pathLength)
            {
                pathLookup[(t.mapStateId, t.RNr(), t.X(), t.Y())] = t;
                pathComponents.Add(pathComponentOrderingIndex(t), t);
            }
        }

        private Traversal GetShortestPath()
        {
            Traversal t = pathComponents.First().Value;
            long key = pathComponents.First().Key;
            pathComponents.Remove(key);
            return t;
        }

        private const long F1 = 10000000000;
        private const long F2 = 2500000;
        private const long F3 = 30000;

        private long pathComponentOrderingIndex(Traversal t)
        {
            long ordinal = RoomControl.GetOrdinal(t.X(), t.Y());
            return t.pathLength * F1 + t.mapStateId * F2 + t.RNr() * F3 + ordinal;
        }

        public List<Traversal> FindPaths(int startRoomNr, int startX, int startY)
        {
            Traversal t = new Traversal(0, startRoomNr, startX, startY, MapTraversal.STOP, MapTraversal.STOP);
            t.pathLength = 0;
            UpdateLookup(t);

            int observedPathLen = 0;
            int nextPathLen = 0;
            int observedMapState = 0;
            int nextMapState = 0;

            // Traversal tShortest = null;

            /*
            // Traversal tShortest = GetShortestPath();
            // while (tShortest.mapStateId < finalGameState)
            while (pathComponents.Count > 0)
            {
                tShortest = GetShortestPath();

                if (tShortest.RNr() == 78)
                {
                    Debug.WriteLine($"{tShortest}");
                }

                if (tShortest.mapStateId == finalGameState)
                {
                    break;
                }

                // Debug.WriteLine($"{tShortest}");
                foreach (Traversal tNext in GetPathActions(tShortest))
                {
                    UpdateLookup(tNext);
                }

                //observedPathLen = tShortest.pathLength;
                //if (observedPathLen != nextPathLen)
                //{
                //    Debug.WriteLine($"{observedPathLen}");
                //    nextPathLen = observedPathLen;
                //}

                //observedMapState = tShortest.mapStateId;
                //if (observedMapState > nextMapState)
                //{
                //    Debug.WriteLine($"map state = {observedMapState}");
                //    nextMapState = observedMapState;
                //}
            }

            // Debug.WriteLine($"{tShortest}");
            // CollectResults(tShortest);

            if (tShortest.mapStateId < finalGameState)
            {
                Debug.WriteLine($"couldn't find a solution");
                Traversal t0 = pathLookup[(0,77,134,129)];
                Traversal t1 = pathLookup[(1,77,134,129)];
                Debug.WriteLine($"{t0}   path-length={t0.pathLength}");
                Debug.WriteLine($"{t1}   path-length={t1.pathLength}");
            }
            */

            Traversal tShortest = GetShortestPath();
            while (tShortest.mapStateId < finalGameState)
            {
                foreach (Traversal tNext in GetPathActions(tShortest))
                {
                    UpdateLookup(tNext);
                }
                tShortest = GetShortestPath();
            }

            return CollectResults(tShortest, startRoomNr, startX, startY);
        }

        public List<Traversal> CollectResults(Traversal tPrev, int startRoomNr, int startX, int startY)
        {
            Stack<Traversal> pathCollected = new();
            pathCollected.Push(tPrev);
            // Debug.WriteLine($"{tPrev}");

            while (true)
            {
                // Debug.WriteLine($"{tPrev}");
                if (tPrev.sourceRoomNr == startRoomNr && tPrev.sourceX == startX && tPrev.sourceY == startY)
                {
                    break;
                }
                tPrev = pathLookup[(tPrev.mapStatePrev, tPrev.sourceRoomNr, tPrev.sourceX, tPrev.sourceY)];
                pathCollected.Push(tPrev);
            }

            if (pathCollected.Count > 1)
            {
                tPrev = pathLookup[(tPrev.mapStatePrev, tPrev.sourceRoomNr, tPrev.sourceX, tPrev.sourceY)];
                pathCollected.Push(tPrev);
            }

            List<Traversal> pathSequenceList = new();
            while (pathCollected.Count > 0)
            {
                Traversal t = pathCollected.Pop();
                pathSequenceList.Add(t);
            }
            return pathSequenceList;
        }
    }
}
