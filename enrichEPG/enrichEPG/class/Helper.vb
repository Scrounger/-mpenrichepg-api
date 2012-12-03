Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Imports Databases
Imports Gentle.Framework
Imports Gentle.Common
Imports MediaPortal.Database
Imports SQLite.NET
Imports enrichEPG.MyLog

Imports TvDatabase
Imports TvLibrary.Interfaces
Imports TvEngine.PowerScheduler.Interfaces

Imports enrichEPG.TvDatabase

Public Class Helper
    Public Shared Function allowedSigns(ByVal expression As String) As String
        Return Replace(System.Text.RegularExpressions.Regex.Replace(expression, "[\?]", "_"), "'", "''")
    End Function
    Public Shared Sub DeleteCPGcache(ByVal path As String, ByVal CachedSeries As ArrayList)

        Try
            Dim Counter As Integer = 0
            Dim di As New DirectoryInfo(path)
            Dim _Series As New TVSeriesDB

            For Each subdi As DirectoryInfo In di.GetDirectories
                Dim _idSeries As Integer = CInt(subdi.Name)

                'wenn in TvSeries DB, dann löschen
                If _Series.SeriesFoundbySeriesId(_idSeries) = True Then
                    Directory.Delete(subdi.FullName, True)
                    Counter = Counter + 1
                End If

                'wenn nicht im Cache (EpisodenScanner), dann löschen
                If Not CachedSeries.Contains(_idSeries) Then
                    Directory.Delete(subdi.FullName, True)
                    Counter = Counter + 1
                End If

            Next

            _Series.Dispose()

            If Counter > 0 Then
                MyLog.Info("")
                MyLog.Info("enrichEPG: [DeleteCPGcache]: cleared ({0} series at {1})", Counter, path)
            Else
                MyLog.Info("")
                MyLog.Info("enrichEPG: [DeleteCPGcache]: nothing to do")
            End If
        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [DeleteCPGcache]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try
    End Sub
    Public Shared ReadOnly Property Version() As String
        Get
            Dim oAssembly As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName

            ' Versionsnummer
            Return oAssembly.Version.ToString()
        End Get
    End Property
    Friend Shared Function MySqlDate(ByVal Datum As Date) As String
        Try
            If Gentle.Framework.Broker.ProviderName = "MySQL" Then
                Return "'" & Datum.Year & "-" & Format(Datum.Month, "00") & "-" & Format(Datum.Day, "00") & " " & Format(Datum.Hour, "00") & ":" & Format(Datum.Minute, "00") & ":00'"
            Else
                Return "'" & Datum.Year & Format(Datum.Month, "00") & Format(Datum.Day, "00") & " " & Format(Datum.Hour, "00") & ":" & Format(Datum.Minute, "00") & ":" & Format(Datum.Second, "00") & "'"
            End If

        Catch ex As Exception
            MyLog.Error("[Helper]: [MySqlDate]: exception err: {0} stack: {1}", ex.Message, ex.StackTrace)
            Return ""
        End Try
    End Function

End Class
