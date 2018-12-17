using System.Collections.Generic;

namespace BuilderPattern
{
    public partial class BuilderPattern
    {
        //Product（产品角色）
        public class Maze
        {
            private List<Room> _list;

            public Maze()
            {
                _list = new List<Room>();
            }

            public void AddRoom(Room room)
            {
                _list.Add(room);
            }

            public Room RoomNo(int roomNo)
            {
                return _list.Find(s => s.GetRoomNo() == roomNo);
            }

            public int GetCount()
            {
                return _list.Count;
            }
        }
    }
}