using UnityEngine;
using System.Collections.Generic;

public class ShoppingCart : MonoBehaviour {

	public GameObject shoppingCart;
    private GameObject scrollView;
    private GameObject cartGrid;
	private GameObject cartGridBG;
    
	public GameObject errorLabel;
	public GameObject cartGridGeneric;
	public GameObject buyButton;
    public GameObject panel;
    public GameObject storeDescription;

    public int money;
    public int total;
    public GameObject moneyLabel;
    public GameObject totalLabel;

    public List<GameObject> cartItems;
	private StoreManager store;
    private PlayerManager player;

	void Awake() {
		store = gameObject.GetComponent<StoreManager>();
        player = gameObject.GetComponent<PlayerManager>();

        scrollView = shoppingCart.transform.GetChild(0).gameObject;
		cartGrid = scrollView.transform.GetChild(1).gameObject;
		cartGridBG = scrollView.transform.GetChild(2).gameObject;
		cartItems = new List<GameObject>();

        money = player.Seeds;
        errorLabel.SetActive(false);
	}

    void Start() {
        moneyLabel.GetComponent<UILabel>().text = money.ToString();
    }

	// Remove the item from the cart and put it back into the shop
	public void RemoveItemFromCart(GameObject item) {
		item.transform.SetParent(store.gridObject.transform);
		Destroy(item.transform.GetChild(2).gameObject);
		store.GetPriceLabel(item).SetActive(true);
		total -= item.GetComponent<ItemManager>().price;
		item.GetComponent<BoxCollider>().enabled = true;
		store.ShowTotalCost();
		RemoveItemFromTempCart(item);
        Grid.Reposition(cartGrid, cartGridBG);
	}

	// Toggle visiblity of the shopping cart
	private void ToggleShoppingCart() {
		shoppingCart.SetActive(!shoppingCart.activeSelf);
		panel.SetActive(!panel.activeSelf);

		if (shoppingCart.activeSelf) {
			store.scrollContainer.SetActive(false);
			player.scrollContainer.SetActive(false);
            Grid.Reposition(cartGrid, cartGridBG);
			MoveFromTempToCart();
		}
	}
		
    // Reparent items in the temporary list from the store to the shopping cart
	private void MoveFromTempToCart() {
		foreach (GameObject item in cartItems) {
			item.transform.SetParent(cartGrid.transform);
			store.GetPriceLabel(item).SetActive(false);
			item.transform.GetChild(0).gameObject.SetActive(false);
			AddDescription(item.GetComponent<ItemManager>());
			item.GetComponent<BoxCollider>().enabled = false;
			Grid.AddGrid(cartGridBG, cartGridGeneric);
		}
        Grid.Reposition(cartGrid, cartGridBG);
        Grid.Reposition(store.Inventory, store.GridBG);
	}

    // Update the seed balance shown in the store 
    public void UpdateSeedBalance(string action) {
        if (action.Equals("buy")) {
            money -= total;
        } else {
            money += total / 2;
        }
        moneyLabel.GetComponent<UILabel>().text = "" + money;
        total = 0;
        totalLabel.GetComponent<UILabel>().text = "Total: " + total;
        player.Seeds = money;
    }

    // Turn off the shopping cart and update the seed balance
    private void CompleteTransaction(string action) {
		UpdateSeedBalance(action);
        Grid.Reposition(store.Inventory, store.GridBG);
        Grid.Reposition(player.Inventory, player.GridBG);
		store.scrollContainer.SetActive(true);
	}

	public void BuyCartItems() {
        if (!shoppingCart.activeSelf) {
            ToggleShoppingCart();
            return;
        }

		if (total > money) {
			ToggleErrorLabel();
			Invoke("ToggleErrorLabel", 2);
			return;
		}

		// we have to continuously take the first child because the indexes are resetting
		for (int i = 0; i < cartItems.Count;) {
			GameObject item = cartGrid.transform.GetChild(i).gameObject;
			store.ConvertToPlayerItem(item);
			RemoveItemFromTempCart(item);
			Grid.RemoveGrid(store.GridBG);

			Dictionary<string, object> entry = store.GetDictionary(store.List, item.name);
#if UNITY_EDITOR
            Serializer.AddJSONItem(player.List, entry, player.Json);
			Serializer.RemoveJSONItem(store.List, entry, store.Json);
#endif
        }
		CompleteTransaction("buy");
        ToggleShoppingCart();
    }

	public void SellCartItems() {
		for (int i = 0; i < cartItems.Count;) {
            GameObject item = cartItems[i];
            Debug.Log(item.name);
            store.ConvertToStoreItem(item);
            RemoveItemFromTempCart(item);
            Grid.RemoveGrid(player.GridBG);

            Dictionary<string, object> entry = store.GetDictionary(player.List, item.name);
#if UNITY_EDITOR
            Serializer.AddJSONItem(store.List, entry, store.Json);
			Serializer.RemoveJSONItem(player.List, entry, player.Json);
#endif
        }
		CompleteTransaction("sell");
        player.ToggleBag();
        store.scrollContainer.SetActive(true);
	}

    // Adds descriptions for items in the shopping cart
    private void AddDescription(ItemManager item) {
        GameObject o = Instantiate(storeDescription, Vector3.zero, Quaternion.identity) as GameObject;
        o.transform.GetChild(0).GetComponent<UILabel>().text = item.name;
        o.transform.GetChild(1).GetComponent<UILabel>().text = item.price.ToString();
        o.transform.GetChild(2).GetComponent<UILabel>().text = item.description;
        o.transform.SetParent(item.transform);
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(235, -25, 0);

        // Add removal behaviour
        UIButton btn = o.transform.GetChild(3).GetComponent<UIButton>();
        EventDelegate del = new EventDelegate(this, "RemoveItemFromCart");
        del.parameters[0] = new EventDelegate.Parameter(item.gameObject, "Item");
        EventDelegate.Set(btn.onClick, del);
    }

    // Toggle Visibility of the error label
    private void ToggleErrorLabel() {
        errorLabel.SetActive(!errorLabel.activeSelf);
    }

    // Add an item to the player's shopping cart 
    public void AddItemToCart(GameObject item) {
        cartItems.Add(item);
    }

    // Remove an item from the player's temporary shopping list
    public void RemoveItemFromTempCart(GameObject item) {
        cartItems.Remove(item);
    }

}
