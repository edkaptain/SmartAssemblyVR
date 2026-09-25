using UnityEngine;
using UnityEngine.UI;

public class GameTag : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private ObjectsDataBase database;
    [SerializeField] private SnapItemType selectedComponent;

    [Header("UI Components")]
    [SerializeField] private Text nameTxt;
    [SerializeField] private Image pictureImage;
    [SerializeField] private Text stepNumber;
    [SerializeField] private Text stepTxt;
    [SerializeField] private Text componentModule;

    private void OnValidate()
    {
        UpdateGameTag();
    }

    private void UpdateGameTag()
    {
        if (database == null)
            return;

        Item item = database.GetItem(selectedComponent);

        if (item == null)
        {
            Debug.LogWarning($"No information found for {selectedComponent}");
            return;
        }

        if (stepNumber != null)
            stepNumber.text = item.stepNumber.ToString();

        if (stepTxt != null)
            stepTxt.text = item.itemModule;

        if (nameTxt != null)
            nameTxt.text = item.name;

        if (stepTxt != null)
            stepTxt.text = "Use in assembly step " + item.stepNumber;

        if (pictureImage != null)
            pictureImage.sprite = item.picture;

        if (componentModule != null)
            componentModule.text = item.itemModule.ToString(); 
    }
}