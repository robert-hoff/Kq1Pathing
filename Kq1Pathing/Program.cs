using System.Diagnostics;
using Kq1Pathing.RoomPathing;
using static Kq1Pathing.RoomPathing.MapTraversal;

namespace Kq1Pathing
{
    internal static class Program
    {
        static void Main()
        {
            // RunInterface();
            // SaveControlData();
            TestMapTraversal();
        }

        [STAThread]
        static void RunInterface()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new TestForm());
        }

        static void SaveControlData()
        {
            PrintRoomControlData.RunTrials();
        }

        static void TestMapTraversal()
        {
            MapTraversal mapTraversal = new();
            Traversal traversal = null;
            // evaluate zip position from (25,115) in room 53, for the direction DIR_E (east)
            traversal = mapTraversal.EvaluateMapTraversal(roomNr: 53, 25, 115, RoomControl.DIR_E);
            Debug.WriteLine($"x,y=({traversal.X()},{traversal.Y()}), room={traversal.RNr()}"); // (119,115)(54)
        }
    }
}
