using System.Diagnostics;

namespace Kq1Pathing.RoomPathing
{
    class RoomBitmapImport
    {
        static string EXPORT_FOLDER = Path.GetFullPath(@"..\..\..\..\image-exports\priority");
        // white can be ignored for control purposes
        // const uint WHITE = 0xffffffff; // 0xffffffff
        const uint BLACK = 0xff000000; // 0xff000000 - black control line
        const uint DARKBLUE = 0xff0000a0; // 0xff0000a0 - darkblue control line
        const uint DARKGREEN = 0xff00a000; // 0xff00a000 - darkgreen action pixels
        const uint CYAN = 0xff00a0a0; // ff00a0a0 - indicates water

        // exports by default are 2x1 pixel wide, this may vary based on the scale given in the export
        // these are the scaling used the x and y directions. Common values are 1 and 2
        // the may differ, for example X_WIDTH,Y-WIDTH = (2,1)
        static int X_PIXEL_SCALE;
        static int Y_PIXEL_SCALE;

        public static int[,] ImportRoom(RoomDefinition roomDefinition)
        {
            // Debug.WriteLine($"importing room = {roomProperties.roomNr}");
            int[,] roomControlDatal = new int[RoomDefinition.ROOM_WIDTH, RoomDefinition.ROOM_HEIGHT];
            Bitmap roomBitmap = GetBitmap(roomDefinition.roomNr);
            DetermineXYPixelScales(roomBitmap);
            DetermineXYPixelScales(roomBitmap);

            for (int i = 0; i < RoomDefinition.ROOM_WIDTH; i++)
            {
                for (int j = 0; j < RoomDefinition.ROOM_HEIGHT; j++)
                {
                    int color = roomBitmap.GetPixel(i * X_PIXEL_SCALE, j * Y_PIXEL_SCALE).ToArgb();
                    switch ((uint) color)
                    {
                        // there is only one behaviour for black pixels, as barriers
                        case BLACK:
                            roomControlDatal[i, j] = RoomControl.BARRIER;
                            break;

                        case DARKBLUE:
                            if (roomDefinition.useBlockControl == RoomDefinition.BLOCKS_UNDEFINED)
                            {
                                throw new Exception("Behaviour for block controls not defined");
                            }
                            else if (roomDefinition.useBlockControl == RoomDefinition.BLOCKS_OBSERVED)
                            {
                                roomControlDatal[i, j] = RoomControl.BARRIER;
                            }
                            else if (roomDefinition.useBlockControl == RoomDefinition.BLOCKS_IGNORED)
                            {
                                roomControlDatal[i, j] = RoomControl.TRAVERSABLE;
                            }
                            else
                            {
                                throw new Exception("Unknown property value for" +
                                    $"roomProperties.useDarkBlueControl {roomDefinition.useBlockControl}");
                            }
                            break;

                        case DARKGREEN:
                            if (roomDefinition.useActionControl == RoomDefinition.ACTION_TILE_UNDEFINED)
                            {
                                throw new Exception("Behaviour for action controls not defined");
                            }
                            else if (roomDefinition.useActionControl == RoomDefinition.ACTION_TILE_DEATH_TRIGGER)
                            {
                                roomControlDatal[i, j] = RoomControl.DEATH_TRIGGER;
                            }
                            else if (roomDefinition.useActionControl == RoomDefinition.ACTION_TILE_ROOM_TRIGGER)
                            {
                                roomControlDatal[i, j] = RoomControl.ROOM_TRIGGER;
                            }
                            else if (
                              roomDefinition.useActionControl == RoomDefinition.ACTION_TILE_SPECIAL ||
                              roomDefinition.useActionControl == RoomDefinition.ACTION_TILE_IGNORED)
                            {
                                roomControlDatal[i, j] = RoomControl.TRAVERSABLE;
                            }
                            else
                            {
                                throw new Exception("Unknown property value for" +
                                    $"roomProperties.useDarkGreenControl {roomDefinition.useActionControl}");
                            }
                            break;

                        case CYAN:
                            roomControlDatal[i, j] = roomDefinition.useWaterControl > RoomDefinition.WATER_TILES_IGNORED ? RoomControl.WATER : RoomControl.TRAVERSABLE;
                            break;

                        default:
                            roomControlDatal[i, j] = RoomControl.TRAVERSABLE;
                            break;
                    }
                }
            }

            // add any special barriers (such as caused by stationary NPC, room77)
            foreach (var rect in roomDefinition.specialBarriers)
            {
                for (int j = rect[1]; j <= rect[3]; j++)
                {
                    for (int i = rect[0]; i <= rect[2]; i++)
                    {
                        roomControlDatal[i, j] = RoomControl.BARRIER;
                    }
                }
            }

            // add any objects as conditional barriers
            if (roomDefinition.hasObject)
            {
                int[] rect = roomDefinition.objectFootprint;
                for (int j = rect[1]; j <= rect[3]; j++)
                {
                    for (int i = rect[0]; i <= rect[2]; i++)
                    {
                        roomControlDatal[i, j] = RoomControl.OBJECT_BARRIER;
                    }
                }
            }

            // add any addtional objects influencing edge-rewriting (these objects can not be picked up)
            for (int ind = 0; ind < roomDefinition.objectsEdgeRewriting.Count; ind++)
            {
                if (roomDefinition.observeObjectBarrier[ind])
                {
                    int[] rect = roomDefinition.objectsEdgeRewriting[ind];
                    for (int j = rect[1]; j <= rect[3]; j++)
                    {
                        for (int i = rect[0]; i <= rect[2]; i++)
                        {
                            roomControlDatal[i, j] = RoomControl.BARRIER;
                        }
                    }
                }
            }
            return roomControlDatal;
        }

        public static Bitmap GetBitmap(int roomNr)
        {
            string filenamepath = @$"{EXPORT_FOLDER}\room{roomNr}.bmp";
            return new Bitmap(filenamepath, true);
        }

        public static void DetermineXYPixelScales(Bitmap roomBitmap)
        {
            X_PIXEL_SCALE = roomBitmap.Width / RoomDefinition.ROOM_WIDTH;
            Y_PIXEL_SCALE = roomBitmap.Height / RoomDefinition.ROOM_HEIGHT;
        }

        public static void ShowControlColorsRoom68()
        {
            string filenamepath = EXPORT_FOLDER + "\\room68.bmp";
            Bitmap room68Bitmap = new Bitmap(filenamepath, true);

            // white priority color 0xffffffff
            // white can be ignored for control purposes
            Debug.WriteLine($"White priority color {room68Bitmap.GetPixel(0, 0).ToArgb():x}");

            // black control color 0xff000000
            Debug.WriteLine($"Black control color {room68Bitmap.GetPixel(93 * 4, 154 * 2).ToArgb():x}");
            Debug.WriteLine($"BLACK test = {room68Bitmap.GetPixel(93 * 4, 154 * 2).ToArgb() == BLACK}");

            // dark-blue control color 0xff0000a0
            Debug.WriteLine($"Dark blue control color {room68Bitmap.GetPixel(94 * 4, 154 * 2).ToArgb():x}");
            Debug.WriteLine($"DARKBLUE test = {room68Bitmap.GetPixel(94 * 4, 154 * 2).ToArgb() == DARKBLUE}");

            // dark-green control color 0xff00a000
            Debug.WriteLine($"Dark green control color {room68Bitmap.GetPixel(100 * 4, 166 * 2).ToArgb():x}");
            Debug.WriteLine($"DARKGREEN test = {room68Bitmap.GetPixel(100 * 4, 166 * 2).ToArgb() == DARKGREEN}");

            Debug.WriteLine($"Cyan control color {room68Bitmap.GetPixel(43 * 4, 43 * 2).ToArgb():x}");
            Debug.WriteLine($"CYAN test = {room68Bitmap.GetPixel(43 * 4, 43 * 2).ToArgb() == CYAN}");
        }
    }
}
