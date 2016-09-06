using UnityEngine;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour {

    public GameObject genericItem;
    public GameObject gridObject;
    public GameObject scrollContainer;

	private ShoppingCart sCart;

    public GameObject ScrollView { get; private set; }
    public GameObject Inventory { get; private set; }
    public GameObject GridBG { get; private set; }
    public Transform Tabs { get; private set; }

    // Managers
    private ScreenManager screen;
    private DressUpManager dressUp;
    private PlayerManager player;

    public List<object> List { get; private set; }
    private string jsonFileName = "store";
    public string Json {
        get { return jsonFileName; }
    }

    void Awake() {
        screen = gameObject.GetComponent<ScreenManager>();
        dressUp = gameObject.GetComponent<DressUpManager>();
        player = gameObject.GetComponent<PlayerManager>();
		sCart = gameObject.GetComponent<ShoppingCart>();

        ScrollView = scrollContainer.transform.GetChild(0).gameObject;
        Tabs = scrollContainer.transform.GetChild(1);
        Inventory = ScrollView.transform.GetChild(1).gameObject;
        GridBG = ScrollView.transform.GetChild(2).gameObject; 
        sCart.money = player.Seeds;

        scrollContainer.SetActive(true);
        sCart.shoppingCart.SetActive(false);
        player.scrollContainer.SetActive(false);
    }
  
    public void StartShop() {
        List = Serializer.DeserializeJSON(jsonFileName);
        player.List = Serializer.DeserializeJSON(player.Json);
        GetItems(List, Inventory, ScrollView, GridBG);
        GetItems(player.List, player.Inventory, player.ScrollView, player.GridBG);
    }

    // Get Items from JSON and set up on screen
    public void GetItems(List<object> list, GameObject parent, GameObject scrollView, GameObject parentGridBG) {
        List<Item> items = Serializer.CreateItemsFromJSON(list);
        foreach (Item item in items) {
            AddItemToInventory(list, item, parent, scrollView.GetComponent<UIScrollView>());
            Grid.AddGrid(parentGridBG, gridObject);
        }
        Grid.NormalizeGrid(parent, parentGridBG, gridObject);
        Grid.Reposition(parent, parentGridBG);
    }

    // Create an instance of generic item and populate with the json objects
    public void AddItemToInventory(List<object> list, Item item, GameObject parent, UIScrollView scrollView) {
        GameObject instance = NGUITools.AddChild(parent, genericItem);
        ItemManager iManager = instance.GetComponent<ItemManager>();

        instance.name = item.itemName;
        instance.tag = item.tag.ToLower();
        instance.GetComponent<UISprite>().spriteName = item.imgName;
        
        iManager.description = item.description;
        iManager.imgName = item.imgName;
        iManager.set = item.set;
        iManager.price = int.Parse(item.price);

        instance.GetComponent<UIDragScrollView>().scrollView = scrollView;
        instance.transform.GetChild(1).gameObject.GetComponent<UILabel>().text = item.price;
    }

    // Diplay an updated total for the store
    public void ShowTotalCost() {
        sCart.totalLabel.GetComponent<UILabel>().text = "Total: " + sCart.total;
    }
    // Convention of prefabs with prices has price label as the second child
    public GameObject GetPriceLabel(GameObject storeDescription) {
        return storeDescription.transform.GetChild(1).gameObject;
    }
  
   // Repurpose the Generic Store Item as an Inventory item
    public void ConvertToPlayerItem(GameObject item) {
        player.ConvertToBagItem(item);
        item.AddComponent<SlotManager>();
		ReparentItem(item, dressUp.Inventory.gameObject, dressUp.GridBG.gameObject, dressUp.ScrollView.gameObject);
        foreach (Transform child in item.transform) {
            child.gameObject.SetActive(false);
        }

		Grid.AddGrid(dressUp.GridBG.gameObject, gridObject);  	
        Grid.Reposition(dressUp.Inventory.gameObject, dressUp.GridBG.gameObject);
        item.GetComponent<SlotManager>().restriction = UIDragDropItem.Restriction.Horizontal;
    }

    // Change the location of an item in the game hierarchy
    public void ReparentItem(GameObject item, GameObject parent, GameObject parentGridBG, GameObject scrollView) {
        item.transform.SetParent(parent.transform);
        item.GetComponent<UIDragScrollView>().scrollView = scrollView.GetComponent<UIScrollView>();
        item.GetComponent<BoxCollider>().enabled = true;
        NGUITools.MarkParentAsChanged(item);
        Grid.Reposition(parent, parentGridBG);
    }

    // Reparent the item under the store inventory
    public void ConvertToStoreItem(GameObject item) {
        ReparentItem(item, Inventory, GridBG, ScrollView);
        item.transform.GetChild(0).gameObject.SetActive(false);
        item.transform.GetChild(1).gameObject.SetActive(true);
        Grid.AddGrid(GridBG, gridObject);
		Grid.RemoveGrid(player.GridBG);
		Grid.RemoveGrid(dressUp.GridBG.gameObject);
    }
    

    // Get the corresponding dictionary entry for the given itemName from the list of dictionaries
    public Dictionary<string, object> GetDictionary(List<object> list, string itemName) {
        foreach (Dictionary<string, object> dict in list) {
            if (dict["itemName"].Equals(itemName)) {
                return dict;
            }
        }
        Debug.LogError("Item was not found in the dictionary");
        return null;
    }
}


