using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandRemover : MonoBehaviour {

	List<List<Vector2>> islands = new List<List<Vector2>>(1000);
	List<Vector2> toBeChecked = new List<Vector2>(1000);

	public void RemoveIslands()
	{
		WorldGenerator worldGenerator = gameObject.GetComponent<WorldGenerator>();
		int biggestIsland = 0;

		for(int x = 0; x < worldGenerator.getWorldWidthInTiles(); x++)
		{
			for(int y = 0; y < worldGenerator.getWorldHeightInTiles(); y++)
			{
				bool goodTile = true;

				if(gameObject.GetComponent<WorldGenerator>().tileAboveWater(x, y))
				{
					foreach(List<Vector2> list in islands)
					{
						foreach(Vector2 tile in list)
						{
							if(tile == new Vector2(x, y))
							{
								goodTile = false;
							}
						}
					}

					if(goodTile)
					{
						findIsland(x, y);
					}
				}
			}
		}

		List<Vector2> biggestIsle = new List<Vector2>();

		foreach(List<Vector2> isle in islands)
		{
			if(isle.Count > biggestIsle.Count)
			{
				biggestIsle = isle;
			}
		}

		foreach(List<Vector2> isle in islands)
		{
			if(isle != biggestIsle)
			{
				foreach(Vector2 tile in isle)
				{
					GameObject tilePrefab = gameObject.GetComponent<WorldGenerator>().getTile((int)tile.x, (int)tile.y).gameObject;
					Destroy(tilePrefab);
				}
			}
		}

//		int count = 0;
//		foreach(List<Vector2> isle in islands)
//		{
//			count++;
//			print("\nIsland number " + count.ToString() + " contains " + isle.Count.ToString() + " tiles.");
//		}


		toBeChecked.Clear();
		islands.Clear();
	}

	private List<Vector2> getNeighbors(Vector2 tile)
	{
		List<Vector2> neighborList = new List<Vector2>(1000);
		int worldWidth = gameObject.GetComponent<WorldGenerator>().getWorldWidthInTiles();
		int worldHeight = gameObject.GetComponent<WorldGenerator>().getWorldHeightInTiles();

		if(tile.x - 1 >= 0)
		{
			if(gameObject.GetComponent<WorldGenerator>().tileAboveWater((int)tile.x - 1, (int)tile.y))
			{
				neighborList.Add(new Vector2(tile.x - 1, tile.y));
			}
		}
		if(tile.y - 1 >= 0)
		{
			if(gameObject.GetComponent<WorldGenerator>().tileAboveWater((int)tile.x, (int)tile.y - 1))
			{
				neighborList.Add(new Vector2(tile.x, tile.y - 1));
			}
		}
//		if(tile.x - 1 >= 0 && tile.y - 1 >= 0)
//		{
//			if(gameObject.GetComponent<WorldGenerator>().tileAboveWater((int)tile.x - 1, (int)tile.y - 1))
//			{
//				neighborList.Add(new Vector2(tile.x - 1, tile.y - 1));
//			}
//		}
		if(tile.x + 1 < worldWidth)
		{
			if(gameObject.GetComponent<WorldGenerator>().tileAboveWater((int)tile.x + 1, (int)tile.y))
			{
				neighborList.Add(new Vector2(tile.x + 1, tile.y));
			}
		}
		if(tile.y + 1 < worldWidth)
		{
			if(gameObject.GetComponent<WorldGenerator>().tileAboveWater((int)tile.x, (int)tile.y + 1))
			{
				neighborList.Add(new Vector2(tile.x, tile.y + 1));
			}
		}
//		if(tile.x + 1 < worldWidth && tile.y + 1 < worldHeight)
//		{
//			if(gameObject.GetComponent<WorldGenerator>().tileAboveWater((int)tile.x + 1, (int)tile.y + 1))
//			{
//				neighborList.Add(new Vector2(tile.x + 1, tile.y + 1));
//			}
//		}

		return neighborList;
	}

	private List<Vector2> getUncheckedNeighbors(Vector2 tile)
	{
		List<Vector2> neighborList = getNeighbors(tile);
		List<Vector2> toRemove = new List<Vector2>();

		for(int neighborCounter = neighborList.Count - 1; neighborCounter >= 0; neighborCounter--)
		{
			for(int islandListCounter = islands.Count - 1; islandListCounter >= 0; islandListCounter--)
			{
				for(int islandCounter = islands[islandListCounter].Count - 1; islandCounter >= 0; islandCounter--)
				{
					if(neighborList[neighborCounter] == islands[islandListCounter][islandCounter])
					{
						toRemove.Add(neighborList[neighborCounter]);
					}
				}
			}
		}

		foreach(Vector2 neighbor in neighborList)
		{
			foreach (Vector2 toBeCheckedItem in toBeChecked)
			{
				if (neighbor == toBeCheckedItem)
				{
					toRemove.Add(neighbor);
				}
			}
		}

		foreach(Vector2 t in toRemove)
		{
			neighborList.Remove(t);
		}

		return neighborList;
	}
		
	private void findIsland(int x, int y)
	{
		islands.Add(new List<Vector2>(1000));
		toBeChecked.Clear();
		toBeChecked.Add(new Vector2(x, y));

		while(toBeChecked.Count != 0)
		{


			Vector2 lastTile = toBeChecked[toBeChecked.Count - 1];

			for(int count = getUncheckedNeighbors(lastTile).Count - 1; count >= 0; count--)
			{
				toBeChecked.Add(getUncheckedNeighbors(lastTile)[count]);
			}
				
			islands[islands.Count - 1].Add(new Vector2(lastTile.x, lastTile.y));
			toBeChecked.Remove(lastTile);
		}
	}
}
