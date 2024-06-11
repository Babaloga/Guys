using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.TestData.Definitions;
using UnityEngine;

public class GenerateNames : MonoBehaviour
{
    public int numberToGenerate = 1000;

    string[] namesList1;
    string[] namesList2;
    string[] nouns;

    void Start()
    {
        string namesList1In = Resources.Load<TextAsset>("Name1").text;

        namesList1 = namesList1In.Split('\n');

        string namesList2In = Resources.Load<TextAsset>("Name2").text;

        namesList2 = namesList2In.Split('\n');

        string nounsListIn = Resources.Load<TextAsset>("nouns").text;

        nouns = nounsListIn.Split('\n');

        //m_lastHit.Value = "";

        string[] names = new string[numberToGenerate];

        for (int i = 0; i < numberToGenerate; i++)
        {
            int rand = UnityEngine.Random.Range(0, 1000);
            if (rand < 1) //1
            {
                names[i] = string.Concat("Good Ol' ", namesList2.RandomEntry().ToTitleCase(), " ", namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
            }
            else if (rand < 6) //5
            {
                names[i] = namesList1.RandomEntry().ToTitleCase().Replace("\r", "");
            }
            else if (rand < 247) //240
            {
                if (rand < 47) //40
                {
                    names[i] = string.Format("{0} the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 87) //40
                {
                    names[i] = string.Format("{0} with the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 127) //40
                {
                    names[i] = string.Format("{0} of the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 167) //40
                {
                    names[i] = string.Format("{0} the {1}less", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 177) //10
                {
                    names[i] = string.Format("{0}, holder of the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 187) //10
                {
                    names[i] = string.Format("{0}, keeper of the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 207) //20
                {
                    names[i] = string.Format("{0} {1} {2}", namesList2.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else //40
                {
                    names[i] = string.Format("{0} from the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
            }
            else if (rand < 307) //60
            {
                if (rand < 257) //10
                {
                    names[i] = string.Format("{0} (not the {1} one)", namesList1.RandomEntry().ToTitleCase(), namesList2.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 267) //10
                {
                    names[i] = string.Format("{0} the {1}-Man", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 277) //10
                {
                    names[i] = string.Format("{0} the {1}-Woman", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 287) //10
                {
                    names[i] = string.Format("{0} the Were{1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToLower()).Replace("\r", "");
                }
                else if (rand < 292) //5
                {
                    names[i] = string.Format("Thoroughly {0} {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 297) //5
                {
                    names[i] = string.Format("Intensely {0} {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 302) //5
                {
                    names[i] = string.Format("Off-Puttingly {0} {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else //5
                {
                    names[i] = string.Format("The {0}er {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
            }
            else //693
            {
                names[i] = string.Concat(namesList1.RandomEntry().ToTitleCase(), " the ", namesList2.RandomEntry().ToTitleCase()).Replace("\r", "");
            }
        }


        File.WriteAllLines(Application.dataPath + "/Resources/NameTests.txt", names);
        
    }

}
