using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class DebugLogPreciseTImer : MonoBehaviour
    {
        public static void HighPrecisionLog(string message)
        {
            float timeInSeconds = Time.realtimeSinceStartup;
            Debug.Log($"[{timeInSeconds:F7}] {message}");
        }
    }
}
