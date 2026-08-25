using UnityEngine;
using System.Collections.Generic;
public class ModularBuildLevel : MonoBehaviour
{
  [SerializeField, HideInInspector]
private List<Transform> _levels = new();
public IReadOnlyList<Transform> Levels => _levels;

[SerializeField, HideInInspector]
private int _activeLevelIndex;
public int ActiveLevelIndex => _activeLevelIndex;

[SerializeField, HideInInspector]
private ModularKitDefinition _kit;

    public Transform ActiveLevel
    {


        get
        {

            if (_levels == null)
            {

                return null;


            }


            if (_levels[_activeLevelIndex] == null)
            {


                return null;


            }
   

            return _levels[_activeLevelIndex];
        }


    }


    private void CreateInitialLevel()
    {

        if (_levels.Count > 0)
        {

            return;


        }

        GameObject levelObject = new GameObject("Level_00");

        levelObject.transform.SetParent(transform);
        levelObject.transform.localPosition = Vector3.zero;

        _levels.Add(levelObject.transform);

        _activeLevelIndex = 0;



    }


    public void Initialize(ModularKitDefinition kit)
    {
        _kit = kit;
        CreateInitialLevel();
    }



    public void AddLevel()
    {

        int newLevelIndex = _levels.Count;

        GameObject newLevel = new GameObject("Level_" + newLevelIndex.ToString("00"));

        newLevel.transform.SetParent(transform);

        float y = newLevelIndex * _kit.FloorHeight;

        newLevel.transform.localPosition = new Vector3(0, y, 0);


        _levels.Add(newLevel.transform);

        _activeLevelIndex = newLevelIndex;

    }


    public void SetActiveLevel(int levelIndex)
    {
        
        if(_levels.Count > levelIndex && levelIndex >= 0)
        {
            

            _activeLevelIndex = levelIndex;


        }



    }







}