namespace Kq1Pathing.RoomPathing
{
    class ClearBarrier
    {
        /*
         *
         * The repositioning is performed by using a scanline that traces
         * the perimeter of growing squares until a free pixel has been found
         *
         *     0         10        20
         *     012345678901234567890
         * 81  ____1___1___1___1___1
         * 82  ____1___1___1___1___1
         * 83  ____1___1___1___1___1
         * 84  ____1_A_1___1___1___1
         * 85  ____1___1___1___1___1
         * 86  ____1___1___1___1___1
         * 87  ____1___1___1___1___1
         * 88  ____1___1___1___1___1
         *
         *
         * When scanning the size 1 enclosing rectangle the first position should be (x-1,y)
         * Here A=6,84 the first position should be 5,84 which is the vertical line from y-i < x < y+i
         *
         * The second scanline (when i=2), showing (dx,dy), traces the square
         *
         *      left side (4,83), (4,84), (4,85)
         *      bottom    (4,86), (5,86), (6,86), (7,86), (8,86)
         *      right     (8,85), (8,84), (8,83)
         *      top       (8,82), (7,82), (6,82), (5,82), (4,82)
         *
         *
         *
         */
        public static (int, int) RepositionPlayer(RoomControl roomControl, int playerX, int playerY)
        {
            if (roomControl.CheckTraversablePosition(playerX, playerY))
            {
                return (playerX, playerY);
            }

            int minX = roomControl.MinimumX();
            int maxX = roomControl.MaximumX();
            int minY = roomControl.MinimumY();
            int maxY = roomControl.MaximumY();

            if (playerX < minX || playerX > maxX || playerY < minY || playerY > maxY)
            {
                throw new Exception($"The player position is outside the bounds of the room {playerX},{playerY}");
            }

            for (int i = 1; i <= 100; i++)
            {
                int dx = -i;
                int dy;
                // Debug.WriteLine($"left line");
                for (dy = -i + 1; dy < i; dy++)
                {
                    if (playerX + dx < minX || playerY + dy < minY)
                    {
                        break;
                    }
                    if (playerY + dy > maxY)
                    {
                        continue;
                    }
                    // Debug.WriteLine($"{playerX + dx},{playerY + dy}");
                    if (roomControl.CheckTraversablePosition(playerX + dx, playerY + dy))
                    {
                        return (playerX + dx, playerY + dy);
                    }
                }
                dy = i;
                // Debug.WriteLine($"bottom line");
                for (dx = -i; dx <= i; dx++)
                {
                    if (playerY + dy > maxY || playerX + dx > maxX)
                    {
                        break;
                    }
                    if (playerX + dx < minX)
                    {
                        continue;
                    }
                    // Debug.WriteLine($"dx = {dx}");
                    // Debug.WriteLine($"{playerX + dx},{playerY + dy}");
                    if (roomControl.CheckTraversablePosition(playerX + dx, playerY + dy))
                    {
                        return (playerX + dx, playerY + dy);
                    }
                }
                dx = i;
                // Debug.WriteLine($"right line");
                for (dy = i - 1; dy > -i; dy--)
                {
                    if (playerX + dx > maxX || playerY + dy < minY)
                    {
                        break;
                    }
                    if (playerY + dy > maxY)
                    {
                        continue;
                    }
                    // Debug.WriteLine($"{playerX + dx},{playerY + dy}");
                    if (roomControl.CheckTraversablePosition(playerX + dx, playerY + dy))
                    {
                        return (playerX + dx, playerY + dy);
                    }
                }
                dy = -i;
                // Debug.WriteLine($"top line");
                for (dx = i; dx >= -i; dx--)
                {
                    if (playerY + dy < minY || playerX + dx < minX)
                    {
                        break;
                    }
                    if (playerX + dx > maxX)
                    {
                        continue;
                    }
                    // Debug.WriteLine($"{playerX + dx},{playerY + dy}");
                    if (roomControl.CheckTraversablePosition(playerX + dx, playerY + dy))
                    {
                        return (playerX + dx, playerY + dy);
                    }
                }
            }

            // throw new Exception("Couldn't complete barrier clearing, something wrong with the data");
            return (0, 0);
        }
    }
}
