using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Any math found in the four funcitons at the bottom has been extrapolated from the publications of Guy Somberg
//The specific spline architecture referenced in this code is based on solutions constructed by CatlikeCoding
public class ProjectToLineSegment : MonoBehaviour
{
    [SerializeField]
    private BezierSpline spline = null;

    [SerializeField]
    [Range(2, 1000)]
    private int resolution = 100;

    [SerializeField]
    private Transform listenerTransform = null;

    [SerializeField]
    [Range(0f, 1000f)]
    private float calculationRadius = 20f;

    [SerializeField]
    private DebugPhases debugPhase = DebugPhases.Segments;

    [SerializeField]
    private SplineMappedEmitter splineEmitter = null;

    private Vector3[] points;
    private Vector3[] startPoints;
    private Vector3[] endPoints;

    private Vector3 lastPos;

    private double splineRadius = 0f;

    private AverageAttenuatedDirection result = new AverageAttenuatedDirection();
    public struct AverageAttenuatedDirection
    {
        public Vector3 closestPoint;
        public Vector3 averageDirection;
        public double closestDistance;
        public double averageSpread;
    }

    public enum DebugPhases { None, Segments, Clipping, Spread, Averaged }

    private void Start()
    {
        points = new Vector3[resolution + 1];
        startPoints = new Vector3[resolution];
        endPoints = new Vector3[resolution];

        if (spline != null)
        {
            for (int i = 0; i <= resolution; i++)
            {
                points[i] = spline.GetPoint((float)i / (float)resolution);
                if (i > 0)
                {
                    Debug.DrawLine(points[i - 1], points[i], Color.white, 99999f);

                    startPoints[i - 1] = points[i - 1];
                    endPoints[i - 1] = points[i];
                }

                double distance = Vector3.Distance(spline.gameObject.transform.position, points[i]);
                if (distance > splineRadius)
                    splineRadius = distance;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debugPhase != DebugPhases.None)
        {
            Gizmos.color = new Color(0f, 1f, 1f, .5f);
            Gizmos.DrawSphere(this.listenerTransform.position, calculationRadius);
        }
    }

    private void Update()
    {
        switch (debugPhase)
        {
            case DebugPhases.None:
                if (spline != null && Vector3.Distance(listenerTransform.position, spline.transform.position) <= calculationRadius + splineRadius + 1f && transform.position != lastPos)
                {
                    lastPos = listenerTransform.position;

                    if (splineEmitter != null && SolveAverageAttenuatedDirection(startPoints, endPoints))
                    {
                        Vector3 translationTarget = listenerTransform.position + (result.averageDirection * (float)result.closestDistance);
                        splineEmitter.TranslateEmitter(new Vector3(translationTarget.x, result.closestPoint.y, translationTarget.z));//!Fix Y axis to closest river point. Only valid without HRTF implementation. Math needs to change otherwise.
                        splineEmitter.UpdateSpread((float)result.averageSpread);
                    }
                }
                break;
            case DebugPhases.Segments:
                if (spline != null && Vector3.Distance(listenerTransform.position, spline.transform.position) <= calculationRadius + splineRadius + 1f)
                {
                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        Vector3 targetPoint = GetNearestLineSegment(startPoints[i], endPoints[i]);

                        if (calculationRadius >= Vector3.Distance(listenerTransform.position, targetPoint))
                        {
                            Debug.DrawLine(listenerTransform.position, targetPoint, Color.green, 0);
                        }
                        else
                        {
                            Debug.DrawLine(listenerTransform.position, targetPoint, Color.red, 0);
                        }
                    }
                }
                break;
            case DebugPhases.Clipping:
                if (spline != null && Vector3.Distance(listenerTransform.position, spline.transform.position) <= calculationRadius + splineRadius + 1f)
                {
                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        bool validLine = ClipLineWithSphere(startPoints[i], endPoints[i], out Vector3 croppedLineStart, out Vector3 croppedLineEnd);

                        if (validLine)
                        {
                            Vector3 targetPoint = GetNearestLineSegment(croppedLineStart, croppedLineEnd);
                            Debug.DrawLine(listenerTransform.position, targetPoint, Color.green, 0);
                        }
                    }
                }
                break;
            case DebugPhases.Spread:
                if (spline != null && Vector3.Distance(listenerTransform.position, spline.transform.position) <= calculationRadius + splineRadius + 1f)
                {
                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        Vector3 targetPoint = TotalAttenuatedLineDirection(startPoints[i], endPoints[i], out double spread);

                        targetPoint = listenerTransform.position + (targetPoint * calculationRadius);
                        Debug.DrawLine(listenerTransform.position, targetPoint, Color.green, 0);

                        Debug.DrawLine(targetPoint, new Vector3(targetPoint.x, targetPoint.y + (float)spread, targetPoint.z), Color.blue, 0);
                    }
                }
                break;
            case DebugPhases.Averaged:
                if (spline != null && Vector3.Distance(listenerTransform.position, spline.transform.position) <= calculationRadius + splineRadius + 1f)
                {
                    if (SolveAverageAttenuatedDirection(startPoints, endPoints))
                    {
                        Debug.DrawLine(listenerTransform.position, result.closestPoint, Color.green, 0);

                        Vector3 projectedAverageDirection = listenerTransform.position + (result.averageDirection * (float)result.closestDistance);
                        Debug.DrawLine(listenerTransform.position, projectedAverageDirection, Color.yellow);
                        Debug.DrawLine(projectedAverageDirection, new Vector3(projectedAverageDirection.x, projectedAverageDirection.y + (float)result.averageSpread, projectedAverageDirection.z), Color.blue, 0);
                    }
                }
                break;
        }
    }

    private Vector3 GetNearestLineSegment(Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 projected = listenerTransform.position - lineStart;
        Vector3 translated = lineEnd - lineStart;
        double proportion = Vector3.Dot(projected, translated);
        if (proportion <= 0.0)
        {
            return lineStart;
        }
        double length_sq = Vector3.SqrMagnitude(translated);
        if (proportion >= length_sq)
        {
            return lineEnd;
        }

        return ((float)(proportion / length_sq) * translated) + lineStart;
    }

    private bool ClipLineWithSphere(Vector3 lineStart, Vector3 lineEnd, out Vector3 newLineStart, out Vector3 newLineEnd)
    {
        newLineStart = lineStart;
        newLineEnd = lineEnd;

        Vector3 closestPointToLine = GetNearestLineSegment(lineStart, lineEnd);
        double distanceToLineSq = Vector3.SqrMagnitude(closestPointToLine - listenerTransform.position);
        double radiusSq = (double)calculationRadius * calculationRadius;

        if (distanceToLineSq + double.Epsilon >= radiusSq)
            return false;

        Vector3 direction = lineEnd - lineStart;
        double segmentLenthSq = Vector3.SqrMagnitude(direction);
        double halfChordLengthSq = radiusSq - distanceToLineSq;
        double lengthToIntersect = Math.Sqrt(halfChordLengthSq / segmentLenthSq);

        Vector3 intersect = (float)lengthToIntersect * direction;

        if (Vector3.SqrMagnitude(lineStart - listenerTransform.position) > radiusSq)
        {
            newLineStart = closestPointToLine - intersect;
        }
        if (Vector3.SqrMagnitude(lineEnd - listenerTransform.position) > radiusSq)
        {
            newLineEnd = closestPointToLine + intersect;
        }
        return true;
    }

    private Vector3 TotalAttenuatedLineDirection(Vector3 preLineStart, Vector3 preLineEnd, out double spread)
    {
        if (ClipLineWithSphere(preLineStart, preLineEnd, out Vector3 lineStart, out Vector3 lineEnd))
        {
            double invRadius = 1.0 / calculationRadius;
            Vector3 start = (float)invRadius * (lineStart - listenerTransform.position);
            Vector3 direction = (float)invRadius * (lineEnd - lineStart);

            double dotEnd = Vector3.SqrMagnitude(direction);
            double dotStart = Vector3.SqrMagnitude(start);
            double dot2 = 2 * Vector3.Dot(start, direction);

            double param1 = Math.Sqrt(dotEnd + dot2 + dotStart);
            double param0 = Math.Sqrt(dotStart);
            double length = Math.Sqrt(dotEnd);

            double intInvRoot1 = Math.Log(Math.Max(double.Epsilon, 2 * length * param1 + 2 * dotEnd + dot2)) / length;
            double intInvRoot0 = Math.Log(Math.Max(double.Epsilon, 2 * length * param0 + dot2)) / length;

            double int_T_InvRoot1 = (param1 / dotEnd) - (dot2 * intInvRoot1) / (2 * dotEnd);
            double int_T_InvRoot0 = (param0 / dotEnd) - (dot2 * intInvRoot0) / (2 * dotEnd);

            //Spread
            double discriminent = 4 * dotEnd * dotStart - dot2 * dot2;
            double totalWeighting1 = 1 - ((2 * dotEnd + dot2) * param1) / (4 * dotEnd) - (discriminent * intInvRoot1) / (8 * dotEnd);
            double totalWeighting0 = -(dot2 * param0) / (4 * dotEnd) - (discriminent * intInvRoot0) / (8 * dotEnd);

            spread = length * (totalWeighting1 - totalWeighting0);

            //!Temp fix to smooth out Vector3 floating point errors.
            if (spread <= 0.0)
            {
                spread = 0.0;
                return Vector3.zero;
            }
            //-----

            double definiteStart = intInvRoot1 - intInvRoot0;
            double definiteEnd = int_T_InvRoot1 - int_T_InvRoot0;

            double x = start.x * definiteStart + direction.x * definiteEnd - start.x - direction.x / 2f;
            double y = start.y * definiteStart + direction.y * definiteEnd - start.y - direction.y / 2f;
            double z = start.z * definiteStart + direction.z * definiteEnd - start.z - direction.z / 2f;

            Vector3 totalDirection = new Vector3((float)x, (float)y, (float)z);
            return (float)length * totalDirection;
        }
        spread = 0.0;
        return Vector3.zero;
    }

    private bool SolveAverageAttenuatedDirection(Vector3[] lineStarts, Vector3[] lineEnds)
    {
        Vector3 totalDirection = Vector3.zero;
        double totalWeighting = 0.0;

        double closestDistanceSq = double.MaxValue;
        bool withinRange = false;
        double radiusSq = (double)calculationRadius * calculationRadius;

        for (int i = 0; i < lineStarts.Length; i++)
        {
            Vector3 closestPoint = GetNearestLineSegment(lineStarts[i], lineEnds[i]);
            double distanceSq = Vector3.SqrMagnitude(closestPoint - listenerTransform.position);

            if (distanceSq < closestDistanceSq)
            {
                closestDistanceSq = distanceSq;
                result.closestPoint = closestPoint;
            }

            if (distanceSq + double.Epsilon < radiusSq)
            {
                totalDirection += TotalAttenuatedLineDirection(lineStarts[i], lineEnds[i], out double weighting);
                Debug.Log(weighting);
                totalWeighting += weighting;
                withinRange = true;
            }
        }

        double totalLength = Math.Sqrt(Vector3.SqrMagnitude(totalDirection));
        result.closestDistance = Math.Sqrt(closestDistanceSq);

        if (totalLength > double.Epsilon)
        {
            result.averageSpread = 1.0 - totalLength / totalWeighting;
            result.averageDirection = totalDirection / (float)totalLength;
        }
        else if (closestDistanceSq < radiusSq)
        {
            result.averageSpread = 1.0;
            result.averageDirection = result.closestPoint;
        }
        else
        {
            result.averageSpread = 0.0;
            result.averageDirection = result.closestPoint;
        }

        return withinRange;
    }
}