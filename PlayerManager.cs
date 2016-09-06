using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    private ScreenManager screen;
    private StoreManager store;
    private DressUpManager dressUp;
    private ShoppingCart sCart;

    private List<object> playerList;
    public List<object> List {
        get { return playerList; }
        set { playerList = value; }
    }

    private List<string> itemList = new List<string>();

    public int Seeds { get; set; }

    public GameObject sellButton;
    public GameObject scrollContainer;
    public GameObject ScrollView { get; private set; }
    public GameObject Inventory { get; private set; }
    public GameObject GridBG { get; private set; }

    public Transform Tabs { get; private set; }

    private const string jsonFileName = "player";
    public string Json {
        get { return jsonFileName; }
    }


    void Awake() {
        screen = gameObject.GetComponent<ScreenManager>();
        store = gameObject.GetComponent<StoreManager>();
        dressUp = gameObject.GetComponent<DressUpManager>();
        sCart = gameObject.GetComponent<ShoppingCart>();

        ScrollView = scrollContainer.transform.GetChild(0).gameObject;
        Inventory = ScrollView.transform.GetChild(1).gameObject;
        GridBG = ScrollView.transform.GetChild(2).gameObject;
        Tabs = scrollContainer.transform.GetChild(1);

        scrollContainer.SetActive(false);
        sellButton.SetActive(false);

        Seeds = 2000;
        playerList = Serializer.DeserializeJSON(jsonFileName);
    }

    

    public void GetPlayerItems() {
        playerList = Serializer.DeserializeJSON(jsonFileName);
        List<Item> items = Serializer.CreateItemsFromJSON(playerList);
		UIScrollView scrollView = dressUp.ScrollView.GetComponent<UIScrollView>();

        foreach (Item item in items) {
            if (!itemList.Contains(item.imgName.Substring(0, item.imgName.Length - 1))) {
                AddItemToInventory(item, dressUp.Inventory, scrollView);
                itemList.Add(item.imgName.Substring(0, item.imgName.Length - 1));
            }
        }
        Grid.NormalizeGrid(dressUp.Inventory.gameObject, dressUp.GridBG.gameObject, dressUp.grid);
        Grid.Reposition(dressUp.Inventory.gameObject, dressUp.GridBG.gameObject);
    }

    // Remove all items in the player's inventory, except the clear button
    private void ClearPlayerInventory() {
        for (int i = 1; i < dressUp.Inventory.childCount; i++) {
            Destroy(dressUp.Inventory.GetChild(i).gameObject);
        }
    }
    
    // Create an instance of each item in the player's inventory and add it to the scene 
    // with a background grid image
    public void AddItemToInventory(Item item, Transform parent, UIScrollView scrollView) {
        GameObject instance = Instantiate(dressUp.dressUpGenericItem, Vector3.zero, Quaternion.identity) as GameObject;
        ItemManager iManager = instance.GetComponent<ItemManager>();

        instance.name = item.itemName;
        instance.tag = item.tag.ToLower();
        instance.GetComponent<UISprite>().spriteName = item.imgName;

        iManager.description = item.description;
        iManager.imgName = item.imgName;
        iManager.set = item.set;
        iManager.price = int.Parse(item.price);

        instance.transform.SetParent(parent);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localScale = Vector3.one;

        instance.GetComponent<UIDragScrollView>().scrollView = scrollView;
        instance.GetComponent<UISprite>().depth = 0;
    }

    public void ToggleBag() {
        scrollContainer.SetActive(!scrollContainer.activeSelf);
        store.scrollContainer.SetActive(!store.scrollContainer.activeSelf);

        sCart.buyButton.SetActive(!sCart.buyButton.activeSelf);
        sellButton.SetActive(!sellButton.activeSelf);

        UILabel label = sCart.panel.transform.GetChild(0).gameObject.GetComponent<UILabel>();
        if (store.Inventory.activeSelf)
            label.text = "STORE";
        else
            label.text = "MY BAG";
    }

    public void ConvertToBagItem(GameObject item) {
        GameObject instance = NGUITools.AddChild(Inventory, item);
        instance.GetComponent<BoxCollider>().enabled = true;
        Destroy(instance.transform.GetChild(2).gameObject);
        Grid.AddGrid(GridBG, store.gridObject);
        Grid.Reposition(Inventory, GridBG);
    }

    
}
