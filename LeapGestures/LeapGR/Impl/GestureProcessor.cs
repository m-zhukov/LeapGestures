using LeapGR.Api;
using LeapGR.GestureModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeapGR.Impl
{
    public delegate void GestureRecognizedHandler(Gesture gesture);

    public class GestureProcessor : Leap.Listener, IGestureProcessor
    {
        #region variables

        const string REGISTRY_FILE = "gestures.xml";
        const float INIT_COORDINATES = 100f;
        const int INIT_COUNTER = 0;
        const int FRAME_INTERVAL = 5000;

        long currentFrameTime;
        long previousFrameTime;
        long frameTimeChange;

        Registry _registry;
        Leap.Controller _controller;
        List<int> _recognized;
        List<float> _coordinates;
        List<int> _number;

        #endregion

        public event GestureRecognizedHandler GestureRecognized;

        void FireEvent(Gesture gesture)
        {
            if (GestureRecognized != null)
                GestureRecognized(gesture);
        }

        public GestureProcessor()
        {
            LoadGestures();
            InitializeSensor();
            InitializeProcessor();
        }

        #region IGestureProcessor

        public void LoadGestures()
        {
            _registry = null;

            string path = System.AppDomain.CurrentDomain.BaseDirectory + "//" + REGISTRY_FILE;

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Registry));
                    _registry = (Registry)xs.Deserialize(sr);
                }
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
        }

        public void InitializeProcessor()
        {
            if(_registry != null)
            {
                int length = _registry.Gestures.Length;

                _recognized = Enumerable.Repeat<int>(INIT_COUNTER, length).ToList();
                _number = Enumerable.Repeat<int>(INIT_COUNTER, length).ToList();
                _coordinates = Enumerable.Repeat<float>(INIT_COORDINATES, length).ToList();
            }
        }

        public void InitializeSensor()
        {
            try
            {
                _controller = new Leap.Controller();
                _controller.AddListener(this);
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
        }

        public void CheckDirection(int gestureIndex, Primitive primitive, Leap.Finger finger)
        {
            float pointCoordinates = float.NaN;

            switch(primitive.Axis)
            {
                case Axis.X:
                    pointCoordinates = finger.TipPosition.x;
                    break;
                case Axis.Y:
                    pointCoordinates = finger.TipPosition.y;
                    break;
                case Axis.Z:
                    pointCoordinates = finger.TipPosition.z;
                    break;
            }

            if (_coordinates[gestureIndex] == INIT_COUNTER)
                _coordinates[gestureIndex] = pointCoordinates;

            else
            {
                switch (primitive.Direction)
                {
                    case 1:
                        if (_coordinates[gestureIndex] < pointCoordinates)
                        {
                            _coordinates[gestureIndex] = pointCoordinates;
                            _number[gestureIndex]++;
                        }
                        else
                            _coordinates[gestureIndex] = INIT_COORDINATES;
                        break;
                    case -1:
                        if (_coordinates[gestureIndex] > pointCoordinates)
                        {
                            _coordinates[gestureIndex] = pointCoordinates;
                            _number[gestureIndex]++;
                        }
                        else
                            _coordinates[gestureIndex] = INIT_COORDINATES;
                        break;
                }
            }

            if(_number[gestureIndex] == primitive.FramesCount)
            {
                _number[gestureIndex] = INIT_COUNTER;
                _recognized[gestureIndex]++;
            }
        }

        public void CheckGesture(Gesture gesture)
        {
            if(_recognized[gesture.GestureIndex] == (gesture.PrimitivesCount - 1))
            {
                FireEvent(gesture);
                _recognized[gesture.GestureIndex] = INIT_COUNTER;
            }
        }

        public void CheckFinger(Gesture gesture, Leap.Finger finger)
        {
            int recognitionValue = _recognized.ElementAt(gesture.GestureIndex);
            Primitive primitive = gesture.Primitives[recognitionValue];
            CheckDirection(gesture.GestureIndex, primitive, finger);
            CheckGesture(gesture);
        }

        public void UninitializeSensor()
        {
            _controller.RemoveListener(this);
            _controller.Dispose();
        }

        public void UninitializeProcessor()
        {
            _number.Clear();
            _coordinates.Clear();
            _recognized.Clear();
        }

        #endregion

        #region Leap

        public override void OnFrame(Leap.Controller ctrl)
        {
            Leap.Frame frame = ctrl.Frame();

            currentFrameTime = frame.Timestamp;
            frameTimeChange = currentFrameTime - previousFrameTime;

            if (frameTimeChange > FRAME_INTERVAL)
            {
                foreach (Gesture gesture in _registry.Gestures)
                {
                    Task.Factory.StartNew(() => 
                        {
                            Leap.Finger finger = frame.Fingers[gesture.FingerIndex];
                            CheckFinger(gesture, finger);
                        });
                }

                previousFrameTime = currentFrameTime;
            }
        }

        #endregion
    }
}
