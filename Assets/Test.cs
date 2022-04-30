using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    public struct A
    {
        public int value;
    }

    public class B
    {
        public int value = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        B b = new B();
        DebugValue(b);
        for (int i = 0; i < 100; i++)
        {
            b.value++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void DebugValue(B b)
    {
        await Task.Delay(2000);
        Debug.Log(b.value.ToString());
    }
}
