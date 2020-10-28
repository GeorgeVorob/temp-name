/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * Various assorted utilities functions
 * */
public static class CMUtilsClass
{

    private static readonly Vector3 Vector3zero = Vector3.zero;
    private static readonly Vector3 Vector3one = Vector3.one;
    private static readonly Vector3 Vector3yDown = new Vector3(0, -1);

    public const int sortingOrderDefault = 5000;

    private static Transform cachedCanvasTransform;

    // Create Text in the World
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    // Create Text in the World
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }


    // Create a Text Popup in the World, no parent
    public static void CreateWorldTextPopup(string text, Vector3 localPosition)
    {
        CreateWorldTextPopup(null, text, localPosition, 40, Color.white, localPosition + new Vector3(0, 20), 1f);
    }

    // Create a Text Popup in the World
    public static void CreateWorldTextPopup(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, Vector3 finalPopupPosition, float popupTime)
    {
        TextMesh textMesh = CreateWorldText(parent, text, localPosition, fontSize, color, TextAnchor.LowerLeft, TextAlignment.Left, sortingOrderDefault);
        Transform transform = textMesh.transform;
        Vector3 moveAmount = (finalPopupPosition - localPosition) / popupTime;
        FunctionUpdater.Create(delegate ()
        {
            transform.position += moveAmount * Time.deltaTime;
            popupTime -= Time.deltaTime;
            if (popupTime <= 0f)
            {
                UnityEngine.Object.Destroy(transform.gameObject);
                return true;
            }
            else
            {
                return false;
            }
        }, "WorldTextPopup");
    }

    // Generate random normalized direction
    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
    {
        return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}