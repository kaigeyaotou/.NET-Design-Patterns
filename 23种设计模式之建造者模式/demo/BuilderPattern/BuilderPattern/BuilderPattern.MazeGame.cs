namespace BuilderPattern
{
    public partial class BuilderPattern
    {  
        //Director（指挥者）
        public class MazeGame
        {
            public Maze CreateMaze(MazeBuilder builder)
            {
                builder.BuildMaze();
                builder.BuildRoom(1);
                builder.BuildRoom(2);
                builder.BuildDoor(1, 2);
                return builder.GetMaze();
            }
        }
    }
}