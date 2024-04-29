# Kq1Pathing

Finds the shortest sequence of straight-line segments (zips) between two endpoints in Kings Quest 1 (Sierra, 1984).
Traversals are determined from the game's "priority data", these are provided as bitmaps in `/image-exports/priority/`.

## Running

Open the project file `Kq1Pathing.sln` in Visual Studio 2022 or later.

## Usage

Use the `PathFinder` class to specify endpoints and intitiate a shortest path-search.
One or more rectangular goal regions can be speficied with `SetGoalRegion`. `FindPathsFrom` describes the
starting point and initiates the search. `ShowSolutions` prints the results. For example:

```csharp
PathFinder pathFinder = new PathFinder();
pathFinder.SetGoalRegion(roomNr: 3, GameState.DAGGER_PICKUP_AREA);
pathFinder.FindPathsFrom(roomNr: 1, 110, 100, MAXDEPTH: 6);
pathFinder.ShowSolutions(omitPositionsSeen: false);
```

Prints

```
 LEN(5)  (110,100)(1)  N 110,103(16)  W  56,103(15) SE 139,69  (2)  N 139,60 (15) SW 117,120 (3)  N  W SE  N SW
 LEN(5)  (110,100)(1) NW 108,60 (15) SW 135,71  (3)  E  84,71  (2) SW   5,150 (2) NW 133,123 (3) NW SW  E SW NW
 LEN(6)  (110,100)(1) SE 129,119 (1) NE  21,72  (8)  S  21,106 (8) NW  13,60 (16) SW 134,132 (3)  W 133,132 (3) SE NE  S NW SW  W
 LEN(6)  (110,100)(1) SE 129,119 (1) NE  21,72  (8)  S  21,106 (8) NW  13,60 (16) SW 134,132 (3) NW 133,131 (3) SE NE  S NW SW NW
 LEN(6)  (110,100)(1) SE 129,119 (1) NW 108,60 (15) SW 135,71  (3)  E  84,71  (2) SW   5,150 (2) NW 133,123 (3) SE NW SW  E SW NW
 LEN(6)  (110,100)(1) SW  89,121 (1)  N  89,78 (16) SE  92,81 (16) SW 142,69  (2)  N 142,60 (15) SW 120,120 (3) SW  N SE SW  N SW
 LEN(6)  (110,100)(1) NW 108,60 (15) SW 135,71  (3) NW 103,160(14)  S 103,91  (3) NE 117,77  (3)  S 117,120 (3) NW SW NW  S NE  S
 ```

Meaning, there are two 5-length zips and five 6-length zips between the starting point
in KQ1 and the dagger pickup. The directions of the two 5-length zips are written as (N,W,SE,N,SW and NW,SW,E,SW,NW)
