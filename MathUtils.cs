using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils {
    //TODO avoid creating array by creating multiple versions
    public static int GetRangeIndex (float val, params float[] ranges) {
        for (int i = 0; i < ranges.Length; i++) {
            if (val < ranges[i]) {
                return i - 1;
            }
        }
        return ranges.Length - 1;
    }
}