using Org.BouncyCastle.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEditor.VersionControl;
using UnityEngine;
/// <summary>
/// Class to read json into c#.
/// </summary>
[System.Serializable]
public class CountryToCode
{
    public string name;
    public string code;
}

/// <summary>
/// Class to read json into c#.
/// </summary>
[System.Serializable]
public class CountriesToCodes
{
    public List<CountryToCode> list;
}

/// <summary>
/// Class for translation between country codes and country names.
/// </summary>
public class CountryCodeTranslation : MonoBehaviour
{
    private List<CountryToCode> countryCodesList;

    public List<CountryToCode> CountryCodesList { get => countryCodesList; set => countryCodesList = value; }
    
    /// <summary>
    /// Method to get country code via country name.
    /// </summary>
    /// <param name="countryName"></param>
    /// <returns></returns>
    public string GetCountryCode(string countryName)
    {
        foreach(CountryToCode codeClass in CountryCodesList)
        {
            if (codeClass.name.ToLower().Contains(countryName.ToLower()) || countryName.ToLower().Contains(codeClass.name.ToLower()))
                return codeClass.code.ToLower();
        }
        return "";
    }

    /// <summary>
    /// Method to get country name via country code.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    public string GetCountryName(string countryCode)
    {
        foreach (CountryToCode codeClass in CountryCodesList)
        {
            if (codeClass.code.ToLower().Equals(countryCode.ToLower()))
                return codeClass.name.ToLower();
        }
        return "";
    }
}
