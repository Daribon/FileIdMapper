# FileIdMapper

A fast, lightweight tool for **World of Warcraft Classic 1.14** modding.

Scans your mod folder and generates a `custom_files.txt` that maps every matched file to its numeric **File ID** from `listfile.csv`.

---

## What it does

1. Reads `listfile.csv` (format: `fileId;filePath`) into a high-performance hash table.
2. Recursively scans the working directory for files.
3. Matches each file against the listfile using its relative path.
4. Outputs `custom_files.txt` sorted by File ID:

```txt
783975;world/maps/kalimdor/kalimdor_28_43.adt
783976;world/maps/kalimdor/kalimdor_28_43_obj0.adt
783977;world/maps/kalimdor/kalimdor_28_43_obj1.adt
...
```

---

## Prerequisites

### 1. Obtain a listfile
Download the latest community listfile from the WoWDev project:  
👉 [https://github.com/wowdev/wow-listfile](https://github.com/wowdev/wow-listfile)

> **Important:** Rename the downloaded file to **`listfile.csv`** and place it in your mod folder before running FileIdMapper.

### 2. (Optional) Convert Vanilla ADTs to Classic 1.14
If you're working with vanilla ADT files, convert them first using **MapUpconverter**:

- Download or compile: [ModernWoWTools/MapUpconverter](https://github.com/ModernWoWTools/MapUpconverter)
- Create an `input` folder and a `settings.json` file in the program directory:

```json
{
  "inputDir": "/path/to/input/",
  "outputDir": "/path/to/output/",
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

- Open the ADTs in **Noggit (Red)** by placing your old custom .mpq file into your WotLK data folder.
- Note: It doesn’t matter if the .mpq is a straight vanilla MPQ, Noggit can still read the ADTs from it.
- In noggit, select the location on the map where your custom tiles exist → Then click **"Save changed tiles"**.
- Place the saved `.adt` files into the MapUpconverter `input` folder.
- Run MapUpconverter and copy the generated `world` folder into your mod directory.

---

## Build (from source)

Requires **.NET 8 SDK** or later.

**Windows:**
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

**Linux:**
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## How to Run

### Step 1: Prepare your working folder
Create a folder with this structure:

```
MyMods/
├── listfile.csv
├── world/
│   └── maps/
│       └── kalimdor/
│           └── kalimdor_28_43.adt
├── sound/
│   └── music/
├── interface/
└── ...
```

### Step 2: Run FileIdMapper

**Option A:** Drop and run (easiest)  
Place the executable (`FileIdMapper.exe` on Windows or `FileIdMapper` on Linux) inside your mod folder and double-click it.

**Option B:** Command line
```bash
# Scan current directory
./FileIdMapper          # Linux
FileIdMapper.exe        # Windows

# Scan specific folder
./FileIdMapper "/home/you/MyMods"
FileIdMapper.exe "C:\Mods\MyMods"
```

### Step 3: Verify output
You should see output similar to:

```
Detected ~1,042,111 CSV lines. Sizing dictionary to 1,146,322 buckets.
Loaded 1,042,111 rows in 847.3 ms (312 MB used)
Matched 42 files in 23.1 ms
Wrote custom_files.txt in 12.4 ms
```

A file named `custom_files.txt` will be created in the folder.

---

## Installing Your Mods

1. **Place `custom_files.txt`** in the mappings folder:
   ```
   World of Warcraft/
   └── mappings/
       └── custom_files.txt
   ```

2. **Copy your custom files** into the files folder, preserving the folder structure:
   ```
   World of Warcraft/
   └── files/
       └── world/
       └── sound/
       └── interface/
   ```

---

## Running the Mods in Arctium

1. Compile the old version of Arctium Launcher that supports custom files:  
   [Arctium/WoW-Launcher (specific commit)](https://github.com/Arctium/WoW-Launcher/tree/3deaa3f50b95ae918ba49ca2a4d9a895247e67f7)

2. Run Arctium with the following argument:
   ```bash
   --version ClassicEra
   ```

---

## License

MIT
