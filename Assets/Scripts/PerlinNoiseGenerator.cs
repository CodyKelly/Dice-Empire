using UnityEngine;
using System.Collections;

public class PerlinNoiseGenerator : MonoBehaviour {

	float[,] noise;					// Random noise array
	int noiseWidth, noiseHeight;	// Array dimensions

/*******************************************************************************
 *                              Generate Noise
 *
 *      Generates a two dimensional array filled with float values 
 * 		ranging from 0.0 - 1.0. The values are then used to generate another
 * 		array of Perlin noise.
 * 
 *      Takes a width and height, a seed for the Random class, the number of
 * 		octaves used in the perlin noise funtion, and the amplitude of the
 * 		noise function.
 ******************************************************************************/

	public float[,] GenerateNoise(int width, int height, int seed, int octaves, float amplitude)
	{
		float[,] resultArray = new float[width, height];	// This will hold the output of the Perlin noise function

		noise = new float[width, height];					// This array will hold plain ole' random values
															// Used for smoothing and calculating the Perlin noise

		Random.seed = seed;									// Set the seed for the random values

		noiseWidth = width;
		noiseHeight = height;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				noise[x, y] = Random.value;					// Fill every element of the noise array with a random value
			}
		}

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				resultArray[x, y] = CalculatePerlin(x * amplitude, y * amplitude, octaves);	// Fill every element in
																							// the result array with
																							// Perlin noise
			}
		}

		return resultArray;
	}

/*******************************************************************************
 *                              Smooth Noise
 *
 *      This function takes all the values in the noise array and averages
 * 		them with their neighbors.
 ******************************************************************************/

	float SmoothNoise(float x, float y)
	{
		float fractX = x - (int)x;								// fract X and Y stores the non-integer parts
		float fractY = y - (int)y;								// of x and y

		float x1 = ((int)x + noiseWidth) % noiseWidth;
		float y1 = ((int)y + noiseHeight) % noiseHeight;

		float x2 = ((int)x + noiseWidth - 1f) % noiseWidth;
		float y2 = ((int)y + noiseHeight - 1f) % noiseHeight;

		float value = 0f;

		value += fractX * fractY * noise[(int)x1, (int)y1];
		value += fractX * (1f - fractY) * noise[(int)x1, (int)y2];
		value += (1f - fractX) * fractY * noise[(int)x2, (int)y1];
		value += (1f - fractX) * (1f - fractY) * noise[(int)x2, (int)y2];

		return value;
	}

/*******************************************************************************
 *                              CalculatePerlin
 *
 *      This function takes an x and y coordinate and the number of octaves
 * 		and returns a perlin noise value.
 ******************************************************************************/

	float CalculatePerlin(float x, float y, int octaves) 
	{
		float value = 0f;
		float currentOctaves = (float)octaves;

		while(currentOctaves >= 1)
		{
			value += SmoothNoise(x / currentOctaves, y / currentOctaves) * currentOctaves;
			currentOctaves /= 2;
		}

		return (value / (float)octaves);
	}
}
