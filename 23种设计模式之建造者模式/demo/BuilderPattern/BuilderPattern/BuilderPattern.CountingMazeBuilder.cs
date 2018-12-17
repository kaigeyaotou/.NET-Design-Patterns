using System;

namespace BuilderPattern
{
    public partial class BuilderPattern
    {
        //ConcreteBuilder（具体建造者）
        public class CountingMazeBuilder:MazeBuilder
        {
            private int _rooms;
            private int _doors;
            public override void BuildMaze()
            {
                Console.WriteLine($"BuildMaze 什么也没干");
            }

            public override void BuildRoom(int roomNo)
            {
                Console.WriteLine($"BuildRoom 房间数量+1");
                _rooms++;
            }

            public override void BuildDoor(int roomFrom, int roomTo)
            {
                Console.WriteLine($"BuildDoor 门数量+1");
                _doors++;
            }

            public void GetCounts(out int rooms, out int doors)
            {
                rooms = _rooms;
                doors = _doors;
            }
        }
    }
}