using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSideBook : MonoBehaviour
{
    public int a = 3;
    public float b;
    public double c;
    public string d;
    public GameObject g;
    public bool e;
    public GameObject[] gs;
    public int[] f;
    public Color color;
    public DoubleSideBook doubleSideBook;

    public AnimationCurve ac;
    public uint ua;
    public List<GameObject> gs2;


    public enum Fruit
    {
        Apple,
        Banana,
        Orange,
        Pinapple,
        Coconut
    }
    public Sprite sp;
    public Fruit myFruit1;
    public Fruit myFruit2;

    // Start is called before the first frame update
    public void Start()
    {
        int result1 = Plus(1,  Minus(1,  Minus(1,  Plus(1, 1))));
        Debug.Log(result1);
    }

    public int Plus(int a, int b)
    {
        return a + b;
    }

    public int Minus(int a, int b)
    {
        return a - b;
    }
}
