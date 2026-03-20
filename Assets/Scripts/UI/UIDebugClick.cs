using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIDebugClick : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                Debug.Log("UIDebug: rien touché");
                return;
            }

            foreach (var result in results)
            {
                Debug.Log($"UIDebug: touché → {result.gameObject.name} | layer: {LayerMask.LayerToName(result.gameObject.layer)} | depth: {result.depth}");
            }
        }
    }
}