// # System
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// # UnityEninge
using UnityEngine;

// # Newtonsoft
using Newtonsoft.Json;


public class JsonManager : MonoBehaviour
{
    public static JsonManager Instance;

    private string directoryPath = default;

    private SerializableChunkData serializableChunkData;
    private SerializableChunkData loadSeralizableChunkData;


    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        serializableChunkData = new SerializableChunkData(); //저장할 청크의 데이터(chunkData)를 저장할 직렬화 클래스를 새로 만들어 저장
        loadSeralizableChunkData = new SerializableChunkData(); //로드한 데이터를 담을 클래스
        directoryPath = Path.Combine(Application.persistentDataPath, "Maps");   //"Maps" 이라는 폴더를 생성한 다음 경로를 할당
    }

    public void Save(Chunk chunkData)
    {
        //X-Y.chunk 형식의 파일이름 생성
        string saveFileName = $"{chunkData.Coord.x}-{chunkData.Coord.y}.chunk";         
        string savePath = Path.Combine(directoryPath, saveFileName);

        //경로가 없다면 경로를 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        //경로에 똑같은 파일이 있는지 검사
        //경로에 데이터가 저장이 되어 있지 않다면 저장
        //경로에 이미 데이터가 저장이 되어 있다면 저장하지 않음
        if (!File.Exists(savePath))
        {
            serializableChunkData.blockInChunk = chunkData.BlockInChunk;

            try
            {
                string data = JsonConvert.SerializeObject(serializableChunkData, Formatting.Indented);
                File.WriteAllText(savePath, data);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError($"직렬화 오류 발생: {e.Message} \n StackTrace : {e.StackTrace}");
            }
            catch (JsonWriterException e)
            {
                Debug.LogError($"Json 작성 오류 발생: {e.Message} \n StackTrace : {e.StackTrace}");
            }
            catch (Exception e)
            {
                Debug.LogError($"알 수 없는 오류 발생: {e.Message} \n StackTrace : {e.StackTrace}");
            }
        }
        else
        {

            return;
        }
    }

    //저장 데이터를 새로운 데이터로 갱신
    public void UpdateSave(Chunk chunkData)
    {
        string saveFileName = $"{chunkData.Coord.x}-{chunkData.Coord.y}.chunk";
        string savePath = Path.Combine(directoryPath, saveFileName);

        //경로에 이미 데이터를 갱신
        if (File.Exists(savePath))
        {
            serializableChunkData.blockInChunk = chunkData.BlockInChunk;

            try
            {
                string data = JsonConvert.SerializeObject(serializableChunkData, Formatting.Indented);
                File.WriteAllText(savePath, data);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError($"역직렬화 오류 발생: {e.Message} \n StackTrace : {e.StackTrace}");
            }
            catch (JsonReaderException e)
            {
                Debug.LogError($"Json 읽기 오류 발생: {e.Message} \n StackTrace : {e.StackTrace}");
            }
            catch (Exception e)
            {
                Debug.LogError($"알 수 없는 오류 발생: {e.Message} \n StackTrace : {e.StackTrace}");
            }
        }
    }

    //데이터 로드
    public BlockData[,,] Load(Vector2Int chunkCoord)
    {
        string saveFileName = $"{chunkCoord.x}-{chunkCoord.y}.chunk";
        string savePath = Path.Combine(directoryPath, saveFileName);

        if (File.Exists(savePath))
        {
            try
            {
                string data = File.ReadAllText(savePath);
                loadSeralizableChunkData = JsonConvert.DeserializeObject<SerializableChunkData>(data);
                return loadSeralizableChunkData.blockInChunk;
            }
            catch (JsonReaderException e)
            {
                Debug.LogError($"JSON 읽기 오류: {e.Message}");
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError($"JSON 직렬화 오류: {e.Message}");
            }
            catch (JsonException e)
            {
                Debug.LogError($"일반적인 JSON 오류: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Message: {e.Message}\nStackTrace: {e.StackTrace}");
            }
        }

        return null;
    }
}

[System.Serializable]
public class SerializableChunkData
{
    public BlockData[,,] blockInChunk;
}

