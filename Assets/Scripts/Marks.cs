using UnityEngine;
using System.Collections;
 
public class Marks : MonoBehaviour
{
    bool isShowTip;
    public bool WindowShow = false;

    void Start()
    {
        isShowTip = false;
    }

    void OnMouseEnter()
    {
        isShowTip = true;
    }

    void OnMouseExit()
    {
        isShowTip = false;
    }

    void OnGUI()
    {
        if (isShowTip) {
            GUIStyle style1= new GUIStyle();
            style1.fontSize = 10;
            style1.normal.textColor = Color.white;
            GUI.Label(new Rect(Input.mousePosition.x + 10, Screen.height - Input.mousePosition.y, 400, 50), this.name, style1);
        }
        
        if (WindowShow) {
            GUI.Window(0, new Rect(30, 30, 200, 100), MyWindow, this.name);
        }
    }

    void MyWindow(int WindowID)
    {
        GUILayout.Label(this.GetComponent<Renderer>().material.name.Substring(0,3));
    }

    void OnMouseDown()
    {
        if (WindowShow)
            WindowShow = false;
        else
            WindowShow = true;
    }
}