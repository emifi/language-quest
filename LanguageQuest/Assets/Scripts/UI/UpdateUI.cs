using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateUI : MonoBehaviour
{
    public GameObject panelUI;
    GameObject updateUI;
        private Color removeColor = new Color( //Red
            255f/256f, 
            121f/256f, 
            121f/256f
        );
        
        private Color failColor = new Color( //Orange
            255f/256f, 
            190f/256f, 
            118f/256f
        );
        private Color failColor2 = new Color( //Yellow
            219f/256f,
            204f/256f, 
            92f/256f
        );
        private Color addColor = new Color( //Green
            186f/256f,
            220f/256f, 
            88f/256f
        );
        /*
        new Color( //Sky Blue
            126f/256f,
            237f/256f, 
            219f/256f
        ),*/
        private Color noteColor = new Color( //Ocean Blue
            124f/256f,
            183f/256f, 
            217f/256f
        );
        private Color startColor = new Color( //Purple
            141f/256f,
            126f/256f, 
            237f/256f
        );
        private Color endColor = new Color( //Lavender
            200f/256f,
            126f/256f, 
            237f/256f
        );
        /*
        new Color( //Pink
            237f/256f,
            126f/256f, 
            235f/256f
        )
        */

    // Start is called before the first frame update
    void Start()
    {
        updateUI = GameObject.Find("UpdateUI");
        GameObject newPanelUI = Instantiate(panelUI);
        newPanelUI.GetComponent<TMP_Text>().text = $"Press 'N' to view your notebook!";
        newPanelUI.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
        newPanelUI.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

        newPanelUI.transform.SetParent(updateUI.transform);
        StartCoroutine(enter(startColor,newPanelUI));
        StartCoroutine(fadeAndRemove(0f, newPanelUI,20.0f));
    }

    public void end(){
        updateUI = GameObject.Find("UpdateUI");
        GameObject newPanelUI = Instantiate(panelUI);
        newPanelUI.GetComponent<TMP_Text>().text = $"Mahsi' eenjit 'in! Thanks for playing!";
        newPanelUI.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
        newPanelUI.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

        newPanelUI.transform.SetParent(updateUI.transform);
        StartCoroutine(enter(startColor,newPanelUI));
        StartCoroutine(fadeAndRemove(0f, newPanelUI,1000.0f));
    }

    public void AddInvDisplay(ItemObject item, int amount){
        //Color color = pallete[Random.Range(0,pallete.Length)];
        //Mahsi' eenjit 'in!
        GameObject newPanelUI = Instantiate(panelUI);
        Color color;

        if(amount==0){
            color = failColor;
            newPanelUI.GetComponent<TMP_Text>().text = $" ! Inventory full... Could not carry {item.englishName}({item.nativeName}).";
        }else{
            color = addColor;
            newPanelUI.GetComponent<TMP_Text>().text = $" + Added {amount}x {item.englishName}({item.nativeName}) to inventory!";
        }
        newPanelUI.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
        newPanelUI.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

        newPanelUI.transform.SetParent(updateUI.transform);
        StartCoroutine(enter(color,newPanelUI));
        StartCoroutine(fadeAndRemove(0f, newPanelUI,7.0f));
    }

    public void RemoveInvDisplay(ItemObject item, int amount){
        //Color color = pallete[Random.Range(0,pallete.Length)];
        GameObject newPanelUI = Instantiate(panelUI);
        newPanelUI.GetComponent<TMP_Text>().text = $" - {amount}x {item.englishName}({item.nativeName} removed from inventory.";
        newPanelUI.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
        newPanelUI.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

        newPanelUI.transform.SetParent(updateUI.transform);
        StartCoroutine(enter(removeColor,newPanelUI));
        StartCoroutine(fadeAndRemove(0f, newPanelUI,7.0f));
    }

    public void RemoveFailDisplay(ItemObject item, int amount){
        //Color color = pallete[Random.Range(0,pallete.Length)];
        GameObject newPanelUI = Instantiate(panelUI);
        newPanelUI.GetComponent<TMP_Text>().text = $" ! You don't have {amount}x {item.englishName}({item.nativeName})!";
        newPanelUI.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
        newPanelUI.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

        newPanelUI.transform.SetParent(updateUI.transform);
        StartCoroutine(enter(failColor2,newPanelUI));
        StartCoroutine(fadeAndRemove(0f, newPanelUI,7.0f));
    }

    public void AddNotebookDisplay(ItemObject item){
        //Color color = pallete[Random.Range(0,pallete.Length)];
        GameObject newPanelUI = Instantiate(panelUI);
        newPanelUI.GetComponent<TMP_Text>().text = $" + New term added to notebook: {item.englishName}({item.nativeName})";
        newPanelUI.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
        newPanelUI.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

        newPanelUI.transform.SetParent(updateUI.transform);
        StartCoroutine(enter(noteColor,newPanelUI));
        StartCoroutine(fadeAndRemove(0f, newPanelUI,7.0f));
    }

    IEnumerator enter(Color color, GameObject newPanelUI) {
        float t = 0.0f;
        while (t < 1.0) {
            newPanelUI.GetComponent<TMP_Text>().color = Color.Lerp(color, new Color(1f, 1f, 1f, 0f), (1.0f-t)/1.0f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator fadeAndRemove(float t, GameObject newPanelUI, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        while (t < 4.0) {
            newPanelUI.GetComponent<TMP_Text>().color = Color.Lerp(newPanelUI.GetComponent<TMP_Text>().color, new Color(1f, 1f, 1f, 0f), t/4.0f);
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(newPanelUI);
    }
}
