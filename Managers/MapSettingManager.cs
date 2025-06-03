    // # System
using Lop.Survivor.Island;
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Static
using static BlockType;
using static BuildingType;

// # Lop
using Lop.Survivor.Island.Buildingbase;
using Lop.Survivor;
using Lop.Survivor.Structures;

public class MapSettingManager : MonoBehaviour
{
    public static MapSettingManager     Instance { get; private set; }

    public  ChunkSync                   chunkSync;             // 청크 공유를 위한 클래스

    private Map                         map;

    [Header("# Map Save & Load Mode")]
    [SerializeField]
    private MapEditorMode               editorMode;

    [Header("[# About Changes in Terrain  Every Day]")]
    [SerializeField]
    private List<FaceProbabilityData>   faceProbabilityDatas = new List<FaceProbabilityData>();
    [SerializeField]
    private int                         blockInitRemovalRange;       // 기본 시작 범위
    [SerializeField]
    private int                         dailyRangeIncrement;         // 하루마다 추가될 범위
    private int                         totalRemovalRange;

    [Header("[# Abuot Chunk Parents ]")]
    [SerializeField]
    private Transform                   waterChunkParent;
    [SerializeField]
    private Transform                   groundChunkParent;
    [SerializeField]
    public float                        parentChunkYPos = -1f;

    [Header("[# Abuot Creating Map]")]
    [SerializeField]
    private GameObject                  chunkPrefab;
    [SerializeField]
    private Material                    mapGroundMaterial;       // 맵 텍스쳐 아틀라스 
    [SerializeField]
    private Material                    mapWaterMaterial;         // 맵 텍스쳐 아틀라스 

    [Header("[# Abuot Map Size ]")]
    [SerializeField]
    private int                         mapWidthChunkValue;     // 맵 청크 가로 개수 
    [SerializeField]
    private int                         mapLengthChunkValue;    // 맵 청크 세로 개수 
    [SerializeField]
    private int                         mapBorderWidth;         // 테두리 너비

    [Header("[# About Setting Noise ]")]
    [SerializeField]
    private float                       scale;                  //  펄린 노이즈의 크기(scale)      
    [SerializeField, Range(0, 2000)]        
    private int                         seed;                   //  랜덤 시드(seed) 값

	[Header("[# Soil Tilling ]")]
    [SerializeField]
    private int                         minSoilTilingLength;    // 땅 경작이 이어지는 최소 칸 수
    [SerializeField]
    private int                         soilDryTime;            // 땅 경작 마르는 시간

    [Header("[# About Data ]")]
    [SerializeField]
    private BlockDataList[]             blockDataList;          // 같은 타입의 블럭 데이터를 담고 있지만, 텍스쳐만 다름 
    [SerializeField]
    private BlockTextureDataList[]      blockTextureDataList;   // 같은 타입의 블럭 데이터를 담고 있지만, 텍스쳐만 다름 
    [SerializeField]
    private BlockCreationRuleData[]     blockCreationRuleList;  // 블럭 생성 규칙

    [Header("[# Tree Setting]")]
    [SerializeField]
    private int                         treePlacePersent;
    [SerializeField]
    private int                         treeTypePersent;

	private int                         oreSpawnProbability = 0; // 광물 생성 확률

    [Header("[# Water Setting]")]
    [SerializeField]
    private float                       waterMapYScale;          //물의 해수면 높이

    #region 프로퍼티 ( Get 기능 )
    // # 프로퍼티 ( Get 기능 )
    public int                      TotalRemovalRange { get => totalRemovalRange; set { totalRemovalRange = Mathf.Max(0, value); } }
    public int                      BlockInitRemovalRange => blockInitRemovalRange;    // 기본 시작 범위
    public int                      DailyRangeIncrement   => dailyRangeIncrement;

    public Map                      Map                  => map;

    public Transform                WaterChunkParent     => waterChunkParent;
    public Transform                GroundChunkParent    => groundChunkParent;

    public Material                 MapGroundMaterial    => mapGroundMaterial;
    public Material                 MapWaterMaterial     => mapWaterMaterial;

    public int                      MapWidthChunkValue   => mapWidthChunkValue;
    public int                      MapLengthChunkValue  => mapLengthChunkValue;

    public int                      MapBorderWidth       => mapBorderWidth;

    public float                    Scale                => scale;
    public int                      Seed                 => seed;

    public float                    ParentChunkYPos      => parentChunkYPos;

    public int                      MinSoilTilingLength  => minSoilTilingLength;

    public int                      OreSpawnProbability => oreSpawnProbability;

    public float                    WaterMapYScale => waterMapYScale;


	public BlockDataList[]          BlockDataList        => blockDataList;
    #endregion

    [Space(20)]
    private  TemperatureType             globalTemperature;
	private TemperatureType             lastTemperature = default;
	private TimeType                    timeType;

    //초기화
    private void Start()
    {
        TemperatureManager.Instance.UpdateTemperature();
    }

    private void Update()
    {
        if(chunkSync == null || !chunkSync.isActiveAndEnabled) { return; }

        int dayCount = TimeManager.Instance.CurrentDay - 1;

        if(dayCount < 0)
        {
            dayCount = 1;
        }

        if(dayCount >= TemperatureManager.Instance.dayTemperatureDataList.Count)
        {
            dayCount = TemperatureManager.Instance.dayTemperatureDataList.Count - 1;
        }

        if (TimeManager.Instance.CurrentTimeData != null)
        {
            //시간이 바뀔때마다 특정 온도로 업데이트
            switch (TimeManager.Instance.CurrentTimeType)
            {
                case TimeType.Day:
                    {
                        globalTemperature = TemperatureManager.Instance.dayTemperatureDataList[dayCount].temperatureTypesArray[0];
                        UpdateTemperature();
                        break;
                    }
                case TimeType.Afternoon:
                    {
                        globalTemperature = TemperatureManager.Instance.dayTemperatureDataList[dayCount].temperatureTypesArray[1];
                        UpdateTemperature();
                        break;
                    }
                case TimeType.Night:
                    {
                        globalTemperature = TemperatureManager.Instance.dayTemperatureDataList[dayCount].temperatureTypesArray[2];
                        UpdateTemperature();
                        break;
                    }
            }
        }

    }

    //온도 업데이트
    private void UpdateTemperature()
    {
        if (globalTemperature != lastTemperature)
        {
            lastTemperature = globalTemperature;
            TemperatureManager.Instance.UpdateTemperature();

        }
    }

    //게임 월드에 나무 설치
    public void PlaceTree()
    {
	    List<GameObject> trees = new List<GameObject>();

        for (int y = 0; y < ChunkData.ChunkHeightValue; y++)
        {
            for (int x = 0; x < MapWidthChunkValue * ChunkData.ChunkWidthValue; x++)
            {
                for (int z = 0; z < MapLengthChunkValue * ChunkData.ChunkLengthValue; z++)
                {
                    if (map.MapBlock[x,y,z].id == Ground)
                    {
                        int blockHeight = map.MapHeight[x, z].BlockHeight;

                        if (map.MapBlock[x, y + 1, z].id != Air || !CheckPlaceTree(new Vector3Int(x, blockHeight + 1, z)))
                        {
                            continue;
                        }

                        if (Random.Range(0, 100) <= treePlacePersent)
                        {
                            float randomNumber = 0;
                            randomNumber = Random.Range(0, 3);

                            switch(randomNumber)
                            {
                                case 0:
                                    {
                                        trees.Add(BuildingManager.Instance.PlaceBuildingOnMapLocal(TreeA, new Vector3Int(x, blockHeight, z)));
                                        break;
                                    }
                                case 1:
                                    {
                                        trees.Add(BuildingManager.Instance.PlaceBuildingOnMapLocal(TreeB, new Vector3Int(x, blockHeight, z)));
                                        break;
                                    }
                                case 2:
                                    {
                                        trees.Add(BuildingManager.Instance.PlaceBuildingOnMapLocal(TreeC, new Vector3Int(x, blockHeight, z)));
                                        break;
                                    }
                            }
                            map.MapBlock[x, blockHeight + 1, z].id = Trees;
                            map.MapBlock[x, blockHeight + 1, z].id = BlockType.Trees;
                        }
                    }
                }
            }
        }

        int excessCount = trees.Count - (int)GetBlockSettingData(BlockType.Trees).value[BlockSettingValue.TreeSettingValue0];

        if (trees.Count == 0 || excessCount <= 0)
        {
            return;
        }
        else
        {
            int[] randomCount = Utils.RandomNumbers(trees.Count, excessCount);

            for (int i = 0; i < excessCount; i++)
            {
                trees[randomCount[i]].GetComponentInChildren<Structure>().DestroyStructure();   
            }
        }
    }

    //나무가 설치 가능한지 검사
    private bool CheckPlaceTree(Vector3Int pos)
    {
        int betweenTreesDistance = (int)GetBlockSettingData(BlockType.Trees).value[BlockSettingValue.TreeSettingValue1];

		for (int k = pos.y - 1; k <= pos.y + 1; k++)
        {
            for (int i = pos.x - betweenTreesDistance; i <= pos.x + betweenTreesDistance; i++)
            {
                for (int j = pos.z - betweenTreesDistance; j <= pos.z + betweenTreesDistance; j++)
                {
                    if (i >= 0 && i < MapWidthChunkValue * ChunkData.ChunkWidthValue &&
                        j >= 0 && j < MapLengthChunkValue * ChunkData.ChunkLengthValue)
                    {
                        if (map.MapBlock[i, k, j].id == BlockType.Trees ||
                            map.MapBlock[i, k, j].id == Ground ||
                            map.MapBlock[i, k, j].id == Stone ||
                            map.MapBlock[i, k, j].id == TilledSoil ||
                            map.MapBlock[i, k, j].id == WetTilledSoil ||
                            map.MapBlock[i, k, j].id == Snow ||
                            map.MapBlock[i, k, j].id == Ice)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
