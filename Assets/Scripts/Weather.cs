using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class TemperatureData {
    public string place;
    public int value;
    public string unit;
}

[Serializable]
public class Temperature {
    public TemperatureData[] data;
    public string recordTime;
}

[Serializable]
public class WeatherInfo {
    public int[] icon;
    public Temperature temperature;
}

public class Weather : MonoBehaviour
{
    public Rain Rain;
    public Light Light;
    public bool Fog = false;
    public bool IsNight = false;

    public Material Sunny;
    public Material Sunny_Periods;
    public Material Cloudy;
    public Material Night_Fine;
    public Material Night_Cloudy;
    public Material Night_MainlyFine;
    public Material Thunderstorm;

    public GameObject WongChukHang;
    public GameObject HongKongPark;
    public GameObject ShauKeiWan;
    public GameObject HappyValley;
    public GameObject Stanley;

    private int mark1 = 0;
    private int mark2 = 0;
    private int mark3 = 0;
    private int mark4 = 0;
    private int mark5 = 0;

    public Material C10;
    public Material C11;
    public Material C12;
    public Material C13;
    public Material C14;
    public Material C15;
    public Material C16;
    public Material C17;
    public Material C18;
    public Material C19;
    public Material C20;
    public Material C21;
    public Material C22;
    public Material C23;
    public Material C24;
    public Material C25;
    public Material C26;
    public Material C27;
    public Material C28;
    public Material C29;
    public Material C30;
    public Material C31;
    public Material C32;
    public Material C33;
    public Material C34;
    public Material C35;

    private TerrainData terrainData;
    private float[, ,] splatmapData;

    void Awake()
    {
        terrainData = Terrain.activeTerrain.terrainData;
        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
    }

    void Start()
    {
        StartCoroutine(SetWeather());
    }
    
    void Updata()
    {
        //SetWeather();
    }

    IEnumerator SetWeather()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://data.weather.gov.hk/weatherAPI/opendata/weather.php?dataType=rhrread&lang=en");

        yield return webRequest.SendWebRequest();
        
        if (webRequest.isHttpError||webRequest.isNetworkError) {
            Debug.Log(webRequest.error);
        } else {
            string savePath = Application.streamingAssetsPath + "/Weather.json";
            File.WriteAllText(savePath, Regex.Unescape(webRequest.downloadHandler.text));
            
            StreamReader streamReader = new StreamReader(savePath);
            string str = streamReader.ReadToEnd();

            char[] str_char = str.ToCharArray();
            string str_new = "";
            foreach (char c in str_char) {
                if (c == (char)10) {
                    continue;
                }
                if (c == (char)13) {
                    continue;
                }
                str_new += c.ToString();
            }

            WeatherInfo weatherInfo = JsonUtility.FromJson<WeatherInfo>(str_new);
            streamReader.Close();

            if (System.DateTime.Now.Hour >= 19 || System.DateTime.Now.Hour <= 6) {
                IsNight = true;
            }

            for (int i = 0; i < weatherInfo.icon.Length; i++) {
                //Sunny
                if (weatherInfo.icon[i] == 50) {
                    RenderSettings.skybox = Sunny;
                    Light.intensity = 1.0f;
                    Rain.RainIntensity = 0.0f;
                //Sunny Periods & Sunny Intervals
                } else if (weatherInfo.icon[i] == 51 || weatherInfo.icon[i] == 52) {
                    RenderSettings.skybox = Sunny_Periods;
                    Light.intensity = 0.8f;
                    Rain.RainIntensity = 0.0f;
                //Sunny Periods with A Few Showers
                } else if (weatherInfo.icon[i] == 53) {
                    RenderSettings.skybox = Sunny_Periods;
                    Light.intensity = 0.8f;
                    Rain.RainIntensity = 0.2f;
                //Sunny Intervals with Showers
                } else if (weatherInfo.icon[i] == 54) {
                    RenderSettings.skybox = Sunny_Periods;
                    Light.intensity = 0.8f;
                    Rain.RainIntensity = 0.3f;
                //Cloudy & Overcast
                } else if (weatherInfo.icon[i] == 60 || weatherInfo.icon[i] == 61) {
                    if (IsNight == true) {
                        RenderSettings.skybox = Night_Cloudy;
                    } else {
                        RenderSettings.skybox = Cloudy;
                    }
                    Light.intensity = 0.5f;
                    Rain.RainIntensity = 0.0f;
                //Light Rain
                } else if (weatherInfo.icon[i] == 62) {
                    if (IsNight == true) {
                        RenderSettings.skybox = Night_Cloudy;
                    } else {
                        RenderSettings.skybox = Cloudy;
                    }
                    Light.intensity = 0.5f;
                    Rain.RainIntensity = 0.3f;
                //Rain
                } else if (weatherInfo.icon[i] == 63) {
                    if (IsNight == true) {
                        RenderSettings.skybox = Night_Cloudy;
                    } else {
                        RenderSettings.skybox = Cloudy;
                    }
                    Light.intensity = 0.5f;
                    Rain.RainIntensity = 0.5f;
                //Heavy Rain
                } else if (weatherInfo.icon[i] == 64) {
                    if (IsNight == true) {
                        RenderSettings.skybox = Night_Cloudy;
                    } else {
                        RenderSettings.skybox = Cloudy;
                    }
                    Light.intensity = 0.4f;
                    Rain.RainIntensity = 0.7f;
                //Thunderstorm
                } else if (weatherInfo.icon[i] == 65) {
                    RenderSettings.skybox = Thunderstorm;
                    Light.intensity = 0.3f;
                    Rain.RainIntensity = 1.0f;
                //Fine (for night)
                } else if (weatherInfo.icon[i] == 70 || weatherInfo.icon[i] == 71 || weatherInfo.icon[i] == 72 || weatherInfo.icon[i] == 73 ||weatherInfo.icon[i] == 74 || weatherInfo.icon[i] == 75) {
                    RenderSettings.skybox = Night_Fine;
                    Light.intensity = 1.0f;
                    Rain.RainIntensity = 0.0f;
                //Mainly Cloudy (for night)
                } else if (weatherInfo.icon[i] == 76) {
                    RenderSettings.skybox = Night_Cloudy;
                    Light.intensity = 0.7f;
                    Rain.RainIntensity = 0.0f;
                //Mainly Fine (for night)
                } else if (weatherInfo.icon[i] == 77) {
                    RenderSettings.skybox = Night_MainlyFine;
                    Light.intensity = 0.8f;
                    Rain.RainIntensity = 0.0f;
                //Fog
                } else if (weatherInfo.icon[i] == 83) {
                    Fog = true;
                    RenderSettings.fogEndDistance = 50;
                //Mist
                } else if (weatherInfo.icon[i] == 84) {
                    Fog = true;
                    RenderSettings.fogEndDistance = 100;
                //Haze
                } else if (weatherInfo.icon[i] == 85) {
                    Fog = true;
                    RenderSettings.fogEndDistance = 1000;
                }
            }

            RenderSettings.fog = Fog;

            for (int i = 0; i < weatherInfo.temperature.data.Length; i++) {
                if (weatherInfo.temperature.data[i].place == "Wong Chuk Hang") {
                    mark1 = weatherInfo.temperature.data[i].value;
                    ChangeColor(WongChukHang, mark1);
                } else if (weatherInfo.temperature.data[i].place == "Hong Kong Park") {
                    mark2 = weatherInfo.temperature.data[i].value;
                    ChangeColor(HongKongPark, mark2);
                } else if (weatherInfo.temperature.data[i].place == "Shau Kei Wan") {
                    mark3 = weatherInfo.temperature.data[i].value;
                    ChangeColor(ShauKeiWan, mark3);
                } else if (weatherInfo.temperature.data[i].place == "Happy Valley") {
                    mark4 = weatherInfo.temperature.data[i].value;
                    ChangeColor(HappyValley, mark4);
                } else if (weatherInfo.temperature.data[i].place == "Stanley") {
                    mark5 = weatherInfo.temperature.data[i].value;
                    ChangeColor(Stanley, mark5);
                }
            }

            SetTerrain();
        }
    }

    private void ChangeColor (GameObject place, int temperature)
    {
        if (temperature == 10) {
            place.GetComponent<Renderer>().material = C10;
        } else if (temperature == 11) {
            place.GetComponent<Renderer>().material = C11;
        } else if (temperature == 12) {
            place.GetComponent<Renderer>().material = C12;
        } else if (temperature == 13) {
            place.GetComponent<Renderer>().material = C13;
        } else if (temperature == 14) {
            place.GetComponent<Renderer>().material = C14;
        } else if (temperature == 15) {
            place.GetComponent<Renderer>().material = C15;
        } else if (temperature == 16) {
            place.GetComponent<Renderer>().material = C16;
        } else if (temperature == 17) {
            place.GetComponent<Renderer>().material = C17;
        } else if (temperature == 18) {
            place.GetComponent<Renderer>().material = C18;
        } else if (temperature == 19) {
            place.GetComponent<Renderer>().material = C19;
        } else if (temperature == 20) {
            place.GetComponent<Renderer>().material = C20;
        } else if (temperature == 21) {
            place.GetComponent<Renderer>().material = C21;
        } else if (temperature == 22) {
            place.GetComponent<Renderer>().material = C22;
        } else if (temperature == 23) {
            place.GetComponent<Renderer>().material = C23;
        } else if (temperature == 24) {
            place.GetComponent<Renderer>().material = C24;
        } else if (temperature == 25) {
            place.GetComponent<Renderer>().material = C25;
        } else if (temperature == 26) {
            place.GetComponent<Renderer>().material = C26;
        } else if (temperature == 27) {
            place.GetComponent<Renderer>().material = C27;
        } else if (temperature == 28) {
            place.GetComponent<Renderer>().material = C28;
        } else if (temperature == 29) {
            place.GetComponent<Renderer>().material = C29;
        } else if (temperature == 30) {
            place.GetComponent<Renderer>().material = C30;
        } else if (temperature == 31) {
            place.GetComponent<Renderer>().material = C31;
        } else if (temperature == 32) {
            place.GetComponent<Renderer>().material = C32;
        } else if (temperature == 33) {
            place.GetComponent<Renderer>().material = C33;
        } else if (temperature == 34) {
            place.GetComponent<Renderer>().material = C34;
        } else if (temperature == 35) {
            place.GetComponent<Renderer>().material = C35;
        }
    }

    private void SetTerrain()
    {
        for (int y = 0; y < terrainData.alphamapHeight; y++) {
            for (int x = 0; x < terrainData.alphamapWidth; x++) {
                int temp_temperature = TemperatureMark(x, y);
                
                for (int layer = 0; layer < 26; layer++) {
                    if ((layer + 10) == temp_temperature) {
                        splatmapData[x, y, layer] = 1;
                    } else {
                        splatmapData[x, y, layer] = 0;
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private int TemperatureMark(int x, int y)
    {
        int temp = ((x - 343) * (x - 343)) + ((y - 175) * (y - 175));
        int result = mark1;

        if((((x - 456) * (x - 456)) + ((y - 126) * (y - 126))) < temp) {
            temp = ((x - 456) * (x - 456)) + ((y - 126) * (y - 126));
            result = mark2;
        }
        if((((x - 451) * (x - 451)) + ((y - 305) * (y - 305))) < temp) {
            temp = ((x - 451) * (x - 451)) + ((y - 305) * (y - 305));
            result = mark3;
        }
        if((((x - 435) * (x - 435)) + ((y - 205) * (y - 205))) < temp) {
            temp = ((x - 435) * (x - 435)) + ((y - 205) * (y - 205));
            result = mark4;
        }
        if((((x - 266) * (x - 266)) + ((y - 289) * (y - 289))) < temp) {
            temp = ((x - 266) * (x - 266)) + ((y - 289) * (y - 289));
            result = mark5;
        }
        
        return result;
    }
}