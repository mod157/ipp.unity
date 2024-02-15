using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamAnt.IPP.Sample
{
    public class SampleGameManager : MonoBehaviour
    {
        [SerializeField] private InputPad inputPad;
        [SerializeField] private CubeMove cubeMove;
        
        private void OnEnable()
        {
            //Add Pad Result Event Action
            inputPad.resultAction += ResultInputPad;
        }

        private void OnDisable()
        {
            //Remove Pad Result Event Action
            inputPad.resultAction -= ResultInputPad;
        }

        private void ResultInputPad(string result)
        {
            switch (result)
            {
                // <- Left + 3
                case "BA":
                case "ED":
                case "HG":
                    cubeMove.Move(-3,0);
                    break;
                // -> Right + 3
                case "BC":
                case "EF":
                case "HI":
                    cubeMove.Move(3,0);
                    break;
                // ^- Top + 3
                case "DA":
                case "EB":
                case "FC":
                    cubeMove.Move(0,3);
                    break;
                // -| Bottom + 3
                case "DG":
                case "EH":
                case "FI":
                    cubeMove.Move(0,-3);
                    break;
                // <-- Left + 10
                case "CBA":
                case "FED":
                case "IHG":
                    cubeMove.Move(-10,0);
                    break;
                // --> Right + 10
                case "ABC":
                case "DEF":
                case "GHI":
                    cubeMove.Move(10,0);
                    break;
                // ^-- Top + 10
                case "GDA":
                case "HEB":
                case "IFC":
                    cubeMove.Move(0,10);
                    break;
                // |-- Bottom + 10
                case "ADG":
                case "BEH":
                case "CFI":
                    cubeMove.Move(0,-10);
                    break;
                
                // ^|- LeftUp + 3
                case "EA":
                    cubeMove.Move(-3,3);
                    break;
                // -|^ RightUp + 3
                case "EC":
                    cubeMove.Move(3,3);
                    break;
                // |- LeftDown + 3
                case "EG":
                    cubeMove.Move(-3,-3);
                    break;
                // -| RightDown + 3
                case "EI":
                    cubeMove.Move(3,-3);
                    break;
            }
        }
    }
}
