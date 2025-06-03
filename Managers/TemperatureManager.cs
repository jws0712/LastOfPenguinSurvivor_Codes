// # System
using System;
using System.Collections.Generic;

// # UnityEngine
using UnityEngine;


// # Projects
using Lop.Survivor.Island.Buildingbase;

public class TemperatureManager : MonoBehaviour
{
    public static TemperatureManager Instance;

    public List<DayTemperatureData> dayTemperatureDataList;

    private Map map;
    private MapSettingManager mapSettingManager;


    
    private void Awake()
    {
        Instance = this;

        mapSettingManager = FindObjectOfType<MapSettingManager>();
        map = mapSettingManager.Map;
    }

    //pos 위치의 온도를 반환
    public int GetTemperature(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        //맵의 범위를 벗어나면 Nomal온도를 반환
        if (x < 0 || x > (mapSettingManager.MapWidthChunkValue * ChunkData.ChunkWidthValue) ||
            z < 0 || z > (mapSettingManager.MapLengthChunkValue * ChunkData.ChunkLengthValue) ||
            y <= mapSettingManager.ParentChunkYPos || y > ChunkData.ChunkHeightValue)
        {
            return (int)TemperatureType.Nomal; //Noaml 온도를 반환
        }

        return (int)map.MapBlock[x, y, z].blockTemperatureType;
    }

    //블럭의 온도를 반환
    public void UpdateTemperature()
    {
        //모닥불 오브젝를 찾아 보닥불 근처 블럭의 온도를 설정
        GameObject[] lightBuildings = GameObject.FindGameObjectsWithTag("Bonfire");

        foreach(var lightBuilding in lightBuildings)
        {
            lightBuilding.GetComponent<AreaEffectManager>().TemperatrueUp();
        }

        //맵의 모든 블럭을 검사
        for (int y = 0; y < ChunkData.ChunkHeightValue; y++)
        {
            for (int x = 0; x < mapSettingManager.MapWidthChunkValue * ChunkData.ChunkWidthValue; x++)
            {
                for (int z = 0; z < mapSettingManager.MapLengthChunkValue * ChunkData.ChunkLengthValue; z++)
                {
                    //검사한 블럭이 공기이면 온도를 설정하지 않음
                    if (map.MapBlock[x, y, z].id == BlockType.Air || x < 0 || z < 0)
                    {
                        continue;
                    }

                    //검사한 블럭이 물이라면 물블럭의 온도를 VeryCold로 설정
                    if (map.MapBlock[x, y, z].id == BlockType.Water)
                    {
                        map.MapBlock[x, y, z].blockTemperatureType = BlockTemperatureType.VeryCold;
                    }
                    else
                    {
                        //검사한 블럭이 물블럭이 아니라면 온도를 전체온도에 따라서 온도를 설정
                        switch (mapSettingManager.globalTemperature)
                        {
                            case TemperatureType.VeryCold:
                                {
                                    map.MapBlock[x, y, z].blockTemperatureType = BlockTemperatureType.VeryCold;
                                    break;
                                }

                            case TemperatureType.Cold:
                                {
                                    map.MapBlock[x, y, z].blockTemperatureType = BlockTemperatureType.Cold;

                                    break;
                                }

                            case TemperatureType.Nomal:
                                {
                                    map.MapBlock[x, y, z].blockTemperatureType = BlockTemperatureType.Nomal;
                                    break;
                                }

                            case TemperatureType.Heat:
                                {
                                    map.MapBlock[x, y, z].blockTemperatureType = BlockTemperatureType.Heat;
                                    break;
                                }

                            case TemperatureType.VeryHeat:
                                {
                                    map.MapBlock[x, y, z].blockTemperatureType = BlockTemperatureType.VeryHeat;
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }
}

[Serializable]
public class DayTemperatureData
{
    public TemperatureType[] temperatureTypesArray;
}
