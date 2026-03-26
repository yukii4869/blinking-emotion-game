using UnityEngine;

public class EARCalculator : MonoBehaviour
{
    private int[] leftEye = { 33, 159, 158, 133, 153, 145 };
    private int[] rightEye = { 362, 386, 387, 263, 374, 380 };
    public float ComputeEAR(Landmark[] lm, int[] idx)
    {
        Vector3 p1 = ToVec(lm[idx[0]]);
        Vector3 p2 = ToVec(lm[idx[1]]);
        Vector3 p3 = ToVec(lm[idx[2]]);
        Vector3 p4 = ToVec(lm[idx[3]]);
        Vector3 p5 = ToVec(lm[idx[4]]);
        Vector3 p6 = ToVec(lm[idx[5]]);

        float vert1 = Vector3.Distance(p2, p6);
        float vert2 = Vector3.Distance(p3, p5);
        float horiz = Vector3.Distance(p1, p4);

        return (vert1 + vert2) / (2f * horiz);
    }
    public float ComputeBothEyes(Landmark[] lm)
    {
        float left = ComputeEAR(lm, leftEye);
        float right = ComputeEAR(lm, rightEye);
        return (left + right) * 0.5f;
    }

    private Vector3 ToVec(Landmark l)
    {
        return new Vector3(l.x, l.y, l.z);
    }
}