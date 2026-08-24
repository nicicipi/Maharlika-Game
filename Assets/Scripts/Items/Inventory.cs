using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private List<ItemsManager> itemsList;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        itemsList = new List<ItemsManager>();
        //Debug.Log("Inventory created test 1.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItems(ItemsManager item)
    {
        print(item.itemName + " added to inventory");
        itemsList.Add(item);
        print(itemsList.Count);
    }
}
