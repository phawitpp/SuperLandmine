using UnityEngine;

namespace SuperLandmine {
    public static class Utils {
        public const float MAX_RAYCAST_DIST = 30000f;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns>A tuple containing the new position and quaternion when projected to the ground</returns>
        public static (Vector3, Quaternion) projectToGround(Vector3 position) {
            Ray ray = new Ray(position, Vector3.down);
            Quaternion quaternion = Quaternion.identity;
            Vector3 pos = position;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, MAX_RAYCAST_DIST)) {
                pos = hit.point;
                quaternion = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
            return (pos, quaternion);
        }

        /// <summary>
        /// A marker class to identify outside landmines
        /// </summary>
        public class OutsideLandmineMarker : MonoBehaviour { }
    }
}
