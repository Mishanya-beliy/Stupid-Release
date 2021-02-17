using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacingTablePosition : MonoBehaviour
{
    private GameObject table;
    public void SetLinkOnTable(GameObject table)
    {
        this.table = table;
    }
    public void Replacing()
    {
        if (table != null)
            table.GetComponent<PlaceTable>().Replacing();
        else
            GameObject.FindGameObjectWithTag("GameTable").GetComponent<PlaceTable>().Replacing();
    }
}
