import time
import pandas as pd

csvFile = "D:\Programming\!Work\AugmentExpertSystems\Explosive Visualiser\Assets\explosivevisualisation/dataset.csv"
df = pd.read_csv(csvFile)

# set empty co-ordinate rows as 0 
df[["XC", "YC", "ZC", "XF", "YF", "ZF"]].fillna(0, inplace=True)
# set empty rows as -1
df.fillna(-1, inplace=True)

# start co-ordinate values at 1
df["XC"] = df["XC"] + 1
df["YC"] = df["YC"] + 1
df["ZC"] = df["ZC"] + 1
df["XF"] = df["XF"] + 1
df["YF"] = df["YF"] + 1
df["ZF"] = df["ZF"] + 1

zeroStartBlocks = df[df["MOVESEQ"]==-1] # blocks that dont move
movingBlocks = df[df["MOVESEQ"]>=0].sort_values("MOVESEQ") # blocks that move

zeroStartBlocks.to_csv("zeroStartBlocks_ts.csv", index=False)
movingBlocks.to_csv("movingBlocks_ts.csv", index=False)
df.to_csv("allBlocks_ts.csv")

print("numTimePoints: ", len(df[df["MOVESEQ"]>=0]))
print("highest x value: ", max(df.XC)-1, ", highest y value: ", max(df.YC)-1, ", highest z value: ", max(df.ZC)-1)
print("lowest x value: ", min(df.XC)-1, ", lowest y value: ", min(df.YC)-1, ", lowest z value: ", min(df.ZC)-1)

print(movingBlocks.tail())