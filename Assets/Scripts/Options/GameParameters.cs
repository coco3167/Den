using System;
using SmartObjects_AI.Agent;

namespace Options
{
    public static class GameParameters
    {
        public static float MouseSensitivity = .1f;
        public static float JoystickSensitivity = 4f;
        public static float TimeScale = 1f;
        public static AgentDynamicParameter CursorMode;
        public static bool IsOutline;
        public static bool HasCloseCaptions;

        public static void SensitivityChange(float value)
        {
            MouseSensitivity += value / 10.0f;
            MouseSensitivity = Math.Clamp(MouseSensitivity, 0.1f, 2);
        }
    }
}
