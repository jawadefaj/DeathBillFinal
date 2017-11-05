using UnityEngine;
using UnityEditor;
using System.Collections;

public class MatChanger : Editor {



    private const string mobileBumpedDiffuse = "Mobile/Bumped Diffuse";
    private const string mobileDiffuse = "Mobile/Diffuse";


    [MenuItem("Arif/MatCheck")]
    public static void MatCheck()
    {
        int total = 0;
        int notNeeded = 0;
        int unchanged = 0;
        foreach (object obj in Selection.objects)
        {
            total++;
            Material m = (Material)obj;
            switch (m.shader.name)
            {
                case "Standard":
                case "Standard (Specular setup)":
                case "Legacy Shaders/Bumped Diffuse":
                    m.shader = Shader.Find(mobileBumpedDiffuse);
                    break;
                case "Legacy Shaders/Specular":
                case "Legacy Shaders/Diffuse":
                    m.shader = Shader.Find(mobileDiffuse);
                    break;

                case "Mobile/Diffuse":
                case "Mobile/Bumped Diffuse":
                    notNeeded++;
                    break;
                default:
                    Debug.Log(m.shader.name);
                    unchanged++;
                    break;
            }
        }
        Debug.Log("Total: " + total.ToString()+", Changed: "+(total -unchanged - notNeeded).ToString()+", Unchanged: "+unchanged.ToString()+", Not Needed: "+notNeeded.ToString());

    }
}
