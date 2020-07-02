using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.classes
{
    public class gcLine : MonoBehaviour
    {
        public void Set(gcLine Data)
        {
            linenr = Data.linenr;
            G = Data.G;
            X = Data.X;
            Y = Data.Y;
            Z = Data.Z;
            F = Data.F;
            I = Data.I;
            J = Data.J;
            K = Data.K;
            L = Data.L;
            M = Data.M;
            N = Data.N;
            P = Data.P;
            R = Data.R;
            S = Data.S;
            T = Data.T;
            M = Data.M;
            volt = Data.volt;
            AUX1 = Data.AUX1;
            travelPath = Data.travelPath;
        }

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
        public bool? travelPath;

        public override string ToString()
        {
            return "" + linenr +
                 (G != null ? " G" + G : "") +
                 (X != null ? " X" + X : "") +
                 (Y != null ? " Y" + Y : "") +
                 (Z != null ? " Z" + Z : "") +
                 (F != null ? " F" + F : "") +
                 (I != null ? " I" + I : "") +
                 (J != null ? " J" + J : "") +
                 (K != null ? " K" + K : "") +
                 (L != null ? " L" + L : "") +
                 (N != null ? " N" + N : "") +
                 (P != null ? " P" + P : "") +
                 (R != null ? " R" + R : "") +
                 (S != null ? " S" + S : "") +
                 (T != null ? " T" + T : "") +
                 (AUX1 != null? ((bool)AUX1 ? " M54 P1" : " M55 P1") : "") +
                 (volt != null? " M3 S" + volt : "");

        }
        public string ToEdingString(bool? _AUX1 = null, float? _Volt = null,float? _Feed = null)
        {
            return "" + linenr +
     (G != null ? " G" + G : "") +
     (X != null ? " X" + X : "") +
     (Z != null ? " Y" + -Y : "") +
     (Y != null ? " Z" + (Z == 0.00001f ? 0 : Z) : "") +
     (_Feed != null ? " F" + _Feed : "") +
     (I != null ? " I" + I : "") +
     (J != null ? " J" + J : "") +
     (K != null ? " K" + K : "") +
     (L != null ? " L" + L : "") +
     (M != null ? " M" + M : "") +
     (N != null ? " N" + N : "") +
     (P != null ? " P" + P : "") +
     (R != null ? " R" + R : "") +
     (S != null ? " S" + S : "") +
     (T != null ? " T" + T : "") +
     (_AUX1 != null ? ((bool)_AUX1  == true? " M54 P1" : " M55 P1") : "") +
     (_Volt != null ? " M3 S" + (float)_Volt : "");
        }
    }
}


