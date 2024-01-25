using System;
using System.Collections;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace ShowLootOnPlanet.Patches
{
    [HarmonyPatch]
    internal class ScanPatch
    {
        static GameObject counter;
        static TextMeshProUGUI textMesh;
        private static float displayLeft;
        private static float DisplayTime = 3f;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
        private static void OnScan(HUDManager __instance, InputAction.CallbackContext context)
        {
            if (GameNetworkManager.Instance.localPlayerController == null)
                return;
            /*if (GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom)
                return;*/

            PlanetModBase.log.LogInfo("SCAN!");
            if (!counter)
                CopyValueCounter();

            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 91);
			int objectAmount = 0;
			int TotalValue = 0;
			int num4 = 0;
			GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
			for (int num5 = 0; num5 < array.Length; num5++)
			{
				if (array[num5].itemProperties.isScrap && !array[num5].isInShipRoom && !array[num5].isInElevator)
				{
					num4 += array[num5].itemProperties.maxValue - array[num5].itemProperties.minValue;
					TotalValue += Mathf.Clamp(random.Next(array[num5].itemProperties.minValue, array[num5].itemProperties.maxValue), array[num5].scrapValue - 6 * num5, array[num5].scrapValue + 9 * num5);
					objectAmount++;
				}
			}
			PlanetModBase.log.LogInfo("Amount of objects: " + objectAmount + " || Value of all: " + TotalValue);

            if (objectAmount > 0) {
                DisplayTime = PlanetModBase.lifetime.Value;
                StyleText();
                textMesh.text = $"MOON: ${TotalValue:F0} (" + objectAmount + ")";
                
                displayLeft = DisplayTime;
                if (!counter.activeSelf)
                    GameNetworkManager.Instance.StartCoroutine(PlanetLoot());
            }
		}

        private static IEnumerator PlanetLoot()
        {
            counter.SetActive(true);
            while (displayLeft > 0f)
            {
                float time = displayLeft;
                displayLeft = 0f;
                yield return new WaitForSeconds(time);
            }
            counter.SetActive(false);
            Object.Destroy(counter);
        }

        private static void StyleText()
        {
            textMesh.fontSize = PlanetModBase.fontSize.Value;
            

            switch (PlanetModBase.colorValue)
            {
                case ColorType.White:
                    textMesh.color = new Color(0.9056604f, 0.9056604f, 0.9056604f);
                    break;
                case ColorType.Red:
                    textMesh.color = new Color(1, 0.3820755f, 0.3820755f);
                    break;
                case ColorType.Orange:
                    textMesh.color = new Color(1, 0.7653924f, 0.3803922f);
                    break;
                case ColorType.Yellow:
                    textMesh.color = new Color(0.9899307f, 1, 0.3803922f);
                    break;
                case ColorType.Green:
                    textMesh.color = new Color(0.3803922f, 1, 0.6797408f);
                    break;
                case ColorType.Blue:
                    textMesh.color = new Color(0.3803922f, 0.8848869f, 1);
                    break;
                case ColorType.Purple:
                    textMesh.color = new Color(0.6813213f, 0.3803922f, 1);
                    break;
                case ColorType.Pink:
                    textMesh.color = new Color(0.9750692f, 0.3803922f, 1);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Copy an existing object loaded by the game for the display of ship loot and put it in the right position.
        /// </summary>
        private static void CopyValueCounter()
        {
            GameObject _counter = GameObject.Find("/Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/ValueCounter");
            if (!counter)
                PlanetModBase.log.LogWarning("Failed to find ValueCounter object to copy!");
            counter = Object.Instantiate(_counter.gameObject, _counter.transform.parent, false);
            
            counter.transform.Translate(0f, 1f, 0f);
            Vector3 pos = counter.transform.localPosition;
            counter.transform.localPosition = new Vector3(pos.x + 50f, -50f, pos.z);
            textMesh = counter.GetComponentInChildren<TextMeshProUGUI>();

            switch (PlanetModBase.offset.Value)
            {
                case OffsetGUI.Original:
                    break;
                case OffsetGUI.Lowered:
                    counter.transform.Translate(0, -.18f, 0f);
                    break;
                case OffsetGUI.Lowered2:
                    counter.transform.Translate(-0, -.3f, 0f);
                    break;
                case OffsetGUI.Uppered:
                    counter.transform.Translate(0, .18f, 0f);
                    break;
                case OffsetGUI.Uppered2:
                    counter.transform.Translate(0, .3f, 0f);
                    break;
            }
        }
    }
}
