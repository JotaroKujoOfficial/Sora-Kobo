using UnityEngine;

namespace 
SoraKobo.CameraSystem
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        public Vector3 offset = new Vector3(0, 1, -10);

        [Header("Smoothing")]
        public float smoothSpeed = 6f;

        [Header("Bounds")]
        public bool useBounds = true;
        public float minX = -10f;
        public float maxX = 110f;
        public float minY = -5f;
        public float maxY = 60f;

        private  
    UnityEngine.Camera _cam;

        void Start()
        {
            _cam = GetComponent<UnityEngine.Camera>();
        }

        void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;

            if (useBounds)
            {
                float halfH = _cam ? _cam.orthographicSize : 5f;
                float halfW = _cam ? halfH * _cam.aspect : 5f;

                desired.x = Mathf.Clamp(desired.x, minX + halfW, maxX - halfW);
                desired.y = Mathf.Clamp(desired.y, minY + halfH, maxY - halfH);
            }

            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform t)
        {
            target = t;
        }

        public void SetBounds(float minx, float maxx, float miny, float maxy)
        {
            minX = minx; maxX = maxx; minY = miny; maxY = maxy;
        }
    }
}