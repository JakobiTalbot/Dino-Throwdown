using System.Collections;
using UnityEngine;

// 4x4 array of objects
[System.Serializable]
public class TextureArrayLayout
{
    // contains the data in the row
    [System.Serializable]
    public struct RowData
    {
        public Texture[] row;
    }

    // contains the rows
    public RowData[] rows = new RowData[4];
}