using CsvHelper;
using System.Collections.Generic;
using System.Linq;

public static class DataReader
{
    static readonly string movingBlocksData = @"D:\Programming\!Work\AugmentExpertSystems\Explosive Visualiser\Assets\explosivevisualisation\moving_block.csv";
    static readonly string allBlocksData = @"D:\Programming\!Work\AugmentExpertSystems\Explosive Visualiser\Assets\explosivevisualisation\all_block.csv";

    public static List<Block> GetAllBlockData()
    {
        return GetData(allBlocksData);
    }
    public static List<Block> GetMovingBlockData()
    {
        return GetData(movingBlocksData);
    }

    static List<Block> GetData(string fileLocation)
    {
        using (var reader = new System.IO.StreamReader(fileLocation))
        using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<Block>().ToList();
        }
    }
}

//  IJK,XC,YC,ZC,PREN,PSTN,DENSITY,TIMING,MOVESEQ,DIPDIRN,BDENSITY,SHADOW,ROP,BELLYSEQ,ZMAG_LOW,ZMAG_HIGH,XF,YF,ZF,BEARNG_A,BEARNG_Z,DISP_A

public class Block
{
    public int IJK { get; set; }
    public int XC { get; set; }
    public int YC { get; set; }
    public int ZC { get; set; }
    public int XF { get; set; }
    public int YF { get; set; }
    public int ZF { get; set; }
    public float TIMING { get; set; }
    public int PSTN { get; set; }
}