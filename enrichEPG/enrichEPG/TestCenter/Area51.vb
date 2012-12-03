Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml

Imports TvControl
Imports MediaPortal.UserInterface.Controls
Imports MediaPortal.Configuration
Imports TvEngine
Imports TvEngine.Events
Imports SetupTv
Imports System.Threading
Imports TvDatabase
Imports enrichEPG
Imports Databases

Namespace SetupTv.Sections

    <CLSCompliant(False)> _
    Public Class NewTvServerPluginConfig

        Inherits SectionSettings

        'define window functions
        <DllImport("User32.dll")> _
        Public Shared Function SetForegroundWindow(ByVal hWnd As Integer) As Int32
        End Function
        <DllImport("user32.dll")> _
        Public Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
        End Function

#Region "Constructor"

        Public Sub New()
            InitializeComponent()
        End Sub
#End Region

#Region "SetupTv.SectionSettings"

        Public Overrides Sub OnSectionActivated()
            MyBase.OnSectionActivated()

        End Sub

        Public Overrides Sub OnSectionDeActivated()
            MyBase.OnSectionDeActivated()
        End Sub
#End Region

        Private Sub Button1Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
            Try
                MyTvSeries.SetMPdatabasePath("C:\ProgramData\Team MediaPortal\MediaPortal\database")

                MsgBox(MyTvSeries.MyEpisode.Search(79349, "Jagdinstpinkt").EpisodeName)

                MsgBox(MyTvSeries.Search("De45 xter").Episode("Jagdi jnstinkt").EpisodeName)
            Catch ex As Exception
                MyLog.Error(ex.Message)
            End Try
        End Sub

        Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

            enrichEPG.MyLog.BackupLogFiles()

            Dim _tvbLayer As New TvBusinessLayer
            Dim _enrichEPG As New enrichEPG.EnrichEPG(_tvbLayer.GetSetting("TvMovieMPDatabase", "C:\ProgramData\Team MediaPortal\MediaPortal\database").Value, _
                    CBool(_tvbLayer.GetSetting("TvMovieImportTvSeriesInfos").Value), _
                    False, _
                    False, _
                    Date.Now, _
                    enrichEPG.EnrichEPG.LogPath.Server, _
                    "", , , _
                    "enrichTesting.log", True)
            _enrichEPG.start()

        End Sub
    End Class
End Namespace

