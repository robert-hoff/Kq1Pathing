using System.Diagnostics;
using static Kq1Pathing.RoomPathing.GameState;

namespace Kq1Pathing.RoomPathing
{
    public class RoomDefinition
    {
        static public bool reportEdgeGlitches = true;
        static Dictionary<(int, int, int, int), int> knownEdgeGlitches = new();
        public const int GLITCH_PERMITTED = 0;
        public const int GLITCH_NOT_PERMITTED = 1;

        // returns true if permitted
        public static bool checkEdgeGlitch(int roomNr, int x, int y, int DIR, int eval)
        {
            if (knownEdgeGlitches.Count == 0)
            {
                knownEdgeGlitches.Add((1, 0, 150, RoomControl.DIR_SW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((1, 0, 160, RoomControl.DIR_SW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((2, 154, 159, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((4, 35, 56, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((5, 134, 56, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((7, 0, 61, RoomControl.DIR_SW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((7, 0, 160, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((7, 0, 161, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((8, 81, 167, RoomControl.DIR_SE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((8, 102, 167, RoomControl.DIR_SW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((8, 154, 54, RoomControl.DIR_SE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((8, 154, 160, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((17, 0, 65, RoomControl.DIR_SW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((17, 10, 57, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((17, 68, 167, RoomControl.DIR_SW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((18, 125, 167, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((18, 154, 65, RoomControl.DIR_SE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((20, 29, 54, RoomControl.DIR_NE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((20, 55, 167, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((20, 81, 54, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((20, 93, 167, RoomControl.DIR_SW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((26, 41, 47, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((23, 94, 47, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((23, 26, 47, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((32, 122, 41, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((33, 145, 44, RoomControl.DIR_NE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((33, 154, 44, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((34, 111, 46, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((34, 94, 46, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((37, 47, 167, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((37, 104, 56, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((37, 110, 167, RoomControl.DIR_SW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((39, 39, 42, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((39, 57, 42, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((40, 2, 43, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((40, 23, 43, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((42, 132, 51, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((42, 154, 76, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((43, 0, 123, RoomControl.DIR_NW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((43, 97, 47, RoomControl.DIR_NW), GLITCH_PERMITTED);

                knownEdgeGlitches.Add((57, 0, 46, RoomControl.DIR_SW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((57, 0, 48, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((57, 154, 46, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((57, 154, 48, RoomControl.DIR_NE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((58, 0, 40, RoomControl.DIR_SW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((58, 0, 42, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((58, 154, 44, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((58, 154, 46, RoomControl.DIR_NE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((59, 0, 34, RoomControl.DIR_SW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((59, 0, 36, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((60, 5, 46, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((60, 154, 157, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((60, 154, 159, RoomControl.DIR_NE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((61, 0, 159, RoomControl.DIR_SW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((61, 0, 161, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((61, 154, 159, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((61, 154, 161, RoomControl.DIR_NE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((62, 0, 158, RoomControl.DIR_SW), GLITCH_NOT_PERMITTED);
                knownEdgeGlitches.Add((62, 0, 160, RoomControl.DIR_NW), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((62, 150, 46, RoomControl.DIR_SE), GLITCH_NOT_PERMITTED); // funny one
                knownEdgeGlitches.Add((64, 154, 128, RoomControl.DIR_SE), GLITCH_PERMITTED);
                knownEdgeGlitches.Add((64, 154, 67, RoomControl.DIR_NE), GLITCH_PERMITTED);

                // This is an interesting one, it glitches graham in the staircase
                knownEdgeGlitches.Add((67, 126, 41, RoomControl.DIR_NE), GLITCH_NOT_PERMITTED);
            }

            if (knownEdgeGlitches.ContainsKey((roomNr, x, y, DIR)))
            {
                return knownEdgeGlitches[(roomNr, x, y, DIR)] == GLITCH_PERMITTED;
            }
            Debug.WriteLine($"unknown edge glitch room{roomNr}" +
                $"{RoomControl.directionNames[DIR]}({x},{y}) eval = {eval}");

            return false;
        }

        public int roomNr;

        public const int DEFAULT_FOOTPRINT = 6;
        private const int DEFAULT_LOWER_LIMIT = 36;
        public const int ROOM_WIDTH = 160;
        public const int ROOM_HEIGHT = 168;

        // if dark-green pixels are found and behaviour is undefined, throw an exception
        public const int ACTION_TILE_UNDEFINED = 1;
        public const int ACTION_TILE_IGNORED = 2;
        public const int ACTION_TILE_DEATH_TRIGGER = 3;
        public const int ACTION_TILE_WATER_TRIGGER = 4;
        public const int ACTION_TILE_ROOM_TRIGGER = 5;
        public const int ACTION_TILE_SPECIAL = 6;

        public const int BLOCKS_UNDEFINED = 1;
        public const int BLOCKS_IGNORED = 2;
        public const int BLOCKS_OBSERVED = 3;

        public const int WATER_TILES_IGNORED = 1;
        public const int WATER_TILES_OBSERVED = 2;
        public const int WATER_TILES_ROOM_TRIGGER = 3;

        public int useActionControl = ACTION_TILE_UNDEFINED;
        public int useBlockControl = BLOCKS_UNDEFINED;
        public int useWaterControl = WATER_TILES_OBSERVED;

        public int roomNorth;
        public int roomEast;
        public int roomSouth;
        public int roomWest;
        public int edgeNorth = int.MinValue;
        public int edgeEast = int.MaxValue;
        public int edgeSouth = int.MaxValue;
        public int edgeWest = int.MinValue;

        public List<int[]> specialBarriers = new();

        // the only time we want variable barriers is for objects
        public int[] objectFootprint = new int[4];

        // if true observe object footprint as a barrier and observe edge rewriting
        // hasObject will be used as the default reference for observing the footprint as
        // a barrier, this can be overriden by the MapTraversal to enable modelling of the key
        public bool hasObject = false;
        public int[] objectPickupArea = new int[4]; // ONLY used for AQ/DIR-AQ actions

        // Definte *additional* objects for the object rewriting
        // write the strips as rectangles, e.g. the castle door
        public List<int[]> objectsEdgeRewriting = new();
        // where true objects are written in as UNCONDITIONAL barriers,
        // otherwise objects are only used for rewriting
        public List<bool> observeObjectBarrier = new();

        // actionTriggerRoom is the room number where the action tiles are used for room change
        public int actionTriggerRoom;

        // rectangular action areas that trigger a room change
        public List<int> actionAreasRoomNumbers = new();
        public List<int[]> actionAreasRoomChange = new();

        // rectangular action areas that sends Graham into the water
        public List<int[]> actionAreasWater = new();

        private RoomDefinition(int roomNr)
        {
            this.roomNr = roomNr;
        }

        public delegate (int, int) RewriteRule(int previousRoom, int x, int y, int footprintWidth);
        public RewriteRule rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
        {
            return (x, y);
        };

        public Dictionary<(int, int), bool> stopPoints = new();

        private int NorthBoundary = DEFAULT_LOWER_LIMIT;
        // the edge is not considered a boundary (should really fix these names though
        // they are confusing as hell)
        public int GetNorthBoundary()
        {
            return edgeNorth >= NorthBoundary ? int.MinValue : NorthBoundary;
        }
        public int GetWestBoundary()
        {
            return edgeWest >= 0 ? int.MinValue : 0;
        }
        public int GetEastBoundary()
        {
            return edgeEast <= ROOM_WIDTH ? int.MaxValue : ROOM_WIDTH;
        }
        public int GetSouthBoundary()
        {
            return edgeSouth <= ROOM_HEIGHT ? int.MaxValue : ROOM_HEIGHT;
        }

        public int MinTraversableY()
        {
            return Math.Max(edgeNorth, NorthBoundary);
        }

        // horizon must be supplied for the north edge
        public void SetNorthRoom(int roomNorth, int horizon)
        {
            this.roomNorth = roomNorth;
            this.edgeNorth = horizon;
        }

        public void SetEastRoom(int roomEast, int edgeEast = ROOM_WIDTH)
        {
            this.roomEast = roomEast;
            this.edgeEast = edgeEast;
        }
        public void SetSouthRoom(int roomSouth, int edgeSouth = ROOM_HEIGHT)
        {
            this.roomSouth = roomSouth;
            this.edgeSouth = edgeSouth;
        }
        public void SetWestRoom(int roomWest, int edgeWest = -1)
        {
            this.roomWest = roomWest;
            this.edgeWest = edgeWest;
        }

        public void AddSpecialBarrier(int x0, int y0, int x1, int y1)
        {
            CheckRectangleBounds(x0, y0, x1, y1);
            int[] rect = { x0, y0, x1, y1 };
            specialBarriers.Add(rect);
        }

        /*
         * Having more than one room trigger is rare, the only known instances are rooms 54 and 69
         */
        public void AddRoomTrigger(int roomNr, int x0, int y0, int x1, int y1)
        {
            CheckRectangleBounds(x0, y0, x1, y1);
            int[] rect = { x0, y0, x1, y1 };
            CheckActionAreaOverlaps(rect);
            actionAreasRoomNumbers.Add(roomNr);
            actionAreasRoomChange.Add(rect);
        }

        public void SetWaterControlAsRoomTrigger(int roomNr)
        {
            if (actionTriggerRoom > 0)
            {
                throw new Exception("cannot have both water and action tiles as room trigger");
            }
            actionTriggerRoom = roomNr;
            useWaterControl = WATER_TILES_ROOM_TRIGGER;
        }

        public void AddWaterTrigger(int x0, int y0, int x1, int y1)
        {
            CheckRectangleBounds(x0, y0, x1, y1);
            int[] rect = { x0, y0, x1, y1 };
            actionAreasWater.Add(rect);
        }

        public void IgnoreWaterTiles()
        {
            useWaterControl = WATER_TILES_IGNORED;
        }

        public void AddObjectEdgeRewrite(int x0, int y0, int x1, int y1, bool observeBarrier)
        {
            int[] rect = { x0, y0, x1, y1 };
            objectsEdgeRewriting.Add(rect);
            observeObjectBarrier.Add(observeBarrier);
        }

        public static void CheckRectangleBounds(int x0, int y0, int x1, int y1)
        {
            if (x0 > x1)
            {
                throw new Exception($"error - x0({x0}) is greater than x1({x1}), (x0,y0) should be the top-left edge");
            }
            if (y0 > y1)
            {
                throw new Exception($"error - y0({y0}) is greater than y1({y1}), (x0,y0) should be the top-left edge");
            }
            if (x0 < 0 || x0 >= ROOM_WIDTH || x1 < 0 || x1 >= ROOM_WIDTH)
            {
                throw new Exception($"rectangle bounds exceeds room dimensions ({x0},{y0})-({x1},{y1})");
            }
            if (y0 < 0 || y0 >= ROOM_HEIGHT || y1 < 0 || y1 >= ROOM_HEIGHT)
            {
                throw new Exception($"rectangle bounds exceeds room dimensions ({x0},{y0})-({x1},{y1})");
            }
        }

        private void CheckActionAreaOverlaps(int[] rect)
        {
            foreach (var actionArea in actionAreasRoomChange)
            {
                // Debug.WriteLine($"({actionArea[0]},{actionArea[1]})-({actionArea[2]},{actionArea[3]})");
                if (RoomControl.CheckPixelContained(rect[0], rect[1], actionArea) ||
                    RoomControl.CheckPixelContained(rect[2], rect[3], actionArea))
                {
                    throw new Exception($"A supplied action area overlaps with an existing one");
                }
            }
            foreach (var actionArea in actionAreasWater)
            {
                if (RoomControl.CheckPixelContained(rect[0], rect[1], actionArea) ||
                    RoomControl.CheckPixelContained(rect[2], rect[3], actionArea))
                {
                    throw new Exception($"A supplied action area overlaps with an existing one");
                }
            }
        }

        public static RoomDefinition GetRoomDefinition(int roomNr)
        {
            return GetRoomDefinition(roomNr, new GameState());
        }

        public static RoomDefinition GetRoomDefinition(int roomNr, GameState gameState)
        {
            RoomDefinition roomDefinition = new(roomNr);
            switch (roomNr)
            {
                default:
                    throw new Exception($"room {roomNr} not implemented!");

                case 1:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetNorthRoom(16, horizon: 50);
                    roomDefinition.SetWestRoom(2);
                    roomDefinition.SetEastRoom(8);
                    roomDefinition.AddObjectEdgeRewrite(120, 161, 134, 161, observeBarrier: false); // alligator 1
                    roomDefinition.AddObjectEdgeRewrite(72, 166, 86, 166, observeBarrier: false); // alligator 2
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 8 && y > 150)
                        {
                            return footprintWidth == 19 ? (141, 144) : (148, 150);
                        }
                        return (x, y);
                    };
                    break;

                case 2:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetNorthRoom(15, horizon: 50);
                    roomDefinition.SetWestRoom(3);
                    roomDefinition.SetEastRoom(1);
                    roomDefinition.AddObjectEdgeRewrite(120, 161, 134, 161, observeBarrier: false); // alligator 1
                    roomDefinition.AddObjectEdgeRewrite(72, 166, 86, 166, observeBarrier: false); // alligator 2
                    roomDefinition.AddObjectEdgeRewrite(104, 120, 125, 120, observeBarrier: true); // castle door

                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 3 && y > 150)
                        {
                            return (2, 150);
                        }
                        if (previousRoom == 55)
                        {
                            return (110, 122);
                        }
                        return (x, y);
                    };
                    break;

                case 3:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(14, horizon: 46);
                    roomDefinition.SetWestRoom(4);
                    roomDefinition.SetEastRoom(2);
                    roomDefinition.SetSouthRoom(46);
                    if (!gameState.HasBoulderMoved())
                    {
                        roomDefinition.AddSpecialBarrier(121, 121, 132, 132);
                        roomDefinition.AddObjectEdgeRewrite(117, 133, 135, 133, observeBarrier: true);
                    }
                    if (gameState.HasBoulderMoved())
                    {
                        roomDefinition.AddObjectEdgeRewrite(117, 145, 135, 145, observeBarrier: true);
                    }
                    break;

                case 4:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(13, horizon: 55);
                    roomDefinition.SetWestRoom(5);
                    roomDefinition.SetEastRoom(3);
                    roomDefinition.SetSouthRoom(45);
                    roomDefinition.AddWaterTrigger(0, 78, 22, 158);
                    roomDefinition.AddWaterTrigger(23, 151, 25, 162);
                    roomDefinition.AddWaterTrigger(26, 147, 42, 152);
                    roomDefinition.AddWaterTrigger(43, 141, 60, 149);
                    roomDefinition.AddWaterTrigger(16, 129, 38, 146);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 13 && x < 60 ? (70, 56) : (x, y);
                    };
                    break;

                case 5:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(12, horizon: 55);
                    roomDefinition.SetWestRoom(6);
                    roomDefinition.SetEastRoom(4);
                    roomDefinition.SetSouthRoom(44);
                    roomDefinition.AddWaterTrigger(71, 110, 88, 139);
                    roomDefinition.AddWaterTrigger(104, 81, 159, 163);
                    roomDefinition.AddWaterTrigger(89, 110, 103, 142);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 4 &&
                            RoomControl.CheckPixelContained(x, y, new int[] { 145, 55, 159, 80 }))
                        {
                            return (140, 75);
                        }
                        if (previousRoom == 4 &&
                            RoomControl.CheckPixelContained(x, y, new int[] { 145, 160, 159, 167 }))
                        {
                            return (150, 164);
                        }
                        if (previousRoom == 12 && x > 105)
                        {
                            return (90, 56);
                        }
                        if (previousRoom == 44 && x > 90)
                        {
                            return (82, 165);
                        }
                        return (x, y);
                    };
                    break;

                case 6:
                    roomDefinition.SetNorthRoom(11, horizon: 40);
                    roomDefinition.SetWestRoom(7);
                    roomDefinition.SetEastRoom(5);
                    roomDefinition.SetSouthRoom(43);
                    break;

                case 7:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(10, horizon: 46);
                    roomDefinition.SetWestRoom(8);
                    roomDefinition.SetEastRoom(6);
                    roomDefinition.SetSouthRoom(42);
                    roomDefinition.AddWaterTrigger(9, 45, 19, 52);
                    roomDefinition.AddWaterTrigger(4, 53, 19, 59);
                    roomDefinition.stopPoints.Add((2, 47), true);
                    // adding this artificial point to capture pathways across to the gnome
                    // but has to enter on it for it to work ....
                    roomDefinition.stopPoints.Add((0, 160), true);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        // let's also add this then
                        if (previousRoom == 42 && x == 7 && y == 167)
                        {
                            return (0, 160);
                        }
                        if (previousRoom == 8 && y < 70)
                        {
                            return (2, 47);
                        }
                        if (previousRoom == 10 && x > 19)
                        {
                            return (34, 47);
                        }
                        if (previousRoom == 10 && x < 20)
                        {
                            return (2, 47);
                        }
                        return (x, y);
                    };
                    break;

                case 8:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(9, horizon: 43);
                    roomDefinition.SetWestRoom(1);
                    roomDefinition.SetEastRoom(7);
                    roomDefinition.SetSouthRoom(41);
                    roomDefinition.AddWaterTrigger(84, 59, 113, 78);
                    roomDefinition.AddWaterTrigger(102, 79, 113, 94);
                    roomDefinition.AddWaterTrigger(100, 95, 113, 128);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 41 ? (45, 166) : (x, y);
                    };
                    break;

                case 9:
                    roomDefinition.SetNorthRoom(24, horizon: 46);
                    roomDefinition.SetWestRoom(16);
                    roomDefinition.SetEastRoom(10);
                    roomDefinition.SetSouthRoom(8);
                    break;

                case 10:
                    roomDefinition.SetNorthRoom(23, horizon: 41);
                    roomDefinition.SetWestRoom(9);
                    roomDefinition.SetEastRoom(11);
                    roomDefinition.SetSouthRoom(7);
                    if (gameState.IsGoatPresent())
                    {
                        roomDefinition.AddObjectEdgeRewrite(94, 97, 113, 97, observeBarrier: false); // goat
                    }
                    break;

                // the edge rewriting to this room are particularly strange when drowning glitched
                // not sure how to fix that
                case 11:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(22, horizon: 41);
                    roomDefinition.SetWestRoom(10);
                    roomDefinition.SetEastRoom(12);
                    roomDefinition.SetSouthRoom(6);
                    if (gameState.IsGoatPresent())
                    {
                        roomDefinition.AddObjectEdgeRewrite(16, 156, 43, 156, observeBarrier: true); // gate
                        roomDefinition.AddObjectEdgeRewrite(42, 97, 60, 97, observeBarrier: false); // goat
                    }
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (gameState.IsGoatPresent() &&
                            footprintWidth == GameState.FOOTPRINT_WIDTH_NORMAL &&
                            previousRoom == 6 && x >= 46 && x <= 48)
                        {
                            return (35, 155);
                        }
                        return (x, y);
                    };
                    break;

                case 12:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(21, horizon: 46);
                    roomDefinition.SetWestRoom(11);
                    roomDefinition.SetEastRoom(13);
                    roomDefinition.SetSouthRoom(5);
                    if (gameState.IsBucketPresent())
                    {
                        roomDefinition.AddObjectEdgeRewrite(36, 119, 49, 119, observeBarrier: true);
                    }
                    break;

                // It appears that in some rooms the player water glitches that occur between
                // rooms are patched in the logic scripts. It is done in this room and strictly
                // speaking makes available a few more pixels that are available for traversal
                case 13:
                    roomDefinition.SetNorthRoom(20, horizon: 46);
                    roomDefinition.SetWestRoom(12);
                    roomDefinition.SetEastRoom(14);
                    roomDefinition.SetSouthRoom(4);
                    break;

                case 14:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(19, horizon: 40);
                    roomDefinition.SetWestRoom(13);
                    roomDefinition.SetEastRoom(15);
                    roomDefinition.SetSouthRoom(3);
                    break;

                case 15:
                    roomDefinition.SetNorthRoom(18, horizon: 46);
                    roomDefinition.SetWestRoom(14);
                    roomDefinition.SetEastRoom(16);
                    roomDefinition.SetSouthRoom(2);
                    break;

                case 16:
                    roomDefinition.SetNorthRoom(17, horizon: 46);
                    roomDefinition.SetWestRoom(15);
                    roomDefinition.SetEastRoom(9);
                    roomDefinition.SetSouthRoom(1);
                    break;

                case 17:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(32, horizon: 56);
                    roomDefinition.SetWestRoom(18);
                    roomDefinition.SetEastRoom(24);
                    roomDefinition.SetSouthRoom(16);
                    roomDefinition.AddWaterTrigger(32, 103, 103, 123);
                    roomDefinition.AddWaterTrigger(0, 96, 58, 108);
                    roomDefinition.AddWaterTrigger(34, 87, 68, 97);
                    roomDefinition.AddWaterTrigger(13, 88, 37, 97);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 32 && x < 50)
                        {
                            return (2, 59);
                        }
                        if (previousRoom == 32 && x >= 50)
                        {
                            return (109, 66);
                        }
                        if (previousRoom == 16 &&
                            RoomControl.CheckPixelContained(x, y, new int[] { 0, 164, 88, 167 }))
                        {
                            return (88, 165);
                        }
                        return (x, y);
                    };
                    break;

                case 18:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(31, horizon: 46);
                    roomDefinition.SetWestRoom(19);
                    roomDefinition.SetEastRoom(17);
                    roomDefinition.SetSouthRoom(15);
                    roomDefinition.AddWaterTrigger(129, 95, 159, 105);
                    roomDefinition.AddWaterTrigger(133, 80, 145, 94);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 15 && x > 100)
                        {
                            return (100, 165);
                        }
                        return (x, y);
                    };
                    break;

                case 19:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(30, horizon: 41);
                    roomDefinition.SetWestRoom(20);
                    roomDefinition.SetEastRoom(18);
                    roomDefinition.SetSouthRoom(14);
                    if (gameState.IsStairWayDoorClosed())
                    {
                        roomDefinition.AddObjectEdgeRewrite(100, 120, 120, 120, observeBarrier: false);
                    }
                    // If door is open ignore blocks and use room trigger
                    if (!gameState.IsStairWayDoorClosed())
                    {
                        roomDefinition.useBlockControl = BLOCKS_IGNORED;
                        roomDefinition.AddRoomTrigger(66, 99, 93, 114, 94);
                    }
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 66 ? (102, 98) : (x, y);
                    };
                    break;

                case 20:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(29, horizon: 53);
                    roomDefinition.SetWestRoom(21);
                    roomDefinition.SetEastRoom(19);
                    roomDefinition.SetSouthRoom(13);
                    roomDefinition.AddWaterTrigger(100, 109, 115, 135);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 29 && x < 50)
                        {
                            return (12, 54);
                        }
                        if (previousRoom == 29 && x >= 50)
                        {
                            return (122, 54);
                        }
                        if (previousRoom == 13)
                        {
                            if (RoomControl.CheckPixelContained(x, y, new int[] { 0, 164, 75, 167 }))
                            {
                                return (37, 165);
                            }
                            else
                            {
                                return (120, 165);
                            }
                        }
                        return (x, y);
                    };
                    break;

                case 21:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(28, horizon: 46);
                    roomDefinition.SetWestRoom(22);
                    roomDefinition.SetEastRoom(20);
                    roomDefinition.SetSouthRoom(12);
                    break;

                case 22:
                    roomDefinition.SetNorthRoom(27, horizon: 41);
                    roomDefinition.SetWestRoom(23);
                    roomDefinition.SetEastRoom(21);
                    roomDefinition.SetSouthRoom(11);
                    roomDefinition.AddRoomTrigger(-2, 148, 71, 149, 88);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 21 && x == 154 && y == 95)
                        {
                            return (153, 96);
                        }
                        if (previousRoom == 21 && y > 78 && y < 96)
                        {
                            return (x, 96);
                        }
                        return (x, y);
                    };
                    break;

                case 23:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(26, horizon: 46);
                    roomDefinition.SetWestRoom(24);
                    roomDefinition.SetEastRoom(22);
                    roomDefinition.SetSouthRoom(10);
                    roomDefinition.AddWaterTrigger(69, 76, 103, 131);
                    roomDefinition.AddWaterTrigger(104, 96, 123, 120);
                    roomDefinition.AddWaterTrigger(124, 96, 132, 105);
                    roomDefinition.AddWaterTrigger(120, 106, 125, 111);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 26 && x < 50)
                        {
                            return (10, 50);
                        }
                        if (previousRoom == 26 && x >= 50)
                        {
                            return (116, 50);
                        }
                        return (x, y);
                    };
                    break;

                case 24:
                    roomDefinition.SetNorthRoom(25, horizon: 46);
                    roomDefinition.SetWestRoom(17);
                    roomDefinition.SetEastRoom(23);
                    roomDefinition.SetSouthRoom(9);
                    if (gameState.IsCloverPresent())
                    {
                        roomDefinition.objectFootprint = new int[] { 52, 134, 59, 134 };
                        roomDefinition.hasObject = true;
                        roomDefinition.objectPickupArea = CLOVER_PICKUP_AREA;
                    }
                    break;

                case 25:
                    roomDefinition.useActionControl = ACTION_TILE_IGNORED;
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(40, horizon: DEFAULT_LOWER_LIMIT);
                    roomDefinition.SetWestRoom(32);
                    roomDefinition.SetEastRoom(26);
                    roomDefinition.SetSouthRoom(24);
                    // adding in a barrrier stopping player from crossing bridge
                    if (gameState.AreTrollBridgesClosed())
                    {
                        roomDefinition.AddSpecialBarrier(20, 76, 59, 78);
                    }
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 32 && y > 83 && y < 101)
                        {
                            return (1, 100);
                        }
                        return (x, y);
                    };
                    break;

                case 26:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(39, horizon: 46);
                    roomDefinition.SetWestRoom(25);
                    roomDefinition.SetEastRoom(27);
                    roomDefinition.SetSouthRoom(23);
                    break;

                case 27:
                    roomDefinition.SetNorthRoom(38, horizon: 46);
                    roomDefinition.SetWestRoom(26);
                    roomDefinition.SetEastRoom(28);
                    roomDefinition.SetSouthRoom(22);
                    break;

                case 28:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(37, horizon: 46);
                    roomDefinition.SetWestRoom(27);
                    roomDefinition.SetEastRoom(29);
                    roomDefinition.SetSouthRoom(21);
                    roomDefinition.AddObjectEdgeRewrite(44, 128, 62, 128, true);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 65)
                        {
                            return (48, 132);
                        }
                        if (previousRoom == 27 && y > 94)
                        {
                            return (1, 155);
                        }
                        return (x, y);
                    };
                    break;

                case 29:
                    roomDefinition.SetNorthRoom(36, horizon: DEFAULT_LOWER_LIMIT);
                    roomDefinition.SetWestRoom(28);
                    roomDefinition.SetEastRoom(30);
                    roomDefinition.SetSouthRoom(20);
                    break;

                case 30:
                    roomDefinition.SetNorthRoom(35, horizon: 50);
                    roomDefinition.SetWestRoom(29);
                    roomDefinition.SetEastRoom(31);
                    roomDefinition.SetSouthRoom(19);
                    break;

                case 31:
                    roomDefinition.SetNorthRoom(34, horizon: 50);
                    roomDefinition.SetWestRoom(30);
                    roomDefinition.SetEastRoom(32);
                    roomDefinition.SetSouthRoom(18);
                    if (gameState.IsBowlPresent())
                    {
                        roomDefinition.objectFootprint = new int[] { 125, 138, 138, 138 };
                        roomDefinition.hasObject = true;
                        roomDefinition.objectPickupArea = BOWL_PICKUP_AREA;
                    }
                    break;

                case 32:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(33, horizon: 40);
                    roomDefinition.SetWestRoom(31);
                    roomDefinition.SetEastRoom(25);
                    roomDefinition.SetSouthRoom(17);
                    roomDefinition.AddWaterTrigger(110, 53, 133, 83);
                    roomDefinition.AddWaterTrigger(100, 39, 106, 57);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 33)
                        {
                            if (x > 130)
                            {
                                return (140, 41);
                            }
                            if (x > 88 && x < 131)
                            {
                                return (88, 41);
                            }
                        }
                        return (x, y);
                    };
                    break;

                case 33:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(48, horizon: 43);
                    roomDefinition.SetWestRoom(34);
                    roomDefinition.SetEastRoom(40);
                    roomDefinition.SetSouthRoom(32);
                    roomDefinition.AddWaterTrigger(14, 66, 54, 107);
                    roomDefinition.AddWaterTrigger(99, 64, 145, 82);
                    roomDefinition.AddWaterTrigger(114, 113, 133, 136);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 32 && x > 103)
                        {
                            return footprintWidth == 19 ? (141, 165) : (143, 166);
                        }
                        return (x, y);
                    };
                    break;

                case 34:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(47, horizon: 45);
                    roomDefinition.SetWestRoom(35);
                    roomDefinition.SetEastRoom(33);
                    roomDefinition.SetSouthRoom(31);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 47 && x > 130 ? (135, 47) : (x, y);
                    };
                    break;

                case 35:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(46, horizon: 44);
                    roomDefinition.SetWestRoom(36);
                    roomDefinition.SetEastRoom(34);
                    roomDefinition.SetSouthRoom(30);
                    break;

                case 36:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    // shield cave backdoor. Blocks may be observed or not depending on whether graham is mini
                    // roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(45, horizon: 42);
                    roomDefinition.SetWestRoom(37);
                    roomDefinition.SetEastRoom(35);
                    roomDefinition.SetSouthRoom(29);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 78 ? (88, 131) : (x, y);
                    };
                    break;

                case 37:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(44, horizon: 55);
                    roomDefinition.SetWestRoom(38);
                    roomDefinition.SetEastRoom(36);
                    roomDefinition.SetSouthRoom(28);
                    roomDefinition.AddWaterTrigger(83, 108, 131, 143);
                    roomDefinition.AddWaterTrigger(37, 101, 82, 145);
                    roomDefinition.AddWaterTrigger(48, 37, 94, 100);
                    roomDefinition.AddWaterTrigger(39, 54, 100, 80);
                    roomDefinition.AddWaterTrigger(85, 146, 108, 155);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 44)
                        {
                            if (x < 75)
                            {
                                return (10, 57);
                            }
                            if (x > 74)
                            {
                                return (140, 57);
                            }
                        }
                        if (previousRoom == 28 && x < 75)
                        {
                            return (5, 164);
                        }
                        if (previousRoom == 28 && x >= 75)
                        {
                            return footprintWidth == 19 ? (141, 162) : (144, 164);
                        }

                        return (x, y);
                    };
                    break;

                case 38:
                    roomDefinition.SetNorthRoom(43, horizon: 40);
                    roomDefinition.SetWestRoom(39);
                    roomDefinition.SetEastRoom(37);
                    roomDefinition.SetSouthRoom(27);
                    break;

                case 39:
                    roomDefinition.useActionControl = ACTION_TILE_IGNORED;
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(42, horizon: 41);
                    roomDefinition.SetWestRoom(40);
                    roomDefinition.SetEastRoom(38);
                    roomDefinition.SetSouthRoom(26);
                    roomDefinition.AddWaterTrigger(24, 44, 68, 58);
                    roomDefinition.AddWaterTrigger(72, 84, 72, 87);
                    // adding in a barrrier stopping player from crossing bridge
                    if (gameState.AreTrollBridgesClosed())
                    {
                        roomDefinition.AddSpecialBarrier(52, 60, 56, 78);
                    }
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 26 && x >= 90 && x <= 95)
                        {
                            switch (x)
                            {
                                case 90:
                                    return (89, 167);
                                case 91:
                                    return (89, 166);
                                case 92:
                                    return (89, 166);
                                case 93:
                                    return (95, 165);
                                case 94:
                                    return (96, 167);
                                case 95:
                                    return (96, 167);
                            }
                        }
                        return (x, y);
                    };
                    break;

                case 40:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(41, horizon: 42);
                    roomDefinition.SetWestRoom(33);
                    roomDefinition.SetEastRoom(39);
                    roomDefinition.SetSouthRoom(25);
                    if (gameState.IsKeyPresent())
                    {
                        roomDefinition.objectFootprint = new int[] { 38, 118, 47, 118 };
                        roomDefinition.hasObject = true;
                        // roomDefinition.objectPickupArea = KEY_PICKUP_AREA;
                    }
                    if (gameState.IsGnomePresent())
                    {
                        roomDefinition.AddObjectEdgeRewrite(33, 104, 50, 104, observeBarrier: false); // gnome
                    }
                    break;

                case 41:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.useActionControl = ACTION_TILE_IGNORED;
                    roomDefinition.SetNorthRoom(8, horizon: 42);
                    roomDefinition.SetWestRoom(48);
                    roomDefinition.SetEastRoom(42);
                    roomDefinition.SetSouthRoom(40);
                    // FIXME -- better to add this in the definitions for the path traverser
                    // Take out this pixel because it's messing up my path searches
                    roomDefinition.AddWaterTrigger(92,120,92,120);
                    // adding in a barrrier stopping player from crossing bridge
                    if (gameState.AreTrollBridgesClosed())
                    {
                        roomDefinition.AddSpecialBarrier(92, 121, 127, 121);
                    }
                    break;

                case 42:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(7, horizon: 50);
                    roomDefinition.SetWestRoom(41);
                    roomDefinition.SetEastRoom(43);
                    roomDefinition.SetSouthRoom(39);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 7 && x > 99)
                        {
                            return (99, 52);
                        }
                        if (previousRoom == 39 && x < 56)
                        {
                            return (6, 165);
                        }
                        if (previousRoom == 39 && x > 74)
                        {
                            return (109, 165);
                        }
                        if (previousRoom == 43 && x > 123)
                        {
                            return (140, 58);
                        }
                        if (previousRoom == 43 && x > 122)
                        {
                            return (147, 150);
                        }
                        return (x, y);
                    };
                    break;

                case 43:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetNorthRoom(6, horizon: 46);
                    roomDefinition.SetWestRoom(42);
                    roomDefinition.SetEastRoom(44);
                    roomDefinition.SetSouthRoom(38);
                    roomDefinition.AddWaterTrigger(96, 82, 115, 99);
                    roomDefinition.AddWaterTrigger(100, 108, 115, 120);
                    roomDefinition.AddWaterTrigger(30, 113, 67, 134);
                    roomDefinition.AddWaterTrigger(90, 121, 112, 130);
                    roomDefinition.AddWaterTrigger(30, 135, 67, 139);
                    roomDefinition.AddWaterTrigger(90, 131, 107, 142);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 6 ? (121, 47) : (x, y);
                    };
                    break;

                case 44:
                    roomDefinition.SetNorthRoom(5, horizon: 41);
                    roomDefinition.SetWestRoom(43);
                    roomDefinition.SetEastRoom(45);
                    roomDefinition.SetSouthRoom(37);
                    roomDefinition.AddRoomTrigger(79, 126, 119, 128, 127);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 79 ? (118, 123) : (x, y);
                    };
                    break;

                case 45:
                    roomDefinition.SetNorthRoom(4, horizon: 43);
                    roomDefinition.SetWestRoom(44);
                    roomDefinition.SetEastRoom(46);
                    roomDefinition.SetSouthRoom(36);
                    break;

                case 46:
                    roomDefinition.SetNorthRoom(3, horizon: 42);
                    roomDefinition.SetWestRoom(45);
                    roomDefinition.SetEastRoom(47);
                    roomDefinition.SetSouthRoom(35);
                    break;

                case 47:
                    roomDefinition.SetWestRoom(46);
                    roomDefinition.SetEastRoom(48);
                    roomDefinition.SetSouthRoom(34);
                    if (gameState.IsMushroomPresent())
                    {
                        roomDefinition.objectFootprint = new int[] { 83, 80, 92, 80 };
                        roomDefinition.hasObject = true;
                        roomDefinition.objectPickupArea = MUSHROOM_PICKUP_AREA;
                    }
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 34 && x >= 77)
                        {
                            return x <= 112 ? (71, 164) : (142, 164);
                        }
                        else
                        {
                            return (x, y);
                        }
                    };
                    break;

                case 48:
                    roomDefinition.SetWestRoom(47);
                    roomDefinition.SetEastRoom(41);
                    roomDefinition.SetSouthRoom(33);
                    roomDefinition.actionTriggerRoom = 73;
                    roomDefinition.useWaterControl = WATER_TILES_ROOM_TRIGGER;
                    break;

                // top of well
                case 49:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    break;

                // dragon cave, rooms 50,51
                case 50:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    break;

                case 51:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    break;

                // bottomr of well
                case 52:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    break;

                // castle, rooms 53-54
                case 53:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetEastRoom(54);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 54)
                        {
                            return (154, 120);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                case 54:
                    roomDefinition.AddRoomTrigger(roomNr: 53, 0, 0, 1, 167);
                    roomDefinition.AddRoomTrigger(roomNr: 55, 2, 166, 159, 167);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 53)
                        {
                            return (x + 2, y);
                        }
                        if (previousRoom == 55)
                        {
                            return (x, y - 2);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                case 55:
                    roomDefinition.SetNorthRoom(54, horizon: 70);
                    roomDefinition.AddRoomTrigger(roomNr: 2, 0, 166, 159, 167);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 2)
                        {
                            return (75, 164);
                        }
                        if (previousRoom == 54)
                        {
                            return (79, 71);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                // rooms 56-62 + 64 cloud area
                case 56:
                    roomDefinition.NorthBoundary = 30;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    break;

                case 57:
                    roomDefinition.NorthBoundary = 30;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetWestRoom(56);
                    roomDefinition.SetEastRoom(58);
                    roomDefinition.SetSouthRoom(60);
                    break;

                // room with the giant
                case 58:
                    roomDefinition.NorthBoundary = 30;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetWestRoom(57);
                    roomDefinition.SetEastRoom(59);
                    roomDefinition.SetSouthRoom(61);
                    break;

                case 59:
                    roomDefinition.NorthBoundary = 30;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetWestRoom(58);
                    roomDefinition.SetSouthRoom(62);
                    roomDefinition.AddRoomTrigger(69, 135, 94, 136, 120);

                    // R: can't remember why I put this, possibly for testing pathing, -2 as a room returns an 'invalid' path
                    // roomDefinition.AddRoomTrigger(-2, 135, 94, 136, 120);

                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 69)
                        {
                            return (116, 110);
                        }
                        else
                        {
                            return (x, y);
                        }
                    };

                    break;

                case 60:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetNorthRoom(57, horizon: 45);
                    roomDefinition.SetEastRoom(61);
                    break;

                case 61:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetNorthRoom(58, horizon: 60);
                    roomDefinition.SetWestRoom(60);
                    roomDefinition.SetEastRoom(62);
                    break;

                case 62:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetNorthRoom(59, horizon: 45);
                    roomDefinition.SetWestRoom(61);
                    break;

                // tree with golder egg
                case 63:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.SetSouthRoom(14);
                    break;

                // extreme west side, giant area
                case 64:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.NorthBoundary = 30;
                    roomDefinition.SetEastRoom(57);
                    break;

                // which's house
                case 65:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    break;

                case 66:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.IgnoreWaterTiles();
                    roomDefinition.SetNorthRoom(67, horizon: 45);
                    roomDefinition.AddRoomTrigger(19, 16, 165, 31, 166);
                    roomDefinition.stopPoints.Add((21, 162), true);
                    roomDefinition.stopPoints.Add((131, 47), true);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 19)
                        {
                            return (21, 162);
                        }
                        if (previousRoom == 67)
                        {
                            return (131, 47);
                        }
                        throw new Exception($"unknown room {previousRoom}");
                    };
                    break;

                case 67:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.IgnoreWaterTiles();
                    roomDefinition.SetNorthRoom(68, horizon: 40);
                    roomDefinition.AddRoomTrigger(66, 5, 150, 6, 167);
                    roomDefinition.stopPoints.Add((10, 151), true);
                    roomDefinition.stopPoints.Add((119, 42), true);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 66)
                        {
                            return (10, 151);
                        }
                        if (previousRoom == 68)
                        {
                            return (119, 42);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                case 68:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.IgnoreWaterTiles();
                    roomDefinition.SetNorthRoom(69, horizon: 40);
                    roomDefinition.AddRoomTrigger(67, 0, 166, 159, 167);
                    roomDefinition.stopPoints.Add((95, 159), true);
                    roomDefinition.stopPoints.Add((44, 42), true);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 67)
                        {
                            return (95, 159);
                        }
                        if (previousRoom == 69)
                        {
                            return (44, 42);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                case 69:
                    roomDefinition.useActionControl = ACTION_TILE_DEATH_TRIGGER;
                    roomDefinition.IgnoreWaterTiles();
                    roomDefinition.AddRoomTrigger(59, 0, 0, 1, 167);
                    roomDefinition.AddRoomTrigger(68, 138, 156, 155, 157);
                    roomDefinition.stopPoints.Add((143, 154), true);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 59)
                        {
                            return (18, 97);
                        }
                        if (previousRoom == 68)
                        {
                            return (143, 154);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                case 73:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.AddRoomTrigger(roomNr: 74, 0, 166, 159, 167);
                    roomDefinition.stopPoints.Add((90, 139), true);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 48 ? (90, 139) : (77, 165);
                    };
                    break;

                case 74:
                    roomDefinition.AddRoomTrigger(roomNr: 73, 65, 51, 100, 52);
                    roomDefinition.AddRoomTrigger(roomNr: 75, 0, 0, 1, 167);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 73)
                        {
                            return (80, 70);
                        }
                        if (previousRoom == 75)
                        {
                            return (x + 2, y);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                case 75:
                    roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetEastRoom(74);
                    //roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    //{
                    //    return previousRoom == 74 ? (x + 150, y) : (34, 125);
                    //};

                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 74)
                        {
                            return (x + 150, y);
                        }
                        if (previousRoom == 76)
                        {
                            return (34, 125);
                        }
                        throw new Exception("unknown room");
                    };

                    break;

                case 76:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.AddRoomTrigger(75, 128, 133, 129, 154);
                    roomDefinition.AddRoomTrigger(77, 0, 166, 159, 167);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 75)
                        {
                            return (113, 146);
                        }
                        if (previousRoom == 77)
                        {
                            return (73, 163);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                case 77:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.SetNorthRoom(76, horizon: 58);
                    roomDefinition.AddRoomTrigger(78, 0, 0, 1, 167);

                    // a way to disable transition to north room
                    // roomDefinition.AddWaterTrigger(0,59,159,59);

                    // 10 leprechauns
                    roomDefinition.AddSpecialBarrier(45, 72, 54, 77);
                    roomDefinition.AddSpecialBarrier(57, 72, 66, 72);
                    roomDefinition.AddSpecialBarrier(111, 72, 120, 78);
                    roomDefinition.AddSpecialBarrier(36, 84, 45, 88);
                    roomDefinition.AddSpecialBarrier(16, 132, 25, 140);
                    roomDefinition.AddSpecialBarrier(30, 144, 39, 150);
                    roomDefinition.AddSpecialBarrier(106, 144, 115, 144);
                    roomDefinition.AddSpecialBarrier(94, 156, 103, 156);
                    roomDefinition.AddSpecialBarrier(120, 156, 129, 156);
                    roomDefinition.AddSpecialBarrier(128, 156, 137, 156);
                    if (gameState.IsShieldPresent())
                    {
                        roomDefinition.objectFootprint = new int[] { 130, 128, 141, 128 };
                        roomDefinition.hasObject = true;
                        roomDefinition.objectPickupArea = SHIELD_PICKUP_AREA;
                    }
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (previousRoom == 76)
                        {
                            return (87, 67);
                        }
                        if (previousRoom == 78)
                        {
                            return (9, 95);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                // shield cave exit. Blocks are observed or not depending on whether graham is mini
                case 78:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    // roomDefinition.useBlockControl = BLOCKS_IGNORED;
                    roomDefinition.SetEastRoom(77);
                    roomDefinition.stopPoints.Add((145, 130), true);
                    roomDefinition.AddRoomTrigger(36, 0, 76, 1, 81);
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        if (footprintWidth > 13)
                        {
                            throw new Exception("this needs implementation");
                        }
                        if (previousRoom == 77)
                        {
                            return (145, 130);
                        }
                        throw new Exception("unknown room");
                    };
                    break;

                // woodcutter hut
                case 79:
                    roomDefinition.useBlockControl = BLOCKS_OBSERVED;
                    roomDefinition.useActionControl = ACTION_TILE_ROOM_TRIGGER;
                    roomDefinition.actionTriggerRoom = 44;
                    if (gameState.IsFiddlePresnt())
                    {
                        roomDefinition.objectFootprint = new int[] { 127, 155, 138, 155 };
                        roomDefinition.hasObject = true;
                        roomDefinition.objectPickupArea = FIDDLE_PICKUP_AREA;
                    }
                    roomDefinition.rewriteRule = (int previousRoom, int x, int y, int footprintWidth) =>
                    {
                        return previousRoom == 44 ? (24, 113) : (x, y);
                    };
                    break;

                // using the blank room to print empty grids
                case 80:
                    break;
            }
            return roomDefinition;
        }
    }
}
