using UnityEngine;
using System;
using System.Collections;

public class TerrainColor : MonoBehaviour
{
    private TerrainData terrainData;
    private float[, ,] splatmapData;

    bool isShowTip;

    void Awake()
    {
        terrainData = Terrain.activeTerrain.terrainData;
        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
    }

    void Start()
    {
        isShowTip = false;
        splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
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
            RaycastHit hitt = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hitt);

            GUIStyle style1= new GUIStyle();
            style1.fontSize = 10;
            style1.normal.textColor = Color.white;
            GUI.Label(new Rect(Input.mousePosition.x + 10, Screen.height - Input.mousePosition.y, 400, 50), getTemperature(hitt.point.x, hitt.point.z), style1);
        }
    }

    string getTemperature(float x_world, float y_world)
    {
        int x = Convert.ToInt32((y_world + 500) / 1000 * 512);
        int y = Convert.ToInt32((x_world + 500) / 1000 * 512);
        int result = 0;

        for (int layer = 0; layer < 26; layer++) {
            if (splatmapData[x, y, layer] == 1) {
                result = layer + 10;
            } 
        }

        return result.ToString() + "C";
    }
}