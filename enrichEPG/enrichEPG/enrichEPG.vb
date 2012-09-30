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

Imports TvDatabase
Imports TvLibrary.Interfaces
Imports TvEngine.PowerScheduler.Interfaces

Imports enrichEPG.TvDatabase
Imports enrichEPG.Helper

Public Class EnrichEPG

#Region "Members"
    Private Shared _MpDatabasePath As String = String.Empty
    Private Shared _EpisodenScannerPath As String = String.Empty
    Private Shared _LogFilePath As LogPath
    Private Shared _LogFileName As String = String.Empty
    Private Shared _MPThumbPath As String = String.Empty
    Private Shared _ClickfinderProgramGuideImportEnable As Boolean

    Private Shared _SeriesEnabled As Boolean
    Private Shared _VideoEnabled As Boolean
    Private Shared _MovPicEnabled As Boolean
    Private Shared _useTheTvDb As Boolean
    Private Shared _TheTvDbLanguage As String
    Private Shared _NewEpisodeString As String
    Private Shared _EpisodeExistsString As String


    Private Shared _ImportStartTime As Date

    Private Shared _IdentifiedPrograms As New ArrayList
    Private Shared _ScheduledDummyRecordingList As New ArrayList


    Public Shared MyTVDBen As New clsTheTVdb("en")
    Public Shared MyTVDBlang As New clsTheTVdb("de")
    

    Private Const _RegkeyMP As String = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server"

    Public Enum LogPath
        auto = 0
        Server = 1
        Client = 2
    End Enum

#End Region

#Region "Properties"
    Private Shared ReadOnly Property ServerInstalled() As Boolean
        Get
            Return CBool(My.Computer.Registry.GetValue(_RegkeyMP, "MementoSection_SecServer", String.Empty))
        End Get
    End Property
    Public Shared ReadOnly Property MpDatabasePath() As String
        Get
            Return _MpDatabasePath
            'layer.GetSetting("TvMovieMPDatabase", "C:\ProgramData\Team MediaPortal\MediaPortal\database").Value
        End Get
    End Property
    Public Shared ReadOnly Property MpThumbPath() As String
        Get
            Return _MPThumbPath
            'layer.GetSetting("TvMovieMPDatabase", "C:\ProgramData\Team MediaPortal\MediaPortal\database").Value
        End Get
    End Property
    Public Shared ReadOnly Property EpisodenScannerPath() As String
        Get
            Return _EpisodenScannerPath
            'layer.GetSetting("TvMovieMPDatabase", "C:\ProgramData\Team MediaPortal\MediaPortal\database").Value
        End Get
    End Property
    Public Shared ReadOnly Property MyLogFilePath()
        Get
            Dim _LogPathClient As String = [String].Format("{0}\Team MediaPortal\MediaPortal", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            Dim _LogPathServer As String = [String].Format("{0}\Team MediaPortal\MediaPortal TV Server", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))

            Select Case _LogFilePath
                Case Is = LogPath.Client
                    Return _LogPathClient
                Case Is = LogPath.Server
                    Return _LogPathServer
                Case Else
                    If EnrichEPG.ServerInstalled = True Then
                        Return _LogPathServer
                    Else
                        Return _LogPathClient
                    End If
            End Select
        End Get
    End Property
    Public Shared ReadOnly Property MyLogFileName()
        Get
            If String.IsNullOrEmpty(_LogFileName) = True Then
                Return "enrichEPG.log"
            Else
                Return _LogFileName
            End If
        End Get
    End Property
    Public Shared Property ClickfinderProgramGuideImportEnable() As Boolean
        Get
            Return _ClickfinderProgramGuideImportEnable
        End Get
        Set(ByVal value As Boolean)
            _ClickfinderProgramGuideImportEnable = value
        End Set
    End Property
    Public Shared ReadOnly Property TheTvDbCacheFolder() As String
        Get
            Return MyLogFilePath & "\cache\enrichEPG"
        End Get
    End Property
    Public Shared ReadOnly Property NewEpisodeString() As String
        Get
            Return _NewEpisodeString
        End Get
    End Property
    Public Shared ReadOnly Property EpisodeExistsString() As String
        Get
            Return _EpisodeExistsString
        End Get
    End Property

#End Region

#Region "Constructors"
    Public Sub New(ByVal MediaPortalDatabasePath As String, ByVal SeriesEnabled As Boolean, ByVal VideoEnabled As Boolean, ByVal MovPicEnabled As Boolean, ByVal ImportStartTime As Date, ByVal LogFilePath As LogPath, Optional ByVal EpisodenSCPath As String = "", Optional ByVal NewEpisodeString As String = "New Episode", Optional ByVal EpisodeExistsString As String = "", Optional ByVal LogFileName As String = "", Optional ByVal UseTheTvDb As Boolean = True, Optional ByVal TheTvDbLanguage As String = "de", Optional ByVal MediaPortalThumbsPath As String = "")
        _MpDatabasePath = MediaPortalDatabasePath
        _EpisodenScannerPath = EpisodenSCPath
        _SeriesEnabled = SeriesEnabled
        _VideoEnabled = VideoEnabled
        _MovPicEnabled = MovPicEnabled
        _LogFilePath = LogFilePath
        _LogFileName = LogFileName
        _ImportStartTime = ImportStartTime
        _useTheTvDb = UseTheTvDb
        _TheTvDbLanguage = TheTvDbLanguage
        _NewEpisodeString = NewEpisodeString
        _MPThumbPath = MediaPortalThumbsPath
        _EpisodeExistsString = EpisodeExistsString
    End Sub

#End Region

#Region "Functions"

    Public Sub start()
        'Prüfen ob ClickfinderProgramGuide Import aktiviert ist
        Dim sb As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(Setting))
        sb.AddConstraint([Operator].Equals, "tag", "ClickfinderDataAvailable")
        Dim stmt As SqlStatement = sb.GetStatement(True)
        Dim _Result As IList(Of Setting) = ObjectFactory.GetCollection(GetType(Setting), stmt.Execute())

        If _Result.Count > 0 Then
            ClickfinderProgramGuideImportEnable = True
        Else
            ClickfinderProgramGuideImportEnable = False
        End If

        Dim oAssembly As System.Reflection.AssemblyName = _
    System.Reflection.Assembly.GetExecutingAssembly().GetName

        ' Versionsnummer
        Dim sVersion As String = oAssembly.Version.ToString()

        'Log Ausgabe: Settings enrichEPG
        MyLog.Info("enrichEPG Version: {0}", sVersion)
        MyLog.Info("Server installed: {0}", ServerInstalled)
        MyLog.Info("Mediaportal Databases path: {0}", MpDatabasePath)
        MyLog.Info("Mediaportal Thumb path: {0}", MpThumbPath)
        MyLog.Info("EpisodenScanner path: {0}", EpisodenScannerPath)
        MyLog.Info("Series Import enabled: {0}", _SeriesEnabled)
        MyLog.Info("Video Import enabled: {0}", _VideoEnabled)
        MyLog.Info("MovPic Import enabled: {0}", _MovPicEnabled)
        MyLog.Info("Use TheTvDb.com: {0}", _useTheTvDb)
        MyLog.Info("log path: {0}", _LogFilePath)
        MyLog.Info("TheTvDb.com language: {0}", _TheTvDbLanguage)
        MyLog.Info("Clickfinder PG Import enabled: {0}", ClickfinderProgramGuideImportEnable)
        MyLog.Info("")
        Dim _lastDummyScheduledRecordings As Setting = Nothing
        Try
            Dim key As New Key(GetType(Setting), True, "tag", "enrichEPGlastScheduleRecordings")
            _lastDummyScheduledRecordings = Setting.Retrieve(key)
            MyLog.Info("Last scheduled recording dummys: {0}", _lastDummyScheduledRecordings.Value)
        Catch ex As Exception
            _lastDummyScheduledRecordings = New Setting("enrichEPGlastScheduleRecordings", "")
            _lastDummyScheduledRecordings.Persist()
        End Try

        If Not _lastDummyScheduledRecordings.Value = String.Empty Then
            Dim _DummySchedulesList As New ArrayList(Split(_lastDummyScheduledRecordings.Value, "|"))
            If _DummySchedulesList.Count > 0 Then
                For i = 0 To _DummySchedulesList.Count - 1
                    Try
                        Dim _DummySchedule As Schedule = Schedule.Retrieve(_DummySchedulesList.Item(i))
                        MyLog.Info("dummy scheduled recording from last run deleted: {0} ({1})", _DummySchedule.ProgramName, _DummySchedule.IdSchedule)
                        _DummySchedule.Remove()

                    Catch ex As Exception
                        MyLog.Info("dummy scheduled recording not exists any more: {0}", _DummySchedulesList.Item(i))
                    End Try
                Next
            End If
            _lastDummyScheduledRecordings.Value = ""
            _lastDummyScheduledRecordings.Persist()
        End If
        MyLog.Info("")
        MyLog.Info("")


        IO.Directory.CreateDirectory(_MPThumbPath & "\Fan Art\Clickfinder ProgramGuide")
        IO.Directory.CreateDirectory(_MPThumbPath & "\MPTVSeriesBanners\Clickfinder ProgramGuide")

        MyTVDBen.tvLanguage = "en"
        MyTVDBlang.tvLanguage = _TheTvDbLanguage

        If _SeriesEnabled = True Then
            GetSeriesInfos()
        End If

        If _MovPicEnabled = True Then
            GetMovingPicturesInfos()
        End If

        If _VideoEnabled = True Then
            GetVideoDatabaseInfos()
        End If

        If _SeriesEnabled = True And String.IsNullOrEmpty(EpisodenScannerPath) = False Then
            RunEpisodenScanner()
        End If

        MyTVDBlang.TheTVdbHandler.ClearCache()
        MyTVDBen.TheTVdbHandler.ClearCache()

        MyTVDBlang.TheTVdbHandler.CloseCache()
        MyTVDBen.TheTVdbHandler.CloseCache()

    End Sub

    Private Sub GetSeriesInfos()
        Try
            MyLog.Info("enrichEPG: [GetSeriesInfos]: start import")

            Dim _Counter As Integer = 0
            Dim _CounterFound As Integer = 0
            Dim _CounterNewEpisode As Integer = 0
            Dim _SQLString As String = String.Empty
            Dim _SeriesImportStartTime As Date = Date.Now
            Dim _NextSeries As Boolean = True
            Dim _lastEpisodeName As String = String.Empty

            IdentifySeries.UpdateEpgEpisodeSeriesNameCounter = 0

            'Alle Serien aus DB laden
            Dim _TvSeriesDB As New TVSeriesDB
            _TvSeriesDB.LoadAllSeries()

            IdentifySeries.SeriesEN = MyTVDBen.TheTVdbHandler.GetSeries(_TvSeriesDB(0).SeriesID, MyTVDBen.DBLanguage, True, False, False)
            IdentifySeries.SeriesLang = MyTVDBlang.TheTVdbHandler.GetSeries(_TvSeriesDB(0).SeriesID, MyTVDBlang.DBLanguage, True, False, False)

            'Nach allen Serien im EPG suchen
            For i As Integer = 0 To _TvSeriesDB.CountSeries - 1
                Dim _logScheduldedRecording As String = String.Empty
                Dim _ScheduldedDummyRecording As Program
                Dim _EpisodeIdentified As Boolean = True

                Try
                    Dim _EpisodeFoundCounter As Integer = 0

                    Dim _Result As New ArrayList

                    'Zunächst nach alle Serien mit SerienName aus TvSeries DB suchen
                    _SQLString = "Select idProgram from program WHERE title LIKE '" & Helper.allowedSigns(_TvSeriesDB(i).SeriesName) & "' ORDER BY episodeName"
                    _Result.AddRange(Broker.Execute(_SQLString).TransposeToFieldList("idProgram", False))

                    MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0} ({1}): {2} episodes found", _TvSeriesDB(i).SeriesName, _TvSeriesDB(i).SeriesID, _Result.Count)

                    'SeriesMappingNamen laden, sofern vorhanden
                    Try
                        Dim _TvMovieSeriesMapping As TvMovieSeriesMapping = TvMovieSeriesMapping.Retrieve(_TvSeriesDB(i).SeriesID)

                        If Not String.IsNullOrEmpty(_TvMovieSeriesMapping.EpgTitle) Then
                            Dim _MappedSeriesNames As New ArrayList(Split(_TvMovieSeriesMapping.EpgTitle, "|"))
                            MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: manuel mapping found: {1}", _TvSeriesDB(i).SeriesName, Replace(_TvMovieSeriesMapping.EpgTitle, "|", ", "))

                            For z As Integer = 0 To _MappedSeriesNames.Count - 1
                                Dim _MappedSeries As New ArrayList
                                _SQLString = "Select idProgram from program WHERE title LIKE '" & Helper.allowedSigns(CStr(_MappedSeriesNames.Item(z))) & "' ORDER BY episodeName"
                                _MappedSeries.AddRange(Broker.Execute(_SQLString).TransposeToFieldList("idProgram", False))

                                If _MappedSeries.Count > 0 Then
                                    _Result.AddRange(_MappedSeries)
                                    MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: {1} episodes found", _MappedSeriesNames.Item(z), _MappedSeries.Count)
                                End If

                            Next
                        End If

                    Catch SeriesMappingEx As Exception

                    End Try

                    'Alle im EPG gefundenen episoden der Serie durchlaufen
                    For d As Integer = 0 To _Result.Count - 1
                        Dim _program As Program = Program.Retrieve(CInt(_Result.Item(d)))
                        Dim _episodeFound As Boolean = False
                        Dim _logNewEpisode As Boolean = False

                        Try

                            'Episode in TvSeries DB gefunden
                            If _TvSeriesDB.EpisodeFound(_TvSeriesDB(i).SeriesID, Helper.allowedSigns(_program.EpisodeName)) = True Then
                                _episodeFound = True

                                'Daten im EPG (program) updaten
                                IdentifySeries.UpdateEpgEpisode(_program, _TvSeriesDB, _TvSeriesDB(i).SeriesName)

                                'Neue Episode -> im EPG Describtion kennzeichnen + Zähler hoch
                                IdentifySeries.MarkEpgEpisodeAsNew(_program, _TvSeriesDB.EpisodeExistLocal)
                                If _TvSeriesDB.EpisodeExistLocal = False Then
                                    _logNewEpisode = True
                                    _CounterNewEpisode = _CounterNewEpisode + 1
                                End If
                               
                                'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                                IdentifySeries.UpdateTvMovieProgram(_program, _TvSeriesDB, i, _episodeFound)

                                _EpisodeFoundCounter = _EpisodeFoundCounter + 1

                                If Not _lastEpisodeName = _program.EpisodeName And _episodeFound = True Then
                                    MyLog.Info("enrichEPG: [GetSeriesInfos]: TvSeriesDB: S{0}E{1} - {2} (newEpisode: {3}{4})", _
                                  _TvSeriesDB.SeasonIndex, _TvSeriesDB.EpisodeIndex, _program.EpisodeName, _logNewEpisode, TVSeriesDB.logLevenstein)
                                End If

                            Else
                                'Episode nicht in TvSeries DB gefunden

                                'auf TheTvDb.com nach episode suchen (language: lang)
                                If _useTheTvDb = True Then

                                    Try
                                        If _NextSeries = True Then
                                            'MyLog.Info("enrichEPG: [GetSeriesInfos]: Daten von TheTvDb.com holen: {0} (idSeries: {1})", _TvSeriesDB(i).SeriesName, _TvSeriesDB(i).SeriesID)
                                            _NextSeries = False
                                            IdentifySeries.SeriesEN = MyTVDBen.TheTVdbHandler.GetSeries(_TvSeriesDB(i).SeriesID, MyTVDBen.DBLanguage, True, False, False)
                                            IdentifySeries.SeriesLang = MyTVDBlang.TheTVdbHandler.GetSeries(_TvSeriesDB(i).SeriesID, MyTVDBlang.DBLanguage, True, False, False)
                                        End If

                                        _episodeFound = IdentifySeries.TheTvDbEpisodeIdentify(_program)

                                        'Daten schreiben wenn gefunden
                                        If _episodeFound = True Then
                                            _TvSeriesDB.LoadEpisodeBySeriesID(_TvSeriesDB(i).SeriesID, IdentifySeries.IdentifiedEpisode.SeasonNumber, IdentifySeries.IdentifiedEpisode.EpisodeNumber)

                                            'Daten im EPG (program) updaten
                                            IdentifySeries.UpdateEpgEpisode(_program, _TvSeriesDB, _TvSeriesDB(i).SeriesName)

                                            'Neue Episode -> im EPG Describtion kennzeichnen
                                            IdentifySeries.MarkEpgEpisodeAsNew(_program, _TvSeriesDB.EpisodeExistLocal)
                                            If _TvSeriesDB.EpisodeExistLocal = False Then
                                                _logNewEpisode = True
                                                _CounterNewEpisode = _CounterNewEpisode + 1
                                            End If

                                            'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                                            IdentifySeries.UpdateTvMovieProgram(_program, _TvSeriesDB, i, _episodeFound)

                                            _EpisodeFoundCounter = _EpisodeFoundCounter + 1

                                        End If

                                        If Not _lastEpisodeName = _program.EpisodeName And _episodeFound = True Then
                                            MyLog.Info("enrichEPG: [GetSeriesInfos]: TheTvDb.com ({0}): S{1}E{2} - {3} (newEpisode: {4})", IdentifySeries.IdentifiedEpisode.Language.Abbriviation, _
                                          IdentifySeries.IdentifiedEpisode.SeasonNumber, IdentifySeries.IdentifiedEpisode.EpisodeNumber, _program.EpisodeName, _logNewEpisode)
                                        End If

                                    Catch exTheTvDB As Exception
                                        MyLog.[Error]("enrichEPG: [GetSeriesInfos]: Error in TheTvDb.com fkt.")
                                        MyLog.[Error]("enrichEPG: [GetSeriesInfos]: exception err:{0} stack:{1}", exTheTvDB.Message, exTheTvDB.StackTrace)
                                    End Try
                                End If
                            End If

                            'Episode identifiziert -> idProgram speichern, benötigt damit CheckEpisodenscanner nicht ncohmal ausgeführt wird
                            If _episodeFound = True Then
                                _IdentifiedPrograms.Add(_program.IdProgram)
                            End If

                            'Episode letztendlich nicht identfiziert -> als neu markieren
                            If _episodeFound = False Then

                                _program.SeriesNum = String.Empty
                                _program.EpisodeNum = String.Empty

                                'EPG SerienName schreiben
                                IdentifySeries.UpdateEpgEpisodeSeriesName(_program, _TvSeriesDB(i).SeriesName)

                                IdentifySeries.MarkEpgEpisodeAsNew(_program, _TvSeriesDB.EpisodeExistLocal)

                                If _TvSeriesDB.EpisodeExistLocal = False Then
                                    _CounterNewEpisode = _CounterNewEpisode + 1
                                End If

                                'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                                IdentifySeries.UpdateTvMovieProgram(_program, _TvSeriesDB, i, _episodeFound)

                                If Not _lastEpisodeName = _program.EpisodeName Then
                                    MyLog.Warn("enrichEPG: [GetSeriesInfos]: {0} ({1}, {2}), episode: {3} - not identified -> marked as New Episode", _TvSeriesDB(i).SeriesName, _TvSeriesDB(i).SeriesID, _program.ReferencedChannel.DisplayName, _program.EpisodeName)
                                    _EpisodeIdentified = False
                                End If
                            End If

                            _ScheduldedDummyRecording = _program
                            _lastEpisodeName = _program.EpisodeName

                        Catch ex As Exception
                            MyLog.[Error]("enrichEPG: [GetSeriesInfos]: title:{0} idchannel:{1} startTime: {2} episodeName: {3}", _program.Title, _program.ReferencedChannel.DisplayName, _program.StartTime, _program.EpisodeName)
                            MyLog.[Error]("enrichEPG: [GetSeriesInfos]: Loop :Result exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                        End Try
                    Next

                    'nicht identifiziert -> Dummy schedule anlegen für EpisodenScanner
                    If _EpisodeIdentified = False And String.IsNullOrEmpty(EpisodenScannerPath) = False Then

                        Dim sb As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(Schedule))
                        sb.AddConstraint([Operator].Equals, "programName", _ScheduldedDummyRecording.Title)
                        sb.SetRowLimit(1)
                        Dim stmt As SqlStatement = sb.GetStatement(True)
                        Dim _Schedule As IList(Of Schedule) = ObjectFactory.GetCollection(GetType(Schedule), stmt.Execute())

                        If _Schedule.Count > 0 Then
                            _logScheduldedRecording = ", scheduled recording exist"
                        Else
                            'Episode am Ende des EPG Zeitraums als Dummy Recording verwenden, damit EpisodenScanner danach sucht
                            Dim sb2 As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(Program))
                            sb2.AddConstraint([Operator].Equals, "title", _ScheduldedDummyRecording.Title)
                            sb2.AddOrderByField(False, "startTime")
                            sb2.SetRowLimit(1)
                            Dim stmt2 As SqlStatement = sb2.GetStatement(True)
                            Dim _DummyProgram As IList(Of Program) = ObjectFactory.GetCollection(GetType(Program), stmt2.Execute())

                            If _DummyProgram.Count > 0 Then

                                Dim _dummy As Schedule = New Schedule(_DummyProgram(0).IdChannel, _DummyProgram(0).Title, _DummyProgram(0).StartTime, _DummyProgram(0).EndTime)
                                _dummy.Persist()
                                _ScheduledDummyRecordingList.Add(_dummy.IdSchedule)

                                Dim key As New Key(GetType(Setting), True, "tag", "enrichEPGlastScheduleRecordings")
                                Dim _lastDummyScheduledRecordings As Setting = Setting.Retrieve(key)

                                If String.IsNullOrEmpty(_lastDummyScheduledRecordings.Value) Then
                                    _lastDummyScheduledRecordings.Value = _dummy.IdSchedule
                                    _lastDummyScheduledRecordings.Persist()
                                Else
                                    _lastDummyScheduledRecordings.Value = _lastDummyScheduledRecordings.Value & "|" & _dummy.IdSchedule
                                    _lastDummyScheduledRecordings.Persist()
                                End If

                                _logScheduldedRecording = String.Format(", scheduled recording dummy created: {0} ({1}), {2}", _DummyProgram(0).Title, _dummy.IdSchedule, _DummyProgram(0).StartTime)
                            End If
                        End If

                    End If


                    MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: {1}/{2} episodes identified (programs renamed {3}{4})", _TvSeriesDB(i).SeriesName, _EpisodeFoundCounter, _Result.Count, IdentifySeries.UpdateEpgEpisodeSeriesNameCounter, _logScheduldedRecording)
                    _CounterFound = _CounterFound + _EpisodeFoundCounter
                    _Counter = _Counter + _Result.Count
                    _NextSeries = True
                    IdentifySeries.UpdateEpgEpisodeSeriesNameCounter = 0
                    MyLog.[Info]("")
                Catch ex As Exception
                    MyLog.[Error]("enrichEPG: [GetSeriesInfos]: Loop exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                End Try
            Next

            _TvSeriesDB.Dispose()

            MyLog.[Info]("enrichEPG: [GetSeriesInfos]: Summary: Infos for {0}/{1} episodes found, {2} new episodes identify", _CounterFound, _Counter, _CounterNewEpisode)
            MyLog.Info("enrichEPG: [GetSeriesInfos]: Import duration: {0}", (Date.Now - _SeriesImportStartTime).Minutes & "min " & (Date.Now - _SeriesImportStartTime).Seconds & "s")
            MyLog.Info("")
            MyLog.Info("")

        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [GetSeriesInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try

    End Sub

    Private Sub RunEpisodenScanner()
        If File.Exists(EpisodenScannerPath & "\episodescanner.exe") = True Then

            Dim StartTime As Date = Date.Now

            Dim App As New Process()
            App.StartInfo.FileName = EpisodenScannerPath & "\episodescanner.exe"
            App.StartInfo.WindowStyle = ProcessWindowStyle.Normal
            App.Start()

            MyLog.Debug("enrichEPG: [RunEpisodenScanner]: {0} started", EpisodenScannerPath & "\episodescanner.exe")

            MyLog.Debug("enrichEPG: [RunEpisodenScanner]: Application: Wait for exit")
            App.WaitForExit()
            MyLog.Debug("enrichEPG: [RunEpisodenScanner]: EpisodenScanner runtime: {0}", (Date.Now - StartTime).Minutes & "min " & (Date.Now - StartTime).Seconds & "s")
            CheckEpisodenscannerImports()
            MyLog.Info("enrichEPG: Import duration: {0}", (Date.Now - _ImportStartTime).Minutes & "min " & (Date.Now - _ImportStartTime).Seconds & "s")
        Else
            MyLog.Error("enrichEPG: [RunEpisodenScanner]: Error - Episodenscanner not found")
        End If
        MyLog.Debug("")
    End Sub

    Private Sub CheckEpisodenscannerImports()

        MyLog.[Info]("enrichEPG: [EScannerImport]: Process start")

        Dim _lastSeriesName As String = String.Empty
        Dim _lastEpisodeName As String = String.Empty

        Dim _Counter As Integer = 0
        Dim _SeriesDummyID As Integer = 0
        Dim _idSeries As Integer = 0

        Dim _CacheSeries As New ArrayList

        Try

            'Serie suchen und in Liste packen
            'Dim _SearchSeriesResult As List(Of TvdbLib.Data.TvdbSearchResult)
            

            'Alle Programme mit EpisodenNummer laden
            Dim sb As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(Program))
            sb.AddConstraint([Operator].GreaterThan, "episodeNum", "")
            sb.AddOrderByField(True, "title")
            sb.AddOrderByField(True, "SeriesNum")
            sb.AddOrderByField(True, "episodeNum")
            Dim stmt As SqlStatement = sb.GetStatement(True)
            Dim _Result As IList(Of Program) = ObjectFactory.GetCollection(GetType(Program), stmt.Execute())

            If _Result.Count > 0 Then

                'gefunden durchlaufen
                For i As Integer = 0 To _Result.Count - 1

                    Try

                        'Prüfen ob die Serie schon mit der GetSeriesInfos fkt. identifiziert wurde, dann nächster Eintrag
                        If _IdentifiedPrograms.Contains(_Result(i).IdProgram) = True Then
                            Continue For
                        End If

                        Dim _SqlString As String = String.Empty
                        Dim _SeriesName As String = String.Empty
                        Dim _SeriesMappingResult As New ArrayList
                        Dim _logSeriesFound As Boolean = False
                        Dim _logNewEpisode As Boolean = False

                        'Prüfen ob EPG-SerienName in TvSeries DB gefunden wird, wenn nicht, schauen ob Verlinkung existiert
                        Dim _checkSeriesName As New TVSeriesDB
                        Dim _checkSeriesNameCounter As Boolean = _checkSeriesName.SeriesFound(_Result(i).Title)
                        _checkSeriesName.Dispose()

                        If _checkSeriesNameCounter = True Then
                            _SeriesName = _Result(i).Title
                        Else
                            'Prüfen ob Serie verlinkt ist
                            _SqlString = "Select * from TvMovieSeriesMapping " & _
                                                "WHERE EpgTitle LIKE '%" & allowedSigns(_Result(i).Title) & "%'"

                            _SeriesMappingResult.AddRange(Broker.Execute(_SqlString).TransposeToFieldList("idSeries", False))

                            'Serie ist verlinkt -> org. SerienName anstatt EPG Name verwenden
                            If _SeriesMappingResult.Count > 0 Then


                                Dim _TvSeriesName As New TVSeriesDB

                                _TvSeriesName.LoadSeriesName(CInt(_SeriesMappingResult.Item(0)))
                                _SeriesName = _TvSeriesName(0).SeriesName

                                _TvSeriesName.Dispose()

                            Else
                                'Nicht verlinkt -> EPG Name verwenden
                                _SeriesName = _Result(i).Title
                            End If
                        End If

                        If Not _lastSeriesName = _Result(i).Title And Not _lastEpisodeName = _Result(i).EpisodeName Then
                            MyLog.Info("")
                            _SeriesDummyID = _SeriesDummyID + 1
                        End If

                        'prüfen ob Episode gefunden wird
                        Dim _TvSeriesDB As New TVSeriesDB
                        _TvSeriesDB.LoadEpisode(_SeriesName, CInt(_Result(i).SeriesNum), CInt(_Result(i).EpisodeNum))

                        'Episode in TvSeries gefunden
                        If _TvSeriesDB.CountSeries > 0 Then
                            _logSeriesFound = True

                            If Not _lastSeriesName = _Result(i).Title And Not _lastEpisodeName = _Result(i).EpisodeName Then
                                MyLog.Info("enrichEPG: [EScannerImport]: Series: {0} ({1}) found in MP-TvSeries db", _SeriesName, _TvSeriesDB(0).SeriesID)
                            End If

                            'Daten im EPG (program) updaten
                            IdentifySeries.UpdateEpgEpisode(_Result(i), _TvSeriesDB, _SeriesName)

                            'Neue Episode -> im EPG Describtion kennzeichnen
                            IdentifySeries.MarkEpgEpisodeAsNew(_Result(i), _TvSeriesDB.EpisodeExistLocal)
                            If _TvSeriesDB.EpisodeExistLocal = False Then
                                _logNewEpisode = True
                                _Counter = _Counter + 1
                            Else
                                _logNewEpisode = False
                            End If

                            'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                            IdentifySeries.UpdateTvMovieProgram(_Result(i), _TvSeriesDB, 0, True)

                            If Not _lastEpisodeName = _Result(i).EpisodeName Then
                                MyLog.Info("enrichEPG: [EScannerImport]: {0}: S{1}E{2} - {3} found in MP-TvSeries DB (newEpisode: {4})", _
                                                                              _SeriesName, _Result(i).SeriesNum, _Result(i).EpisodeNum, _Result(i).EpisodeName, _
                                                                                _logNewEpisode)
                            End If

                        Else
                            'Serie nicht in TvSeries DB gefunden (=neue Aufnahme), dann als neu markieren im EPG

                            'Daten ohne TheTvDb.com übergeben
                            _logSeriesFound = False
                            Dim _rating As Integer = _Result(i).StarRating
                            Dim _idEpisode As String = String.Empty

                            'TheTvDb nutzen
                            If _useTheTvDb = True And ClickfinderProgramGuideImportEnable = True Then
                                'MyLog.Info("TMP: {0}, {1}, S{2}E{3}", _Result(i).Title, _Result(i).EpisodeName, _Result(i).SeriesNum, _Result(i).EpisodeNum)

                                'Information pro Serie nur einmal laden
                                If Not _lastSeriesName = _Result(i).Title Then

                                    IdentifySeries.TheTvDb.ResetCoverAndFanartPath()

                                    MyLog.Info("enrichEPG: [EScannerImport]: TheTvDb.com: {0} searching...", _SeriesName)
                                    IdentifySeries.TheTvDb.SearchSeries(_Result(i).Title)

                                    If IdentifySeries.TheTvDb.SeriesFound = True Then
                                        'Serie gefunden & Daten übergeben
                                        _idSeries = IdentifySeries.TheTvDb.IdSeries
                                        _CacheSeries.Add(_idSeries)

                                        _rating = Replace(IdentifySeries.SeriesEN.Rating, ".", ",")

                                        MyLog.Info("enrichEPG: [EScannerImport]: TheTvDb.com ({0}): {1} ({2}) found", IdentifySeries.TheTvDb.SeriesFoundLanguage, _Result(i).Title, _idSeries)

                                        'Cover & FanArt der Serie herunterladen
                                        IdentifySeries.TheTvDb.LoadCoverAndFanart()
                                        MyLog.Info("enrichEPG: [EScannerImport]: TheTvDb.com: get data for Clickfinder PG (Cover: {0}, FanArt: {1})", IdentifySeries.TheTvDb.PosterImageStatus, IdentifySeries.TheTvDb.FanArtImageStatus)
                                    Else
                                        'Serie nicht gefunden auf TheTvDb
                                        _idSeries = _SeriesDummyID
                                        MyLog.Warn("enrichEPG: [EScannerImport]: TheTvDb.com: {0} (DummyID: {1}) - not found -> mark all episodes as new", _SeriesName, _idSeries)
                                    End If
                                End If

                                'Information pro Episode nur einmal laden, wenn Serie gefunden
                                If IdentifySeries.TheTvDb.SeriesFound = True And Not _lastEpisodeName = _Result(i).EpisodeName Then

                                    IdentifySeries.TheTvDb.ResetEpisodeImagePath()

                                    IdentifySeries.TheTvDb.SearchEpisode(CInt(_Result(i).SeriesNum), CInt(_Result(i).EpisodeNum))

                                    If IdentifySeries.TheTvDb.EpisodeFound = True Then
                                        'Episode gefunden & Daten übergeben
                                        _idEpisode = IdentifySeries.TheTvDb.IdEpisode

                                        _rating = Replace(IdentifySeries.IdentifiedEpisode.Rating, ".", ",")

                                        'Episode Image herunterladen
                                        IdentifySeries.TheTvDb.LoadEpisodeImage()

                                        MyLog.Info("enrichEPG: [EScannerImport]: {0}: S{1}E{2} - {3} found on TheTvDb.com (newEpisode: {4}, Image: {5}, {6})", _
                                                                            _SeriesName, _Result(i).SeriesNum, _Result(i).EpisodeNum, _Result(i).EpisodeName, True, IdentifySeries.TheTvDb.EpisodeImageStatus, _idEpisode)
                                    Else
                                        'Episode nicht gefunden auf TheTvDb
                                        _idEpisode = String.Empty
                                        MyLog.Warn("enrichEPG: [EScannerImport]: TheTvDb.com: episode: {0} - not found -> mark all episodes as new", _SeriesName, _idSeries)
                                    End If

                                End If
                            Else
                                _idSeries = _SeriesDummyID
                            End If

                            'Daten im EPG (program) updaten
                            _Result(i).StarRating = _rating
                            _Result(i).Persist()

                            'Neue Episode -> im EPG Describtion kennzeichnen
                            IdentifySeries.MarkEpgEpisodeAsNew(_Result(i), _TvSeriesDB.EpisodeExistLocal)
                            If _TvSeriesDB.EpisodeExistLocal = False Then
                                _logNewEpisode = True
                            End If

                            'Sofern Clickfinder Plugin aktiviert -> daten in TvMovieProgam schreiben mit Dummy idSeries
                            If ClickfinderProgramGuideImportEnable = True Then
                                Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_Result(i).IdProgram)
                                _TvMovieProgram.idSeries = _idSeries
                                _TvMovieProgram.idEpisode = _idEpisode
                                _TvMovieProgram.needsUpdate = True
                                _TvMovieProgram.local = False
                                _TvMovieProgram.TVMovieBewertung = 6

                                'MyLog.Info("{0}, {1},idseries: {2},idepisode: {3}, local: {4}", _Result(i).Title, _Result(i).EpisodeName, _TvMovieProgram.idSeries, _TvMovieProgram.idEpisode, _TvMovieProgram.local)

                                'Serien Poster Image
                                If Not String.IsNullOrEmpty(IdentifySeries.TheTvDb.SeriesPosterPath) = True Then
                                    _TvMovieProgram.SeriesPosterImage = IdentifySeries.TheTvDb.SeriesPosterPath
                                End If

                                'FanArt Image
                                If Not String.IsNullOrEmpty(IdentifySeries.TheTvDb.FanArtPath) = True Then
                                    _TvMovieProgram.FanArt = IdentifySeries.TheTvDb.FanArtPath
                                End If

                                'Episoden Image
                                If Not String.IsNullOrEmpty(IdentifySeries.TheTvDb.EpisodeImagePath) = True Then
                                    _TvMovieProgram.EpisodeImage = IdentifySeries.TheTvDb.EpisodeImagePath
                                End If

                                _TvMovieProgram.Persist()
                            End If

                            _Counter = _Counter + 1
                        End If

                        _TvSeriesDB.Dispose()

                        _lastSeriesName = _Result(i).Title
                        _lastEpisodeName = _Result(i).EpisodeName
                    Catch ex As Exception
                        MyLog.[Error]("enrichEPG: [EScannerImport]: Loop: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    End Try
                Next
            End If

            If ClickfinderProgramGuideImportEnable = True And String.IsNullOrEmpty(_MPThumbPath) = False Then
                Helper.DeleteCPGcache(_MPThumbPath & "\MPTVSeriesBanners\Clickfinder ProgramGuide\", _CacheSeries)
                Helper.DeleteCPGcache(_MPThumbPath & "\Fan Art\Clickfinder ProgramGuide\", _CacheSeries)
            End If

            If _ScheduledDummyRecordingList.Count > 0 Then
                MyLog.Info("")
                For i = 0 To _ScheduledDummyRecordingList.Count - 1
                    Dim _dummy As Schedule = Schedule.Retrieve(_ScheduledDummyRecordingList(i))
                    MyLog.Info("enrichEPG: [DeleteDummySchedule]: dummy schedule deleted: {0} ({1}), {2}", _dummy.ProgramName, _dummy.IdSchedule, _dummy.StartTime)
                    _dummy.Remove()

                    Dim key As New Key(GetType(Setting), True, "tag", "enrichEPGlastScheduleRecordings")
                    Dim _lastDummyScheduledRecordings As Setting = Setting.Retrieve(key)
                    If InStr(_lastDummyScheduledRecordings.Value, "|" & _ScheduledDummyRecordingList(i)) > 0 Then
                        _lastDummyScheduledRecordings.Value = Replace(_lastDummyScheduledRecordings.Value, "|" & _ScheduledDummyRecordingList(i), "")
                    Else
                        _lastDummyScheduledRecordings.Value = Replace(_lastDummyScheduledRecordings.Value, _ScheduledDummyRecordingList(i), "")
                    End If
                    _lastDummyScheduledRecordings.Persist()

                Next
            End If

            MyLog.Info("")
            MyLog.[Info]("enrichEPG: [EScannerImport]: Process success - {0} Episodes imported", _Counter)

        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [EScannerImport]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub GetMovingPicturesInfos()
        Try
            MyLog.[Debug]("enrichEPG: [GetMovingPicturesInfos]: start import")

            Dim _SQLString As String = String.Empty
            Dim _MovingPicturesDB As New MovingPicturesDB
            _MovingPicturesDB.LoadAllMovingPicturesFilms()

            Dim EPGCounter As Integer = 0
            Dim MovieCounter As Integer = 0

            For i As Integer = 0 To _MovingPicturesDB.Count - 1

                Try

                    Dim _Result As New ArrayList

                    'nach Mov.Pic Titel suchen
                    _SQLString = _
                        "Select idProgram from program WHERE title LIKE '" & allowedSigns(_MovingPicturesDB(i).Title) & "' "

                    'nach Mov.Pic Titel über Dateiname suchen
                    If Not String.IsNullOrEmpty(_MovingPicturesDB(i).TitlebyFilename) Then
                        _SQLString = _SQLString & _
                        "OR title LIKE '" & allowedSigns(_MovingPicturesDB(i).TitlebyFilename) & "' "
                    End If

                    'nach Mov.Pic alternateTitel 
                    If Not String.IsNullOrEmpty(_MovingPicturesDB(i).AlternateTitle) Then
                        _SQLString = _SQLString & _
                        "OR title LIKE '" & allowedSigns(_MovingPicturesDB(i).AlternateTitle) & "' "
                    End If

                    _Result.AddRange(Broker.Execute(_SQLString).TransposeToFieldList("idProgram", False))

                    If _Result.Count > 0 Then
                        MovieCounter = MovieCounter + 1
                    End If

                    For d As Integer = 0 To _Result.Count - 1

                        Dim _program As Program = Program.Retrieve(CInt(_Result.Item(d)))

                        Try

                            'Daten im EPG (program) updaten
                            _program.StarRating = _MovingPicturesDB(i).Rating
                            If InStr(_program.Description, "existiert lokal") = 0 And String.IsNullOrEmpty(_program.SeriesNum) Then
                                _program.Description = "existiert lokal" & vbNewLine & _program.Description
                            End If

                            _program.Persist()

                            'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                            If ClickfinderProgramGuideImportEnable = True Then

                                'idProgram in TvMovieProgram suchen & Daten aktualisieren
                                Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_program.IdProgram)
                                _TvMovieProgram.idMovingPictures = _MovingPicturesDB(i).MovingPicturesID
                                _TvMovieProgram.local = True

                                If Not String.IsNullOrEmpty(_MovingPicturesDB(i).Cover) And String.IsNullOrEmpty(_TvMovieProgram.Cover) Then
                                    _TvMovieProgram.Cover = _MovingPicturesDB(i).Cover
                                End If

                                If Not String.IsNullOrEmpty(_MovingPicturesDB(i).FanArt) And String.IsNullOrEmpty(_TvMovieProgram.FanArt) Then
                                    _TvMovieProgram.FanArt = _MovingPicturesDB(i).FanArt
                                End If

                                If Not String.IsNullOrEmpty(_MovingPicturesDB(i).Filename) And String.IsNullOrEmpty(_TvMovieProgram.FileName) Then
                                    _TvMovieProgram.FileName = _MovingPicturesDB(i).Filename
                                End If

                                _TvMovieProgram.needsUpdate = True
                                _TvMovieProgram.Persist()

                            End If

                            'ausgegeben Zahl in log ist höher als import in TvMovieProgram
                            'wegen SQLabfrage nach title & epsiodeName -> Überschneidungen.
                            EPGCounter = EPGCounter + 1

                        Catch ex As Exception
                            MyLog.[Error]("enrichEPG: [GetMovingPicturesInfos]: Loop _Result exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                            MyLog.[Error]("enrichEPG: [GetMovingPicturesInfos]: title:{0} idchannel:{1} startTime: {2}", _program.Title, _program.ReferencedChannel.DisplayName, _program.StartTime)
                        End Try
                    Next

                Catch ex As Exception
                    MyLog.[Error]("enrichEPG: [GetMovingPicturesInfos]: Loop _MovingPicturesDB - exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                End Try
            Next
            MyLog.Info("")
            MyLog.[Info]("enrichEPG: [GetMovingPicturesInfos]: Summary: {0} MovingPictures Films found in {1} EPG entries", MovieCounter, EPGCounter)
            MyLog.Info("")
            MyLog.Info("")
        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [GetMovingPicturesInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub GetVideoDatabaseInfos()

        Try
            MyLog.[Debug]("TVMovie: [GetVideoDatabaseInfos]: start import")

            Dim _VideoDB As New VideoDB
            _VideoDB.LoadAllVideoDBFilms()

            Dim EPGCounter As Integer = 0
            Dim MovieCounter As Integer = 0
            Dim _SQLString As String = String.Empty

            For i As Integer = 0 To _VideoDB.Count - 1

                Try
                    Dim _Result As New ArrayList

                    'nach Mov.Pic Titel suchen
                    _SQLString = _
                        "Select idProgram from program WHERE title LIKE '" & allowedSigns(_VideoDB(i).Title) & "' "

                    'nach Mov.Pic Titel über Dateiname suchen
                    If Not String.IsNullOrEmpty(_VideoDB(i).TitlebyFileName) Then
                        _SQLString = _SQLString & _
                        "OR title LIKE '" & allowedSigns(_VideoDB(i).TitlebyFileName) & "' " & _
                        "OR episodeName LIKE '" & allowedSigns(_VideoDB(i).TitlebyFileName) & "' "
                    End If

                    _Result.AddRange(Broker.Execute(_SQLString).TransposeToFieldList("idProgram", False))

                    If _Result.Count > 0 Then
                        MovieCounter = MovieCounter + 1
                    End If

                    For d As Integer = 0 To _Result.Count - 1

                        Dim _program As Program = Program.Retrieve(CInt(_Result.Item(d)))

                        Try
                            'Daten im EPG (program) updaten

                            'Wenn TvMovieImportMovingPicturesInfos deaktiviert, dann Rating aus VideoDatabase nehmen
                            If _MovPicEnabled = False Then
                                _program.StarRating = _VideoDB(i).Rating
                            End If

                            If InStr(_program.Description, "existiert lokal") = 0 And String.IsNullOrEmpty(_program.SeriesNum) Then
                                _program.Description = "existiert lokal" & vbNewLine & _program.Description
                            End If

                            _program.Persist()

                            'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                            If ClickfinderProgramGuideImportEnable = True Then

                                'idProgram in TvMovieProgram suchen & Daten aktualisieren
                                Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_program.IdProgram)
                                _TvMovieProgram.idVideo = _VideoDB(i).VideoID
                                _TvMovieProgram.local = True


                                If Not String.IsNullOrEmpty(_VideoDB(i).FileName) And String.IsNullOrEmpty(_TvMovieProgram.FileName) Then
                                    _TvMovieProgram.FileName = _VideoDB(i).FileName
                                End If

                                _TvMovieProgram.needsUpdate = True
                                _TvMovieProgram.Persist()

                            End If

                            EPGCounter = EPGCounter + 1

                        Catch ex As Exception
                            MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: Loop _Result exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                            MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: title:{0} idchannel:{1} startTime: {2}", _program.Title, _program.ReferencedChannel.DisplayName, _program.StartTime)
                        End Try
                    Next

                Catch ex As Exception
                    MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: Loop _VideoDB - exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                End Try
            Next
            MyLog.Info("")
            MyLog.[Info]("TVMovie: [GetVideoDatabaseInfos]: Summary: {0} Video Films found in {1} EPG entries", MovieCounter, EPGCounter)
            MyLog.Info("")
            MyLog.Info("")
        Catch ex As Exception
            MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try

    End Sub

#End Region


End Class
