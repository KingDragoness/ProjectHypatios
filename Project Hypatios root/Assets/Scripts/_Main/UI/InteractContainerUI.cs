using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class InteractContainerUI : MonoBehaviour
{

    public QuickLootInventoryButton button;
    public Text titleLabel;
    public RectTransform parentContainer;
    public int index = 0;

    public List<QuickLootInventoryButton> allQuickLootButtons = new List<QuickLootInventoryButton>();

    [ShowInInspector] [ReadOnly]
    private Interact_Container currentContainer;

    private void OnEnable()
    {
        currentContainer = InteractableCamera.instance.currentInteractable as Interact_Container;

        if (currentContainer != null)
        {
            RefreshUI();
        }    
    }


    void Update()
    {
        var _interact1 = InteractableCamera.instance.currentInteractable as Interact_Container;

        if (_interact1 != null)
        {
            Hypatios.Game.RuntimeTutorialHelp(Hypatios.CodexList.Container);
            if (_interact1 != currentContainer)
            {
                currentContainer = _interact1;
                RefreshUI();
            }

        }
        if (currentContainer.inventory.allItemDatas.Count <= 0)
            return;

        var mouseVector = Hypatios.Input.SwitchWeapon.ReadValue<float>();


        if (mouseVector < 0f && Hypatios.Input.SwitchWeapon.triggered)
        {
            if (index >= allQuickLootButtons.Count - 1)
            {
                index = 0;
                RefreshSelect();
            }
            else
            {
                index++;
                RefreshSelect();
            }

        }
        if (mouseVector > 0f && Hypatios.Input.SwitchWeapon.triggered)
        {
            if (index <= 0)
            {
                index = allQuickLootButtons.Count - 1;
                RefreshSelect();
            }
            else
            {
                index--;
                RefreshSelect();
            }
        }

        if (Hypatios.Input.Interact.triggered)
        {
            //grab item
            QuickLoot();
        }

        if (allQuickLootButtons.Count > index)
        {
            if (allQuickLootButtons[index] != null)
            {
                RefreshSelect();
            }
        }
    }

    public void QuickLoot()
    {
        var itemDat = currentContainer.inventory.TransferTo(Hypatios.Player.Inventory, index);
        MainGameHUDScript.Instance.lootItemUI.NotifyItemLoot(itemDat);
        RefreshUI();
    }

    private void RefreshSelect()
    {
        allQuickLootButtons[index].selectable.Select();
    }

    private void RefreshUI()
    {
        allQuickLootButtons.RemoveAll(x => x == null);
        foreach (var button in allQuickLootButtons)
        {
            Destroy(button.gameObject);
        }
        allQuickLootButtons.Clear();

        titleLabel.text = currentContainer.ContainerName;

        int lastIndexSelected = index;
        if (lastIndexSelected >= currentContainer.inventory.allItemDatas.Count - 1) lastIndexSelected = currentContainer.inventory.allItemDatas.Count - 1;
        if (lastIndexSelected <= 0) lastIndexSelected = 0;

        bool b = false;
        int count = 0;

        foreach (var itemDat in currentContainer.inventory.allItemDatas)
        {
            var newButton = Instantiate(button, parentContainer);
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            if (itemClass == null) continue;

            newButton.gameObject.SetActive(true);
            newButton.nameLabel.text = itemClass.GetDisplayText();
            newButton.countLabel.text = itemDat.count.ToString();
            newButton.inventoryIcon.sprite = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory).sprite;


            if (lastIndexSelected == count && b == false)
            {
                index = lastIndexSelected;
                newButton.selectable.Select();
                b = true;
            }

            count++;
            allQuickLootButtons.Add(newButton);
        }
    }

}
