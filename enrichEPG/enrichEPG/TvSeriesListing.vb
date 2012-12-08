'Imports System.Collections
'Imports System.Collections.Generic
'Imports System.ComponentModel
'Imports System.Diagnostics
'Imports System.Drawing
'Imports System.Globalization
'Imports System.IO
'Imports System.Net
'Imports System.Net.Sockets
'Imports System.Runtime.InteropServices
'Imports System.Text
'Imports System.Windows.Forms
'Imports System.Xml

'Imports Gentle.Framework

'Imports TvControl
'Imports MediaPortal.UserInterface.Controls
'Imports MediaPortal.Configuration
'Imports TvEngine
'Imports TvEngine.Events
'Imports SetupTv
'Imports System.Threading
'Imports TvDatabase
'Imports enrichEPG.Database
'Imports Databases
'Imports enrichEPG
'Imports enrichEPG.IdentifySeries
'Imports enrichEPG.TvDatabase

Public Class TvSeriesListing
    Public Shared isNewEpisodeMapping As Boolean

    Private Sub TvSeriesListing_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If TvSeriesListing.isNewEpisodeMapping = False Then
            seriesManagement.Label_idseries = DGVSeriesList.Rows(DGVSeriesList.CurrentCell.RowIndex).Cells(0).Value.ToString
            seriesManagement.Label_TvSeriesName = DGVSeriesList.Rows(DGVSeriesList.CurrentCell.RowIndex).Cells(1).Value.ToString
        Else
            seriesManagement.Label_idepisode = DGVSeriesList.Rows(DGVSeriesList.CurrentCell.RowIndex).Cells(0).Value.ToString
            seriesManagement.Label_TvSeriesEpisodeName = DGVSeriesList.Rows(DGVSeriesList.CurrentCell.RowIndex).Cells(1).Value.ToString

        End If

        Me.Close()
    End Sub
End Class