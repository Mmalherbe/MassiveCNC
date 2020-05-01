using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.classes
{
    public class gcLine
    {
        public int? linenr;
        public int? G;
        public float? X;
        public float? Y;
        public float? Z;
        public float? F;
        public float? I;
        public float? J;
        public float? K;
        public float? L;
        public float? N;
        public float? P;
        public float? R;
        public float? S;
        public float? T;
        public int? M;
        public int? volt;
        public bool? AUX1;

        public override string ToString()
        {
            return "" + linenr +
                " " + (G != null ? "G" + G : "") +
                " " + (X != null ? "X" + X : "") +
                " " + (Y != null ? "Y" + Y : "") +
                " " + (Z != null ? "Z" + Z : "") +
                " " + (F != null ? "F" + F : "") +
                " " + (I != null ? "I" + I : "") +
                " " + (J != null ? "J" + J : "") +
                " " + (K != null ? "K" + K : "") +
                " " + (L != null ? "L" + L : "") +
                " " + (N != null ? "N" + N : "") +
                " " + (P != null ? "P" + P : "") +
                " " + (R != null ? "R" + R : "") +
                " " + (S != null ? "S" + S : "") +
                " " + (T != null ? "T" + T : "") +
                " " + (AUX1 != null? ((bool)AUX1 ? "M54 P1" : "M54 P0") : "") +
                " " + (volt != null? "M3 S" + volt : "");

        }
        public string ToEdingString()
        {
            return "" + linenr +
    " " + (G != null ? "G" + G : "") +
    " " + (X != null ? "X" + X : "") +
    " " + (Z != null ? "Y" + -Y: "") +
    " " + (Y != null ? "Z" + (Z == 0.00001f ? 0 : Z) : "") +
    " " + (F != null ? "F" + F : "") +
    " " + (I != null ? "I" + I : "") +
    " " + (J != null ? "J" + J : "") +
    " " + (K != null ? "K" + K : "") +
    " " + (L != null ? "L" + L : "") +
    " " + (M != null ? "M" + M : "") +
    " " + (N != null ? "N" + N : "") +
    " " + (P != null ? "P" + P : "") +
    " " + (R != null ? "R" + R : "") +
    " " + (S != null ? "S" + S : "") +
    " " + (T != null ? "T" + T : "") +
    " " + (AUX1 != null ? ((bool)AUX1 ? "M54 P1" : "M54 P0") : "") +
    " " + (volt != null ? "M3 S" + volt : "");

    ;

        }
    }
}


