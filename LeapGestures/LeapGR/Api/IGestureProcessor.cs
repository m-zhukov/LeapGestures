using LeapGR.GestureModel;

namespace LeapGR.Api
{
    public interface IGestureProcessor
    {
        void LoadGestures();
        void InitializeProcessor();
        void InitializeSensor();
        void CheckDirection(int gestureIndex, Primitive primitive, Leap.Finger finger);
        void CheckGesture(Gesture gesture);
        void CheckFinger(Gesture gesture, Leap.Finger finger);
        void UninitializeSensor();
        void UninitializeProcessor();
    }
}
