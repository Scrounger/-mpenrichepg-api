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
Imports enrichEPG.Database

Public Class EnrichEPG

#Region "Members"
    Private Shared _ImportStartTime As Date
    Private Shared _IdentifiedPrograms As New ArrayList
    Private Shared _ScheduledDummyRecordingList As New ArrayList
    Public Shared MyTVDBen As New clsTheTVdb("en")
    Public Shared MyTVDBlang As New clsTheTVdb("de")
#End Region

#Region "Constructors"
    Public Sub New(ByVal MediaPortalDatabasePath As String, ByVal SeriesEnabled As Boolean, _
                   ByVal VideoEnabled As Boolean, ByVal MovPicEnabled As Boolean, _
                   ByVal ImportStartTime As Date, ByVal LogFilePath As MySettings.LogPath, _
                   Optional ByVal EpisodenSCPath As String = "", _
                   Optional ByVal NewEpisodeString As String = "New Episode", _
                   Optional ByVal EpisodeExistsString As String = "", _
                   Optional ByVal LogFileName As String = "", Optional ByVal UseTheTvDb As Boolean = True, _
                   Optional ByVal TheTvDbLanguage As String = "de", _
                   Optional ByVal MediaPortalThumbsPath As String = "")

        MySettings.SetSettings(MediaPortalDatabasePath, SeriesEnabled, _
                              VideoEnabled, MovPicEnabled, _
                              LogFilePath, LogFileName, EpisodenSCPath, _
                              NewEpisodeString, _
                              EpisodeExistsString, UseTheTvDb, TheTvDbLanguage, MediaPortalThumbsPath)

        _ImportStartTime = ImportStartTime
    End Sub

#End Region

#Region "Functions"

    Public Sub start()

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

        IO.Directory.CreateDirectory(MySettings.MpThumbPath & "\Fan Art\Clickfinder ProgramGuide")
        IO.Directory.CreateDirectory(MySettings.MpThumbPath & "\MPTVSeriesBanners\Clickfinder ProgramGuide")

        MyTVDBen.tvLanguage = "en"
        MyTVDBlang.tvLanguage = MySettings.TheTvDbLanguage

        If MySettings.TvSeriesEnabled = True Then
            GetSeriesInfos()
        End If

        If MySettings.MovPicEnabled = True Then
            GetMovingPicturesInfos()
        End If

        If MySettings.VideoEnabled = True Then
            GetVideoDatabaseInfos()
        End If

        If String.IsNullOrEmpty(MySettings.EpisodenScannerPath) = False Then
            RunEpisodenScanner()
        End If

        If _ScheduledDummyRecordingList.Count > 0 Then
            MyLog.Info("")
            For i = 0 To _ScheduledDummyRecordingList.Count - 1
                Dim _dummy As Schedule = Schedule.Retrieve(_ScheduledDummyRecordingList(i))
                MyLog.Info("dummy schedule deleted: {0} ({1}), {2}", _dummy.ProgramName, _dummy.IdSchedule, _dummy.StartTime)
                _dummy.Remove()

                Dim key As New Key(GetType(Setting), True, "tag", "enrichEPGlastScheduleRecordings")
                _lastDummyScheduledRecordings = Setting.Retrieve(key)
                If InStr(_lastDummyScheduledRecordings.Value, "|" & _ScheduledDummyRecordingList(i)) > 0 Then
                    _lastDummyScheduledRecordings.Value = Replace(_lastDummyScheduledRecordings.Value, "|" & _ScheduledDummyRecordingList(i), "")
                Else
                    _lastDummyScheduledRecordings.Value = Replace(_lastDummyScheduledRecordings.Value, _ScheduledDummyRecordingList(i), "")
                End If
                _lastDummyScheduledRecordings.Persist()

            Next
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
            Dim _TvSeriesList As IList(Of MyTvSeries) = MyTvSeries.ListAll

            MyLog.Info("enrichEPG: [GetSeriesInfos]: {0} series loaded from database", _TvSeriesList.Count)
            MyLog.Info("")
            IdentifySeries.SeriesEN = MyTVDBen.TheTVdbHandler.GetSeries(_TvSeriesList(0).idSeries, MyTVDBen.DBLanguage, True, False, False)
            IdentifySeries.SeriesLang = MyTVDBlang.TheTVdbHandler.GetSeries(_TvSeriesList(0).idSeries, MyTVDBlang.DBLanguage, True, False, False)

            'Alle Serien durchgehen
            For Each _TvSeries In _TvSeriesList
                Try
                    'Alle Episoden der Serie in Speicher laden
                    _TvSeries.LoadAllEpisodes()
                    MyLog.Info("enrichEPG: [GetSeriesInfos]: {0}: {1} episodes loaded from database", _TvSeries.Title, _TvSeries.EpisodesList.Count)

                    Dim _logScheduldedRecording As String = String.Empty
                    Dim _ScheduldedDummyRecording As Program = Nothing
                    Dim _EpisodeIdentified As Boolean = True

                    Dim _EpisodeFoundCounter As Integer = 0

                    'EPG nach Serien Name (+ mapped Serien Namen) durchsuchen = Episoden
                    Dim _programList As List(Of Program) = GetSeriesFromEPG(_TvSeries)

                    'Entfernen: kein EpisodenName
                    _programList = _programList.FindAll(Function(x) x.EpisodeName.Length > 0)

                    'Daten von TheTvDb für Serie downloaden / cache
                    If _NextSeries = True And MySettings.useTheTvDb = True Then
                        'MyLog.Info("enrichEPG: [GetSeriesInfos]: Daten von TheTvDb.com holen: {0} (idSeries: {1})", _TvSeriesDB(i).SeriesName, _TvSeriesDB(i).SeriesID)
                        _NextSeries = False
                        IdentifySeries.SeriesEN = MyTVDBen.TheTVdbHandler.GetSeries(_TvSeries.idSeries, MyTVDBen.DBLanguage, True, False, False)
                        IdentifySeries.SeriesLang = MyTVDBlang.TheTVdbHandler.GetSeries(_TvSeries.idSeries, MyTVDBlang.DBLanguage, True, False, False)
                    End If

                    'Alle gefundenen Episoden der Serie durchlaufen
                    For Each _program As Program In _programList
                        Try
                            Dim _episodeFound As Boolean = False
                            Dim _logNewEpisode As Boolean = False

                            'Episode identifziert (inkl. Daten update program + TvMovieProgram & log Ausgabe)
                            If IdentifySeries.EpisodeIdentifed(_program, _TvSeries) = True Then
                                _episodeFound = True
                                _EpisodeFoundCounter = _EpisodeFoundCounter + 1

                                'log + counter
                                If IdentifySeries.TvSeriesEpisode.ExistLocal = False Then
                                    _logNewEpisode = True
                                    _CounterNewEpisode = _CounterNewEpisode + 1
                                End If
                            Else
                                _EpisodeIdentified = False

                                _ScheduldedDummyRecording = _program
                                _lastEpisodeName = _program.EpisodeName
                            End If

                            'Gefunden zur List, damit EPscanner nicht prüfen
                            If _episodeFound = True Then
                                _IdentifiedPrograms.Add(_program.IdProgram)
                            End If

                        Catch ex As Exception
                            MyLog.[Error]("enrichEPG: [GetSeriesInfos]: title:{0} idchannel:{1} startTime: {2} episodeName: {3}", _program.Title, _program.ReferencedChannel.DisplayName, _program.StartTime, _program.EpisodeName)
                            MyLog.[Error]("enrichEPG: [GetSeriesInfos]: Loop :Result exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                        End Try
                    Next

                    'nicht identifiziert -> Dummy schedule anlegen für EpisodenScanner
                    If _EpisodeIdentified = False And String.IsNullOrEmpty(MySettings.EpisodenScannerPath) = False Then

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

                    MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: {1}/{2} episodes identified (programs renamed {3}{4})", _TvSeries.Title, _EpisodeFoundCounter, _programList.Count, IdentifySeries.UpdateEpgEpisodeSeriesNameCounter, _logScheduldedRecording)
                    _CounterFound = _CounterFound + _EpisodeFoundCounter
                    _Counter = _Counter + _programList.Count
                    _NextSeries = True
                    IdentifySeries.UpdateEpgEpisodeSeriesNameCounter = 0
                    MyLog.[Info]("")
                Catch ex As Exception
                    MyLog.[Error]("enrichEPG: [GetSeriesInfos]: Loop exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                End Try
            Next

            MyLog.[Info]("enrichEPG: [GetSeriesInfos]: Summary: {0}/{1} episodes identified ({2}%), {3} new episodes identified", _CounterFound, _Counter, Format(_CounterFound / _Counter * 100, "00"), _CounterNewEpisode)
            MyLog.Info("enrichEPG: [GetSeriesInfos]: Import duration: {0}", (Date.Now - _SeriesImportStartTime).Minutes & "min " & (Date.Now - _SeriesImportStartTime).Seconds & "s")
            MyLog.Info("")
            MyLog.Info("")
        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [GetSeriesInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try

    End Sub

    Private Function GetSeriesFromEPG(ByVal TvSeries As MyTvSeries) As IList(Of Program)

        'Zunächst nach alle Serien mit SerienName aus TvSeries DB suchen
        Dim _SQLString As String = "Select * from program WHERE title LIKE '" & MyTvSeries.Helper.allowedSigns(TvSeries.Title) & "' ORDER BY episodeName"

        _SQLString = Replace(_SQLString, " * ", " Program.IdProgram, Program.Classification, Program.Description, Program.EndTime, Program.EpisodeName, Program.EpisodeNum, Program.EpisodePart, Program.Genre, Program.IdChannel, Program.OriginalAirDate, Program.ParentalRating, Program.SeriesNum, Program.StarRating, Program.StartTime, Program.state, Program.Title ")
        Dim _SQLstate As SqlStatement = Broker.GetStatement(_SQLString)
        Dim _ProgramList As List(Of Program) = ObjectFactory.GetCollection(GetType(Program), _SQLstate.Execute())

        MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0} ({1}): {2} episodes found in EPG", TvSeries.Title, TvSeries.idSeries, _ProgramList.Count)

        'SeriesMappingNamen laden, sofern vorhanden
        Try
            Dim _TvMovieSeriesMapping As TvMovieSeriesMapping = TvMovieSeriesMapping.Retrieve(TvSeries.idSeries)

            If Not String.IsNullOrEmpty(_TvMovieSeriesMapping.EpgTitle) Then
                Dim _MappedSeriesNames As New ArrayList(Split(_TvMovieSeriesMapping.EpgTitle, "|"))
                MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: manuel mapping found: {1}", TvSeries.Title, Replace(_TvMovieSeriesMapping.EpgTitle, "|", ", "))

                'EPG nach gemappten Serien Namen durchsuchen
                For z As Integer = 0 To _MappedSeriesNames.Count - 1
                    _SQLString = "Select * from program WHERE title LIKE '" & MyTvSeries.Helper.allowedSigns(_MappedSeriesNames.Item(z)) & "' " & _
                    "ORDER BY episodeName"

                    _SQLString = Replace(_SQLString, " * ", " Program.IdProgram, Program.Classification, Program.Description, Program.EndTime, Program.EpisodeName, Program.EpisodeNum, Program.EpisodePart, Program.Genre, Program.IdChannel, Program.OriginalAirDate, Program.ParentalRating, Program.SeriesNum, Program.StarRating, Program.StartTime, Program.state, Program.Title ")
                    Dim _SQLstate2 As SqlStatement = Broker.GetStatement(_SQLString)
                    Dim _MappedSeries As List(Of Program) = ObjectFactory.GetCollection(GetType(Program), _SQLstate2.Execute())

                    If _MappedSeries.Count > 0 Then
                        _ProgramList.AddRange(_MappedSeries)
                        MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: {1} episodes found in EPG (series is mapped)", _MappedSeriesNames.Item(z), _MappedSeries.Count)
                    End If
                Next
            End If

            If _TvMovieSeriesMapping.disabled = True Then
                MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0} is disabled (local = true) !", _TvMovieSeriesMapping.TvSeriesTitle)
            End If

            If _TvMovieSeriesMapping.minSeasonNum > 0 Then
                MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: minSeasonNumber >= {1} (local = true) !", _TvMovieSeriesMapping.TvSeriesTitle, _TvMovieSeriesMapping.minSeasonNum)
            End If
        Catch SeriesMappingEx As Exception
            'Exception wenn keine mappings gefunden
            'MyLog.Warn("enrichEPG: [GetSeriesInfos]: SeriesMapping Error: {0}, stack: {1}", SeriesMappingEx.Message, SeriesMappingEx.StackTrace)
        End Try

        Return _ProgramList

    End Function

    Private Sub RunEpisodenScanner()
        If File.Exists(MySettings.EpisodenScannerPath & "\episodescanner.exe") = True Then

            Dim StartTime As Date = Date.Now

            Dim App As New Process()
            App.StartInfo.FileName = MySettings.EpisodenScannerPath & "\episodescanner.exe"
            App.StartInfo.WindowStyle = ProcessWindowStyle.Normal
            App.Start()

            MyLog.Debug("enrichEPG: [RunEpisodenScanner]: {0} started", MySettings.EpisodenScannerPath & "\episodescanner.exe")

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
        Dim _SeriesDummyID As Integer = 1
        Dim _idSeries As Integer = 0
        Dim _idEpisode As String = String.Empty

        Dim _CacheSeries As New ArrayList

        Try

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

                        If Not _lastSeriesName = _Result(i).Title And Not _lastEpisodeName = _Result(i).EpisodeName Then
                            MyLog.Info("")
                        End If

                        Dim _SqlString As String = String.Empty
                        Dim _SeriesMappingResult As New List(Of TvMovieSeriesMapping)
                        Dim _logSeriesFound As Boolean = False
                        Dim _logNewEpisode As Boolean = False
                        Dim _Episode As MyTvSeries.MyEpisode = Nothing
                        Dim _Series As MyTvSeries = Nothing

                        Try
                            'Serie in TvSeries DB gefunden
                            _Series = MyTvSeries.Search(_Result(i).Title)
                            _Episode = _Series.Episode(CInt(_Result(i).SeriesNum), CInt(_Result(i).EpisodeNum))
                            _idSeries = _Series.idSeries
                        Catch ex As Exception
                            'Series nicht gefunden -> Verlinkung prüfen
                            _SqlString = "Select * from TvMovieSeriesMapping " & _
                                                "WHERE EpgTitle LIKE '%" & allowedSigns(_Result(i).Title) & "%'"

                            Dim _SQLstate As SqlStatement = Broker.GetStatement(_SqlString)
                            _SeriesMappingResult = ObjectFactory.GetCollection(GetType(TvMovieSeriesMapping), _SQLstate.Execute())

                            If _SeriesMappingResult.Count > 0 Then
                                'vorhanden
                                _Series = MyTvSeries.Retrieve(_SeriesMappingResult(0).idSeries)
                                _Episode = _Series.Episode(CInt(_Result(i).SeriesNum), CInt(_Result(i).EpisodeNum))
                            Else
                                'Nicht verlinkt -> EPG Name verwenden

                            End If
                        End Try


                        If Not _Episode Is Nothing Then
                            If Not _lastSeriesName = _Result(i).Title And Not _lastEpisodeName = _Result(i).EpisodeName Then
                                MyLog.Info("enrichEPG: [EScannerImport]: Series: {0} ({1}) found in MP-TvSeries db", _Series.Title, _Series.idSeries)
                            End If

                            IdentifySeries.UpdateProgramAndTvMovieProgram(_Result(i), _Series, _Episode, _Episode.ExistLocal, True)

                            If _Episode.ExistLocal = False Then
                                _logNewEpisode = True
                                _Counter = _Counter + 1
                            Else
                                _logNewEpisode = False
                            End If

                            If Not _lastEpisodeName = _Result(i).EpisodeName Then
                                MyLog.Info("enrichEPG: [EScannerImport]: {0}: S{1}E{2} - {3} found in MP-TvSeries DB (newEpisode: {4})", _
                                                                              _Series.Title, _Result(i).SeriesNum, _Result(i).EpisodeNum, _Result(i).EpisodeName, _
                                                                                _logNewEpisode)
                            End If

                        Else
                            'Serie nicht in TvSeries DB gefunden (=neue Aufnahme), dann als neu markieren im EPG

                            'Daten ohne TheTvDb.com übergeben
                            _logSeriesFound = False
                            Dim _rating As Integer = _Result(i).StarRating

                            'TheTvDb nutzen
                            If MySettings.useTheTvDb = True And MySettings.ClickfinderProgramGuideImportEnable = True Then
                                'MyLog.Info("TMP: {0}, {1}, S{2}E{3}", _Result(i).Title, _Result(i).EpisodeName, _Result(i).SeriesNum, _Result(i).EpisodeNum)

                                'Information pro Serie nur einmal laden
                                If Not _lastSeriesName = _Result(i).Title Then

                                    IdentifySeries.TheTvDb.ResetCoverAndFanartPath()

                                    MyLog.Info("enrichEPG: [EScannerImport]: TheTvDb.com: {0} searching...", _Result(i).Title)
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
                                        _idEpisode = String.Empty
                                        _SeriesDummyID = _SeriesDummyID + 1
                                        MyLog.Warn("enrichEPG: [EScannerImport]: TheTvDb.com: {0} (DummyID: {1}) - not found -> mark all episodes as new", _Result(i).Title, _idSeries)
                                    End If
                                End If

                                'Information pro Episode nur einmal laden, wenn Serie gefunden
                                If IdentifySeries.TheTvDb.SeriesFound = True And Not _lastEpisodeName = _Result(i).EpisodeName Then

                                    IdentifySeries.TheTvDb.ResetEpisodeImagePath()

                                    IdentifySeries.TheTvDb.SearchEpisode(CInt(_Result(i).SeriesNum), CInt(_Result(i).EpisodeNum))

                                    If IdentifySeries.TheTvDb.EpisodeFound = True Then
                                        'Episode gefunden & Daten übergeben
                                        _idEpisode = IdentifySeries.TheTvDb.IdEpisode

                                        _rating = Replace(IdentifySeries.TheTvDbEpisode.Rating, ".", ",")

                                        'Episode Image herunterladen
                                        IdentifySeries.TheTvDb.LoadEpisodeImage()

                                        MyLog.Info("enrichEPG: [EScannerImport]: {0}: S{1}E{2} - {3} found on TheTvDb.com (newEpisode: {4}, Image: {5}, {6})", _
                                                                            _Result(i).Title, _Result(i).SeriesNum, _Result(i).EpisodeNum, _Result(i).EpisodeName, True, IdentifySeries.TheTvDb.EpisodeImageStatus, _idEpisode)
                                    Else
                                        'Episode nicht gefunden auf TheTvDb
                                        _idEpisode = String.Empty
                                        MyLog.Warn("enrichEPG: [EScannerImport]: TheTvDb.com: episode: {0} - not found -> mark all episodes as new", _Result(i).Title, _idSeries)
                                    End If
                                End If
                            Else
                                'Kein TheTvDb.com nutzen
                                'Information pro Serie nur einmal laden
                                If Not _lastSeriesName = _Result(i).Title Then
                                    _idEpisode = String.Empty
                                    _idSeries = _SeriesDummyID
                                    _SeriesDummyID = _SeriesDummyID + 1
                                End If

                                MyLog.Info("enrichEPG: [EScannerImport]: {0}: S{1}E{2} - {3} Series not identified (newEpisode: {4})", _
                                            _Result(i).Title, _Result(i).SeriesNum, _Result(i).EpisodeNum, _Result(i).EpisodeName, True)
                            End If

                                'Daten im EPG (program) updaten
                                _Result(i).StarRating = _rating
                                _Result(i).Persist()

                                'Neue Episode -> im EPG Describtion kennzeichnen
                                IdentifySeries.MarkEpgEpisodeAsNew(_Result(i), False)

                                'Sofern Clickfinder Plugin aktiviert -> daten in TvMovieProgam schreiben mit Dummy idSeries
                                If MySettings.ClickfinderProgramGuideImportEnable = True Then
                                    Dim _TvMovieProgram As TVMovieProgram = TVMovieProgram.Retrieve(_Result(i).IdProgram)
                                    _TvMovieProgram.idSeries = _idSeries
                                    _TvMovieProgram.idEpisode = _idEpisode
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

                        _lastSeriesName = _Result(i).Title
                        _lastEpisodeName = _Result(i).EpisodeName
                    Catch ex As Exception
                        MyLog.[Error]("enrichEPG: [EScannerImport]: Loop: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    End Try
                Next
            End If

            If MySettings.ClickfinderProgramGuideImportEnable = True And String.IsNullOrEmpty(MySettings.MpThumbPath) = False Then
                Helper.DeleteCPGcache(MySettings.MpThumbPath & "\MPTVSeriesBanners\Clickfinder ProgramGuide\", _CacheSeries)
                Helper.DeleteCPGcache(MySettings.MpThumbPath & "\Fan Art\Clickfinder ProgramGuide\", _CacheSeries)
            End If

            
            MyLog.Info("")
            MyLog.[Info]("enrichEPG: [EScannerImport]: Process success - {0} Episodes identifed by EpisodenScanner", _Counter)

        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [EScannerImport]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub GetMovingPicturesInfos()
        Try
            Dim _MovPicTimer As Date = Date.Now
            Dim _MovieCounter As Integer = 0
            Dim _EPGcounter As Integer = 0
            MyLog.Info("enrichEPG: [GetMovingPicturesInfos]: start import")

            Dim _MovieList As IList(Of MyMovingPictures) = MyMovingPictures.ListAll

            MyLog.Info("enrichEPG: [GetMovingPicturesInfos]: {0} movies loaded from Moving Pictures database", _MovieList.Count)

            For Each _movie In _MovieList

                Dim _Sqlstring As String = String.Empty

                'nach Mov.Pic Titel im EPG suchen
                _Sqlstring = String.Format("Select * from program WHERE OriginalAirDate = {0} AND (title LIKE '{1}'", _
                                            Helper.MySqlDate(CDate(_movie.year)), MyMovingPictures.Helper.allowedSigns(_movie.Title))

                'nach Alternate Mov.Pic Title im EPG suchen
                If Not String.IsNullOrEmpty(_movie.AlternateTitles) Then
                    _Sqlstring = _Sqlstring & " " & _
                                String.Format("OR title LIKE '{0}'", _
                                MyMovingPictures.Helper.allowedSigns(_movie.AlternateTitles))
                End If

                'nach TitleByFilename Mov.Pic Title im EPG suchen
                If Not String.IsNullOrEmpty(_movie.AlternateTitles) Then
                    _Sqlstring = _Sqlstring & " " & _
                                String.Format("OR title LIKE '{0}'", _
                                MyMovingPictures.Helper.allowedSigns(_movie.TitleByFileName))
                End If

                'Abschließend noch Klammer wegen OR
                _Sqlstring = _Sqlstring & ") ORDER BY startTime"

                'List: programs des Movies laden
                Dim _SQLstate As SqlStatement = Broker.GetStatement(_Sqlstring)
                Dim _Result As IList(Of Program) = ObjectFactory.GetCollection(GetType(Program), _SQLstate.Execute())

                'Movie im EPG gefunden
                If _Result.Count > 0 Then
                    _MovieCounter = _MovieCounter + 1
                    _EPGcounter = _EPGcounter + _Result.Count
                    MyLog.Info("enrichEPG: [GetMovingPicturesInfos]: {0} ({1}) found in {2} epg entries", _movie.Title, _movie.year.Year, _Result.Count)

                    For Each _program In _Result
                        Try
                            'Daten im EPG (program) updaten
                            _program.StarRating = _movie.Rating
                            _program.ParentalRating = _movie.Certification
                            If InStr(_program.Description, "existiert lokal" & vbNewLine) = 0 And String.IsNullOrEmpty(_program.SeriesNum) Then
                                _program.Description = "existiert lokal" & vbNewLine & _program.Description
                            End If

                            _program.Persist()

                            'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                            If MySettings.ClickfinderProgramGuideImportEnable = True Then

                                'ggf. einen TvMovieProgram Eintrag erstellen, wenn Source z.B. Epg grab
                                Try
                                    Dim _TvMovieProgramTest As TVMovieProgram = TVMovieProgram.Retrieve(_program.IdProgram)
                                Catch ex As Exception
                                    Dim _newTvMovieProgram As New TVMovieProgram(_program.IdProgram)
                                    _newTvMovieProgram.Persist()
                                End Try

                                Dim _TvMovieProgram As TVMovieProgram = TVMovieProgram.Retrieve(_program.IdProgram)

                                _TvMovieProgram.idMovingPictures = _movie.ID
                                _TvMovieProgram.local = True

                                _TvMovieProgram.Cover = _movie.Cover
                                _TvMovieProgram.FanArt = _movie.FanArt
                                _TvMovieProgram.FileName = _movie.FileName

                                _TvMovieProgram.Persist()

                                'Nach Wiederholung suchen & TvMovieProgram anlegen

                                'SQLstring: Alle Movies (2 > TvMovieBewertung < 6), inkl. TagesTipps des Tages laden
                                _Sqlstring = _
                                    "Select * from program INNER JOIN TvMovieProgram ON program.idprogram = TvMovieProgram.idProgram " & _
                                    "WHERE title LIKE '" & Helper.allowedSigns(_program.Title) & "' " & _
                                    "AND episodeName LIKE '" & Helper.allowedSigns(_program.EpisodeName) & "'"


                                'List: Daten laden
                                _Sqlstring = Replace(_Sqlstring, " * ", " TVMovieProgram.idProgram, TVMovieProgram.Action, TVMovieProgram.Actors, TVMovieProgram.BildDateiname, TVMovieProgram.Country, TVMovieProgram.Cover, TVMovieProgram.Describtion, TVMovieProgram.Dolby, TVMovieProgram.EpisodeImage, TVMovieProgram.Erotic, TVMovieProgram.FanArt, TVMovieProgram.Feelings, TVMovieProgram.FileName, TVMovieProgram.Fun, TVMovieProgram.HDTV, TVMovieProgram.idEpisode, TVMovieProgram.idMovingPictures, TVMovieProgram.idSeries, TVMovieProgram.idVideo, TVMovieProgram.KurzKritik, TVMovieProgram.local, TVMovieProgram.Regie, TVMovieProgram.Requirement, TVMovieProgram.SeriesPosterImage, TVMovieProgram.ShortDescribtion, TVMovieProgram.Tension, TVMovieProgram.TVMovieBewertung ")
                                Dim _SQLstate1 As SqlStatement = Broker.GetStatement(_Sqlstring)
                                Dim _RepeatList As List(Of TVMovieProgram) = ObjectFactory.GetCollection(GetType(TVMovieProgram), _SQLstate1.Execute())

                                If _RepeatList.Count > 0 Then

                                    _TvMovieProgram.TVMovieBewertung = _RepeatList(0).TVMovieBewertung
                                    _TvMovieProgram.BildDateiname = _RepeatList(0).BildDateiname
                                    _TvMovieProgram.KurzKritik = _RepeatList(0).KurzKritik
                                    _TvMovieProgram.Fun = _RepeatList(0).Fun
                                    _TvMovieProgram.Action = _RepeatList(0).Action
                                    _TvMovieProgram.Feelings = _RepeatList(0).Feelings
                                    _TvMovieProgram.Erotic = _RepeatList(0).Erotic
                                    _TvMovieProgram.Tension = _RepeatList(0).Tension
                                    _TvMovieProgram.Requirement = _RepeatList(0).Requirement
                                    _TvMovieProgram.Actors = _RepeatList(0).Actors
                                    _TvMovieProgram.Dolby = _RepeatList(0).Dolby
                                    _TvMovieProgram.HDTV = _RepeatList(0).HDTV
                                    _TvMovieProgram.Country = _RepeatList(0).Country
                                    _TvMovieProgram.Regie = _RepeatList(0).Regie
                                    _TvMovieProgram.Describtion = _RepeatList(0).Describtion
                                    _TvMovieProgram.ShortDescribtion = _RepeatList(0).ShortDescribtion

                                    _TvMovieProgram.idMovingPictures = _movie.ID
                                    _TvMovieProgram.local = True

                                    _TvMovieProgram.Cover = _movie.Cover
                                    _TvMovieProgram.FanArt = _movie.FanArt
                                    _TvMovieProgram.FileName = _movie.FileName

                                    _TvMovieProgram.Persist()

                                    'MyLog.Info("enrichEPG: [GetMovingPicturesInfos]: repeat found -> create TvMovieProgram")
                                Else
                                    MyLog.Error("enrichEPG: [GetMovingPicturesInfos]: TvMovieProgram not found ! (idProgram: {0}, start: {1})", _program.IdProgram, _program.StartTime)
                                End If
                            End If
                            Catch ex3 As Exception
                            MyLog.Error("enrichEPG: [GetMovingPicturesInfos]: exception err: {0}, stack: {1}", ex3.Message, ex3.StackTrace)
                        End Try
                    Next
                End If
            Next
            MyLog.Info("")
            MyLog.[Info]("enrichEPG: [GetMovingPicturesInfos]: Summary: {0} MovingPictures Films found in {1} EPG entries ({2}s)", _MovieCounter, _EPGcounter, (DateTime.Now - _MovPicTimer).TotalSeconds)

        Catch ex As Exception
            MyLog.Error("enrichEPG: [GetMovingPicturesInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
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

                    'nach Mov.Pic Titel suchen
                    _SQLString = _
                        "Select * from program WHERE title LIKE '" & allowedSigns(_VideoDB(i).Title) & "' "

                    'nach Mov.Pic Titel über Dateiname suchen
                    If Not String.IsNullOrEmpty(_VideoDB(i).TitlebyFileName) Then
                        _SQLString = _SQLString & _
                        "OR title LIKE '" & allowedSigns(_VideoDB(i).TitlebyFileName) & "' " & _
                        "OR episodeName LIKE '" & allowedSigns(_VideoDB(i).TitlebyFileName) & "' "
                    End If

                    _SQLString = Replace(_SQLString, " * ", " Program.IdProgram, Program.Classification, Program.Description, Program.EndTime, Program.EpisodeName, Program.EpisodeNum, Program.EpisodePart, Program.Genre, Program.IdChannel, Program.OriginalAirDate, Program.ParentalRating, Program.SeriesNum, Program.StarRating, Program.StartTime, Program.state, Program.Title ")
                    Dim _SQLstate As SqlStatement = Broker.GetStatement(_SQLString)
                    Dim _Result As List(Of Program) = ObjectFactory.GetCollection(GetType(Program), _SQLstate.Execute())

                    If _Result.Count > 0 Then
                        MovieCounter = MovieCounter + 1
                    End If

                    For Each _program As Program In _Result

                        Try
                            'Daten im EPG (program) updaten

                            'Wenn TvMovieImportMovingPicturesInfos deaktiviert, dann Rating aus VideoDatabase nehmen
                            If MySettings.MovPicEnabled = False Then
                                _program.StarRating = _VideoDB(i).Rating
                            End If

                            If InStr(_program.Description, "existiert lokal") = 0 And String.IsNullOrEmpty(_program.SeriesNum) Then
                                _program.Description = "existiert lokal" & vbNewLine & _program.Description
                            End If

                            _program.Persist()

                            'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                            If MySettings.ClickfinderProgramGuideImportEnable = True Then

                                'ggf. einen TvMovieProgram Eintrag erstellen, wenn Source z.B. Epg grab
                                Try
                                    Dim _TvMovieProgramTest As TVMovieProgram = TVMovieProgram.Retrieve(_program.IdProgram)
                                Catch ex As Exception
                                    Dim _newTvMovieProgram As New TVMovieProgram(_program.IdProgram)
                                    _newTvMovieProgram.Persist()
                                End Try

                                'idProgram in TvMovieProgram suchen & Daten aktualisieren
                                Dim _TvMovieProgram As TVMovieProgram = TVMovieProgram.Retrieve(_program.IdProgram)
                                _TvMovieProgram.idVideo = _VideoDB(i).VideoID
                                _TvMovieProgram.local = True


                                If Not String.IsNullOrEmpty(_VideoDB(i).FileName) And String.IsNullOrEmpty(_TvMovieProgram.FileName) Then
                                    _TvMovieProgram.FileName = _VideoDB(i).FileName
                                End If

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
