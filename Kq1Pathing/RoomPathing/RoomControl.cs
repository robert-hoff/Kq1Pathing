using System.Diagnostics;
using Kq1Pathing.FileIO;
using static Kq1Pathing.RoomPathing.ClearBarrier;
using static Kq1Pathing.RoomPathing.RoomDefinition;

namespace Kq1Pathing.RoomPathing
{
    /*
     *
     * Initialisation of RoomControl depends on
     *
     *          RoomDefinition roomProperties
     *
     * Properties that specify directives for how to interpret different control information, for which there are
     * four types. TRAVERSABLE, BARRIER, ACTIONTRIGGER and WATER
     *
     * We have two main internal data structures
     *
     *          int[,] roomControlData
     *
     * These types go in the roomControlData[] array, which is a 1:1 mapping of each pixel in the bitmap,
     * but throws out all colors except ones that are relevant to
     * player traversal.
     *
     *          int[,] walkAdjacencies
     *
     * Has a size [HEIGHT * WIDTH, 9] where the (x,y) control-data is translated to an ordinal identifying 2D coordinates
     * as a single value (an ordinal). For eaxh of these the walk adjacencies describes the results of
     * a single pixel walk in one of eight directions 1-8 corresponding to N,NE,E,SE,S,SW,W,NW
     *
     *
     *
     */

    public class RoomControl
    {
        public int roomNr;
        private readonly RoomDefinition roomDefinition;
        private readonly int[,] roomControlData;

        // control information
        public const int TRAVERSABLE = 0; // traversable pixels
        public const int BARRIER = 1; // non-passable barrier that Graham will come to rest against
        public const int DEATH_TRIGGER = 2; // if any part of Graham touches these results in death
        public const int WATER = 3; // if Graham's entire footprint is on these the player transitions into water
        public const int OBJECT_BARRIER = 4;
        public const int WATER_TRIGGER = 5; // if any part of Graham touches these he falls into water
        public const int ROOM_TRIGGER = 6; // if any part of Graham touches these a room-change will occur

        // directions observe the numbering used in the AGI interpreter
        public const int DIR_UNDEFINED = 0;
        public const int DIR_N = 1;
        public const int DIR_NE = 2;
        public const int DIR_E = 3;
        public const int DIR_SE = 4;
        public const int DIR_S = 5;
        public const int DIR_SW = 6;
        public const int DIR_W = 7;
        public const int DIR_NW = 8;
        public const int STOP = 9;
        public const int AQ = 10; // acquire object

        public static readonly int[,] dxdy = {
            { 0, 0}, // event or action at the current pixel
            { 0,-1}, // N
            { 1,-1}, // NE
            { 1, 0}, // E
            { 1, 1}, // SE
            { 0, 1}, // S
            {-1, 1}, // SW
            {-1, 0}, // W
            {-1,-1}, // NW
        };

        public readonly static string[] directionNames = { "", "N", "NE", "E", "SE", "S", "SW", "W", "NW", "ST", "AQ" };

        // --- the EVAL's go in the adjacency map
        // the 'eval' constants are used to encode the result of a traversal
        // const int EVAL_TRAVERSABLE = TRAVERSABLE;
        public const int EVAL_BARRIER = -BARRIER;
        public const int EVAL_DEATH = -DEATH_TRIGGER;
        public const int EVAL_WATER = -WATER;
        public const int EVAL_ROOMTRIGGER = -ROOM_TRIGGER;
        public const int EVAL_ROOMACTION_AREA1 = -ROOM_TRIGGER - 1;
        public const int EVAL_ROOMACTION_AREA2 = -ROOM_TRIGGER - 2;
        // Used to indicate traversal into an adjacent room (by crossing a room boundary or horizon)
        public const int EVAL_EXIT_NORTH = -9;
        public const int EVAL_EXIT_EAST = -10;
        public const int EVAL_EXIT_SOUTH = -11;
        public const int EVAL_EXIT_WEST = -12;

        public readonly int footprintWidth;
        public readonly int[,] walkAdjacencies = new int[ROOM_WIDTH * ROOM_HEIGHT, 9];
        private const int ADJACENCIES_NOT_FOUND = 0;
        private const int ADJACENCIES_FOUND = 1;

        // written in as A,B,C,D
        private List<int[]> edgeRewrites = new();
        private int lastDirection = STOP;

        public RoomControl(int roomNr) : this(roomNr, new GameState())
        { }

        public RoomControl(int roomNr, GameState gameState)
        {
            this.roomNr = roomNr;
            this.roomDefinition = RoomDefinition.GetRoomDefinition(roomNr, gameState);
            this.roomControlData = RoomBitmapImport.ImportRoom(roomDefinition);
            this.footprintWidth = gameState.footprintWidth;

            // Transfer objects into edge-rewrites defined by A,B,C,D
            bool[] edgeInfluence = new bool[ROOM_WIDTH];
            foreach (var rect in roomDefinition.objectsEdgeRewriting)
            {
                for (int i = rect[0] + 1 - footprintWidth; i <= rect[2]; i++)
                {
                    edgeInfluence[i] = true;
                }
            }
            if (roomDefinition.hasObject)
            {
                int[] rect = roomDefinition.objectFootprint;
                for (int i = rect[0] + 1 - footprintWidth; i <= rect[2]; i++)
                {
                    edgeInfluence[i] = true;
                }
            }
            // collect rewrites
            bool rewriteFound = false;
            int A = 0, B = 0, C = 0, D = 0;
            for (int i = 0; i < ROOM_WIDTH; i++)
            {
                if (edgeInfluence[i] && !rewriteFound)
                {
                    A = i;
                    rewriteFound = true;
                }
                if (!edgeInfluence[i] && rewriteFound)
                {
                    D = i - 1;
                    B = (A + D) / 2;
                    C = B + 1;
                    rewriteFound = false;
                    edgeRewrites.Add(new int[] { A, B, C, D });
                }
            }
        }

        /*
         * There are a couple of exmamples that suggests the script repositioning
         * is applied first
         *
         *      mapTraversal.EvaluateMapTraversal(roomNr: 3, 154, 150, DIR_SW)
         *      mapTraversal.EvaluateMapTraversal(roomNr: 75, 154, 108, DIR_E)
         *
         * However the following example doesn't seem to agree
         *
         *
         *
         */
        public (int, int, bool) RoomEntryEdgeTransition(int previousRoom, int x, int y)
        {
            int clampX = Math.Clamp(x, MinimumX(), MaximumX());
            int clampY = Math.Clamp(y, MinimumY(), MaximumY());

            if (clampX < MinimumX() || clampX > MaximumX() ||
                clampY < MinimumY() || clampY > MaximumY())
            {
                throw new Exception($"Room entry not along the perimeter {previousRoom},{roomNr} " +
                    $"player position = {clampX},{clampY}");
            }

            // Apply rewrite rules from (from the room's logic scripts)
            // then check if barriers need to be cleared
            (int x, int y) P1 = roomDefinition.rewriteRule(previousRoom, clampX, clampY, footprintWidth);
            bool stopPlayer = roomDefinition.stopPoints.ContainsKey((P1.x, P1.y));
            if (stopPlayer)
            {
                return (P1.x, P1.y, true);
            }

            // if rewrite rules have been applied, do the barrier clearance and return
            if (P1.x != clampX || P1.y != clampY)
            {
                (int x, int y) P2 = ClearBarrier.RepositionPlayer(this, P1.x, P1.y);
                return (P2.x, P2.y, stopPlayer);
            }

            // Otherwise check for object rewrite rules
            // Note that checking N and S entries like this disregards min and max x values
            // the only known object that touches the edge is the goat's pen when in the glitched state
            // North entry
            if (clampY == MinimumY() && (clampX != MinimumX() || clampX != MaximumX()))
            {
                foreach (var edgeRewrite in edgeRewrites)
                {
                    if (clampX >= edgeRewrite[0] && clampX <= edgeRewrite[1])
                    {
                        return (edgeRewrite[0] - 1, clampY, false);
                    }
                    if (clampX >= edgeRewrite[2] && clampX <= edgeRewrite[3])
                    {
                        return clearEdgeRewriteNorth(edgeRewrite[3] + 1, clampY + edgeRewrite[3] + 1 - clampX);
                    }
                }
            }

            // South entry
            if (clampY == MaximumY() && (clampX != MinimumX() || clampX != MaximumX()))
            {
                foreach (var edgeRewrite in edgeRewrites)
                {
                    if (clampX >= edgeRewrite[0] && clampX <= edgeRewrite[1])
                    {
                        return clearEdgeRewriteSouth(edgeRewrite[0] - 1, clampY + edgeRewrite[0] - clampX);
                    }
                    if (clampX >= edgeRewrite[2] && clampX <= edgeRewrite[3])
                    {
                        return (edgeRewrite[3] + 1, clampY, false);
                    }
                }
            }

            // apply barrier clearance and return
            (int x, int y) P3 = ClearBarrier.RepositionPlayer(this, clampX, clampY);
            return (P3.x, P3.y, false);
        }

        public (int, int, bool) clearEdgeRewriteNorth(int x, int y)
        {
            while (CheckFootprint(x, y) == EVAL_BARRIER)
            {
                y--;
            }
            return (x, y, false);
        }

        public (int, int, bool) clearEdgeRewriteSouth(int x, int y)
        {
            while (CheckFootprint(x, y) == EVAL_BARRIER)
            {
                y++;
            }
            return (x, y, false);
        }

        public (int, int, bool) RoomTriggerTransition(int previousRoom, int x, int y)
        {
            (int x, int y) rewriteXY = roomDefinition.rewriteRule(previousRoom, x, y, footprintWidth);
            int entryX = rewriteXY.x;
            int entryY = rewriteXY.y;
            bool stopPlayer = roomDefinition.stopPoints.ContainsKey((entryX, entryY));

            if (stopPlayer)
            {
                return (entryX, entryY, stopPlayer);
            }

            // There is a room transition from 74 to 75 where this was needed
            // going from (87,163)(74) W
            (int x, int y) PNew = ClearBarrier.RepositionPlayer(this, entryX, entryY);
            return (PNew.x, PNew.y, false);
        }

        public int GetRoomTriggered(int evaluation)
        {
            switch (evaluation)
            {
                case EVAL_ROOMTRIGGER:
                    return roomDefinition.actionTriggerRoom;
                case EVAL_ROOMACTION_AREA1:
                    return roomDefinition.actionAreasRoomNumbers[0];
                case EVAL_ROOMACTION_AREA2:
                    return roomDefinition.actionAreasRoomNumbers[1];
                default:
                    throw new Exception($"wrong value for this method ({evaluation})");
            }
        }

        public int GetAdjacentRoomNr(int evaluation)
        {
            switch (evaluation)
            {
                case EVAL_EXIT_NORTH:
                    return roomDefinition.roomNorth;
                case EVAL_EXIT_SOUTH:
                    return roomDefinition.roomSouth;
                case EVAL_EXIT_EAST:
                    return roomDefinition.roomEast;
                case EVAL_EXIT_WEST:
                    return roomDefinition.roomWest;
                default:
                    throw new Exception($"wrong value for this method ({evaluation})");
            }
        }

        public (int evaluation, int x, int y) EvaluateTraversal(int ordinal, int DIR)
        {
            return EvaluateTraversal(GetX(ordinal), GetY(ordinal), DIR, roomDefinition.hasObject);
        }

        public (int evaluation, int x, int y) EvaluateTraversal(int ordinal, int DIR, bool observeObjectBarrier)
        {
            return EvaluateTraversal(GetX(ordinal), GetY(ordinal), DIR, observeObjectBarrier);
        }

        public (int evaluation, int x, int y) EvaluateTraversal(int x, int y, int DIR)
        {
            return EvaluateTraversal(x, y, DIR, roomDefinition.hasObject);
        }

        public (int evaluation, int x, int y) EvaluateTraversal(int x, int y, int DIR, bool observeObjectBarrier)
        {
            if (DIR == STOP)
            {
                lastDirection = STOP;
                return (TRAVERSABLE, x, y);
            }

            if (DIR == AQ)
            {
                if (lastDirection == STOP)
                {
                    return (TRAVERSABLE, x, y);
                }
                DIR = lastDirection;
                observeObjectBarrier = false;
            }
            else
            {
                lastDirection = DIR;
            }

            if (!CheckTraversablePosition(x, y))
            {
                throw new Exception($"not a traversable position ({x},{y})({roomNr})");
            }
            // also get the evaluation at the starting position, which may be an event
            int initialEvaluation = CheckFootprint(x, y);
            if (initialEvaluation < 0)
            {
                return (initialEvaluation, x, y);
            }
            int initialEvaluationArea = EvaluateActionRegions(x, y);
            if (initialEvaluationArea < 0)
            {
                return (initialEvaluationArea, x, y);
            }

            int originOrdinal = GetOrdinal(x, y);
            if (walkAdjacencies[originOrdinal, 0] == ADJACENCIES_NOT_FOUND)
            {
                FindAdjacencies(x, y);
            }
            int nextOrdinal = originOrdinal;

            // FIXME the algorithm used here is simply ridiculous ... but it is working
            // NOTE - object barriers are only checked against the player's footprint!
            if (observeObjectBarrier)
            {
                int nextOrdinal2 = walkAdjacencies[nextOrdinal, DIR];
                int rightOrdinal = GetRightOrdinal(nextOrdinal2, footprintWidth);
                while (walkAdjacencies[nextOrdinal, DIR] > 0 &&
                    GetControl(nextOrdinal2) != OBJECT_BARRIER &&
                    GetControl(rightOrdinal) != OBJECT_BARRIER)
                {
                    // this request increments the ordinal
                    nextOrdinal = walkAdjacencies[nextOrdinal, DIR];
                    nextOrdinal2 = walkAdjacencies[nextOrdinal, DIR];
                    rightOrdinal = GetRightOrdinal(nextOrdinal2, footprintWidth);

                    //Debug.WriteLine($"{nextOrdinal},{rightOrdinal}");
                    //ShowXY(nextOrdinal);
                    //ShowXY(rightOrdinal);
                }
            }
            else
            {
                while (walkAdjacencies[nextOrdinal, DIR] > 0)
                {
                    nextOrdinal = walkAdjacencies[nextOrdinal, DIR];
                }
            }
            if (walkAdjacencies[nextOrdinal, DIR] >= 0 && observeObjectBarrier)
            {
                int nextOrdinal2 = walkAdjacencies[nextOrdinal, DIR];
                int rightOrdinal = GetRightOrdinal(nextOrdinal2, footprintWidth);
                if (GetControl(nextOrdinal2) == OBJECT_BARRIER ||
                    GetControl(rightOrdinal) == OBJECT_BARRIER)
                {
                    return (TRAVERSABLE, GetX(nextOrdinal), GetY(nextOrdinal));
                }
            }

            if (walkAdjacencies[nextOrdinal, DIR] == EVAL_BARRIER)
            {
                return (TRAVERSABLE, GetX(nextOrdinal), GetY(nextOrdinal));
            }
            else if (
                walkAdjacencies[nextOrdinal, DIR] == EVAL_DEATH ||
                walkAdjacencies[nextOrdinal, DIR] == EVAL_WATER)
            {
                int xResult = GetX(nextOrdinal) + dxdy[DIR, 0];
                int yResult = GetY(nextOrdinal) + dxdy[DIR, 1];
                if (xResult <= roomDefinition.GetWestBoundary() ||
                    xResult > roomDefinition.GetEastBoundary() - footprintWidth)
                {
                    xResult -= dxdy[DIR, 0];
                }
                return (walkAdjacencies[nextOrdinal, DIR], xResult, yResult);
            }
            else
            {
                return (walkAdjacencies[nextOrdinal, DIR], GetX(nextOrdinal), GetY(nextOrdinal));
            }
        }

        public static int GetRightOrdinal(int ordinal, int footprintWidth)
        {
            int x = GetX(ordinal);
            int y = GetY(ordinal);
            return GetOrdinal(x + footprintWidth - 1, y);
        }

        public void ShowXY(int ordinal)
        {
            int x = GetX(ordinal);
            int y = GetY(ordinal);
            Debug.WriteLine($"({x},{y})");
        }

        public void FindAdjacencies(int x, int y)
        {
            if (!CheckTraversablePosition(x, y))
            {
                throw new Exception($"Not a traversable position ({x},{y})");
            }
            Stack<int> nextAdjacencies = new();
            nextAdjacencies.Push(GetOrdinal(x, y));
            walkAdjacencies[GetOrdinal(x, y), 0] = ADJACENCIES_FOUND;
            while (nextAdjacencies.Count > 0)
            {
                int nextAdjacency = nextAdjacencies.Pop();
                Traverse(nextAdjacency, nextAdjacencies);
            }
        }

        private void Traverse(int ordinal, Stack<int> nextAdjacencies)
        {
            // for each ordinal determine the adjacency type (evaluation) in each direction
            int x = GetX(ordinal);
            int y = GetY(ordinal);
            walkAdjacencies[ordinal, DIR_N] = EvaluateDirection(x, y, DIR_N);
            walkAdjacencies[ordinal, DIR_NE] = EvaluateDirection(x, y, DIR_NE);
            walkAdjacencies[ordinal, DIR_E] = EvaluateDirection(x, y, DIR_E);
            walkAdjacencies[ordinal, DIR_SE] = EvaluateDirection(x, y, DIR_SE);
            walkAdjacencies[ordinal, DIR_S] = EvaluateDirection(x, y, DIR_S);
            walkAdjacencies[ordinal, DIR_SW] = EvaluateDirection(x, y, DIR_SW);
            walkAdjacencies[ordinal, DIR_W] = EvaluateDirection(x, y, DIR_W);
            walkAdjacencies[ordinal, DIR_NW] = EvaluateDirection(x, y, DIR_NW);

            for (int DIR = 1; DIR <= 8; DIR++)
            {
                int evaluation = walkAdjacencies[ordinal, DIR];
                if (evaluation > 0 && walkAdjacencies[evaluation, 0] == ADJACENCIES_NOT_FOUND)
                {
                    nextAdjacencies.Push(evaluation);
                    walkAdjacencies[evaluation, 0] = ADJACENCIES_FOUND;
                }
            }
        }

        public bool CheckTraversablePosition(int ordinal)
        {
            return CheckTraversablePosition(GetX(ordinal), GetY(ordinal));
        }

        /*
         *
         * Make a disctinction between room edges and room boundaries or bounds
         * using DEFAULT_LOWER_LIMIT for the y boundary which is set to 36
         *
         */
        public bool CheckTraversablePosition(int x, int y)
        {
            // HACK!!
            if (x >= 142 && footprintWidth == 19)
            {
                x = 141;
            }

            if (x < 0 || y <= roomDefinition.GetNorthBoundary() || x > ROOM_WIDTH - footprintWidth || y >= ROOM_HEIGHT)
            {
                throw new Exception($"position out of bounds ({x},{y})");
            }
            if (y <= roomDefinition.edgeNorth || y >= roomDefinition.edgeSouth)
            {
                return false;
            }
            if (x <= roomDefinition.edgeWest || x > roomDefinition.edgeEast - footprintWidth)
            {
                return false;
            }
            for (int i = 0; i < footprintWidth; i++)
            {
                if (roomControlData[x + i, y] == BARRIER)
                {
                    return false;
                }
            }
            return true;
        }

        /*
         * This evaluates the result of 1 pixel traversals only for one of
         * the given 8 directions
         */
        public int EvaluateDirection(int x, int y, int DIR)
        {
            int dx = dxdy[DIR, 0];
            int dy = dxdy[DIR, 1];

            // check north-south room edges first
            if (y + dy == roomDefinition.edgeNorth)
            {
                // if on one of the corners we can return the evaluation
                if (x + dx < 0 || x + dx > ROOM_WIDTH - footprintWidth)
                {
                    // Debug.WriteLine($"north corner ({x},{y}) dir={directionNames[DIR]}");
                    return EVAL_EXIT_NORTH;
                }
                int result = CheckFootprint(x + dx, y); // hold y the same
                // edge glitch with inconsistent behaviour
                if (result < EVAL_BARRIER)
                {
                    if (RoomDefinition.checkEdgeGlitch(roomNr, x, y, DIR, result))
                    {
                        return EVAL_EXIT_NORTH;
                    }
                }
                return result < 0 ? result : EVAL_EXIT_NORTH;
            }

            // NEEDS IMPLEMENTATION AND TESTING
            if (y + dy == roomDefinition.edgeSouth)
            {
                // if on one of the corners we can return the evaluation
                if (x + dx < 0 || x + dx > ROOM_WIDTH - footprintWidth)
                {
                    // Debug.WriteLine($"south corner ({x},{y}) dir={directionNames[DIR]}");
                    return EVAL_EXIT_SOUTH;
                }
                int result = CheckFootprint(x + dx, y); // hold y the same
                // edge glitch with inconsistent behaviour
                if (result < EVAL_BARRIER)
                {
                    if (RoomDefinition.checkEdgeGlitch(roomNr, x, y, DIR, result))
                    {
                        return EVAL_EXIT_SOUTH;
                    }
                }
                return result < 0 ? result : EVAL_EXIT_SOUTH;
            }

            if (x + dx == roomDefinition.edgeEast - footprintWidth + 1)
            {
                // if this is against a *boundary* (not an edge) then this traversal
                // is not permitted
                if (y + dy == roomDefinition.GetNorthBoundary())
                {
                    return EVAL_BARRIER;
                }
                if (y + dy <= roomDefinition.MinTraversableY() || y + dy >= ROOM_HEIGHT)
                {
                    return EVAL_EXIT_EAST;
                }
                int result = CheckFootprint(x, y + dy); // hold x the same
                // edge glitch with inconsistent behaviour
                if (result < EVAL_BARRIER)
                {
                    if (RoomDefinition.checkEdgeGlitch(roomNr, x, y, DIR, result))
                    {
                        return EVAL_EXIT_EAST;
                    }
                }
                return result < 0 ? result : EVAL_EXIT_EAST;
            }

            if (x + dx == roomDefinition.edgeWest)
            {
                // if this is against a *boundary* (not an edge) then this traversal
                // is not permitted
                if (y + dy == roomDefinition.GetNorthBoundary())
                {
                    return EVAL_BARRIER;
                }
                // Here, if y+dy has *exceeded* the edges (traversable positions)
                // there is no more checking necessary
                if (y + dy <= roomDefinition.MinTraversableY() || y + dy >= ROOM_HEIGHT)
                {
                    return EVAL_EXIT_WEST;
                }
                int result = CheckFootprint(x, y + dy); // hold x the same
                // edge glitch with inconsistent behaviour
                if (result < EVAL_BARRIER)
                {
                    if (RoomDefinition.checkEdgeGlitch(roomNr, x, y, DIR, result))
                    {
                        return EVAL_EXIT_WEST;
                    }
                }
                return result < 0 ? result : EVAL_EXIT_WEST;
            }

            // Out of bounds traversal
            if (x + dx < 0 || x + dx == ROOM_WIDTH - footprintWidth + 1)
            {
                dx = 0;
            }
            if (y + dy <= roomDefinition.GetNorthBoundary() || y + dy == ROOM_HEIGHT)
            {
                dy = 0;
            }
            if (dx == 0 && dy == 0)
            {
                return EVAL_BARRIER;
            }

            int evaluation = CheckFootprint(x + dx, y + dy);
            if (evaluation == EVAL_DEATH || evaluation == EVAL_BARRIER)
            {
                return evaluation;
            }

            int areaEvaluation = EvaluateActionRegions(x + dx, y + dy);
            if (areaEvaluation == EVAL_WATER && evaluation < EVAL_WATER)
            {
                throw new Exception($"duplicate action water/room on {directionNames[DIR]}({x},{y})");
            }
            if (areaEvaluation == EVAL_WATER && evaluation < EVAL_WATER)
            {
                throw new Exception($"duplicate action water/room on {directionNames[DIR]}({x},{y})");
            }
            if ((areaEvaluation == EVAL_ROOMACTION_AREA1 ||
                areaEvaluation == EVAL_WATER) &&
                evaluation == EVAL_ROOMACTION_AREA2)
            {
                throw new Exception($"duplicate action water/room on {directionNames[DIR]}({x},{y})");
            }
            return areaEvaluation < 0 ? areaEvaluation : evaluation;
        }

        private int EvaluateActionRegions(int x, int y)
        {
            // Check water action areas
            foreach (var rect in roomDefinition.actionAreasWater)
            {
                if (CheckPixelContained(x, y, rect))
                {
                    return EVAL_WATER;
                }
            }

            // Check room action areas
            for (int i = 0; i < roomDefinition.actionAreasRoomChange.Count; i++)
            {
                int[] actionRectangle = roomDefinition.actionAreasRoomChange[i];
                if (CheckPixelContained(x, y, actionRectangle))
                {
                    if (i == 0)
                    {
                        return EVAL_ROOMACTION_AREA1;
                    }
                    if (i == 1)
                    {
                        return EVAL_ROOMACTION_AREA2;
                    }
                    if (i > 1)
                    {
                        throw new Exception($"not implemented more than 2 room action areas");
                    }
                }
            }
            return 0;
        }

        private int CheckFootprint(int x, int y)
        {
            // Debug.WriteLine($"checking footprint at ({x},{y})");
            // HACK!!
            if (x >= 142 && footprintWidth == 19)
            {
                x = 141;
            }

            if (x < 0 || x >= ROOM_WIDTH - footprintWidth + 1)
            {
                throw new Exception($"Unexpected value x = {x} for position ({x},{y})");
            }
            if (y < 0 || y >= ROOM_HEIGHT)
            {
                throw new Exception($"Unexpected value y = {y} for position ({x},{y})");
            }

            bool deathTriggerFound = false;
            bool roomTriggerFound = false;
            int waterCount = 0;
            for (int i = 0; i < footprintWidth; i++)
            {
                // if a barrier is found return is immediate as traversal is prohibited
                if (roomControlData[x + i, y] == BARRIER)
                {
                    return EVAL_BARRIER;
                }
                if (roomControlData[x + i, y] == WATER)
                {
                    waterCount++;
                }
                if (roomControlData[x + i, y] == DEATH_TRIGGER)
                {
                    deathTriggerFound = true;
                }
                if (roomControlData[x + i, y] == ROOM_TRIGGER)
                {
                    roomTriggerFound = true;
                }
            }

            if (waterCount == footprintWidth)
            {
                return roomDefinition.useWaterControl == WATER_TILES_ROOM_TRIGGER ? EVAL_ROOMTRIGGER : EVAL_WATER;
            }
            if (deathTriggerFound)
            {
                return EVAL_DEATH;
            }
            if (roomTriggerFound)
            {
                return EVAL_ROOMTRIGGER;
            }
            return GetOrdinal(x, y);
        }

        public int GetControl(int ordinal)
        {
            return roomControlData[GetX(ordinal), GetY(ordinal)];
        }

        public static int GetOrdinal(int x, int y)
        {
            return x + ROOM_WIDTH * y;
        }

        public static int GetX(int ordinal)
        {
            return ordinal % ROOM_WIDTH;
        }

        public static int GetY(int ordinal)
        {
            return ordinal / ROOM_WIDTH;
        }

        public int MinimumX()
        {
            return 0;
        }
        public int MaximumX()
        {
            return ROOM_WIDTH - footprintWidth;
        }
        public int MinimumY()
        {
            return roomDefinition.MinTraversableY() + 1;
        }
        public int MaximumY()
        {
            return ROOM_HEIGHT - 1;
        }

        public static bool CheckPixelContained(int ordinal, int[] rectangle)
        {
            return CheckPixelContained(GetX(ordinal), GetY(ordinal), rectangle);
        }

        public static bool CheckPixelContained(int x0, int y0, int[] rectangle)
        {
            return x0 >= rectangle[0] && x0 <= rectangle[2] && y0 >= rectangle[1] && y0 <= rectangle[3];
        }

        public static bool CheckPixelContained(int xP, int yP, int x0, int y0, int x1, int y1)
        {
            return xP >= x0 && xP <= x1 && yP >= y0 && yP <= y1;
        }

        public static bool IsWithinDistance(int distance, int objX, int objY, int x, int y)
        {
            return Math.Abs(objX - x) + Math.Abs(objY - y) <= distance;
        }

        public static void ShowPosition(int ordinal)
        {
            if (ordinal >= 0)
            {
                Debug.WriteLine($"{GetX(ordinal)},{GetY(ordinal)}");
            }
            else
            {
                Debug.WriteLine($"{ordinal}");
            }
        }

        public void ShowControlData(HandleOutputWrite outputWriter = null)
        {
            PrintControlData(roomControlData, outputWriter);
        }

        public void ShowWaterAreaTriggers()
        {
            int[,] roomDataCopy = CopyControlData(roomControlData);
            foreach (var rect in roomDefinition.actionAreasWater)
            {
                for (int i = rect[0]; i <= rect[2]; i++)
                {
                    for (int j = 1; j <= 3; j += 2)
                    {
                        if (roomDataCopy[i, rect[j]] == TRAVERSABLE ||
                            roomDataCopy[i, rect[j]] == WATER)
                        {
                            roomDataCopy[i, rect[j]] = PRINT_INFO_AREA_PERIMETER;
                        }
                    }
                }
                for (int j = rect[1]; j <= rect[3]; j++)
                {
                    for (int i = 0; i <= 2; i += 2)
                    {
                        if (roomDataCopy[rect[i], j] == TRAVERSABLE ||
                            roomDataCopy[rect[i], j] == WATER)
                        {
                            roomDataCopy[rect[i], j] = PRINT_INFO_AREA_PERIMETER;
                        }
                    }
                }
            }
            PrintControlData(roomDataCopy);
        }

        public void ShowRoomAreaTriggers()
        {
            int[,] roomDataCopy = CopyControlData(roomControlData);
            foreach (var rect in roomDefinition.actionAreasRoomChange)
            {
                for (int i = rect[0]; i <= rect[2]; i++)
                {
                    for (int j = 1; j <= 3; j += 2)
                    {
                        if (roomDataCopy[i, rect[j]] == TRAVERSABLE ||
                            roomDataCopy[i, rect[j]] == WATER)
                        {
                            roomDataCopy[i, rect[j]] = PRINT_INFO_AREA_PERIMETER;
                        }
                    }
                }
                for (int j = rect[1]; j <= rect[3]; j++)
                {
                    for (int i = 0; i <= 2; i += 2)
                    {
                        if (roomDataCopy[rect[i], j] == TRAVERSABLE ||
                            roomDataCopy[rect[i], j] == WATER)
                        {
                            roomDataCopy[rect[i], j] = PRINT_INFO_AREA_PERIMETER;
                        }
                    }
                }
            }

            int minY = roomDefinition.MinTraversableY();
            for (int i = 0; i < ROOM_WIDTH; i++)
            {
                roomDataCopy[i, minY - 1] = PRINT_INFO_AREA_PERIMETER;
            }

            PrintControlData(roomDataCopy);
        }

        private static int[,] CopyControlData(int[,] controlData)
        {
            int[,] copy = new int[controlData.GetLength(0), controlData.GetLength(1)];
            for (int i = 0; i < controlData.GetLength(0); i++)
            {
                for (int j = 0; j < controlData.GetLength(1); j++)
                {
                    copy[i, j] = controlData[i, j];
                }
            }
            return copy;
        }

        public void ShowTraversablePositions()
        {
            int[,] roomDataCopy = new int[ROOM_WIDTH, ROOM_HEIGHT];
            for (int j = 0; j < ROOM_HEIGHT; j++)
            {
                for (int i = 0; i < ROOM_WIDTH; i++)
                {
                    roomDataCopy[i, j] = roomControlData[i, j];
                    int ordinal = GetOrdinal(i, j);
                    if (walkAdjacencies[ordinal, 0] == ADJACENCIES_FOUND)
                    {
                        roomDataCopy[i, j] = PRINT_INFO_TRAVERSABLE_PATH;
                    }
                }
            }
            PrintControlData(roomDataCopy);
        }

        public static void ShowWalkAdjacencies(RoomControl roomControl, int x, int y)
        {
            int ordinal = RoomControl.GetOrdinal(x, y);
            for (int i = 1; i <= 8; i++)
            {
                string directionName = RoomControl.directionNames[i];
                string reportResult;
                if (roomControl.walkAdjacencies[ordinal, i] > 0)
                {
                    int xDestination = RoomControl.GetX(roomControl.walkAdjacencies[ordinal, i]);
                    int yDestination = RoomControl.GetY(roomControl.walkAdjacencies[ordinal, i]);
                    reportResult = $"({xDestination},{yDestination})";
                }
                else
                {
                    reportResult = $"{roomControl.walkAdjacencies[ordinal, i]}";
                }
                Debug.WriteLine($"{directionName,2} ({x},{y}) = {reportResult}");
            }
        }

        public void ShowRoomWithTraversals(int DIR)
        {
            int[,] roomDataCopy = new int[ROOM_WIDTH, ROOM_HEIGHT];
            for (int j = 0; j < ROOM_HEIGHT; j++)
            {
                for (int i = 0; i < ROOM_WIDTH; i++)
                {
                    roomDataCopy[i, j] = roomControlData[i, j];
                    int ordinal = GetOrdinal(i, j);
                    if (walkAdjacencies[ordinal, DIR] < 0)
                    {
                        roomDataCopy[i, j] = walkAdjacencies[ordinal, DIR];
                    }
                }
            }
            PrintControlData(roomDataCopy);
        }

        const int PRINT_INFO_TRAVERSABLE_PATH = -100;
        const int PRINT_INFO_AREA_PERIMETER = -101;

        private static void PrintControlData(int[,] roomData, HandleOutputWrite outputWriter = null)
        {
            if (outputWriter == null)
            {
                outputWriter = (x) => Debug.Write(x);
            }

            outputWriter($"    ");
            for (int i = 0; i < ROOM_WIDTH; i++)
            {
                if (i % 10 == 0)
                {
                    outputWriter($"{i,-10}");
                }
            }
            outputWriter($"\n");
            outputWriter($"    ");
            for (int i = 0; i < ROOM_WIDTH; i++)
            {
                outputWriter($"{i % 10}");
            }
            outputWriter($"\n");

            for (int j = 0; j < ROOM_HEIGHT; j++)
            {
                outputWriter($"{j,-4}");
                for (int i = 0; i < ROOM_WIDTH; i++)
                {
                    switch (roomData[i, j])
                    {
                        case TRAVERSABLE:
                            outputWriter($"_");
                            break;

                        default:
                            outputWriter($"{roomData[i, j]}");
                            break;

                        case PRINT_INFO_TRAVERSABLE_PATH:
                            outputWriter($".");
                            break;
                        case EVAL_DEATH:
                            outputWriter($"D");
                            break;
                        case EVAL_WATER:
                            outputWriter($"W");
                            break;
                        case EVAL_ROOMTRIGGER:
                        case EVAL_ROOMACTION_AREA1:
                        case EVAL_ROOMACTION_AREA2:
                        case EVAL_EXIT_NORTH:
                        case EVAL_EXIT_EAST:
                        case EVAL_EXIT_SOUTH:
                        case EVAL_EXIT_WEST:
                            outputWriter($"R");
                            break;

                        case PRINT_INFO_AREA_PERIMETER:
                            outputWriter($"X");
                            break;
                    }
                }
                outputWriter("\n");
            }
        }

        public void ShowWalkAdjacencies(int x, int y)
        {
            Debug.WriteLine($"walk adjacencies for position ({x},{y})");
            for (int i = 1; i <= 8; i++)
            {
                Debug.WriteLine($"{directionNames[i],2} {walkAdjacencies[GetOrdinal(x, y), i],2}");
            }
            Debug.WriteLine($"");
        }

        public void ShowRoomTraversableExtent()
        {
            Debug.WriteLine($"Top-boundary or horizon = {MinimumY()}");
            Debug.WriteLine($"Bottom-boundary = {MaximumY()}");
            Debug.WriteLine($"Left-boundary = {MinimumX()}");
            Debug.WriteLine($"Right-boundary = {MaximumX() + footprintWidth}");
        }

        public void ShowObjectRewrites()
        {
            if (edgeRewrites.Count == 0)
            {
                Debug.WriteLine($"no object rewrites");
            }
            foreach (var item in edgeRewrites)
            {
                Debug.WriteLine($"A,B,C,D = {item[0]},{item[1]},{item[2]},{item[3]}");
            }
        }
    }
}
