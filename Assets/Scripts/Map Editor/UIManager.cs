using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class UIManager : MonoBehaviour 
{
    public delegate void ChangeBrushType(BrushType type);
    public  static event ChangeBrushType changeBrushType;

    public delegate void SaveLevel(string fileName);
    public static event SaveLevel saveLevel;

    public delegate void LoadLevel(string fileName);
    public static event LoadLevel loadLevel;

    public delegate void Generate();
    public static event Generate generate;



    [SerializeField]
    private GameObject m_saveObject;
    [SerializeField]
    private GameObject m_loadObject;
    [SerializeField]
    private GameObject m_brushes;

    // TODO: Rubber object.

    [SerializeField]
    private Dropdown m_dropdown;
    [SerializeField]
    private Button m_loadButton;
    [SerializeField]
    private Button m_saveButton;
    [SerializeField]
    private Button m_brushesButton;

    [SerializeField]
    private bool m_brushesEnabled = false;
    [SerializeField]
    private bool m_saveEnabled = false;
    [SerializeField]
    private bool m_loadEnabled = false;

    private List<string> m_fileNames = new List<string>();



    private void Start()
    {
        m_fileNames = new List<string>(FileManager.SaveFiles());
        RefreshDropdownMenu();
    }

    private void Update()
    {
        m_brushes.SetActive(m_brushesEnabled);
        m_saveObject.SetActive(m_saveEnabled);
        m_loadObject.SetActive(m_loadEnabled);
        
        // TODO: Make other buttons not enabled non-interactable.

    }


    public void SelectRubber()
    {
       if(changeBrushType != null) changeBrushType(BrushType.Erase);
    }


    public void SelectBrush()
    {
        m_brushesEnabled = !m_brushesEnabled;

        if(!m_brushesEnabled) SelectBrushType("Unassigned");
    }


    public void SelectBrushType(string typeStr)
    {
        var type = System.Enum.Parse(typeof(BrushType), typeStr);
        if(type == null)
        {
            Debug.LogError("Error: Type name - " + typeStr + " does not exist in BrushType Enum");
            return;
        }
        if(changeBrushType != null) changeBrushType((BrushType)type);
    }


    public void ToggleLoad()
    {
        m_loadEnabled = !m_loadEnabled;
        m_loadObject.SetActive(m_loadEnabled);

        TileEditor.Pause = m_loadEnabled;
    }



    public void NewLoad()
    {
        if(!m_loadEnabled) return;

        // Auto save before loading a new level
        //NewSave();

        if(loadLevel != null) loadLevel(m_dropdown.captionText.text);

    }

    // TODO: Get rid of 
    public void ToggleSave()
    {
        m_saveEnabled = !m_saveEnabled;
        TileEditor.Pause = m_saveEnabled;
    }



    public void NewSave()
    {
        if(!m_saveEnabled) return;
        
        var textObjs = m_saveObject.GetComponentsInChildren<Text>();
        var fileName = "";

        foreach(var text in textObjs)
            if(text.gameObject.name == "File Name")
                fileName = text.text;
        
        if(fileName == "")
            return;

        if(saveLevel != null)
        {
            saveLevel(fileName + ".txt");
            m_fileNames = new List<string>(FileManager.SaveFiles());
            RefreshDropdownMenu();
            // TODO: Update the DropDown menu
        }
    }



    public void Regenerate()
    {
        if(generate != null)
            generate();
    }


    private void RefreshDropdownMenu()
    {
        m_dropdown.options.Clear();
        foreach(string name in m_fileNames)
        {
            m_dropdown.options.Add(new Dropdown.OptionData(name));
        }
    }
}
