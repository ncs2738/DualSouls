using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{ 
    private const string fileExtension = "txt";

    private static readonly string saveFolder = Application.dataPath + "/Saves/";
    private static bool isInit = false;

    public static void Init()
    {
        if(!isInit)
        {
            isInit = true;

            //Make sure the folder exists
            if (!Directory.Exists(saveFolder))
            {
                //Create the folder if it doesn't exist
                Directory.CreateDirectory(saveFolder);
            }
        }
    }

    public static void Save(string fileName, string saveString, bool overwrite)
    {
        Init();
        string savedFileName = fileName;

        //If we're making a new file...
        if(!overwrite)
        {
            //Make a save file number to make sure it's a unique file & doesn't overwrite any others
            int saveNumber = 1;

            //Make sure there's no other files with the same name...
            while (File.Exists(saveFolder + savedFileName + "." + fileExtension))
            {
                saveNumber++;
                savedFileName = fileName + "_" + saveNumber;
            }
        }

        //Write the file
        File.WriteAllText(saveFolder + savedFileName + "." + fileExtension, saveString);
    }

    public static string Load(string fileName)
    {
        Init();
        if (File.Exists(saveFolder + fileName + "." + fileExtension))
        {
            string savedFile = File.ReadAllText(saveFolder + fileName + "." + fileExtension);
            return savedFile;
        }
        else
        {
            return null;
        }
    }

    public static GridManager.SaveObject LoadResource()
    {
        TextAsset savedFile = Resources.Load<TextAsset>("Saves/save");
        GridManager.SaveObject saveObject = JsonUtility.FromJson<GridManager.SaveObject>(savedFile.text);
        return saveObject;
    }

    public static string LoadMostRecentFile()
    {
        Init();
        DirectoryInfo directoryInfo = new DirectoryInfo(saveFolder);

        //get all the saved files
        FileInfo[] savedFiles = directoryInfo.GetFiles("*." + fileExtension);

        //cycle through the files & find the most recent one
        FileInfo mostRecentFile = null;
        foreach(FileInfo file in savedFiles)
        {
            if(mostRecentFile == null)
            {
                mostRecentFile = file;
            }
            else
            {
                if(file.LastWriteTime > mostRecentFile.LastWriteTime)
                {
                    mostRecentFile = file;
                }
            }
        }

       //if there's a file, load it.
       if(mostRecentFile != null)
        {
            string savedFile = File.ReadAllText(mostRecentFile.FullName);
            return savedFile;
        }
        else
        {
            return null;
        }
    }

    public static void SaveObject(object saveObject)
    {
        SaveObject("save", saveObject, false);
    }

    public static void SaveObject(string fileName, object saveObject, bool overwrite)
    {
        Init();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overwrite);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>()
    {
        Init();
        string savedFile = LoadMostRecentFile();

        if (savedFile != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(savedFile);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }

    public static TSaveObject LoadOBject<TSaveObject>(string fileName)
    {
        Init();
        string savedFile = Load(fileName);

        if (savedFile != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(savedFile);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }
}