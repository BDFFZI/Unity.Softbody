using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class NativeArrayUtility
{
    public static int Min<TDataType>(this NativeArray<TDataType> array, out TDataType maxValue)
    where TDataType : struct, IComparable<TDataType>
    {
        return Max<TDataType>(array, -1, out maxValue);
    }

    public static int Max<TDataType>(this NativeArray<TDataType> array, out TDataType maxValue)
        where TDataType : struct, IComparable<TDataType>
    {
        return Max<TDataType>(array, 1, out maxValue);
    }

    static int Max<TDataType>(NativeArray<TDataType> array, float coefficient, out TDataType maxValue)
       where TDataType : struct, IComparable<TDataType>
    {
        if (array.Length == 0)
        {
            maxValue = default;
            return -1;
        }

        int maxIndex = 0;
        maxValue = array[0];

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i].CompareTo(maxValue) * coefficient > 0)
            {
                maxIndex = i;
                maxValue = array[i];
            }
        }

        return maxIndex;
    }
}
