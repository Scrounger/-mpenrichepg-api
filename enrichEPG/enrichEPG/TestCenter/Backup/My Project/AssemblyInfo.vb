' 
' *	Copyright (C) 2005-2008 Team MediaPortal
' *	http://www.team-mediaportal.com
' *
' *  This Program is free software; you can redistribute it and/or modify
' *  it under the terms of the GNU General Public License as published by
' *  the Free Software Foundation; either version 2, or (at your option)
' *  any later version.
' *   
' *  This Program is distributed in the hope that it will be useful,
' *  but WITHOUT ANY WARRANTY; without even the implied warranty of
' *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
' *  GNU General Public License for more details.
' *   
' *  You should have received a copy of the GNU General Public License
' *  along with GNU Make; see the file COPYING.  If not, write to
' *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
' *  http://www.gnu.org/copyleft/gpl.html
' *  
' 


Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports MediaPortal.Common.Utils

' Allgemeine Informationen über eine Assembly werden über die folgenden 
' Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
' die mit einer Assembly verknüpft sind.
<Assembly: AssemblyTitle("Clickfinder EPG Import")> 
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyConfiguration("")> 
<Assembly: AssemblyCompany("Team MediaPortal")> 
<Assembly: AssemblyProduct("MyTVServerPlugins")> 
<Assembly: AssemblyCopyright("Copyright ©  2008-2011")> 
<Assembly: AssemblyTrademark("")> 
<Assembly: AssemblyCulture("")> 

' Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
' für COM-Komponenten. Wenn Sie auf einen Typ in dieser Assembly von 
' COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
<Assembly: ComVisible(False)> 

' Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
<Assembly: Guid("740517dd-59dd-44dd-969a-d5d9fcd91905")> 

' Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
'
'      Hauptversion
'      Nebenversion 
'      Buildnummer
'      Revision
'
' Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
' übernehmen, indem Sie "*" eingeben:
' [assembly: AssemblyVersion("1.0.*")]
<Assembly: AssemblyVersion("1.2.0.8")> 
<Assembly: AssemblyFileVersion("1.2.0.8")> 

'new
'[assembly: CompatibleVersion("Own")]
<Assembly: UsesSubsystem("TVE")> 
<Assembly: CompatibleVersion("1.1.6.27652")> 

<Assembly: CLSCompliant(True)> 
