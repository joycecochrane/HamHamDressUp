using UnityEngine;
using System.Collections.Generic;
using System.IO;
using MiniJSON;

public static class Serializer {
    private static string location = "/Resources/JsonFiles/";
    private static string folder = "JsonFiles/";

    public static List<object> DeserializeJSON(string fileName) {
        TextAsset fileTxt = Resources.Load(folder + fileName, typeof(TextAsset)) as TextAsset;
        return Json.Deserialize(fileTxt.text) as List<object>;
    }

    public static void RemoveJSONItem(List<object> list, object item, string fileName) {
        list.Remove(item);
        File.WriteAllText(GetFilePath(fileName), Json.Serialize(list));
    }

    public static void AddJSONItem(List<object> list, object item, string fileName) {
        list.Add(item);
        File.WriteAllText(GetFilePath(fileName), Json.Serialize(list));
    }

    private static string GetFilePath(string fileName) {
        return Application.dataPath + location + fileName + ".json";
    }

    // Return a list of Items read from a JSON file
    public static List<Item> CreateItemsFromJSON(List<object> list) {
        List<Item> items = new List<Item>();

        // Add each json object (as a Dictionary) to a list
        for (int i = 0; i < list.Count; i++) {
            var dict = list[i] as Dictionary<string, object>;
            try {
                Item item = new Item();
                item.itemName = dict["itemName"] as string;
                item.tag = dict["tag"] as string;
                item.imgName = dict["imgName"] as string;
                item.description = dict["description"] as string;
                item.set = dict["set"] as string;
                item.price = dict["price"] as string;

                //Debug.Log(item.itemName + " " + item.tag + " " + item.imgName +
                //    " " + item.description + " " + item.set + " " + item.price);

                items.Add(item);
            } catch (KeyNotFoundException knfe) {
                Debug.Log("Store was not loaded because an item attribute was missing "
                    + knfe.Message);
            }
        }
        if (items == null) {
            throw new UnityException("CreateItemsFromJSON failed");
        }
        return items;
    }

}

public class Item {
    public string itemName;
    public string tag;
    public string imgName;
    public string description;
    public string set;
    public string price;
}
