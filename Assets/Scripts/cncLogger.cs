/* MassiveCNC Playground. An Unity3D based framework for controller CNC-based machines.
    Created and altered by Max Malherbe.
    
    Originally created by Sven Hasemann, altered and rewritten by me.

    Origibal Project : GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class cncLogger
{
    // Start is called before the first frame update
   public static void Log(object txt)
    {
        Debug.Log(txt);


    }

    public static void RealTimeLog(object text)
    {
        Debug.Log(text);
    }
    public static void Warn(object text)
    {
        throw new System.NullReferenceException();
    }
    public static void ShowError(object text)
    {
        throw new System.NullReferenceException();
    }
}
