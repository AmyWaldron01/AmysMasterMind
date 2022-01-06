using System;

public class GameData
{
    public int rows;
    public int[,] posData;

    public GameData()
	{
        rows = 0;
        posData = new int[10,4];

        for (int i = 0; i < posData.GetLength(0); i++)
        {
            for (int j = 0; j < posData.GetLength(1); j++)
            {
                posData[i, j] = -1;
            }
        }
    }
}
