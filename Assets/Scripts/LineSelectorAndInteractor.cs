using Assets.Scripts.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class LineSelectorAndInteractor : MonoBehaviour
    {

        internal List<GameObject> _SelectedLineObjects = new List<GameObject>();
        internal List<gcLine> _SelectedGcLines = new List<gcLine>();

        internal List<gcLine> SelectedGcLines
        {
            get => _SelectedGcLines;
            set => _SelectedGcLines = value;
        }

        internal List<GameObject> SelectedLineObjects
        {
            get => _SelectedLineObjects;
            set
            {
                _SelectedGcLines.Clear();
                foreach (GameObject line in value)
                {
                    _SelectedGcLines.Add(line.GetComponent<gcLine>());
                }
                _SelectedLineObjects = value;
            }

        }


        internal bool SelectedLinesAUXOn
        {
            get
            {
                return SelectedGcLines.Where(x => x.AUX1 == true).Count() == 0;
            }
        }
        internal int? SelectedLinesVolt
        {
            get
            {
                if (SelectedGcLines.Where(x => x.volt == SelectedGcLines[0].volt).Count() == SelectedGcLines.Count())
                {
                    return SelectedGcLines[0].volt;
                }

                return null;
            }
        }

        internal void ToggleAux(bool switchOn)
        {
            foreach (gcLine line in SelectedGcLines)
            {
                line.AUX1 = switchOn;
            }
        }

        internal void SetVolt(int value)
        {
            foreach (gcLine line in SelectedGcLines)
            {
                line.volt = value;
            }
        
        }


    }
}
