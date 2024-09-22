using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using WebCommonLibrary.Models.MasterDatabase;

public class VersionConfig
{
    private FMasterVersionApp _versionApp;
    private FMasterVersionData _versionData;

    public VersionConfig()
    {
        SetAppVersion(Application.version);
    }

    public FMasterVersionApp APP
    {
        get 
        { 
            return _versionApp; 
        } 
    }

    public FMasterVersionData Data
    {
        get
        {
            return _versionData;
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
        _versionApp = new FMasterVersionApp();
        _versionApp.major = major;
        _versionApp.minor = minor;
        _versionApp.patch = patch;
    }

    public void SetDataVersion(int major, int minor, int patch)
    {
        _versionData = new FMasterVersionData();
        _versionData.major = major;
        _versionData.minor = minor;
        _versionData.patch = patch;
    }

}
