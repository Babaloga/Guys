using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenPointer : MonoBehaviour
{
    public Camera camera;
    public GameObject pointerPrefab;
    Dictionary<GuyBehavior, GameObject> activePointers;

    // Start is called before the first frame update
    void Start()
    {
        activePointers = new Dictionary<GuyBehavior, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        List<GuyBehavior> toRemove = new List<GuyBehavior>();

        foreach(KeyValuePair<GuyBehavior, GameObject> pair in activePointers)
        {
            if (pair.Key == null) toRemove.Add(pair.Key);
        }

        foreach(GuyBehavior g in toRemove)
        {
            Destroy(activePointers[g]);
            activePointers.Remove(g);
        }

        if (GuyBehavior.activeGuys == null || GuyBehavior.activeGuys.Count == 0) return;

        Vector2 halfScreenSize = new Vector2(Screen.width, Screen.height) / 2;

        foreach(GuyBehavior gb in GuyBehavior.activeGuys)
        {

            Vector2 screenPosObj = camera.WorldToScreenPoint(new Vector3(gb.transform.position.x, 0, gb.transform.position.z));

            if (gb.transform.position.y > 20)
            {
                GameObject pointer;

                if (!activePointers.TryGetValue(gb, out pointer))
                {
                    pointer = Instantiate(pointerPrefab, transform);
                    activePointers.Add(gb, pointer);
                }

                pointer.GetComponent<RectTransform>().anchoredPosition = new Vector3(Mathf.Clamp(screenPosObj.x, 0, Screen.width), 0, 0);

                TMPro.TMP_Text text = pointer.GetComponentInChildren<TMPro.TMP_Text>();
                text.text = string.Format("{0}\n{1}m", gb.m_guyName.Value, gb.transform.position.y.ToString("#######0.0"));

            }
            else
            {
                if (activePointers.ContainsKey(gb))
                {
                    Destroy(activePointers[gb]);
                    activePointers.Remove(gb);
                }
            }
        }

    }
}
