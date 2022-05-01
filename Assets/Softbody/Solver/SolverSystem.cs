using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Solver : MonoBehaviour
{
    public abstract void Solve(Softbody softbody, float deltaTime);
}

public class SolverSystem : MonoBehaviour
{
    [SerializeField] Softbody[] softbodies;
    [SerializeField] Solver[] solvers;
    //[SerializeField] float deltaTime;

    private void FixedUpdate()
    {
        for (int solverIndex = 0; solverIndex < solvers.Length; solverIndex++)
        {
            for (int softbodyIndex = 0; softbodyIndex < softbodies.Length; softbodyIndex++)
            {
                solvers[solverIndex].Solve(softbodies[softbodyIndex], Time.fixedDeltaTime);
            }
        }
    }

    //float counter;
    //void Update()
    //{
    //    counter += Time.deltaTime;

    //    while (counter > deltaTime)
    //    {
    //        for (int solverIndex = 0; solverIndex < solvers.Length; solverIndex++)
    //        {
    //            for (int softbodyIndex = 0; softbodyIndex < softbodies.Length; softbodyIndex++)
    //            {
    //                solvers[solverIndex].Solve(softbodies[softbodyIndex], deltaTime);
    //            }
    //        }

    //        counter -= deltaTime;
    //    }
    //}
}
