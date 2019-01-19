using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineExtensions {
    public const int defaultOpsCountPerFrame = 1000;
    public static void ExecuteSync (IEnumerator coroutine) {
        while (coroutine.MoveNext ()) {
            //just executing
        }
    }

    static bool IncOperationsCountAndCheck (ref int operations, int incBy, int compareWith) {
        operations += incBy;
        if (operations >= compareWith) {
            operations = operations % compareWith;
            return true;
        }
        return false;
    }

    private static DontWait dontWaitInstance = new DontWait ();

    public static object WaitIfNeeded (ref int currentOperationsCount, int operationsDoneThisTime = 1, int waitIfCurrentOperationsCountExceeds = 1000) {
        if (IncOperationsCountAndCheck (ref currentOperationsCount, operationsDoneThisTime, waitIfCurrentOperationsCountExceeds)) {
            return null;
        } else {
            return dontWaitInstance;
        }
    }

    public static YieldInstruction StartCoroutineExtended (this MonoBehaviour monoBehaviour, IEnumerator routine) {
        var wrapper = CoroutineWrapper (routine, dontWaitInstance);
        while (wrapper.MoveNext ()) {
            if (wrapper.Current is DontWait)
                continue;
            var newWrapper = CoroutineWrapper (wrapper, wrapper.Current);
            return monoBehaviour.StartCoroutine (newWrapper);
        }
        return dontWaitInstance;
    }

    private static IEnumerator CoroutineWrapper (IEnumerator originalRoutine, object firstYield) {
        if (!(firstYield is DontWait)) {
            yield return firstYield;
        }
        while (originalRoutine.MoveNext ()) {
            if (originalRoutine.Current is DontWait) {
                continue;
            } else {
                yield return originalRoutine.Current;
            }
        }
    }

    public class OperationsCounter {
        public int operationsPerFrame;
        public int currentFrameOperations;

        public OperationsCounter (int operationsPerFrame) {
            this.operationsPerFrame = operationsPerFrame;
        }

        public object WaitIfNeeded (int operationsCount = 1) {
            return CoroutineExtensions.WaitIfNeeded (ref currentFrameOperations, operationsCount, operationsPerFrame);
        }
    }
}
public class DontWait : YieldInstruction { }