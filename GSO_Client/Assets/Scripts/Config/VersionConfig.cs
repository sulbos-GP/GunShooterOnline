using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using WebCommonLibrary.Models.MasterDatabase;

public class VersionConfig
{
    private FMasterVersionApp _app;
    private FMasterVersionData _data;

    public VersionConfig()
    {
        
    }

    public FMasterVersionApp App
    {
        get 
        { 
            return _app; 
        } 
    }

    public FMasterVersionData Data
    {
        get
        {
            return _data;
        }
    }

    public string AppName
    {
        get
        {
            return _app.major.ToString() +
                _app.minor.ToString() +
                _app.patch.ToString();
        }
    }

    public string DataName
    {
        get
        {
            return _data.major.ToString() +
                _data.minor.ToString() +
                _data.patch.ToString();
        }
    }

    public void SetAppVersion(string versionName)
    {
        string[] version = versionName.Split('.');
        int major = int.Parse(version[0]);
        int minor = int.Parse(version[1]);
        int patch = int.Parse(version[2]);
        SetAppVersion(major, minor, patch);
    }

    public void SetAppVersion(int major, int minor, int patch)
    {
        _app = new FMasterVersionApp();
        _app.major = major;
        _app.minor = minor;
        _app.patch = patch;
    }

    public void SetDataVersion(string versionName)
    {
        string[] version = versionName.Split('.');
        int major = int.Parse(version[0]);
        int minor = int.Parse(version[1]);
        int patch = int.Parse(version[2]);
        SetDataVersion(major, minor, patch);
    }

    public void SetDataVersion(int major, int minor, int patch)
    {
        _data = new FMasterVersionData();
        _data.major = major;
        _data.minor = minor;
        _data.patch = patch;
    }

}
