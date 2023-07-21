// Remove the line above if you are submitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using GameAI;


namespace GameAIStudent
{

    public class ThrowMethods
    {

        public const string StudentName = "Sebastian Brumm";


        // Note: You have to implement the following method with prediction:
        // Either directly solved (e.g. Law of Cosines or similar) or iterative.
        // You cannot modify the method signature. However, if you want to do more advanced
        // prediction (such as analysis of the navmesh) then you can make another method that calls
        // this one. 
        // Be sure to run the editor mode unit test to confirm that this method runs without
        // any gamemode-only logic
        public static bool PredictThrow(
            // The initial launch position of the projectile
            Vector3 projectilePos,
            // The initial ballistic speed of the projectile
            float maxProjectileSpeed,
            // The gravity vector affecting the projectile (likely passed as Physics.gravity)
            Vector3 projectileGravity,
            // The initial position of the target
            Vector3 targetInitPos,
            // The constant velocity of the target (zero acceleration assumed)
            Vector3 targetConstVel,
            // The forward facing direction of the target. Possibly of use if the target
            // velocity is zero
            Vector3 targetForwardDir,
            // For algorithms that approximate the solution, this sets a limit for how far
            // the target and projectile can be from each other at the interceptT time
            // and still count as a successful prediction
            float maxAllowedErrorDist,
            // Output param: The solved projectileDir for ballistic trajectory that intercepts target
            out Vector3 projectileDir,
            // Output param: The speed the projectile is launched at in projectileDir such that
            // there is a collision with target. projectileSpeed must be <= maxProjectileSpeed
            out float projectileSpeed,
            // Output param: The time at which the projectile and target collide
            out float interceptT,
            // Output param: An alternate time at which the projectile and target collide
            // Note that this is optional to use and does NOT coincide with the solved projectileDir
            // and projectileSpeed. It is possibly useful to pass on to an incremental solver.
            // It only exists to simplify compatibility with the ShootingRange
            out float altT)
        {
            // TODO implement an accurate throw with prediction. This is just a placeholder

            // FYI, if Minion.transform.position is sent via param targetPos,
            // be aware that this is the midpoint of Minion's capsuleCollider
            // (Might not be true of other agents in Unity though. Just keep in mind for future game dev)

            // Only going 2D for simple demo. this is not useful for proper prediction
            // Basically, avoiding throwing down at enemies since we aren't predicting accurately here.
            var targetPos2d = new Vector3(targetInitPos.x, 0f, targetInitPos.z);
            var launchPos2d = new Vector3(projectilePos.x, 0f, projectilePos.z);

            var relVec = (targetPos2d - launchPos2d);
            interceptT = relVec.magnitude / maxProjectileSpeed;
            altT = -1f;

            // This is a hard-coded approximate sort of of method to figure out a loft angle
            // This is NOT the right thing to do for your prediction code!
            // Refer to assignment reqs and ballistic trajectory lecture!
            var normAngle = Mathf.Lerp(0f, 20f, interceptT * 0.007f);
            var v = Vector3.Slerp(relVec.normalized, Vector3.up, normAngle);

            // Make sure this is normalized! (The direction of your throw)
            projectileDir = v;

            // You'll probably want to leave this as is. For some prediction methods you can slow your throw down
            // You don't need to predict the speed of your throw. Only the direction assuming full speed.
            // Note that Law of Cosines with holdback WILL require adjusting this.
            projectileSpeed = maxProjectileSpeed;

            Vector3 delta = targetInitPos - projectilePos;

            for (int i = 0; i < 5; i++)
            {
                if (i != 0)
                {
                    delta = targetInitPos + (targetConstVel * interceptT) - projectilePos;
                }
                
                float inner = Mathf.Pow(Vector3.Dot(projectileGravity, delta) + Mathf.Pow(projectileSpeed, 2), 2) - (Mathf.Pow(Vector3.Magnitude(projectileGravity), 2) * Mathf.Pow(Vector3.Magnitude(delta), 2));
                if (inner < 0)
                {
                    return false;
                }

                float tPlus = Mathf.Sqrt(Vector3.Dot(projectileGravity, delta) + Mathf.Pow(projectileSpeed, 2) + Mathf.Sqrt(inner)) / Mathf.Sqrt(0.5f * Mathf.Pow(Vector3.Magnitude(projectileGravity), 2));
                float tMinus = Mathf.Sqrt(Vector3.Dot(projectileGravity, delta) + Mathf.Pow(projectileSpeed, 2) - Mathf.Sqrt(inner)) / Mathf.Sqrt(0.5f * Mathf.Pow(Vector3.Magnitude(projectileGravity), 2));

                if (tPlus < 0 && tMinus < 0 && i == 0)
                {
                    return false;
                }
                else if (tPlus < 0 && tMinus < 0)
                {
                    break;
                }
                else if (tPlus < 0)
                {
                    interceptT = tMinus;
                }
                else if (tMinus < 0)
                {
                    interceptT = tPlus;
                }
                else if (tMinus < tPlus)
                {
                    interceptT = tMinus;
                }
                else
                {
                    interceptT = tPlus;
                }
            }

            projectileDir = ((2 * delta) - (projectileGravity * Mathf.Pow(interceptT, 2))) / (2 * projectileSpeed * interceptT);

            // TODO return true or false based on whether target can actually be hit
            // This implementation just thinks, "I guess so?", and returns true.
            // Implementations that don't exactly solve intercepts will need to test the approximate
            // solution with maxAllowedErrorDist. If your solution does solve exactly, you will
            // probably want to add a debug assertion to check your solution against it.
            return true;

        }



    }

}