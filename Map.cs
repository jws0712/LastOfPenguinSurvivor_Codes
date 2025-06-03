// # Unity
using Mirror.BouncyCastle.Bcpg;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static BlockType;

public class Map
{
    //플레이어 주변의 청크를 검사
    public void CheckPlayerAroundChunk(Vector3 pos)
    {
        Debug.Log("업데이트");

        mapSettingManager.iceBlockList.Clear();

        int chunkX = Mathf.FloorToInt(pos.x / ChunkData.ChunkWidthValue);
        int chunkZ = Mathf.FloorToInt(pos.z / ChunkData.ChunkWidthValue);

        int checkChunkXMin = chunkX - checkDistance;
        int checkChunkXMax = chunkX + checkDistance;

        int checkChunkZMin = chunkZ - checkDistance;
        int checkChunkZMax = chunkZ + checkDistance;


        for (int x = checkChunkXMin; x <= checkChunkXMax; x++)
        {
            for (int z = checkChunkZMin; z <= checkChunkZMax; z++)
            {
                if (IsChunkInMap(x, z))
                {
                    GetBlockInChunk(new Vector2Int(x, z), Ice, ref mapSettingManager.iceBlockList);
                }
            }
        }
    }

    //청크 안에 특정블록을 가져옴
    private void GetBlockInChunk(Vector2Int chunkPosition, string blockType, ref List<Vector3Int> blockList)
    {
        for (int voxelInChunkY = 0; voxelInChunkY < ChunkData.ChunkHeightValue; voxelInChunkY++)
        {
            for (int voxelInChunkX = 0; voxelInChunkX < ChunkData.ChunkWidthValue; voxelInChunkX++)
            {
                for (int voxelInChunkZ = 0; voxelInChunkZ < ChunkData.ChunkWidthValue; voxelInChunkZ++)
                {
                    if (groundChunks[chunkPosition.x, chunkPosition.y].BlockInChunk[voxelInChunkX, voxelInChunkY, voxelInChunkZ].id == blockType)
                    {
                        blockList.Add(new Vector3Int(chunkPosition.x * ChunkData.ChunkWidthValue + voxelInChunkX,
                        voxelInChunkY,
                        chunkPosition.y * ChunkData.ChunkWidthValue + voxelInChunkZ));
                    }
                }
            }
        }
    }

    //블록의 상하좌우 위치에 물 블록이 있는지 검사
    private bool CheckBlockNearWater(Vector3Int pos)
    {
        Vector3Int upPos = new Vector3Int(pos.x, pos.y + 1, pos.z);
        Vector3Int downPos = new Vector3Int(pos.x, pos.y - 1, pos.z);
        Vector3Int rightPos = new Vector3Int(pos.x + 1, pos.y, pos.z);
        Vector3Int leftPos = new Vector3Int(pos.x - 1, pos.y, pos.z);
        Vector3Int forwardPos = new Vector3Int(pos.x, pos.y, pos.z + 1);
        Vector3Int backwardPos = new Vector3Int(pos.x, pos.y, pos.z - 1);

        if (GetBlockDataInChunk(upPos).id == Water) return true;
        if (GetBlockDataInChunk(downPos).id == Water) return true;
        if (GetBlockDataInChunk(rightPos).id == Water) return true;
        if (GetBlockDataInChunk(leftPos).id == Water) return true;
        if (GetBlockDataInChunk(forwardPos).id == Water) return true;
        if (GetBlockDataInChunk(backwardPos).id == Water) return true;
        return false;
    }

    //블록의 위치가 담긴 리스트를 넘겨 받아 블럭을 파괴
    public void DistroyBlockGroup(ref List<Vector3Int> blockList)
    {
        int meltingCount = 0;
        int loopCount = 0;

        if (blockList == null)
        {
            return;
        }

        while(meltingCount != meltingBlockCount && loopCount != blockList.Count)
        {
            int num = Random.Range(0, blockList.Count);
            if (CheckBlockNearWater(blockList[num]))
            {
                Debug.Log("블파괴");
                EditBlock(blockList[num], Air);
                blockList.RemoveAt(num);
                meltingCount++;
                continue;
            }
            else
            {
                loopCount++;
            }
        }
    }

    //청크에 있는 블록을 수정
    public void EditBlock(Vector3 blockEditPosition, string blockEditType)
    {
        GetChunkFromPosition(blockEditPosition, ChunkType.Ground).EditVoxel(blockEditPosition, MapSettingManager.Instance.FindBlockType(blockEditType));
    }

    public void OnTick()
    {
        if (TickManager.Instance.elapsedTicks % meltingTime == 0)
        {
            DistroyBlockGroup(ref mapSettingManager.iceBlockList);
        }
    }
}