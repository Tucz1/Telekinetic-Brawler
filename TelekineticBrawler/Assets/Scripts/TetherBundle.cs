using UnityEngine;
using System.Collections.Generic;

public class TetherBundle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform playerAnchor;
    [SerializeField] Transform targetObject;
    [SerializeField] Collider targetCollider;

    [Header("Tether Settings")]
    [SerializeField] GameObject tetherPrefab;
    [SerializeField] int tetherCount = 16;
    [SerializeField] float attachRadius = 0.6f;

    [Header("Motion")]
    [SerializeField] float lagAmount = 0.12f;
    [SerializeField] float wobbleAmplitude = 0.08f;
    [SerializeField] float wobbleSpeed = 5f;

    [Header("Curve")]
    [SerializeField] int segments = 5;
    [SerializeField] float curveHeight = 0.25f;

    class Tether
    {
        public LineRenderer line;
        public Vector3 localAttachPoint;

        public Vector3 midPoint;
        public Vector3 midVelocity;

        public float phase;
    }

    List<Tether> tethers = new List<Tether>();

    void Start()
    {
        CreateTethers();
    }

    void CreateTethers()
    {
        for (int i = 0; i < tetherCount; i++)
        {
            GameObject obj = Instantiate(tetherPrefab, transform);
            LineRenderer line = obj.GetComponent<LineRenderer>();

            line.positionCount = segments;

            Tether tether = new Tether();

            tether.line = line;
            tether.phase = Random.Range(0f, 100f);

            tether.localAttachPoint = GetRandomAttachPoint();

            tether.midPoint = playerAnchor.position;

            tethers.Add(tether);
        }
    }

    Vector3 GetRandomAttachPoint()
    {
        if (targetCollider != null)
        {
            Bounds b = targetCollider.bounds;

            Vector3 random = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            );

            return targetObject.InverseTransformPoint(
                b.center + Vector3.Scale(random, b.extents)
            );
        }

        Debug.LogWarning("Collider not found");

        return Random.insideUnitSphere * attachRadius;
    }

    void Update()
    {
        if (targetObject == null) return;

        foreach (var tether in tethers)
        {
            Vector3 start = playerAnchor.position;
            Vector3 end = targetObject.TransformPoint(tether.localAttachPoint);

            Vector3 desiredMid = (start + end) * 0.5f;

            tether.midPoint = Vector3.SmoothDamp(
                tether.midPoint,
                desiredMid,
                ref tether.midVelocity,
                lagAmount
            );

            float wobble = Mathf.Sin(Time.time * wobbleSpeed + tether.phase) * wobbleAmplitude;
            Vector3 wobbleOffset = Random.onUnitSphere * wobble;

            tether.midPoint += wobbleOffset;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1); // The point at which a segment is at in the chain 0-1

                // Create two points to form a quadratic bezier curve
                Vector3 p1 = Vector3.Lerp(start, tether.midPoint, t); // Create point between start and middle
                Vector3 p2 = Vector3.Lerp(tether.midPoint, end, t); // Create point between middle and end

                Vector3 curve = Vector3.Lerp(p1, p2, t); // lerp between p1 & p2 blend the points for a curve

                float arc = Mathf.Sin(t * Mathf.PI) * curveHeight; // Add arc AKA how much of a peak the curve has

                curve += Vector3.up * arc; // It only goes up but fuck it who cares, magic

                tether.line.SetPosition(i, curve);
            }
        }
    }
}