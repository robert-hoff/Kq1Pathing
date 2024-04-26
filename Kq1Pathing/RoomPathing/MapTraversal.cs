using System.Diagnostics;

namespace Kq1Pathing.RoomPathing
{
    public class MapTraversal
    {
        public const int DIR_N = RoomControl.DIR_N;
        public const int DIR_NE = RoomControl.DIR_NE;
        public const int DIR_E = RoomControl.DIR_E;
        public const int DIR_SE = RoomControl.DIR_SE;
        public const int DIR_S = RoomControl.DIR_S;
        public const int DIR_SW = RoomControl.DIR_SW;
        public const int DIR_W = RoomControl.DIR_W;
        public const int DIR_NW = RoomControl.DIR_NW;
        public const int STOP = RoomControl.STOP;
        public const int AQ = RoomControl.AQ; // 10
        public const int STAQ = AQ + 1;
        public const int NAQ = AQ + 2;
        public const int NEAQ = AQ + 3;
        public const int EAQ = AQ + 4;
        public const int SEAQ = AQ + 5;
        public const int SAQ = AQ + 6;
        public const int SWAQ = AQ + 7;
        public const int WAQ = AQ + 8;
        public const int NWAQ = AQ + 9;

        public readonly static string[] actionNames = {"", "N", "NE", "E", "SE", "S", "SW", "W", "NW", "ST", "AQ", "STAQ",
                                        "NAQ", "NEAQ", "EAQ", "SEAQ", "SAQ", "SWAQ", "WAQ", "NWAQ"};

        public readonly static int[] directions = { -1, DIR_N, DIR_NE, DIR_E, DIR_SE, DIR_S, DIR_SW, DIR_W, DIR_NW,
                STOP, -1, STOP, DIR_N, DIR_NE, DIR_E, DIR_SE, DIR_S, DIR_SW, DIR_W, DIR_NW };

        private int prevDirection = STOP;
        private int mapStateId;
        private readonly GameState gameState;
        private readonly GoalState goalState;
        public readonly RoomControl[] roomControl = new RoomControl[80];
        private readonly RoomDefinition[] roomDefinitions = new RoomDefinition[80];

        public MapTraversal(GameState gameState, GoalState goalState) : this(0, gameState, goalState)
        { }

        public MapTraversal(int mapStateId = 0, GameState gameState = null, GoalState goalState = null)
        {
            this.mapStateId = mapStateId;
            this.gameState = gameState == null ? new GameState() : gameState;
            this.goalState = goalState == null ? new GoalState() : goalState;
        }

        public RoomControl GetRoomControl(int roomNr)
        {
            if (roomControl[roomNr] == null)
            {
                RoomControl rc = new(roomNr, gameState);
                roomControl[roomNr] = rc;
            }
            return roomControl[roomNr];
        }

        private RoomDefinition GetRoomDefinition(int roomNr)
        {
            if (roomDefinitions[roomNr] == null)
            {
                RoomDefinition rd = RoomDefinition.GetRoomDefinition(roomNr, gameState);
                roomDefinitions[roomNr] = rd;
            }
            return roomDefinitions[roomNr];
        }

        public Stack<Traversal> GetPathActions(Traversal traversal)
        {
            // Debug.WriteLine($"{traversal}");
            return GetPathActions(traversal.RNr(), traversal.X(), traversal.Y(), traversal.direction);
        }

        public Stack<Traversal> GetPathActions(int roomNr, int x, int y, int useDirection = -1)
        {
            int DIR_AQ = useDirection > 0 ? useDirection : prevDirection;
            Stack<Traversal> pathActions = new Stack<Traversal>();
            Stack<int> candidateActions = new Stack<int>();

            for (int i = DIR_NW; i >= DIR_N; i--)
            {
                Traversal traversal = EvaluateMapTraversal(roomNr, x, y, i);

                // Debug.WriteLine($"{traversal.Evaluation()}");

                if (traversal.Evaluation() != RoomControl.TRAVERSABLE && traversal.Evaluation() != EVAL_STOP_NEW_ROOM)
                {
                    continue;
                }
                if (!(traversal.RNr() == roomNr && traversal.X() == x && traversal.Y() == y))
                {
                    pathActions.Push(traversal);
                }
                else if (traversal.direction != DIR_AQ)
                {
                    // here I'm trying to match the traversal direction with the DIR-AQ action
                    // additing STAQ completely depends on the ordering these constants have been defined in
                    candidateActions.Push(traversal.direction + STAQ); // <-- This is pretty horrible
                }
            }

            if (goalState.goalRoomNr == 3 && goalState.GoalReached(roomNr, x, y))
            {
                Traversal traversalStop = EvaluateMapTraversal(roomNr, x, y, STOP);
                traversalStop.mapStateId = mapStateId + 1;
                pathActions.Push(traversalStop);
                return pathActions;
            }

            if (!goalState.IsGoalAcquireObject() || (goalState.IsGoalAcquireObject() && !goalState.GoalReached(roomNr, x, y)))
            {
                return pathActions;
            }
            // -- extra stack is to build a nice ordering
            Stack<Traversal> AQActions = new Stack<Traversal>();
            Traversal traversalAQ = EvaluateMapTraversal(roomNr, x, y, AQ, DIR_AQ);
            bool hasAcqAction = traversalAQ.Evaluation() == 0;
            if (hasAcqAction)
            {
                traversalAQ.mapStateId++;
                pathActions.Push(traversalAQ);
            }
            if (!hasAcqAction || traversalAQ.X() != x || traversalAQ.Y() != y)
            {
                Traversal traversalSTAQ = EvaluateMapTraversal(roomNr, x, y, STAQ);
                traversalSTAQ.mapStateId++;
                pathActions.Push(traversalSTAQ);
            }
            while (candidateActions.Count > 0)
            {
                int action = candidateActions.Pop();
                Traversal traversal = EvaluateMapTraversal(roomNr, x, y, action);
                traversal.mapStateId++;
                if (traversal.Evaluation() == 0 && (traversal.X() != x || traversal.Y() != y))
                {
                    AQActions.Push(traversal);
                }
            }
            foreach (Traversal dirActions in pathActions)
            {
                AQActions.Push(dirActions);
            }
            return AQActions;
        }

        public Traversal EvaluateMapTraversal(int roomNr, int x, int y, int action, int useDirection = -1)
        {
            int playerDir = directions[action];
            if (useDirection > 0)
            {
                playerDir = useDirection;
            }
            else if (playerDir < 0)
            {
                playerDir = prevDirection;
            }
            prevDirection = playerDir;
            Traversal traversal = new(mapStateId, roomNr, x, y, playerDir, action);

            //if (traversal.mapStateId == 1 && traversal.RNr() == 6 && traversal.X() < 35)
            //{
            //    Debug.WriteLine($"{traversal}");
            //};

            ResolveTraversal(traversal);

            // For the specific case of the traversal being room3 we want to also add the path-action STOP
            // because the player will be resting against the boulder and must perform this action to complete
            // this game stage's goal
            if (goalState.goalRoomNr == 3)
            {
                return traversal;
            }

            if (goalState.IsGoalAcquireObject())
            {
                return traversal;
            }
            if (goalState.GoalReached(traversal.RNr(), traversal.X(), traversal.Y()))
            {
                traversal.mapStateId++;
            }

            return traversal;
        }

        private void ResolveTraversal(Traversal traversal)
        {
            while (!traversal.EvaluationComplete())
            {
                int evaluation = traversal.Evaluation();
                switch (evaluation)
                {
                    case EVAL_START:
                    {
                        Step nextStep = EvaluateRoomTraversal(
                            traversal.sourceRoomNr, traversal.sourceX, traversal.sourceY,
                            traversal.direction, observeObjects: !(traversal.action >= AQ));
                        traversal.AddStep(nextStep);
                        break;
                    }

                    case EVAL_ENTRY_NEW_ROOM:
                    {
                        Step nextStep = EvaluateRoomTraversal(traversal.RNr(), traversal.X(), traversal.Y(), traversal.direction);
                        traversal.AddStep(nextStep);
                        break;
                    }

                    case RoomControl.EVAL_EXIT_NORTH:
                    case RoomControl.EVAL_EXIT_EAST:
                    case RoomControl.EVAL_EXIT_WEST:
                    case RoomControl.EVAL_EXIT_SOUTH:
                    {
                        Step nextStep = EvaluateEdgeTransition(
                            evaluation, traversal.RNr(), traversal.X(), traversal.Y(), traversal.direction);
                        traversal.AddStep(nextStep);
                        break;
                    }

                    case RoomControl.EVAL_ROOMTRIGGER:
                    case RoomControl.EVAL_ROOMACTION_AREA1:
                    case RoomControl.EVAL_ROOMACTION_AREA2:
                    {
                        Step nextStep = EvaluateRoomTriggerTransition(
                            evaluation, traversal.RNr(), traversal.X(), traversal.Y(), traversal.direction);
                        traversal.AddStep(nextStep);
                        break;
                    }

                    default:
                        throw new Exception($"Unknown step evaluation = {traversal.Evaluation()} " +
                            $"(or endpoint reached)");
                }

                // indicate room disabled in the Step if this is configured
                if (goalState.RoomIsDisallowed(traversal.RNr()))
                {
                    traversal.SetRoomDisabled();
                }

                if (traversal.traversalSteps.Count == MAXSTEPS_PERMITTED)
                {
                    traversal.AddStep(EVAL_MAXIMUM_STEPS_REACHED, 0, 0, 0);
                }
            }
        }

        private Step EvaluateRoomTraversal(int roomNr, int x, int y, int direction, bool observeObjects = true)
        {
            RoomControl roomControl = GetRoomControl(roomNr);

            //Debug.WriteLine($"roomNr={roomNr} ({x},{y})");

            (int ev, int x, int y) result = roomControl.EvaluateTraversal(x, y, direction, observeObjects);
            return new Step(result.ev, result.x, result.y, roomNr);
        }

        private Step EvaluateEdgeTransition(int evaluation, int previousRoomNr, int x, int y, int direction)
        {
            RoomControl roomControl = GetRoomControl(previousRoomNr);
            int nextRoomNumber = roomControl.GetAdjacentRoomNr(evaluation);
            RoomControl nextRoomControl = GetRoomControl(nextRoomNumber);
            (int x, int y, bool stop) entryP = EvaluateRoomEntry(nextRoomControl, evaluation, previousRoomNr, x, y, direction);
            return entryP.stop ?
                new Step(EVAL_STOP_NEW_ROOM, entryP.x, entryP.y, nextRoomNumber) :
                new Step(EVAL_ENTRY_NEW_ROOM, entryP.x, entryP.y, nextRoomNumber);
        }

        private (int, int, bool) EvaluateRoomEntry(RoomControl nextRoomControl,
            int evaluation, int previousRoomNr, int x, int y, int direction)
        {
            int dx = RoomControl.dxdy[direction, 0];
            int dy = RoomControl.dxdy[direction, 1];
            switch (evaluation)
            {
                case RoomControl.EVAL_EXIT_NORTH:
                {
                    int xEnter = x + dx;
                    int yEnter = RoomDefinition.ROOM_HEIGHT;
                    return nextRoomControl.RoomEntryEdgeTransition(previousRoomNr, xEnter, yEnter);
                }
                case RoomControl.EVAL_EXIT_SOUTH:
                {
                    int xEnter = x + dx;
                    int yEnter = 0;
                    return nextRoomControl.RoomEntryEdgeTransition(previousRoomNr, xEnter, yEnter);
                }
                case RoomControl.EVAL_EXIT_EAST:
                {
                    int xEnter = 0;
                    int yEnter = y + dy;
                    return nextRoomControl.RoomEntryEdgeTransition(previousRoomNr, xEnter, yEnter);
                }
                case RoomControl.EVAL_EXIT_WEST:
                {
                    int xEnter = RoomDefinition.ROOM_WIDTH;
                    int yEnter = y + dy;
                    return nextRoomControl.RoomEntryEdgeTransition(previousRoomNr, xEnter, yEnter);
                }
                default:
                    throw new Exception("this shouldn't happen!");
            }
        }

        private Step EvaluateRoomTriggerTransition(int evaluation, int previousRoomNr, int x, int y, int direction)
        {
            RoomControl roomControl = GetRoomControl(previousRoomNr);
            int nextRoomNumber = roomControl.GetRoomTriggered(evaluation);
            if (nextRoomNumber < 0)
            {
                return new Step(EVAL_ROOM_DISABLED, x, y, previousRoomNr);
            }

            RoomControl nextRoomControl = GetRoomControl(nextRoomNumber);
            (int x, int y, bool stop) entryP = nextRoomControl.RoomTriggerTransition(previousRoomNr, x, y);
            return entryP.stop ?
                new Step(EVAL_STOP_NEW_ROOM, entryP.x, entryP.y, nextRoomNumber) :
                new Step(EVAL_ENTRY_NEW_ROOM, entryP.x, entryP.y, nextRoomNumber);
        }

        public const int MAXSTEPS_PERMITTED = 16;
        public const int EVAL_START = -19;
        public const int EVAL_ENTRY_NEW_ROOM = -20;
        public const int EVAL_STOP_NEW_ROOM = -21;
        public const int EVAL_ROOM_DISABLED = -22;
        public const int EVAL_MAXIMUM_STEPS_REACHED = -23;

        public static string EvaluationLabel(int evaluation)
        {
            switch (evaluation)
            {
                case RoomControl.EVAL_DEATH:
                    return "DEATH";

                case RoomControl.EVAL_WATER:
                    return "WATER";

                case EVAL_ROOM_DISABLED:
                    return "INVALID";

                case RoomControl.EVAL_ROOMTRIGGER:
                case RoomControl.EVAL_ROOMACTION_AREA1:
                case RoomControl.EVAL_ROOMACTION_AREA2:
                    return "ROOM";

                case RoomControl.EVAL_EXIT_NORTH:
                    return "EXN";

                case RoomControl.EVAL_EXIT_EAST:
                    return "EXE";

                case RoomControl.EVAL_EXIT_SOUTH:
                    return "EXS";

                case RoomControl.EVAL_EXIT_WEST:
                    return "EXW";

                case RoomControl.TRAVERSABLE:
                    return "";

                case EVAL_ENTRY_NEW_ROOM:
                    return "IN";

                case EVAL_STOP_NEW_ROOM:
                    return "STOP";

                case EVAL_MAXIMUM_STEPS_REACHED:
                    Debug.WriteLine($"Maximum steps reached");
                    return "MAX";

                default:
                    return $"({evaluation})";
            }
        }

        public static bool EvaluationIsEndpoint(int evaluation)
        {
            return
                evaluation == RoomControl.TRAVERSABLE ||
                evaluation == EVAL_STOP_NEW_ROOM ||
                evaluation == RoomControl.EVAL_DEATH ||
                evaluation == RoomControl.EVAL_WATER ||
                evaluation == EVAL_ROOM_DISABLED ||
                evaluation == EVAL_MAXIMUM_STEPS_REACHED;
        }

        public class Traversal
        {
            public int mapStateId;
            public int mapStatePrev;
            public int sourceRoomNr;
            public int sourceX;
            public int sourceY;
            public int direction;
            public int action;
            public List<Step> traversalSteps = new();
            private Step lastStep = null;
            public int pathLength = 0;

            //public Traversal(int roomNr, int sourceX, int sourceY, int direction, int action = -1) :
            //    this(mapStateId, roomNr, sourceX, sourceY, direction, action)
            //{ }

            public Traversal(int mapStateId, int roomNr, int sourceX, int sourceY, int direction, int action = -1)
            {
                this.mapStateId = mapStateId;
                this.mapStatePrev = mapStateId;
                this.sourceRoomNr = roomNr;
                this.sourceX = sourceX;
                this.sourceY = sourceY;
                this.direction = direction;
                this.action = action == -1 ? direction : action;
            }
            public int Evaluation()
            {
                int num = traversalSteps.Count == 0 ? EVAL_START : lastStep.evaluation;
                return traversalSteps.Count == 0 ? EVAL_START : lastStep.evaluation;
            }
            public bool EvaluationComplete()
            {
                return traversalSteps.Count > 0 && EvaluationIsEndpoint(lastStep.evaluation);
            }
            public int X()
            {
                return traversalSteps.Count > 0 ? lastStep.x : sourceX;
            }
            public int Y()
            {
                return traversalSteps.Count > 0 ? lastStep.y : sourceY;
            }
            public int RNr()
            {
                return traversalSteps.Count > 0 ? lastStep.roomNr : sourceRoomNr;
            }
            public void AddStep(int evaluation, int x, int y, int roomNr)
            {
                AddStep(new Step(evaluation, x, y, roomNr));
            }
            public void AddStep(Step step)
            {
                traversalSteps.Add(step);
                lastStep = step;
            }
            public void SetRoomDisabled()
            {
                lastStep.evaluation = EVAL_ROOM_DISABLED;
            }
            public override string ToString()
            {
                string actionName = actionNames[action];
                string reportString = $"{mapStateId,2} {pathLength,2}  {actionName,2}({sourceX,3},{sourceY,3})({sourceRoomNr})";
                foreach (var step in traversalSteps)
                {
                    string stepLabel = EvaluationLabel(step.evaluation);
                    reportString += $" {stepLabel}({step.x,3},{step.y,3})({step.roomNr})";
                }
                return reportString;
            }
        }

        public class Step
        {
            public int evaluation;
            public int x;
            public int y;
            public int roomNr;

            public Step(int evaluation, int x, int y, int roomNr)
            {
                this.evaluation = evaluation;
                this.x = x;
                this.y = y;
                this.roomNr = roomNr;
            }
        }
        public void ShowMapTraversal(int roomNr, int x, int y, int DIR)
        {
            Traversal t = EvaluateMapTraversal(roomNr, x, y, DIR);
            Debug.WriteLine($"{t}");
        }
    }
}
