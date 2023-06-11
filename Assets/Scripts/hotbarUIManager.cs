using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hotbarUIManager : MonoBehaviour
{
    public Sprite unselected;
    public Sprite selected;
    public Image[] hotbarFrames;
    public Image[] hotbarItems;
    public PlayerBehavior player;
    public gameManager gm;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.U)) {
            render();
        }
    }

    public void render() {
        for (int ri = 0; ri < 5; ri++) {
            if (player.selectedSlot == ri) {
                hotbarFrames[ri].sprite = selected;
                hotbarFrames[ri].rectTransform.sizeDelta = Vector2.one * 110;
            } else {
                hotbarFrames[ri].sprite = unselected;
                hotbarFrames[ri].rectTransform.sizeDelta = Vector2.one * 100;
            }
            if (player.inventory[ri] == -1) {
                hotbarItems[ri].enabled = false;
            } else {
                hotbarItems[ri].enabled = true;
                hotbarItems[ri].sprite = gm.gameItems[player.inventory[ri]].Sprite;
                
            }
        }
    }
}
