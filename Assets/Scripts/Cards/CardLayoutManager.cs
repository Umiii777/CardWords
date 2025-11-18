using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardLayoutManager
{
    private float cardSpacing = 80 / 6;
    private float cardWidth = 200;


    private float[] rowTransformsX;
    private float[] mainRowTransforms;

    public float[] SetRowLayoutX(int rows)
    {
        rowTransformsX = new float[rows];
        for (int i = 0; i < rows; i++)
        {
            rowTransformsX[i] = cardSpacing + 0.5f * cardWidth + i * (cardSpacing + cardWidth);
        }
        return rowTransformsX;
    }
    public float[] SetMainRowLayoutX(int mainRows)
    {
        mainRowTransforms = new float[mainRows];
        for (int i = 0; i < mainRows; i++)
        {
            mainRowTransforms[i] = cardSpacing + 0.5f * cardWidth + i * (cardSpacing + cardWidth);
        }
        return mainRowTransforms;
    }
}
