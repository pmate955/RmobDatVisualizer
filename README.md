# RMOB DAT Visualizer

## Introduction

This is a small project for visualizing radio meteor detection data from two file formats:
- **SpectrumLab-type** `RMOB-YYYYMM.dat` files
- **RmobTxt format** `*MMYYYY rmob.txt` files (radio meteor detector output)

**Note:** This project is still in progress!

### Example `RMOB-202408.dat` file:

```
2024080100 , 00 , 84
2024080101 , 01 , 81
2024080102 , 02 , 80
```

The CSV format uses commas (`,`) as delimiters:  
- The **first column** contains the date and hour in the format `YYYYMMDDHH` (UTC).  
- The **second column** shows the hour.  
- The **third column** contains the meteor count for the given hour.  
Line endings must be **CRLF** (`\r\n`).

### Example `Szeged_022024rmob.txt` file:

```
feb| 00h| 01h| 02h| 03h| 04h| 05h| 06h| 07h| 08h| 09h| 10h| 11h| 12h| 13h| 14h| 15h| 16h| 17h| 18h| 19h| 20h| 21h| 22h| 23h|
 01| 145| 133| 142| 119|  83|  89|  99|  97|  76|  66|  73|  91|  67|  70|  52|  67|  49|  48|  72|  89| 112|  95| 117| 158|
 02| 147| 146| 133| 106|  86|  78|  66|  87| ???| ???| ???| ???| ???| ???| ???| ???|   9|   9|  12|  37|  40|  57|  74|   0|
```

The RmobTxt format is a pipe-delimited table:
- **First column** contains the day of the month (01-31).
- **First row** contains hour labels (00h-23h).
- **Data cells** contain meteor counts for each day and hour.
- **Missing data** is marked as `???` and is automatically skipped.
- **Filename format**: `[Location_]MMYYYY rmob.txt` (e.g., `Szeged_022024rmob.txt` for February 2024)
  - Month and year are extracted from the filename to set the correct date for the data.

## Prerequisities
- Microsoft Windows (tested on 10)
- [.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)  
  *(Will be upgraded to .NET 8 or 10 in the future)*
---

## Usage

1. Click the **"Open"** button to select one or more `.dat` files.  
   *(The order of the files doesn’t matter – the app will sort them automatically.)*
2. Choose the type of visualization:
   - **RMOB visualization**: Similar to [rmob.org](https://rmob.org), but with:
     - Multiple scale options
     - Daily maximum view
     - Multi-month support  
     ![rmob](./Docs/202501-05.png)
   - **Meteor shower view**: Displays hourly meteor counts for a short time range (e.g., a few days).  
     ![meteorshower](./Docs/2025-quadrantids.png)
3. Press the **"Generate"** button!
4. A new window will appear showing the result, or an info dialog in case of errors.  
   ![results](./Docs/result_window.png)

   You can:
   - Save the image
   - Copy to clipboard and paste into any image editor

---

## Options

### RMOB Visualization

- **Months per row** – When multiple files are selected, choose how many months to display per row.
- **Scale** – Choose the preferred color scale.
- **Show daily maximums** – Display a bar chart underneath the main visualization.

### Meteor Shower

- **Start date** – The beginning of the diagram (00:00).
- **End date** – The end of the diagram (23:59).
- **Show grid** – Display a grid overlay on the bar chart.

---

## Other Notes

- Any issues or feature suggestions are welcome!
- The code is actively being improved – stay tuned.
- Sample files are available in the `Examples` folder.
- Check out the **Szeged Observatory livestream**:  
  [https://www.youtube.com/@szegedicsillagvizsgalo5848/live](https://www.youtube.com/@szegedicsillagvizsgalo5848/live)