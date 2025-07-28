using UnityEngine;
using TMPro;

public class LandscapeWarning : MonoBehaviour
{

    public static LandscapeWarning Instance { get; private set; }
    private TextMeshProUGUI tmp;
    // Set up singleton pattern when object awakens
    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string newText)
    {
        tmp.text = newText;
    }
}