using LeapGR.GestureModel;
using System.IO;
using System.Xml.Serialization;

namespace Tests
{
    public static class SerializationTest
    {
        public static void TestSerialize()
        {
            Registry r = new Registry();

            Gesture g = new Gesture { GestureIndex = 0, GestureName = "Tap" };

            Primitive p1 = new Primitive { Axis = Axis.Z, Direction = -1, FramesCount = 30, Order = 0 };
            Primitive p2 = new Primitive { Axis = Axis.Z, Direction = 1, FramesCount = 30, Order = 1 };

            g.Primitives = new Primitive[] { p1, p2 };
            g.PrimitivesCount = 2;

            r.Gestures = new Gesture[] { g };

            XmlSerializer ser = new XmlSerializer(typeof(Registry));
            StreamWriter sw = new StreamWriter("test.xml");
            ser.Serialize(sw, r);
        }

        public static void TestDeserialize()
        {
            Registry gr = null;
            XmlSerializer ser = new XmlSerializer(typeof(Registry));
            StreamReader reader = new StreamReader("gestures.xml");
            gr = (Registry)ser.Deserialize(reader);
            reader.Close();
        }
    }
}
