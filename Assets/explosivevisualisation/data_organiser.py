import sys, csv

#File location of csv file to be sorted
csvFile = r'D:\Programming\!Work\AugmentExpertSystems\Explosive Visualiser\Assets\explosivevisualisation\dataset.csv'

#Open csv file
data = csv.reader(open(csvFile), delimiter=',')
#Save header of file
header = next(data)

zeroStartBlocks = list()
movingBlocks = list()
allBlocks = list()

highestX, highestY, highestZ = 0, 0, 0

#Loop through rows in the file
for row in data: 

    row[1] = str(int(row[1]) + 1)
    row[2] = str(int(row[1]) + 1)
    row[3] = str(int(row[1]) + 1)

    #santize collumn 15, 16, 20, 21, 22 as cells cannot have null values
        #if collumn has a null value a value of -1 is inserted
        # Add 1 to all co-ordinates to start at 1 instead of 0
    if (row[14] == ""): 
        row[14] = "-1"
    if (row[15] == ""): 
        row[15] = "-1"
    if (row[16] == ""): 
        row[16] = "-1"
    else:
        row[16] = str(int(row[1]) + 1)
    if (row[17] == ""): 
        row[17] = "-1"
    else: 
        row[17] = str(int(row[1]) + 1)
    if (row[18] == ""): 
        row[18] = "-1"
    else: 
        row[18] = str(int(row[1]) + 1)
    if (row[19] == ""): 
        row[19] = "-1"
    if (row[20] == ""): 
        row[20] = "-1"
    if (row[21] == ""): 
        row[21] = "-1"



    #Find the highest XC, YC, YZ values
    if(int(row[1]) > highestX): 
        highestX = int(row[1])
    if(int(row[2]) > highestY): 
        highestY = int(row[2])
    if(int(row[3]) > highestZ): 
        highestZ = int(row[3])


    # Add to all blocks csv file
    allBlocks.append(row)
    #Go through each row(block) in csv file
        #If a block has a timing of 0 (eg. doesnt move) put in zero start blocks list otherwise put in moving blocks list
    if(row[7] == '0'): 
        zeroStartBlocks.append(row)
    else: 
        movingBlocks.append(row)

#Sort moving blocks by time of movement
movingBlocks = sorted(movingBlocks, key=lambda row: row[7])


#Sorted File Creation
#Csv file created with blocks that dont move
with open('zero_start_block.csv', mode='w', newline='') as zeroBlockFile: 
    blockWriter = csv.writer(zeroBlockFile)
    blockWriter.writerow(header)
    blockWriter.writerows(zeroStartBlocks)

#Csv file created for blocks that move sorted by time of moving
with open('moving_block.csv', mode='w', newline='') as movingBlockFile: 
    blockWriter = csv.writer(movingBlockFile)
    blockWriter.writerow(header)
    blockWriter.writerows(movingBlocks)

#Csv file created for all blocks
with open('all_block.csv', mode='w', newline='') as allBlockFile: 
    blockWriter = csv.writer(allBlockFile)
    blockWriter.writerow(header)
    blockWriter.writerows(allBlocks)

print("highest x value: ", highestX - 1, ", highest y value: ", highestY - 1, ", highest z value: ", highestZ - 1)
input()