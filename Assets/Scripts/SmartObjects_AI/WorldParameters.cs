using System.Collections.Generic;

namespace SmartObjects_AI
{
    //TODO en faire un singleton ou un ScriptableObject au choix (pour pouvoir facilement modifier ses valeurs
    public static class WorldParameters
    {
        public static Dictionary<WorldParameterType, float> Parameters;
    }

    public enum WorldParameterType
    {
        None
    }
}