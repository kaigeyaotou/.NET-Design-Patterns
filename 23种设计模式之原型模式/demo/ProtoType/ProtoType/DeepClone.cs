using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtoType
{

    public class Product:ICloneable
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        public NumberFlag Number { get; set; }

        public class NumberFlag
        {
            public string Num { get; set; }
        }
        /// <summary>
        /// 克隆方法
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// 深拷贝方法
        /// </summary>
        /// <returns></returns>
        //public Product DeepClone()
        //{
        //    using (System.IO.Stream ms = new System.IO.MemoryStream())
        //    {
        //        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        //        formatter.Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Clone);

        //        formatter.Serialize(ms, this);

        //        ms.Position = 0;

        //        return formatter.Deserialize(ms) as Product;
        //    }
        //}
    }

}
