Imports Gentle.Framework
Imports TvDatabase

Public Class MySettings

#Region "Members"
    Private Shared _LogFilePath As LogPath
    Private Shared _LogFileName As String = String.Empty
    Private Shared _MpDatabasePath As String = String.Empty
    Private Shared _MPThumbPath As String = String.Empty
    Private Shared _EpisodenScannerPath As String = String.Empty
    Private Shared _ClickfinderProgramGuideImportEnable As Boolean
    Private Shared _NewEpisodeString As String
    Private Shared _EpisodeExistsString As String

    Private Shared _TvSeriesEnabled As Boolean
    Private Shared _VideoEnabled As Boolean
    Private Shared _MovPicEnabled As Boolean
    Private Shared _useTheTvDb As Boolean
    Private Shared _TheTvDbLanguage As String

    Private Const _RegkeyMP As String = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server"

    Public Enum LogPath
        auto = 0
        Server = 1
        Client = 2
    End Enum
#End Region

#Region "Properties"
    Public Shared Property MpDatabasePath() As String
        Get
            Return _MpDatabasePath
            'layer.GetSetting("TvMovieMPDatabase", "C:\ProgramData\Team MediaPortal\MediaPortal\database").Value
        End Get
        Set(ByVal value As String)
            _MpDatabasePath = value
        End Set
    End Property
    Public Shared Property MpThumbPath() As String
        Get
            Return _MPThumbPath
            'layer.GetSetting("TvMovieMPDatabase", "C:\ProgramData\Team MediaPortal\MediaPortal\database").Value
        End Get
        Set(ByVal value As String)
            _MPThumbPath = value
        End Set
    End Property
    Public Shared Property EpisodenScannerPath() As String
        Get
            Return _EpisodenScannerPath
            'layer.GetSetting("TvMovieMPDatabase", "C:\ProgramData\Team MediaPortal\MediaPortal\database").Value
        End Get
        Set(ByVal value As String)
            _EpisodenScannerPath = value
        End Set
    End Property
    Public Shared Property LogFilePath() As LogPath
        Get
            Return _LogFilePath
        End Get
        Set(ByVal value As LogPath)
            _LogFilePath = value
        End Set
    End Property
    Public Shared Property LogFileName() As String
        Get
            If String.IsNullOrEmpty(_LogFileName) = True Then
                Return "enrichEPG.log"
            Else
                Return _LogFileName
            End If
        End Get
        Set(ByVal value As String)
            _LogFileName = value
        End Set
    End Property
    Public Shared Property ClickfinderProgramGuideImportEnable() As Boolean
        Get
            Return _ClickfinderProgramGuideImportEnable
        End Get
        Set(ByVal value As Boolean)
            _ClickfinderProgramGuideImportEnable = value
        End Set
    End Property

    Public Shared Property NewEpisodeString() As String
        Get
            Return _NewEpisodeString
        End Get
        Set(ByVal value As String)
            _NewEpisodeString = value
        End Set

    End Property
    Public Shared Property EpisodeExistsString() As String
        Get
            Return _EpisodeExistsString
        End Get
        Set(ByVal value As String)
            _EpisodeExistsString = value
        End Set
    End Property
    Public Shared Property TvSeriesEnabled() As Boolean
        Get
            Return _TvSeriesEnabled
        End Get
        Set(ByVal value As Boolean)
            _TvSeriesEnabled = value
        End Set
    End Property
    Public Shared Property VideoEnabled() As Boolean
        Get
            Return _VideoEnabled
        End Get
        Set(ByVal value As Boolean)
            _VideoEnabled = value
        End Set
    End Property
    Public Shared Property MovPicEnabled() As Boolean
        Get
            Return _MovPicEnabled
        End Get
        Set(ByVal value As Boolean)
            _MovPicEnabled = value
        End Set
    End Property
    Public Shared Property useTheTvDb() As Boolean
        Get
            Return _useTheTvDb
        End Get
        Set(ByVal value As Boolean)
            _useTheTvDb = value
        End Set
    End Property
    Public Shared Property TheTvDbLanguage() As String
        Get
            Return _TheTvDbLanguage
        End Get
        Set(ByVal value As String)
            _TheTvDbLanguage = value
        End Set
    End Property

    Public Shared ReadOnly Property ServerInstalled() As Boolean
        Get
            Return CBool(My.Computer.Registry.GetValue(_RegkeyMP, "MementoSection_SecServer", String.Empty))
        End Get
    End Property
    Public Shared ReadOnly Property TheTvDbCacheFolder() As String
        Get
            Return MySettings.LogFilePath & "\cache\enrichEPG"
        End Get
    End Property
#End Region

    Public Shared Sub SetSettings(ByVal MediaPortalDatabasePath As String, ByVal SeriesEnabled As Boolean, ByVal VideoEnabled As Boolean, ByVal MovPicEnabled As Boolean, ByVal LogFilePath As MySettings.LogPath, Optional ByVal LogFileName As String = "", Optional ByVal EpisodenSCPath As String = "", Optional ByVal NewEpisodeString As String = "New Episode", Optional ByVal EpisodeExistsString As String = "", Optional ByVal UseTheTvDb As Boolean = True, Optional ByVal TheTvDbLanguage As String = "de", Optional ByVal MediaPortalThumbsPath As String = "")
        MySettings.MpDatabasePath = MediaPortalDatabasePath
        MySettings.EpisodenScannerPath = EpisodenSCPath
        MySettings.TvSeriesEnabled = SeriesEnabled
        MySettings.VideoEnabled = VideoEnabled
        MySettings.MovPicEnabled = MovPicEnabled
        MySettings.LogFilePath = LogFilePath
        MySettings.LogFileName = LogFileName
        MySettings.useTheTvDb = UseTheTvDb
        MySettings.TheTvDbLanguage = TheTvDbLanguage
        MySettings.NewEpisodeString = NewEpisodeString
        MySettings.MpThumbPath = MediaPortalThumbsPath
        MySettings.EpisodeExistsString = EpisodeExistsString

        'Prüfen ob ClickfinderProgramGuide Import aktiviert ist
        Dim sb As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(Setting))
        sb.AddConstraint([Operator].Equals, "tag", "ClickfinderDataAvailable")
        Dim stmt As SqlStatement = sb.GetStatement(True)
        Dim _Result As IList(Of Setting) = ObjectFactory.GetCollection(GetType(Setting), stmt.Execute())

        If _Result.Count > 0 Then
            MySettings.ClickfinderProgramGuideImportEnable = True
        Else
            MySettings.ClickfinderProgramGuideImportEnable = False
        End If

        'Log Ausgabe: Settings enrichEPG
        MyLog.Info("enrichEPG Version: {0}", Helper.Version)
        MyLog.Info("Mediaportal Databases path: {0}", MySettings.MpDatabasePath)
        MyLog.Info("Mediaportal Thumb path: {0}", MySettings.MpThumbPath)
        MyLog.Info("EpisodenScanner path: {0}", MySettings.EpisodenScannerPath)
        MyLog.Info("Series Import enabled: {0}", MySettings.TvSeriesEnabled)
        MyLog.Info("Video Import enabled: {0}", MySettings.VideoEnabled)
        MyLog.Info("MovPic Import enabled: {0}", MySettings.MovPicEnabled)
        MyLog.Info("Use TheTvDb.com: {0}", MySettings.useTheTvDb)
        MyLog.Info("TvServer installed: {0}", MySettings.ServerInstalled)
        MyLog.Info("log path: {0}", MySettings.LogFilePath)
        MyLog.Info("log file: {0}", MySettings.LogFileName)
        MyLog.Info("TheTvDb.com language: {0}", MySettings.TheTvDbLanguage)
        MyLog.Info("Clickfinder PG Import enabled: {0}", MySettings.ClickfinderProgramGuideImportEnable)
        MyLog.Info("")

    End Sub

End Class
