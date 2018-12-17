using System;

namespace BuilderPattern
{
    public partial class BuilderPattern
    {
        public void NewGame()
        {
            Console.WriteLine($"New Game");
            Console.WriteLine();
            Maze maze;
            MazeGame game=new MazeGame();
            Console.WriteLine($"StandMazeBuilder build Maze");

            StandMazeBuilder builder=new StandMazeBuilder();
            game.CreateMaze(builder);
            maze = builder.GetMaze();

            Console.WriteLine($"StandMazeBuilder build complete");
            Console.WriteLine($"Maze has {maze.GetCount()} Room");
            Console.WriteLine();
        }

        public void NewGameCount()
        {

            Console.WriteLine($"New Game CountingMaze");
            MazeGame game = new MazeGame();
            Console.WriteLine($"CountingMazeBuilder build Maze");
            CountingMazeBuilder countingMazeBuilder = new CountingMazeBuilder();
            game.CreateMaze(countingMazeBuilder);
            Console.WriteLine($"CountingMazeBuilder build complete");
            int rooms;
            int doors;
            countingMazeBuilder.GetCounts(out rooms, out doors);

            Console.WriteLine($"Maze has Room Count {rooms} ,Door  Count {doors} ");
        }
        public enum Direction { North,South,East,West}

        public abstract class MapSit
        {
           public abstract void Enter();
        }

        public class Room:MapSit
        {
            private int _roomNumber = 0;
            private MapSit[] _side;
            public  Room(int roomNumber)
            {
                _roomNumber = roomNumber;
                _side= new MapSit[4];
            }

            public int GetRoomNo()
            {
                return _roomNumber;
            }
            public MapSit GetSide(Direction direction)
            {
                return _side[(int) direction];
            }

            public void SetSide(Direction direction , MapSit map)
            {
                _side[(int) direction] = map;
            }

            public override void Enter()
            {
                Console.WriteLine(@"Enter Room");
            }

        }

        public class Wall : MapSit
        {
            public override void Enter()
            {
                Console.WriteLine(@"Enter Wall");
            }
        }

        public class Door : MapSit
        {
            private Room _room1;
            private Room _room2;
            private bool _isOpen;
            public Door(Room room1, Room room2)
            {
                _room1 = room1;
                _room2 = room2;
                _isOpen = false;
            }

            public Room OtherSideFrom(Room room)
            {
                throw new NotSupportedException();
            }

            public override void Enter()
            {
                Console.WriteLine(@"Enter Door");
            }
        }
        

    }
}
