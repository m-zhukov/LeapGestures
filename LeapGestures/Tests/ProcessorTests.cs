using LeapGR.Impl;
using System;

namespace Tests
{
    class ProcessorTests
    {
        public static void TestProcessor()
        {
            GestureProcessor gp = new GestureProcessor();
            Console.WriteLine("GestureProcessor started");

            gp.GestureRecognized += gp_GestureRecognized;
        }

        static void gp_GestureRecognized(LeapGR.GestureModel.Gesture gesture)
        {
            Console.WriteLine(gesture.GestureName);
        }
    }
}
