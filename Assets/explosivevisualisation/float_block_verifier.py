import pandas as pd

#File location of csv file to be sorted
allBlockFile = r'D:\Programming\!Work\AugmentExpertSystems\Explosive Visualiser\Assets\explosivevisualisation\all_block.csv'
movingBlockFile = r'D:\Programming\!Work\AugmentExpertSystems\Explosive Visualiser\Assets\explosivevisualisation\moving_block.csv'

#Open csv file
allBlock = pd.read_csv(allBlockFile)
movingBlock = pd.read_csv(movingBlockFile)

def FindLowerBlock(row):
	x = str(int(row['XF']))
	y = str(int(row['YF']))
	z = int(row['ZF'])

	cResults = allBlock.loc[(allBlock['XC'] == x) & (allBlock['YC'] == y) & (allBlock['ZC'] == str(z-1))]
	fResults = allBlock.loc[(allBlock['XF'] == x) & (allBlock['YF'] == y) & (allBlock['ZF'] == str(z-1))]

	if (cResults.size == 0 & fResults.size == 0): 
		print(f"Found floating block: x {x} y {y} z {z}")

movingBlock.apply( FindLowerBlock, axis=1)

