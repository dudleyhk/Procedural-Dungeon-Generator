using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class UIManager : MonoBehaviour 
{
    public delegate void ChangeBrushType(BrushType type);
    public  static event ChangeBrushType changeBrushType;









    [SerializeField]
    private bool m_brushesEnabled = false;






    public void SelectRubber()
    {
       if(changeBrushType != null) changeBrushType(BrushType.Erase);
    }


    public void SelectBrush(GameObject brushes)
    {
        // Enable brush selection
        m_brushesEnabled = !m_brushesEnabled;
        brushes.SetActive(m_brushesEnabled);

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
}
