﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DesignMaster : EditorWindow
{
    Vector2 scrollPosition;
    [SerializeField] static List<WeaponType> weaponTypes = new List<WeaponType>();


    [MenuItem("Window/Design Master")]
    static void Init()
    {
        DesignMaster designMaster = (DesignMaster)EditorWindow.GetWindow(typeof(DesignMaster));
        weaponTypes = LoadFromJSON();
        designMaster.Show();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Create New Weapon Type"))
        {
            CreateWeapon();
        }
        if (GUILayout.Button("Load From JSON"))
        {
            weaponTypes = LoadFromJSON();
        }
        if (GUILayout.Button("Save To JSON"))
        {
            SaveToJSON();
        }


        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal();
        DisplayWeaponTypes();
        EditorGUILayout.Space(20);

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(20);
        EditorGUILayout.EndScrollView();

    }

    void CreateWeapon ()
    {
        WeaponType weaponType = new WeaponType();
        weaponType.weaponName = "New Weapon";
        weaponTypes.Add(weaponType);
        
    }


    void DisplayWeaponTypes()
    {
        if (weaponTypes != null)
        {
            for (int i = 0; i < weaponTypes.Count; i++)
            {
             
                EditorGUILayout.BeginVertical();
              


                GUILayout.Label(weaponTypes[i].weaponName, EditorStyles.boldLabel);

                EditorGUILayout.Space(16);
                GUILayout.Label("Universal Parameters", EditorStyles.boldLabel);
                EditorGUILayout.Space(16);

                EditorGUILayout.Space(8);
                GUILayout.Label("Name", EditorStyles.boldLabel);
                weaponTypes[i].weaponName = EditorGUILayout.TextField( weaponTypes[i].weaponName);

                EditorGUILayout.Space(8);
                GUILayout.Label("Sprite Prefab Name", EditorStyles.boldLabel);
                weaponTypes[i].spritePrefabName = EditorGUILayout.TextField( weaponTypes[i].spritePrefabName);
                
                EditorGUILayout.Space(8);
                GUILayout.Label("Weapon Type", EditorStyles.boldLabel);
                weaponTypes[i].weaponUseType = (WeaponUseType)EditorGUILayout.EnumPopup(weaponTypes[i].weaponUseType);

                if (weaponTypes[i].weaponUseType == WeaponUseType.SingleShot || weaponTypes[i].weaponUseType == WeaponUseType.Multishot || weaponTypes[i].weaponUseType == WeaponUseType.Throwable)
                {
                    EditorGUILayout.Space(8);
                    GUILayout.Label("Projectile Type", EditorStyles.boldLabel);
                    weaponTypes[i].projectileName = EditorGUILayout.TextField(weaponTypes[i].projectileName);
                }

                

                //EditorGUILayout.Space(8);
                //GUILayout.Label("Sprite", EditorStyles.boldLabel);
                //weaponTypes[i].weaponSprite = (Sprite)EditorGUILayout.ObjectField(weaponTypes[i].weaponSprite, typeof(Sprite), false);

                //EditorGUILayout.Space(8);
                //GUILayout.Label("Projectile Type", EditorStyles.boldLabel);
                //weaponTypes[i].projectileType = (GameObject)EditorGUILayout.ObjectField(weaponTypes[i].projectileType, typeof(GameObject), false);

                EditorGUILayout.Space(8);
                GUILayout.Label("Damage", EditorStyles.boldLabel);
                weaponTypes[i].damage = EditorGUILayout.IntField(weaponTypes[i].damage);

                EditorGUILayout.Space(8);
                GUILayout.Label("Fire Rate", EditorStyles.boldLabel);
                weaponTypes[i].fireRate = EditorGUILayout.FloatField(weaponTypes[i].fireRate);

                EditorGUILayout.Space(8);
                GUILayout.Label("Ammo Count", EditorStyles.boldLabel);
                weaponTypes[i].ammoCount = EditorGUILayout.IntField(weaponTypes[i].ammoCount);

                EditorGUILayout.Space(8);
                GUILayout.Label("Range", EditorStyles.boldLabel);
                weaponTypes[i].range = EditorGUILayout.FloatField(weaponTypes[i].range);

                if (weaponTypes[i].weaponUseType == WeaponUseType.SingleShot || weaponTypes[i].weaponUseType == WeaponUseType.Multishot || weaponTypes[i].weaponUseType == WeaponUseType.Throwable )
                {
                    EditorGUILayout.Space(8);
                    GUILayout.Label("Initial Force", EditorStyles.boldLabel);
                    weaponTypes[i].initialForce = EditorGUILayout.FloatField(weaponTypes[i].initialForce);

                    
                }

                if (weaponTypes[i].weaponUseType == WeaponUseType.SingleShot || weaponTypes[i].weaponUseType == WeaponUseType.Multishot)
                {
                    EditorGUILayout.Space(8);
                    GUILayout.Label("Bullet Spread", EditorStyles.boldLabel);
                    weaponTypes[i].spread = EditorGUILayout.FloatField(weaponTypes[i].spread);

                }


                    if (weaponTypes[i].weaponUseType == WeaponUseType.Multishot)
                {
                    EditorGUILayout.Space(16);
                    GUILayout.Label("Multishot Parameters", EditorStyles.boldLabel);
                    EditorGUILayout.Space(16);

                    GUILayout.Label("Bullets fired per shot", EditorStyles.boldLabel);
                    weaponTypes[i].bulletsFiredPerShot = EditorGUILayout.IntField(weaponTypes[i].bulletsFiredPerShot);
                    EditorGUILayout.Space(8);

                    GUILayout.Label("Spray Amount", EditorStyles.boldLabel);
                    weaponTypes[i].sprayAmount = EditorGUILayout.FloatField(weaponTypes[i].sprayAmount);
                    EditorGUILayout.Space(8);
                }

                if (weaponTypes[i].weaponUseType == WeaponUseType.Throwable)
                {
                    EditorGUILayout.Space(16);
                    GUILayout.Label("Throwable Parameters", EditorStyles.boldLabel);
                    EditorGUILayout.Space(16);

                    GUILayout.Label("Explosion Force", EditorStyles.boldLabel);
                    weaponTypes[i].explosionSize = EditorGUILayout.DelayedFloatField(weaponTypes[i].explosionSize);
                    EditorGUILayout.Space(8);

                    GUILayout.Label("Explosion Time", EditorStyles.boldLabel);
                    weaponTypes[i].explosionTime = EditorGUILayout.DelayedFloatField(weaponTypes[i].explosionTime);
                }

                    EditorGUILayout.Space(16);
             

                if (GUILayout.Button("Delete Weapon"))
                {
                    weaponTypes.Remove(weaponTypes[i]);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(10);

                if (i % 4 == 0 && i != 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(20);
                    EditorGUILayout.BeginHorizontal();
                }
            }
              
        }
    }

    void SaveToJSON ()
    {
        SaveObject saveObject = new SaveObject() { savedWeapons = weaponTypes };
        string json = JsonUtility.ToJson(saveObject);
        Debug.Log(json);

        File.WriteAllText(Application.dataPath + "/DesignMaster.txt", json);
    }

    static List<WeaponType> LoadFromJSON()
    {
 
        string file = Application.dataPath + "/DesignMaster.txt";
        File.ReadAllText(file);
        Debug.Log(File.ReadAllText(file));
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(File.ReadAllText(file));
        return saveObject.savedWeapons;


    }
}

[System.Serializable]
public class SaveObject
{
    public  List<WeaponType> savedWeapons;
}

