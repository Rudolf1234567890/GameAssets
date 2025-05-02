using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChoosePanel : MonoBehaviour
{
    public GameObject weaponstorePanel;
    public GameObject powerupPanel;
    public GameObject choosePanel;

    public LoadoutScript loadoutscript;

    private void Start()
    {
        powerupPanel.SetActive(false);
        weaponstorePanel.SetActive(false);
        choosePanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (choosePanel != null)
            {
                choosePanel.SetActive(true);
            }
            Cursor.visible = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (choosePanel != null)
                choosePanel.SetActive(false);
            if (powerupPanel != null)
                powerupPanel.SetActive(false);
            if (weaponstorePanel != null)
                weaponstorePanel.SetActive(false);
           
            Cursor.visible = false;
        }
    }

    public void PowerUpPanel()
    {
        powerupPanel.SetActive(true);
        weaponstorePanel.SetActive(false);
        choosePanel.SetActive(false);
    }

    public void WeaponstorePanel()
    {
        powerupPanel.SetActive(false);
        loadoutscript.OpenStorePanel();
        choosePanel.SetActive(false);
    }
}
