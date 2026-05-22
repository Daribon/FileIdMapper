# FileIdMapper

A fast tool for World of Warcraft Classic 1.14 modding. 
Scans your mod folder and generates a custom_files.txt that maps every matched file to its numeric File ID from listfile.csv. 

---

## What it does

1. Reads `listfile.csv` (format: `fileId;filePath`) into a high-performance hash table.
2. Recursively scans the working directory for files.
3. Matches each discovered file against the listfile by relative path.
4. Outputs `custom_files.txt` sorted by File ID:
   ```
   783975;world/maps/kalimdor/kalimdor_28_43.adt
   783976;world/maps/kalimdor/kalimdor_28_43_obj0.adt
   783977;world/maps/kalimdor/kalimdor_28_43_obj1.adt
   ...
   ```

---

## Prerequisites

### 1. Obtain a listfile
Download the latest community listfile from the WoWDev project:

👉 **https://github.com/wowdev/wow-listfile**

> ⚠️ **Important:** Rename the downloaded file to **`listfile.csv`** and place it in the same folder as your custom files before running FileIdMapper.

### 2. (Optional) Convert vanilla ADTs to Classic 1.14
If you are working with vanilla WoW ADT files, convert them to the Classic 1.14 format first:

👉 **https://github.com/ModernWoWTools/MapUpconverter**
Download or compile it then inside the directory of the program, create a folder named `input`.
Then create a file named `settings.json`, copy paste the below into it:
```bash
{
  "inputDir": "/home/username/Desktop/porteritup/input/",
  "outputDir": "/home/username/Desktop/porteritup/output/",
  "mapName": "azeroth",
  "generateWDTWDL": false,
  "rootWDTFileDataID": 0,
  "exportTarget": "Generic",
  "convertOnSave": false,
  "clientRefresh": false,
  "casRefresh": false,
  "mapID": -1,
  "targetVersion": 927,
  "useAdvancedLightConfig": false
}
```
Change **`inputDir`**, **`outputDir`**, and **`mapName`** to match your own paths and the map you want to convert.

### 1. Edit the ADT in Noggit
Open the ADTs you want to convert in **Noggit (red)**, then click **“Save changed tiles”**.

### 2. Prepare files for MapUpconverter
Place the saved `.adt` files from Noggit into the **MapUpconverter input folder**.

### 3. Run MapUpconverter
Run MapUpconverter, then open the **output** folder.  
Copy the generated **world** folder into the same directory as the **FileIdMapper** binary.

### 4. Generate `custom_files.txt`
Run **FileIdMapper** to generate the matching `custom_files.txt`.

### 5. FileIdMapper setup
See below for how to set up **FileIdMapper**.

---

## Build

Requires the .NET 8 SDK or later.

### Windows
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

### Linux
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## How to Run

### Step 1 — Prepare your working folder
Create a folder containing:
- `listfile.csv` (renamed)
- Your custom/modified game files in their correct folder structure

Example layout:
```
MyMods/
├── listfile.csv
├── world/
│   └── maps/
│       └── kalimdor/
│           └── kalimdor_28_43.adt
├── sound/
│   └── music/
│       └── citymusic/
│           └── darnassus/
│               └── darnassus intro.mp3
└── interface/
    └── cinematics/
        └── logo_1024.avi
```

### Step 2 — Run FileIdMapper

**Option A: Drop the executable into the folder and double-click**

Simply place `FileIdMapper.exe` (Windows) or `FileIdMapper` (Linux) inside `MyMods/` and run it. It will scan the current directory and output `custom_files.txt` right next to your files.

**Option B: Run from command line**

```bash
# Windows — scan current folder
FileIdMapper.exe

# Windows — scan a specific folder
FileIdMapper.exe "C:\Users\You\MyMods"

# Linux — scan current folder
./FileIdMapper

# Linux — scan a specific folder
./FileIdMapper "/home/you/MyMods"
```

### Step 3 — Check the output

If everything worked, you will see console output like:

```
Detected ~1,042,111 CSV lines. Sizing dictionary to 1,146,322 buckets.
Loaded 1,042,111 rows in 847.3 ms (312 MB used)
Matched 42 files in 23.1 ms
Wrote custom_files.txt in 12.4 ms
```

And `custom_files.txt` will be created in the scanned folder:

```
MyMods/
├── listfile.csv
├── custom_files.txt      ← generated
├── world/
├── sound/
└── interface/
```

## Installing Your Mods

After `custom_files.txt` is generated, follow these two steps to install everything into your World of Warcraft client.

### 1. Place `custom_files.txt` in the mappings folder

```
\World of Warcraft\
    └── mappings\
        └── custom_files.txt
```

### 2. Place your custom files (mods) in the files folder

Copy all your custom assets into the `files` directory, maintaining the correct folder structure.

```
\World of Warcraft\
    └── files\
        └── world/
            └── maps/
                └── kalimdor/
                    └── kalimdor_28_43.
```

## Running the mods in arctium
Once everything above is complete, compile old version of arctium launcher that supported custom files:
https://github.com/Arctium/WoW-Launcher/tree/3deaa3f50b95ae918ba49ca2a4d9a895247e67f7

Once compiled, run arctium with ```--version ClassicEra``` and your mods should display in game.

---

## License

MIT
