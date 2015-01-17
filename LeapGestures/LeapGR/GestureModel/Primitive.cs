using System.Xml.Serialization;

namespace LeapGR.GestureModel
{
    public class Primitive
    {
        [XmlElement(ElementName = "Axis", Type = typeof(Axis))]         //ось выполнения движения
        public Axis Axis { get; set; }

        [XmlElement(ElementName = "Direction")]                         //направление: +1 -> положительное изменение
        public int Direction { get; set; }                              //             -1 -> отрицательное

        [XmlElement(ElementName = "Order", IsNullable = true)]          //порядок выполнения части движения
        public int? Order { get; set; }

        [XmlElement(ElementName = "FramesCount")]                       //количество кадров для выполнения части движения
        public int FramesCount { get; set; }
    }

    /// <summary>
    /// ось выполнения движения
    /// </summary>
    public enum Axis
    {
        [XmlEnum("X")]
        X,
        [XmlEnum("Y")]
        Y,
        [XmlEnum("Z")]
        Z
    };
}
