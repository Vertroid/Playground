using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class CircleImage : Image
{
    public float Radius;
    [Range(MIN_TRIANGLE_NUM, MAX_TRIANGLE_NUM)]
    public int TriangleNum;

    private const int MIN_TRIANGLE_NUM = 1;
    private const int MAX_TRIANGLE_NUM = 20;

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        Vector4 v = GetDrawingDimensions(false);
        Vector4 uv = overrideSprite == null ? Vector4.zero : DataUtility.GetOuterUV(overrideSprite);

        var color32 = color;
        toFill.Clear();
        float r = Radius;
        if (r > (v.z - v.x) / 2)
            r = (v.z - v.x) / 2;
        else if (r > (v.w - v.y) / 2)
            r = (v.w - v.y) / 2;
        else if (r < 0)
            r = 0;

        float uvRadiusX = r / (v.z - v.x);
        float uvRadiusY = r / (v.w - v.y);

        // x = left, y = bottom, z = right, w = top
        //0，1
        toFill.AddVert(new Vector3(v.x, v.w - r), color32, new Vector2(uv.x, uv.w - uvRadiusY));
        toFill.AddVert(new Vector3(v.x, v.y + r), color32, new Vector2(uv.x, uv.y + uvRadiusY));

        //2，3，4，5
        toFill.AddVert(new Vector3(v.x + r, v.w), color32, new Vector2(uv.x + uvRadiusX, uv.w));
        toFill.AddVert(new Vector3(v.x + r, v.w - r), color32, new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
        toFill.AddVert(new Vector3(v.x + r, v.y + r), color32, new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
        toFill.AddVert(new Vector3(v.x + r, v.y), color32, new Vector2(uv.x + uvRadiusX, uv.y));

        //6，7，8，9
        toFill.AddVert(new Vector3(v.z - r, v.w), color32, new Vector2(uv.z - uvRadiusX, uv.w));
        toFill.AddVert(new Vector3(v.z - r, v.w - r), color32, new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
        toFill.AddVert(new Vector3(v.z - r, v.y + r), color32, new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
        toFill.AddVert(new Vector3(v.z - r, v.y), color32, new Vector2(uv.z - uvRadiusX, uv.y));

        //10，11
        toFill.AddVert(new Vector3(v.z, v.w - r), color32, new Vector2(uv.z, uv.w - uvRadiusY));
        toFill.AddVert(new Vector3(v.z, v.y + r), color32, new Vector2(uv.z, uv.y + uvRadiusY));

        //左边的矩形
        toFill.AddTriangle(1, 0, 3);
        toFill.AddTriangle(1, 3, 4);
        //中间的矩形
        toFill.AddTriangle(5, 2, 6);
        toFill.AddTriangle(5, 6, 9);
        //右边的矩形
        toFill.AddTriangle(8, 7, 10);
        toFill.AddTriangle(8, 10, 11);

        //开始构造四个角
        List<Vector2> vCenterList = new List<Vector2>();
        List<Vector2> uvCenterList = new List<Vector2>();
        List<int> vCenterVertList = new List<int>();

        //右上角的圆心
        vCenterList.Add(new Vector2(v.z - r, v.w - r));
        uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
        vCenterVertList.Add(7);

        //左上角的圆心
        vCenterList.Add(new Vector2(v.x + r, v.w - r));
        uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
        vCenterVertList.Add(3);

        //左下角的圆心
        vCenterList.Add(new Vector2(v.x + r, v.y + r));
        uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
        vCenterVertList.Add(4);

        //右下角的圆心
        vCenterList.Add(new Vector2(v.z - r, v.y + r));
        uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
        vCenterVertList.Add(8);

        //每个三角形的顶角, 90度除以数量
        float degreeDelta = (float)(Mathf.PI / 2 / TriangleNum);
        //当前的角度
        float curDegree = 0;

        for (int i = 0; i < vCenterVertList.Count; i++)
        {
            int preVertNum = toFill.currentVertCount;
            for (int j = 0; j <= TriangleNum; j++)
            {
                float cosA = Mathf.Cos(curDegree);
                float sinA = Mathf.Sin(curDegree);
                Vector3 vPosition = new Vector3(vCenterList[i].x + cosA * r, vCenterList[i].y + sinA * r);
                Vector3 uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX, uvCenterList[i].y + sinA * uvRadiusY);
                toFill.AddVert(vPosition, color32, uvPosition);
                curDegree += degreeDelta;
            }
            curDegree -= degreeDelta;
            for (int j = 0; j <= TriangleNum - 1; j++)
            {
                toFill.AddTriangle(vCenterVertList[i], preVertNum + j + 1, preVertNum + j);
            }
        }
    }

    private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
    {
        var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
        Rect r = GetPixelAdjustedRect();
        var size = overrideSprite == null ? new Vector2(r.width, r.height) : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

        int spriteW = Mathf.RoundToInt(size.x);
        int spriteH = Mathf.RoundToInt(size.y);

        // 保持图像宽高
        if(shouldPreserveAspect && size.sqrMagnitude > 0.0f)
        {
            var spriteRatio = size.x / size.y;
            var rectRatio = r.width / r.height;
            if (spriteRatio > rectRatio)
            {
                var oldHeight = r.height;
                r.height = r.width * (1.0f / spriteRatio);
                r.y += (oldHeight - r.height) * rectTransform.pivot.y;
            }
            else
            {
                var oldWidth = r.width;
                r.width = r.height * spriteRatio;
                r.x += (oldWidth - r.width) * rectTransform.pivot.x;
            }
        }

        var v = new Vector4(
            padding.x / spriteW,
            padding.y / spriteH,
            (spriteW - padding.z) / spriteW,
            (spriteH - padding.w) / spriteH
            );
        v = new Vector4(
            r.x + r.width * v.x,
            r.y + r.height * v.y,
            r.x + r.width * v.z,
            r.y + r.height * v.w
            );
        return v;
    }
}
