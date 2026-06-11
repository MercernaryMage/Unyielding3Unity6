using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WispEmitter : MonoBehaviour
{
    public GameObject wispBasePrefab;

    float time = 0;
    int chance = 2;

    List<int> count = new List<int>()
                        {1,1,1,2,2,3 };

    List<Tile> validTiles = new List<Tile>();

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "FancyLevelEditor")
        {
            return;
        }
        Invoke(nameof(FinishWaiting), 0);
    }

    public void FinishWaiting()
    {
        foreach (Tile t in TileGrid.Instance.tiles)
        {
            //if (!t.IsWall() && !t.IsEmpty())
            {
                validTiles.Add(t);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (validTiles.Count == 0)
        {
            return;
        }

        float timeScalar = validTiles.Count / 72.0f;

        time += Time.deltaTime * timeScalar;
        if (time >= 1)
        {
            if (Util.Range(0, chance) == 0)
            {
                int i = Util.Range(0, count.Count);
                i = count[i];
                for (int j = 0; j < i; j++)
                {
                    MakeWisp();
                }
            }
            time = 0;
        }
    }

    void MakeWisp()
    {
        GameObject obj = Instantiate(wispBasePrefab);

        int index = Util.Range(0, validTiles.Count);
        obj.transform.position = validTiles[index].transform.position;
        obj.AddComponent<VerticalWisp>();
    }
}
