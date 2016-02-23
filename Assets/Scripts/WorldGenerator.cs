using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour {
	public bool removeIslands = true;

	public bool randomSeed = true;
	public int seed = 0;

	public int smoothCycles = 4;

	public enum worldTypes{Normal, Island, Smooth};
	public worldTypes worldType = worldTypes.Normal;

	public int chunkSizeInTiles = 10;
	public int worldWidthInChunks = 1, worldHeightInChunks = 1;
	private int worldWidthInTiles, worldHeightInTiles;

	private float[,] valueArray;
	private GameObject[,] tileArray;

	// Temporary:
	public GameObject tileToClone;

	public Sprite[] grassTiles;
	public Sprite[] waterTiles;
	public Sprite[] sandTiles;

	public float deepWaterRatio = 0.5f;
	public float waterRatio = 1.0f;
	public float sandRatio = 1.1f;

	public float amplitude = 10;
	public int octaves = 6;


	void Start () 
	{
		ResetWorld();
	}

	void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			ResetWorld();
		}
		if (Input.GetKeyDown(KeyCode.Equals))
		{
			IncreaseWater();
			foreach(Transform child in gameObject.transform)
			{
				Destroy(child.gameObject);
			}
		}
		if (Input.GetKeyDown(KeyCode.Minus))
		{
			DecreaseWater();
			foreach(Transform child in gameObject.transform)
			{
				Destroy(child.gameObject);
			}
		}

		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			seed--;
			ResetWorld();
			foreach(Transform child in gameObject.transform)
			{
				Destroy(child.gameObject);
			}
			gameObject.GetComponent<IslandRemover>().RemoveIslands();
		}
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			seed++;
			ResetWorld();
			foreach(Transform child in gameObject.transform)
			{
				Destroy(child.gameObject);
			}
			gameObject.GetComponent<IslandRemover>().RemoveIslands();
		}
		if(transform.childCount == 0)
		{
			ResetWorld();
		}
	}

	void GenerateMap()
	{
		if(randomSeed)
		{
			seed = (int)Mathf.Abs(System.Environment.TickCount/10000);
		}

		if(valueArray != null)
		{
			Clear();
		}

		if (worldType == worldTypes.Normal)
		{
			valueArray = gameObject.GetComponent<PerlinNoiseGenerator>().GenerateNoise(worldWidthInTiles, worldHeightInTiles, seed, octaves, amplitude);
			for(int x = 0; x < smoothCycles; x++)
			{
				Smooth();
			}
		}
		else if (worldType == worldTypes.Smooth)
		{
			UnityEngine.Random.seed = seed;
			float value = UnityEngine.Random.value;

			for (int x = 0; x < worldWidthInTiles; x++)
			{
				for (int y = 0; y < worldHeightInTiles; y++)
				{
					valueArray[x, y] = (Mathf.PerlinNoise((float)x/100f, (float)y/100f) + value) / 2 + 0.5f;
				}
			}

			for(int x = 0; x < smoothCycles; x++)
			{
				Smooth();
			}
		}

		GenerateTileSprites();

		gameObject.GetComponent<IslandRemover>().RemoveIslands();
	}

	void ApplyFilter()
	{

	}

	void Smooth()
	{
		for(int x = 0; x < worldWidthInTiles; x++)
		{
			for(int y = 0; y < worldHeightInTiles; y++)
			{
				float average = 0f;
				int times = 0;

				if(x - 1 >= 0)
				{
					average += valueArray[x - 1, y];
					times += 1;
				}
				if(x + 1 < worldWidthInTiles - 1)
				{
					average += valueArray[x + 1, y];
					times += 1;
				}
				if(y - 1 >= 0)
				{
					average += valueArray[x, y - 1];
					times += 1;
				}
				if(y + 1 < worldHeightInTiles - 1)
				{
					average += valueArray[x, y+1];
					times += 1;
				}
				if (x - 1 >= 0 && y - 1 >= 0)
				{
					average += valueArray[x - 1, y - 1];
					times += 1;
				}
				if(x + 1 < worldWidthInTiles && y - 1 >= 0)
				{
					average += valueArray[x + 1, y - 1];
					times += 1;
				}
				if(x - 1 >= 0 && y + 1 < worldHeightInTiles)
				{
					average += valueArray[x - 1, y + 1];
					times += 1;
				}
				if(x + 1 < worldWidthInTiles && y + 1 < worldHeightInTiles)
				{
					average += valueArray[x + 1, y + 1];
					times += 1;
				}

				average += valueArray[x, y];
				times += 1;

				average /= times;

				valueArray[x, y] = average;
			}
		}
	}

	void GenerateTileSprites()
	{
		Clear();
		int count = 0;
		for (int x = 0; x < worldWidthInTiles; x++)
		{
			for (int y = 0; y < worldHeightInTiles; y++)
			{
				if(valueArray[x, y] > getWaterLine())
					{
					float tileX = x + transform.position.x;
					float tileY;

					if(x % 2 == 0)
					{
						tileY = y + transform.position.y;
					}
					else
					{
						tileY = y + transform.position.y + 0.5f;
					}

					Vector3 tilePos = new Vector3(tileX, tileY, 0);

					GameObject newTile = (GameObject)Instantiate(tileToClone, tilePos, Quaternion.identity);

					newTile.transform.SetParent(transform);

					newTile.GetComponent<SpriteRenderer>().sprite = ChooseTile(valueArray[x, y]);

					if(valueArray[x, y] < getWaterLine())
					{
						newTile.name = "WaterTile#" + count.ToString() + "(" + valueArray[x, y].ToString() + ")";
					}else
					{
						newTile.name = "GrassTile#" + count.ToString() + "(" + valueArray[x, y].ToString() + ")";
					}

					tileArray[x, y] = newTile;

					count++;
				}
			}
		}
	}

	private Sprite ChooseTile(float value)
	{
		if(value < getWaterLine())
		{
			return null;
		} 
		else
		{ 
			return grassTiles[UnityEngine.Random.Range(0, grassTiles.Length - 1)]; 
		}
	}

	public void Clear()
	{
		for (int x = 0; x < worldWidthInTiles; x++)
		{
			for (int y = 0; y < worldHeightInTiles; y++)
			{
				if(tileArray[x, y] != null)
				{
					Destroy(tileArray[x, y].gameObject);
				}
			}
		}
	}

	public void ResetWorld()
	{
		foreach(Transform child in gameObject.transform)
		{
			Destroy(child.gameObject);
		}

		worldWidthInTiles = worldWidthInChunks * chunkSizeInTiles;
		worldHeightInTiles = worldHeightInChunks * chunkSizeInTiles;

		tileArray = new GameObject[worldWidthInTiles, worldHeightInTiles];
		valueArray = new float[worldWidthInTiles, worldHeightInTiles];

		GenerateMap();
	}

	public float getAvgValue()
	{
		float total = 0.0f;

		for (int x = 0; x < worldWidthInTiles; x++)
		{
			for (int y = 0; y < worldHeightInTiles; y++)
			{
				total += valueArray[x, y];
			}
		}

		total /= valueArray.GetLength(0) * valueArray.GetLength(1);

		return total;
	}

	public void IncreaseWater()
	{
		deepWaterRatio += 0.05f;
		waterRatio += 0.05f;
	}

	public void DecreaseWater()
	{
		deepWaterRatio -= 0.05f;
		waterRatio -= 0.05f;
	}

	public float getDeepWaterLine()
	{
		return deepWaterRatio * getAvgValue();
	}

	public float getWaterLine()
	{
		return waterRatio * getAvgValue();
	}

	public float getSandLine()
	{
		return sandRatio * getAvgValue();
	}

	public float getHighestPoint()
	{
		float highest = 0f;

		for(int x = 0; x < worldWidthInTiles; x++)
		{
			for (int y = 0; y < worldHeightInTiles; y++)
			{
				if (valueArray[x, y] > highest)
				{
					highest = valueArray[x, y];
				}
			}
		}

		return highest;
	}

	public int getWorldWidthInTiles()
	{
		return worldWidthInTiles;
	}

	public int getWorldHeightInTiles()
	{
		return worldHeightInTiles;
	}

	public float[,] getValueArray()
	{
		return valueArray;
	}

	public bool tileAboveWater(int x, int y)
	{
		return (valueArray[x, y] > getWaterLine());
	}

	public GameObject getTile(int x, int y)
	{
		float posX = (float)x, posY = (float)y;

		if(posX % 2 != 0)
		{
			posY += 0.5f;
		}

		foreach(GameObject t in GameObject.FindGameObjectsWithTag("Tile"))
		{
			if(t.transform.position == new Vector3(posX, posY, 0))
			{
				return t;
			}
		}

		return null;
	}
}
