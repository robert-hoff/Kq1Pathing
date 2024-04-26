using System.Diagnostics;

namespace Kq1Pathing.RoomPathing
{
    public class GameState
    {
        public static readonly int[] BOULDER_INTERACTION_AREA = { 105, 120, 121, 128 };
        public static readonly int[] DAGGER_PICKUP_AREA = { 112, 108, 133, 133 };
        public static readonly int[] BAG_PICKUP_AREA = { 0, 103, 35, 146 };
        public static readonly int[] WALNUT_PICKUP_AREA = { 9, 112, 98, 167 };
        public static readonly int[] CLOVER_PICKUP_AREA = { 36, 128, 70, 141 };
        public static readonly int[] BOWL_PICKUP_AREA = { 116, 128, 136, 148 };
        public static readonly int[] GNOME_INTERACTION_AREA = { 0, 86, 62, 137 };
        public static readonly int[] MUSHROOM_PICKUP_AREA = { 74, 70, 94, 90 };
        public static readonly int[] SHIELD_PICKUP_AREA = { 121, 118, 144, 138 };
        public static readonly int[] FIDDLE_PICKUP_AREA = { 120, 140, 158, 166 };
        public static readonly int[] STAIRCASE_INTERACTION_AREA = { 99, 96, 114, 96 };
        public static readonly int[] WELL_INTERACTION_AREA = { 10, 100, 75, 150 };
        public static readonly int[] WELL_LOWER_BUCKET_AREA = { 60, 108, 77, 140 };
        public static readonly int[] WELL_LOWER_ROPE_AREA = { 60, 108, 77, 129 };
        public static readonly int[] KING_JUMP_AREA = { 98, 88, 104, 96 };

        public static readonly int[] KEY_PICKUP_AREA1 = { 24, 114, 56, 120 };
        public static readonly int[] KEY_PICKUP_AREA2 = { 57, 115, 58, 120 };
        public static readonly int[] KEY_PICKUP_AREA3 = { 41, 131, 47, 137 };

        public static readonly (int x, int y) START_POS = (110, 100);
        public static readonly (int x, int y) KEY_POS = (40, 118);
        public static readonly (int x, int y) SHIELD_EXIT_POS = (88, 131); // assumes room 36
        public static readonly (int x, int y) DRAGON_EXIT_POS = (113, 83); // assumes room 22
        public static readonly (int x, int y) CHEST_END_POS = (27, 143); // assumes room 66
        public static readonly int KEY_INTERACTIVE_DISTANCE = 20;

        public const int BOULDER_MOVED = 1;
        public const int GOAT_PRESENT = 2;
        public const int GATE_CLOSED = 3;
        public const int BUCKET_PRESENT = 4;
        public const int STAIRWAY_DOOR_CLOSED = 5;
        public const int CLOVER_PRESENT = 6;
        public const int TROLL_BRIDGES_CLOSED = 7;
        public const int BOWL_PRESENT = 8;
        public const int GNOME_PRESENT = 9;
        public const int KEY_PRESENT = 10;
        public const int MUSHROOM_PRESENT = 11;
        public const int SHIELD_PRESENT = 12;
        public const int FIDDLE_PRESENT = 13;

        public const int START_ROOM = 1;
        public const int DAGGER_ROOM = 3;
        public const int BAG_ROOM = 6;
        public const int FAIRY_ROOM = 9;
        public const int CARROT_EAST_ROOM = 16;
        public const int DRAGON_EXIT_ROOM = 22;
        public const int CLOVER_ROOM = 24;
        public const int NUT_ROOM = 24;
        public const int SHIELD_EXIT_ROOM = 36;
        public const int GNOME_ROOM = 40;
        public const int MUSHROOM_ROOM = 47;
        public const int STAIRCASE_BOTTOM_ROOM = 66;
        public const int SHIELD_CAVE_STARTROOM = 73;

        public const int FOOTPRINT_WIDTH_NORMAL = 6;
        public const int FOOTPRINT_WIDTH_MINI = 4; // when eating the mushroom
        public const int FOOTPRINT_WIDTH_FALLING = 11;
        public const int FOOTPRINT_WIDTH_SWIMMING = 13;
        public const int FOOTPRINT_WIDTH_DROWNING = 19;

        public int footprintWidth;
        private bool[] stateDescription = new bool[14];

        public GameState(int footprintWidth = FOOTPRINT_WIDTH_NORMAL)
        {
            // Debug.WriteLine($"{footprintWidth}");

            this.footprintWidth = footprintWidth;
            stateDescription[BOULDER_MOVED] = false;
            stateDescription[GOAT_PRESENT] = true;
            stateDescription[GATE_CLOSED] = true;
            stateDescription[BUCKET_PRESENT] = true;
            stateDescription[STAIRWAY_DOOR_CLOSED] = true;
            stateDescription[CLOVER_PRESENT] = true;
            stateDescription[TROLL_BRIDGES_CLOSED] = true;
            stateDescription[BOWL_PRESENT] = true;
            stateDescription[GNOME_PRESENT] = true;
            stateDescription[KEY_PRESENT] = false;
            stateDescription[MUSHROOM_PRESENT] = true;
            stateDescription[SHIELD_PRESENT] = true;
            stateDescription[FIDDLE_PRESENT] = true;
        }

        public void SetBoulderMoved()
        {
            stateDescription[BOULDER_MOVED] = true;
        }
        public bool HasBoulderMoved()
        {
            return stateDescription[BOULDER_MOVED];
        }
        public bool IsGoatPresent()
        {
            return stateDescription[GOAT_PRESENT];
        }
        public void SetGateOpen()
        {
            stateDescription[GATE_CLOSED] = false;
        }
        public void SetBucketAcquired()
        {
            stateDescription[BUCKET_PRESENT] = false;
        }
        public bool IsBucketPresent()
        {
            return stateDescription[BUCKET_PRESENT];
        }
        public void SetStairwayDoorOpen()
        {
            stateDescription[STAIRWAY_DOOR_CLOSED] = false;
        }
        public bool IsStairWayDoorClosed()
        {
            return stateDescription[STAIRWAY_DOOR_CLOSED];
        }
        public void SetCloverAcquired()
        {
            stateDescription[CLOVER_PRESENT] = false;
        }
        public bool IsCloverPresent()
        {
            return stateDescription[CLOVER_PRESENT];
        }
        public void SetTrollBridgesOpen()
        {
            stateDescription[TROLL_BRIDGES_CLOSED] = false;
        }
        public void SetTrollBridgesClosed()
        {
            stateDescription[TROLL_BRIDGES_CLOSED] = false;
        }
        public bool AreTrollBridgesClosed()
        {
            return stateDescription[TROLL_BRIDGES_CLOSED];
        }
        public void SetBowlAcquired()
        {
            stateDescription[BOWL_PRESENT] = false;
        }
        public bool IsBowlPresent()
        {
            return stateDescription[BOWL_PRESENT];
        }
        public void SetKeyDropped()
        {
            stateDescription[GNOME_PRESENT] = false;
            stateDescription[KEY_PRESENT] = true;
        }
        public bool IsKeyPresent()
        {
            return stateDescription[KEY_PRESENT];
        }
        public bool IsGnomePresent()
        {
            return stateDescription[GNOME_PRESENT];
        }
        public void SetKeyAcquired()
        {
            stateDescription[KEY_PRESENT] = false;
        }
        public void SetMushroomAcquired()
        {
            stateDescription[MUSHROOM_PRESENT] = false;
        }
        public bool IsMushroomPresent()
        {
            return stateDescription[MUSHROOM_PRESENT];
        }
        public void SetShieldAcquired()
        {
            stateDescription[SHIELD_PRESENT] = false;
        }
        public bool IsShieldPresent()
        {
            return stateDescription[SHIELD_PRESENT];
        }
        public void SetFiddleAcquired()
        {
            stateDescription[FIDDLE_PRESENT] = false;
        }
        public bool IsFiddlePresnt()
        {
            return stateDescription[FIDDLE_PRESENT];
        }

        public GameState CloneState()
        {
            GameState gameState = new GameState();
            gameState.footprintWidth = footprintWidth;
            for (int i = 0; i < stateDescription.Length; i++)
            {
                gameState.stateDescription[i] = stateDescription[i];
            }
            return gameState;
        }
    }
}
