using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                " " + (T != null ? "T" + T : "");

        }
        public string ToEdingString()
        {
            return "" + linenr +
    " " + (G != null ? "G" + G : "") +
    " " + (X != null ? "X" + X : "") +
    " " + (Z != null ? "Y" + -Y : "") +
    " " + (Y != null ? "Z" + Z : "") +
    " " + (F != null ? "F" + F : "") +
    " " + (I != null ? "I" + I : "") +
    " " + (J != null ? "J" + J : "") +
    " " + (K != null ? "K" + K : "") +
    " " + (L != null ? "L" + L : "") +
    " " + (N != null ? "N" + N : "") +
    " " + (P != null ? "P" + P : "") +
    " " + (R != null ? "R" + R : "") +
    " " + (S != null ? "S" + S : "") +
    " " + (T != null ? "T" + T : "");

        }
    }
}


