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
Imports enrichEPG.Database
Imports Databases
Imports enrichEPG
Imports enrichEPG.IdentifySeries
Imports Gentle.Framework
Imports enrichEPG.TvDatabase

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
                MySettings.SetSettings("C:\ProgramData\Team MediaPortal\MediaPortal\database", True, False, False, MySettings.LogPath.Server, "Area51.log")


                MsgBox(String.Compare("test", "TEst", True))

                Dim _list As List(Of MyTvSeries) = MyTvSeries.ListAll

                Dim _Series As MyTvSeries = MyTvSeries.Retrieve(73739)

                MsgBox(_Series.FanArt)

                'MyLog.Info("TESSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSST")

                'Dim _list As MyTvSeries = MyTvSeries.Retrieve(79349)

                ' _list.EpisodesList = _list.EpisodesList.Select(Function(x As MyTvSeries.MyEpisode) levenshtein(x.EpisodeName, "Jagdinstpinkt") <= 2 WHERE)

                '_list.EpisodesList = _list.EpisodesList.ToList.FindAll(Function(x) levenshtein(x.EpisodeName, "Jagdinstpinkt") <= 2)

                'MsgBox(_list.EpisodesList.Count & vbNewLine & MyTvSeries.Helper.logLevenstein)

                'MsgBox(MyTvSeries.MyEpisode.Search(79349, "Jagdinstpinkt").EpisodeName)

                'MsgBox(MyTvSeries.Search("De45 xter").Episode("Jagdi jnstinkt").EpisodeName)
            Catch ex As Exception
                MsgBox(ex.Message)
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
                    MySettings.LogPath.Server, _
                    "", , , _
                    "enrichTesting.log", True)
            _enrichEPG.start()

        End Sub

        Public Shared Function ReplaceSearchingString(ByVal expression As String) As String
            Return System.Text.RegularExpressions.Regex.Replace(expression, "[\:?,.!'-*()_]", "")
        End Function
        Public Shared Function levenshtein(ByVal a As [String], ByVal b As [String]) As Int32
            a = UCase(ReplaceSearchingString(a))
            b = UCase(ReplaceSearchingString(b))

            If String.IsNullOrEmpty(a) Then
                If Not String.IsNullOrEmpty(b) Then
                    Return b.Length
                End If
                Return 0
            End If

            If String.IsNullOrEmpty(b) Then
                If Not String.IsNullOrEmpty(a) Then
                    Return a.Length
                End If
                Return 0
            End If

            Dim cost As Int32
            Dim d As Int32(,) = New Integer(a.Length, b.Length) {}
            Dim min1 As Int32
            Dim min2 As Int32
            Dim min3 As Int32

            For i As Int32 = 0 To d.GetUpperBound(0)
                d(i, 0) = i
            Next

            For i As Int32 = 0 To d.GetUpperBound(1)
                d(0, i) = i
            Next

            For i As Int32 = 1 To d.GetUpperBound(0)
                For j As Int32 = 1 To d.GetUpperBound(1)
                    cost = Convert.ToInt32(Not (a(i - 1) = b(j - 1)))

                    min1 = d(i - 1, j) + 1
                    min2 = d(i, j - 1) + 1
                    min3 = d(i - 1, j - 1) + cost
                    d(i, j) = Math.Min(Math.Min(min1, min2), min3)
                Next
            Next

            Return d(d.GetUpperBound(0), d.GetUpperBound(1))

        End Function


        Private Shared _ImportStartTime As Date
        Private Shared _IdentifiedPrograms As New ArrayList
        Private Shared _ScheduledDummyRecordingList As New ArrayList
        Public Shared MyTVDBen As New clsTheTVdb("en")
        Public Shared MyTVDBlang As New clsTheTVdb("de")

        Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
            'Muss wieder raus
            MySettings.LogFileName = "Area51.log"
            MySettings.LogFilePath = MySettings.LogPath.Server
            MyLog.BackupLogFiles()
            MySettings.SetSettings("C:\ProgramData\Team MediaPortal\MediaPortal\database", True, False, False, MySettings.LogPath.Server, "Area51.log")
            '----------------

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

                        'Alle gefundenen Episoden der Serie durchlaufen
                        For Each _program As Program In _programList
                            Try
                                Dim _episodeFound As Boolean = False
                                Dim _logNewEpisode As Boolean = False


                                '---------------- TvSeriesDB --------------------------------
                                'Episode suchen
                                Try
                                    Dim _Episode As MyTvSeries.MyEpisode = _TvSeries.Episode(_program.EpisodeName)
                                    _episodeFound = True
                                    _EpisodeFoundCounter = _EpisodeFoundCounter + 1

                                    'UpdateProgramAndTvMovieProgram
                                    If IdentifySeries.UpdateProgramAndTvMovieProgram(_program, _TvSeries, _Episode, _Episode.ExistLocal, True) = False Then
                                        _logNewEpisode = True
                                        _CounterNewEpisode = _CounterNewEpisode + 1
                                    End If

                                    MyLog.Info("enrichEPG: [GetSeriesInfos]: TvSeriesDB: S{0}E{1} - {2} (newEpisode: {3}{4})", _
                                                _Episode.SeriesNum, _Episode.EpisodeNum, _program.EpisodeName, _logNewEpisode, MyTvSeries.Helper.logLevenstein)
                                Catch ex As Exception
                                    _episodeFound = False
                                    'Falls Episode nicht gefunden wird, Exception abfangen
                                End Try


                                '---------------- TheTvDB --------------------------------
                                'auf TheTvDb.com nach episode suchen (language: lang)
                                Try
                                    If _episodeFound = False And MySettings.useTheTvDb = True Then

                                        'Daten von TheTvDb für Serie downloaden / cache
                                        If _NextSeries = True Then
                                            'MyLog.Info("enrichEPG: [GetSeriesInfos]: Daten von TheTvDb.com holen: {0} (idSeries: {1})", _TvSeriesDB(i).SeriesName, _TvSeriesDB(i).SeriesID)
                                            _NextSeries = False
                                            IdentifySeries.SeriesEN = MyTVDBen.TheTVdbHandler.GetSeries(_TvSeries.idSeries, MyTVDBen.DBLanguage, True, False, False)
                                            IdentifySeries.SeriesLang = MyTVDBlang.TheTVdbHandler.GetSeries(_TvSeries.idSeries, MyTVDBlang.DBLanguage, True, False, False)
                                        End If

                                        If IdentifySeries.TheTvDbEpisodeIdentify(_program) = True Then
                                            'Try
                                            'Episode auf TheTvDb gefunden
                                            Dim _Episode As MyTvSeries.MyEpisode = _TvSeries.Episode(IdentifySeries.IdentifiedEpisode.SeasonNumber, IdentifySeries.IdentifiedEpisode.EpisodeNumber)
                                            _episodeFound = True
                                            _EpisodeFoundCounter = _EpisodeFoundCounter + 1

                                            'UpdateProgramAndTvMovieProgram2
                                            If IdentifySeries.UpdateProgramAndTvMovieProgram(_program, _TvSeries, _Episode, _Episode.ExistLocal, True) = False Then
                                                _logNewEpisode = True
                                                _CounterNewEpisode = _CounterNewEpisode + 1
                                            End If

                                            MyLog.Info("enrichEPG: [GetSeriesInfos]: TheTvDb.com ({0}): S{1}E{2} - {3} (newEpisode: {4}{5})", IdentifySeries.IdentifiedEpisode.Language.Abbriviation, _
                                              IdentifySeries.IdentifiedEpisode.SeasonNumber, IdentifySeries.IdentifiedEpisode.EpisodeNumber, _program.EpisodeName, _logNewEpisode, IdentifySeries.logLevenstein)
                                            'Catch exZ As Exception
                                            '    _episodeFound = False
                                            '    'Falls Episode nicht gefunden wird, Exception abfangen
                                            'End Try
                                        End If
                                    End If
                                Catch exTheTvDb As Exception
                                    _episodeFound = False
                                    MyLog.[Error]("enrichEPG: [GetSeriesInfos]: TheTvDB identifier error !")
                                    MyLog.[Error]("enrichEPG: [GetSeriesInfos]: exception err:{0} stack:{1}", exTheTvDb.Message, exTheTvDb.StackTrace)
                                End Try


                                '---------------- TvMovieEpisodeMapping --------------------------------
                                'Verlinkung in TvMovieEpisodeMapping suchen
                                Try
                                    If _episodeFound = False Then

                                        'Nach Verlinkung suchen
                                        'Zunächst nach alle Serien mit SerienName aus TvSeries DB suchen
                                        _SQLString = "Select * from TvMovieEpisodeMapping " & _
                                        "WHERE idSeries = " & _TvSeries.idSeries & " " & _
                                        "AND EPGEpisodeName LIKE '%" & MyTvSeries.Helper.allowedSigns(_program.EpisodeName) & "%'"

                                        '_SQLString = Replace(_SQLString, " * ", " Program.IdProgram, Program.Classification, Program.Description, Program.EndTime, Program.EpisodeName, Program.EpisodeNum, Program.EpisodePart, Program.Genre, Program.IdChannel, Program.OriginalAirDate, Program.ParentalRating, Program.SeriesNum, Program.StarRating, Program.StartTime, Program.state, Program.Title ")
                                        Dim _SQLstate2 As SqlStatement = Broker.GetStatement(_SQLString)
                                        Dim _EpisodeMappingList As List(Of TVMovieEpisodeMapping) = ObjectFactory.GetCollection(GetType(TVMovieEpisodeMapping), _SQLstate2.Execute())
                                        'Mapping EpisodeName gefunden
                                        If _EpisodeMappingList.Count > 0 Then
                                            _episodeFound = True
                                            _EpisodeFoundCounter = _EpisodeFoundCounter + 1

                                            'Try
                                            'UpdateProgramAndTvMovieProgram
                                            Dim _Episode As MyTvSeries.MyEpisode = _TvSeries.Episode(_EpisodeMappingList(0).seriesNum, _EpisodeMappingList(0).episodeNum)
                                            If IdentifySeries.UpdateProgramAndTvMovieProgram(_program, _TvSeries, _Episode, _Episode.ExistLocal, True) = False Then
                                                _logNewEpisode = True
                                                _CounterNewEpisode = _CounterNewEpisode + 1
                                            End If
                                            MyLog.Info("enrichEPG: [GetSeriesInfos]: TvMovieEpisodeMapping: S{0}E{1} - {2} (newEpisode: {3})", _
                                              _Episode.SeriesNum, _Episode.EpisodeNum, _program.EpisodeName, _logNewEpisode)
                                            'Catch ex As Exception
                                            '    _episodeFound = False
                                            '    'Falls Episode nicht gefunden wird, Exception abfangen
                                            'End Try
                                        End If
                                    End If
                                Catch exMap As Exception
                                    _episodeFound = False
                                    MyLog.[Error]("enrichEPG: [GetSeriesInfos]: TvMovieEpisodeMapping identifier error !")
                                    MyLog.[Error]("enrichEPG: [GetSeriesInfos]: exception err:{0} stack:{1}", exMap.Message, exMap.StackTrace)
                                End Try


                                '---------------- Nicht gefunden --------------------------------
                                If _episodeFound = False Then

                                    If IdentifySeries.UpdateProgramAndTvMovieProgram(_program, _TvSeries, Nothing, False, False) = True Then
                                        _logNewEpisode = True
                                        _CounterNewEpisode = _CounterNewEpisode + 1
                                    End If

                                    If Not _lastEpisodeName = _program.EpisodeName Then
                                        MyLog.Warn("enrichEPG: [GetSeriesInfos]: {0} ({1}, {2}), episode: {3} - not identified -> marked as New Episode (local = 0)", _TvSeries.Title, _TvSeries.idSeries, _program.ReferencedChannel.DisplayName, _program.EpisodeName)
                                        _EpisodeIdentified = False
                                    End If

                                    _ScheduldedDummyRecording = _program
                                    _lastEpisodeName = _program.EpisodeName
                                End If

                                'Episode identifiziert -> idProgram speichern, benötigt damit CheckEpisodenscanner nicht ncohmal ausgeführt wird
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
                            MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0}: {1} episodes found in EPG", _MappedSeriesNames.Item(z), _MappedSeries.Count)
                        End If
                    Next
                End If

                If _TvMovieSeriesMapping.disabled = True Then
                    MyLog.[Info]("enrichEPG: [GetSeriesInfos]: {0} is disabled (local = true) !", _TvMovieSeriesMapping.TvSeriesTitle)
                End If
            Catch SeriesMappingEx As Exception
                'Exception wenn keine mappings gefunden
                'MyLog.[Info]("enrichEPG: [GetSeriesInfos]: SeriesMapping Error: {0}, stack: {1}", SeriesMappingEx.Message, SeriesMappingEx.StackTrace)
            End Try

            Return _ProgramList

        End Function

    End Class
End Namespace

