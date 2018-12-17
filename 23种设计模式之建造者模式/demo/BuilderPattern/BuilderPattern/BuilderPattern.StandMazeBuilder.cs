using System;

namespace BuilderPattern
{
    public partial class BuilderPattern
    {
        //ConcreteBuilder（具体建造者）
        public class StandMazeBuilder : MazeBuilder
        {
            private Maze _currentMaze;

            private Direction CommonWall(Room roomFrom, Room roomTo)
            {
                var left = roomFrom.GetRoomNo() > roomTo.GetRoomNo();
              
                return left?Direction.West:Direction.East;
            }
            public override void BuildMaze()
            {
                Console.WriteLine($"BuildMaze 初始化迷宫");
                _currentMaze =new Maze();
            }
 

            public override void BuildRoom(int roomNo)
            {
                Console.WriteLine($"BuildRoom {roomNo}");

                if (_currentMaze.RoomNo(roomNo) == null)
                {
                    var room =new Room(roomNo);
                    _currentMaze.AddRoom(room);
                    room.SetSide(Direction.North, new Wall());
                    room.SetSide(Direction.South, new Wall());
                    room.SetSide(Direction.East, new Wall());
                    room.SetSide(Direction.West, new Wall());
                }
            }

            public override void BuildDoor(int roomFrom, int roomTo)
            {
                Console.WriteLine($"BuildDoor roomFrom ={roomFrom} roomTo {roomTo}");

                var from = _currentMaze.RoomNo(roomFrom);
                var to = _currentMaze.RoomNo(roomTo);
                var door = new Door(from,to);
                from.SetSide(CommonWall(from,to),door);
                to.SetSide(CommonWall(to, from), door);
            }

            public override Maze GetMaze()
            {
                return _currentMaze;
            }
        }
    }
}