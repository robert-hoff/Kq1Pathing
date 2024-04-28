using Kq1Pathing.RoomPathing;

namespace Kq1Pathing.RoomPathingDemos
{
    public class FindPaths
    {
        public static void Run()
        {
            StartToDagger();
        }

        /*
         * Find all paths upto len-8 from start position to the dagger (boulder)
         */
        public static void StartToDagger()
        {
            PathFinder pathFinder = new PathFinder();
            pathFinder.SetGoalRegion(roomNr: 3, GameState.DAGGER_PICKUP_AREA);
            pathFinder.FindPathsFrom(roomNr: 1, 110, 100, MAXDEPTH: 8);
            pathFinder.ShowSolutions(omitPositionsSeen: false);
        }
    }
}
