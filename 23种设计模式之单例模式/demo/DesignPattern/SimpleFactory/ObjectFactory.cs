using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFactory
{
    /// <summary>
    /// 简单工厂
    /// </summary>
    public class ObjectFactory
    {
        /// <summary>
        /// 细节没有消失  只是转移
        /// 转移了矛盾，并没有消除矛盾
        /// 
        /// 集中了矛盾
        /// </summary>
        /// <param name="raceType"></param>
        /// <returns></returns>
        public static IRace CreateRace(RaceType raceType)
        {
            IRace iRace = null;
            switch (raceType)
            {
                case RaceType.Human:
                    iRace = new Human();
                    break;
                case RaceType.Undead:
                    iRace = new Undead();
                    break;
                case RaceType.ORC:
                    iRace = new ORC();
                    break;
                case RaceType.NE:
                    iRace = new NE();
                    break;

                    //增加一个分支
                default:
                    throw new Exception("wrong raceType");
            }
            return iRace;
        }







        private static string IRacTypeConfig = ConfigurationManager.AppSettings["IRacTypeConfig"];
        public static IRace CreateRaceConfig()
        {
            RaceType raceType = (RaceType)Enum.Parse(typeof(RaceType), IRacTypeConfig);

            IRace iRace = null;
            switch (raceType)
            {
                case RaceType.Human:
                    iRace = new Human();
                    break;
                case RaceType.Undead:
                    iRace = new Undead();
                    break;
                case RaceType.ORC:
                    iRace = new ORC();
                    break;
                case RaceType.NE:
                    iRace = new NE();
                    break;
                default:
                    throw new Exception("wrong raceType");
            }
            return iRace;
        }





        //1 方法增加个参数
        //2 给不同的参数，配置不同的IRacTypeConfigReflection

            //多方法是不对的，因为没法扩展新种族
            //泛型也不对  泛型需要上端知道具体类型

        private static string IRacTypeConfigReflection = ConfigurationManager.AppSettings["IRacTypeConfigReflection"];
        private static string DllName = IRacTypeConfigReflection.Split(',')[1];
        private static string TypeName = IRacTypeConfigReflection.Split(',')[0];
        /// <summary>
        /// ioc的雏形
        /// </summary>
        /// <returns></returns>
        public static IRace CreateRaceConfigReflection()
        {
            Assembly assembly = Assembly.Load(DllName);
            Type type = assembly.GetType(TypeName);
            IRace iRace = Activator.CreateInstance(type) as IRace;

            return iRace;
        }
    }

    public enum RaceType
    {
        Human,
        Undead,
        ORC,
        NE
    }
}
