using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TetherBundle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerAnchor;
    private Transform targetObject;
    private Collider targetCollider;
    private List<GameObject> gameObjectList = new();
    private List<LineRenderer> lineList = new();
    private List <Tether> tetherList = new();

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

    bool connectedTethers;

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
        // CreateTethers();
    }

    public void CreateTethers(Transform _targetObject, Collider _targetCollider)
    {
        // Debug.Log($"player anchor: {playerAnchor}");
        targetObject = _targetObject;
        // Debug.Log($"target object: {targetObject}");
        targetCollider = _targetCollider;
        // Debug.Log($"target collider: {targetCollider}");
        // Debug.LogError("Pausing");

        for (int i = 0; i < tetherCount; i++)
        {
            GameObject obj = Instantiate(tetherPrefab, transform);
            gameObjectList.Add(obj);

            LineRenderer line = obj.GetComponent<LineRenderer>();
            lineList.Add(line);

            line.positionCount = segments;

            Tether tether = new Tether();
            tetherList.Add(tether);

            tether.line = line;
            tether.phase = Random.Range(0f, 100f);

            tether.localAttachPoint = GetRandomAttachPoint();

            tether.midPoint = playerAnchor.position;

            tethers.Add(tether);
        }
        connectedTethers = true;
    }

    public void ClearTethers()
    {
        connectedTethers = false;

        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }

        // foreach (Tether tether in tetherList)
        // {
            
        // }

        gameObjectList.Clear();
        lineList.Clear();
        tetherList.Clear();
        tethers.Clear();
    }

    Vector3 GetRandomAttachPoint()
    {
        if (targetCollider != null)
        {
            BoxCollider box = targetCollider as BoxCollider;

            Vector3 local = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0f, 0.5f),
                Random.Range(-0.5f, 0.5f)
            );

            Vector3 worldPoint = box.transform.TransformPoint(
                Vector3.Scale(local, box.size)
            );

            return targetObject.InverseTransformPoint(worldPoint);
        }

        Debug.LogWarning("Collider not found");

        return Random.insideUnitSphere * attachRadius;
    }

    void Update()
    {
        if (connectedTethers == false) return;
        if (targetObject == null) return;

        foreach (var tether in tethers)
        {
            Vector3 start = playerAnchor.position;
            Vector3 end = targetObject.TransformPoint(tether.localAttachPoint);
            // Vector3 end = weaponRoot.TransformPoint(tether.localAttachPoint);

            Debug.DrawLine(start, end, Color.red);
            Debug.DrawRay(end, Vector3.up * 0.2f, Color.green);

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