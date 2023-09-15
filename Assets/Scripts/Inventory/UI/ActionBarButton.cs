using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;
        private bool canUse;

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }
        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }

        private void OnUpdateGameStateEvent(GameState gameState)
        {
            canUse = gameState == GameState.GamePlay;
        }

        private void Update()
        {
            if (Input.GetKeyDown(key) && canUse)
            {
                if (slotUI.itemAmount == 0) return;
                if (slotUI != null)
                {
                    slotUI.isSelected = !slotUI.isSelected;

                    if (slotUI.isSelected)
                        slotUI.inventoryUI.UpdataSlotHightlight(slotUI.slotIndex);
                    else
                        slotUI.inventoryUI.UpdataSlotHightlight(-1);
                    EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
                }
            }
        }
    }
}