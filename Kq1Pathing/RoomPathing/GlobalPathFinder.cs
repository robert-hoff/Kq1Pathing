using System.Diagnostics;
using static Kq1Pathing.RoomPathing.GameState;
using static Kq1Pathing.RoomPathing.MapTraversal;

namespace Kq1Pathing.RoomPathing
{
    class GlobalPathFinder
    {
        List<GlobalTraverser> traversers;
        GlobalTraverser currentTraverser;

        //    3    1      2     5            7     9     8    10
        // 1 [NUT] [DGR] [CLV] [KEY] [MSH] [SHD] [CHS] [MRR] [KNG]
        public const int GOAL_DAGGER = 1;
        public const int GOAL_CLOVER = 2;
        public const int GOAL_NUT = 3;
        public const int GOAL_BAG = 4;
        public const int GOAL_KEY1 = 5;
        public const int GOAL_KEY2 = 6;
        public const int GOAL_SHIELD = 7; // gets the mushroom *and* finds the shield entrance
        public const int GOAL_MIRROR = 8;
        public const int GOAL_CHEST = 9;
        public const int GOAL_KING = 10;

        public GlobalPathFinder()
        {
            traversers = new();
            currentTraverser = new GlobalTraverser(new GameState(), START_ROOM, START_POS.x, START_POS.y);
            traversers.Add(currentTraverser);
        }

        public GlobalPathFinder(int roomNr, int startX, int startY)
        {
            traversers = new();
            currentTraverser = new GlobalTraverser(new GameState(), roomNr, startX, startY);
            traversers.Add(currentTraverser);
        }

        public void NextGoal(int goal)
        {
            switch (goal)
            {
                case GOAL_DAGGER:
                    currentTraverser.SetGoalDagger();
                    break;

                case GOAL_CLOVER:
                    currentTraverser.SetGoalClover();
                    break;

                case GOAL_NUT:
                    currentTraverser.SetGoalNut();
                    break;

                case GOAL_BAG:
                    currentTraverser.SetGoalBag();
                    break;

                case GOAL_KEY1:
                    currentTraverser.SetGoalGnome();
                    currentTraverser.SetGoalKey1();
                    break;

                case GOAL_KEY2:
                    currentTraverser.SetGoalGnome();
                    currentTraverser.SetGoalLeaveGnomeRoom();
                    currentTraverser.SetGoalKey2();
                    break;

                case GOAL_SHIELD:
                {
                    currentTraverser.SetGoalMushroom();
                    currentTraverser.SetGoalShieldEntrance();
                    GameState nextGameState = currentTraverser.GetNextGameState();
                    currentTraverser = new GlobalTraverser(nextGameState, SHIELD_EXIT_ROOM, SHIELD_EXIT_POS.x, SHIELD_EXIT_POS.y);
                    traversers.Add(currentTraverser);
                    break;
                }

                case GOAL_MIRROR:
                {
                    currentTraverser.SetGoalMirror();
                    GameState nextGameState = currentTraverser.GetNextGameState();
                    currentTraverser = new GlobalTraverser(nextGameState, DRAGON_EXIT_ROOM, DRAGON_EXIT_POS.x, DRAGON_EXIT_POS.y);
                    traversers.Add(currentTraverser);
                    break;
                }

                case GOAL_CHEST:
                {
                    currentTraverser.SetGoalChest();
                    GameState nextGameState = currentTraverser.GetNextGameState();
                    currentTraverser = new GlobalTraverser(nextGameState, STAIRCASE_BOTTOM_ROOM, CHEST_END_POS.x, CHEST_END_POS.y);
                    traversers.Add(currentTraverser);
                    break;
                }

                case GOAL_KING:
                {
                    currentTraverser.SetGoalKing();
                    break;
                }

                default:
                    throw new Exception($"unknown goal {goal}");
            }
        }

        public void FindPathLengths()
        {
            foreach (GlobalTraverser traverser in traversers)
            {
                List<Traversal> pathSequence = traverser.FindPathSequence();
                foreach (Traversal t in pathSequence)
                {
                    Debug.WriteLine($"{t}");
                }
            }
        }

        public void ShowPathLengths(int enumerationId = 0)
        {
            int totalLen = 0;

            foreach (GlobalTraverser traverser in traversers)
            {
                traverser.DeterminePathLength();
                totalLen += traverser.pathLength;
            }
            Debug.Write($"{enumerationId,3}.   {totalLen,3}   ");
            foreach (GlobalTraverser traverser in traversers)
            {
                Debug.Write($"---- {traverser.pathLength} {traverser.pathDescription}  ");
            }
            Debug.WriteLine($"");
        }
    }
}
