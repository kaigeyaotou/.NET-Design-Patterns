namespace BuilderPattern
{
    public partial class BuilderPattern
    {
        //Builder（抽象建造者）
        public abstract class MazeBuilder
        {
            public  abstract void BuildMaze();

            public abstract void BuildRoom(int roomNo);

            public abstract void BuildDoor(int roomFrom,int roomTo);

            public virtual Maze GetMaze()
            {
                return null;
            }
        }
    }
}