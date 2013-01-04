#Region "Copyright (C) 2005-2011 Team MediaPortal"

' Copyright (C) 2005-2011 Team MediaPortal
' http://www.team-mediaportal.com
' 
' MediaPortal is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 2 of the License, or
' (at your option) any later version.
' 
' MediaPortal is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.IO
Imports SQLite.NET
Imports System.Runtime.CompilerServices

Imports MediaPortal.Database
Imports TvDatabase

Namespace Database
    Public Class MyTvSeries

#Region "Member"
        Private Shared _onLineSeriesCoulumns As String = "ID, Pretty_Name, origName, PosterBannerFileName, Summary, Rating, Network, Status, (Select LocalPath FROM Fanart WHERE seriesID = online_series.ID AND LocalPath LIKE '_%') as FanArt, (SELECT SeasonIndex FROM season Where SeriesID = online_series.ID order by SeasonIndex DESC Limit 1) as SeasonCount, EpisodeCount"
        Private Shared _SqlSeriesConstructor As String = String.Format("Select {0} FROM online_series", _onLineSeriesCoulumns)
#End Region

#Region "Properties"
#Region "Values"
        Private m_idSeries As Integer
        Private m_title As String
        Private m_SeriesorigName As String
        Private m_SeriesPosterImage As String
        Private m_Summary As String
        Private m_Rating As Integer
        Private m_Network As String
        Private m_Status As String
        Private m_EpisodesList As New List(Of MyTvSeries.MyEpisode)
        Private m_Episode As MyTvSeries.MyEpisode
        Private m_FanArt As String
        Private m_EpisodeCount As Integer
        Private m_SeasonCount As Integer
#End Region
        Public Property idSeries() As Integer
            Get
                Return m_idSeries
            End Get
            Set(ByVal value As Integer)
                m_idSeries = value
            End Set
        End Property
        Public Property Title() As String
            Get
                Return m_title
            End Get
            Set(ByVal value As String)
                m_title = value
            End Set
        End Property
        Public Property SeriesorigName() As String
            Get
                Return m_SeriesorigName
            End Get
            Set(ByVal value As String)
                m_SeriesorigName = value
            End Set
        End Property
        Public Property SeriesPosterImage() As String
            Get
                Return m_SeriesPosterImage
            End Get
            Set(ByVal value As String)
                m_SeriesPosterImage = value
            End Set
        End Property
        Public Property Summary() As String
            Get
                Return m_Summary
            End Get
            Set(ByVal value As String)
                m_Summary = value
            End Set
        End Property
        Public Property Rating() As Integer
            Get
                Return m_Rating
            End Get
            Set(ByVal value As Integer)
                m_Rating = value
            End Set
        End Property
        Public Property Network() As String
            Get
                Return m_Network
            End Get
            Set(ByVal value As String)
                m_Network = value
            End Set
        End Property
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property
        Public Property FanArt() As String
            Get
                Return m_FanArt
            End Get
            Set(ByVal value As String)
                m_FanArt = value
            End Set
        End Property

        ''' <summary>
        ''' Alle Episoden der Serie (ggf. aus TvSeriesDB laden)
        ''' </summary>
        Public Property EpisodesList() As IList(Of MyTvSeries.MyEpisode)
            Get
                Select Case (m_EpisodesList.Count)
                    Case Is = 0
                        m_EpisodesList = MyEpisode.ListAll(idSeries)
                        Return m_EpisodesList
                    Case Else
                        If m_EpisodesList(0).SeriesID = idSeries Then
                            Return m_EpisodesList
                        Else
                            m_EpisodesList = MyEpisode.ListAll(idSeries)
                            Return m_EpisodesList
                        End If
                End Select
            End Get
            Set(ByVal value As IList(Of MyTvSeries.MyEpisode))
                m_EpisodesList = value
            End Set
        End Property

        ''' <summary>
        ''' Episode der Serie (über SeriesNum und EpisodeNum)
        ''' </summary>
        Public Property Episode(ByVal SeriesNum As Integer, ByVal EpisodeNum As Integer) As MyTvSeries.MyEpisode
            Get
                If m_EpisodesList.Count = 0 Then
                    Return MyTvSeries.MyEpisode.Retrieve(idSeries, SeriesNum, EpisodeNum)
                Else
                    Dim _tmpList As List(Of MyTvSeries.MyEpisode) = m_EpisodesList.FindAll(Function(x) x.SeriesNum = SeriesNum AndAlso x.EpisodeNum = EpisodeNum)

                    If _tmpList.Count > 0 Then
                        Return _tmpList(0)
                    End If
                End If

                Throw New Exception(String.Format("MyTvSeries.Episode: Episode 'S{0}E{1}' not found in TvSeries database!", SeriesNum, EpisodeNum))
            End Get
            Set(ByVal value As MyTvSeries.MyEpisode)
                m_Episode = value
            End Set
        End Property
        ''' <summary>
        ''' Episode der Serie suchen (über EpisodenName)
        ''' </summary>
        Public Property Episode(ByVal EpisodeName As String) As MyTvSeries.MyEpisode
            Get
                If m_EpisodesList.Count = 0 Then
                    Helper.logLevenstein = String.Empty
                    Return MyTvSeries.MyEpisode.Search(idSeries, EpisodeName)
                Else
                    Dim _tmpList As List(Of MyTvSeries.MyEpisode) = m_EpisodesList.FindAll(Function(x) _
                        String.Compare(Helper.ReplaceSearchingString(x.EpisodeName), Helper.ReplaceSearchingString(EpisodeName), True) = 0)

                    If _tmpList.Count > 0 Then
                        Helper.logLevenstein = String.Empty
                        Return _tmpList(0)
                    Else
                        _tmpList = m_EpisodesList.FindAll(Function(x) Helper.levenshtein(x.EpisodeName, EpisodeName) <= 2)
                        If _tmpList.Count > 0 Then
                            Helper.logLevenstein = String.Format(", levenshtein: {0}", _tmpList(0).EpisodeName)
                            Return _tmpList(0)
                        Else
                            Helper.logLevenstein = String.Empty
                        End If
                    End If
                End If

                Throw New Exception(String.Format("MyTvSeries.Episode: Episode '{0}' not found in TvSeries database!", EpisodeName))
            End Get
            Set(ByVal value As MyTvSeries.MyEpisode)
                m_Episode = value
            End Set
        End Property

        Public Property SeasonCount() As Integer
            Get
                Return m_SeasonCount
            End Get
            Set(ByVal value As Integer)
                m_SeasonCount = value
            End Set
        End Property
        Public Property EpisodeCount() As Integer
            Get
                Return m_EpisodeCount
            End Get
            Set(ByVal value As Integer)
                m_EpisodeCount = value
            End Set
        End Property
#End Region

#Region "Functions"

        ''' <summary>
        ''' FanArt Pfad aus TvSeriesDB laden, nicht vorhanden = string.empty
        ''' </summary>

        Public Sub LoadAllEpisodes()
            m_EpisodesList = MyTvSeries.MyEpisode.ListAll(idSeries)
        End Sub
#End Region

#Region "Retrieval"
        ''' <summary>
        ''' Alle Serien aus TvSeriesDB laden, ORDER BY Title ASC
        ''' </summary>
        Public Shared Function ListAll() As IList(Of MyTvSeries)
            Dim _SqlString As String = String.Format("{0} WHERE ID > 0 ORDER BY Pretty_Name", _
                                                     _SqlSeriesConstructor)

            Return Helper.GetSeries(_SqlString)
        End Function

        ''' <summary>
        ''' einzelne Serie über idSeries aus TvSeriesDB laden
        ''' </summary>
        Public Shared Function Retrieve(ByVal idSeries As Integer) As MyTvSeries
            Dim _SqlString As String = String.Format("{0} WHERE ID = {1}", _
                                                     _SqlSeriesConstructor, idSeries)
            Return Helper.GetSeries(_SqlString).Item(0)
        End Function

        ''' <summary>
        ''' einzelne Serie suchen (levenshtein)
        ''' </summary>
        Public Shared Function Search(ByVal SeriesName As String) As MyTvSeries
            Dim _SqlString As String = String.Format("{0} WHERE Pretty_Name LIKE '{1}' OR SortName LIKE '{1}' OR origName LIKE '{1}'", _
                                    _SqlSeriesConstructor, Helper.allowedSigns(SeriesName))
            Try
                'Direkt gefunden, SeriesName identisch
                Return Helper.GetSeries(_SqlString).Item(0)
            Catch ex As Exception
                'Nicht gefunden, levenshtein Vergleich
                Dim _SeriesList As List(Of MyTvSeries) = MyTvSeries.ListAll.ToList
                Dim _tmpList As New List(Of MyTvSeries)

                _tmpList = _SeriesList.FindAll(Function(x) Helper.levenshtein(x.Title, SeriesName) <= 2)

                If _tmpList.Count = 1 Then
                    Return _tmpList(0)
                Else
                    'Keine Übereinstimmung, prüfen ob "-" in SerienName, danach alles abschneiden und levenshtein
                    If InStr(SeriesName, "-") > 0 Then
                        'SeriesName: alles nach "-" rauswerfen, ggf. " " am Ende entfernen
                        SeriesName = Microsoft.VisualBasic.Left(SeriesName, InStr(SeriesName, "-") - 1)
                        If SeriesName(SeriesName.Length - 1) = " " Then
                            SeriesName = Microsoft.VisualBasic.Left(SeriesName, SeriesName.Length - 1)
                        End If

                        _tmpList = _SeriesList.FindAll(Function(x) Helper.levenshtein(x.Title, SeriesName) <= 2)

                        If _tmpList.Count = 1 Then
                            Return _tmpList(0)
                        End If
                    End If
                End If

                Throw New Exception(String.Format("MyTvSeries.Search: Series '{0}' not found in TvSeries database!", SeriesName))
            End Try
        End Function
#End Region

        '----------------------------------------------------------------------------------------------------------------------
#Region "Class MyEpisode"
        Public Class MyEpisode

#Region "Members"
            Private Const _onLineEpisodesCoulumns As String = _
                    "online_episodes.CompositeID, online_episodes.EpisodeName, online_episodes.SeasonIndex, online_episodes.EpisodeIndex, online_episodes.SeriesID, online_episodes.Rating, online_episodes.thumbFilename, local_episodes.IsAvailable, local_episodes.EpisodeFilename"

            Private Shared _SqlEpisodeConstructor As String = _
                    String.Format("Select {0} FROM online_episodes LEFT JOIN local_episodes ON online_episodes.CompositeID = local_episodes.CompositeID", _onLineEpisodesCoulumns)
#End Region

#Region "Properties"
#Region "Values"
            Private m_CompositeID As String
            Private m_SeriesID As Integer
            Private m_SeriesNum As Integer
            Private m_EpisodeNum As Integer
            Private m_Rating As Integer
            Private m_ThumbFilename As String
            Private m_EpisodeName As String
            Private m_IsAvailable As Boolean
            Private m_EpisodeFilename As String
#End Region
            Public Property idEpisode() As String
                Get
                    Return m_CompositeID
                End Get
                Set(ByVal value As String)
                    m_CompositeID = value
                End Set
            End Property
            Public Property EpisodeName() As String
                Get
                    Return m_EpisodeName
                End Get
                Set(ByVal value As String)
                    m_EpisodeName = value
                End Set
            End Property
            Public Property SeriesNum() As Integer
                Get
                    Return m_SeriesNum
                End Get
                Set(ByVal value As Integer)
                    m_SeriesNum = value
                End Set
            End Property
            Public Property EpisodeNum() As Integer
                Get
                    Return m_EpisodeNum
                End Get
                Set(ByVal value As Integer)
                    m_EpisodeNum = value
                End Set
            End Property
            Public Property SeriesID() As Integer
                Get
                    Return m_SeriesID
                End Get
                Set(ByVal value As Integer)
                    m_SeriesID = value
                End Set
            End Property
            Public Property Rating() As Integer
                Get
                    Return m_Rating
                End Get
                Set(ByVal value As Integer)
                    m_Rating = value
                End Set
            End Property
            Public Property ThumbFilename() As String
                Get
                    Return m_ThumbFilename
                End Get
                Set(ByVal value As String)
                    m_ThumbFilename = value
                End Set
            End Property
            Public Property ExistLocal() As Boolean
                Get
                    Return m_IsAvailable
                End Get
                Set(ByVal value As Boolean)
                    m_IsAvailable = value
                End Set
            End Property
            Public Property EpisodeFilename() As String
                Get
                    Return m_EpisodeFilename
                End Get
                Set(ByVal value As String)
                    m_EpisodeFilename = value
                End Set
            End Property

#End Region

#Region "Retrieval"
            ''' <summary>
            ''' Alle Episoden einer Serie laden (über idSeries)
            ''' </summary>
            Public Shared Function ListAll(ByVal idseries As Integer) As IList(Of MyTvSeries.MyEpisode)
                Dim _SqlString As String = String.Format("{0} WHERE online_episodes.SeriesID = {1} ORDER BY online_episodes.SeasonIndex ASC, online_episodes.EpisodeIndex ASC", _
                                                         _SqlEpisodeConstructor, idseries)

                Return Helper.GetEpisodes(_SqlString)
            End Function

            ''' <summary>
            ''' Episode einer Serie laden (über idSeries, SeriesNum und EpisodeNum
            ''' </summary>
            Public Shared Function Retrieve(ByVal idSeries As Integer, ByVal SeriesNum As Integer, ByVal EpisodeNum As Integer) As MyTvSeries.MyEpisode
                Dim _SqlString As String = String.Format("{0} WHERE online_episodes.SeriesID = {1} AND online_episodes.SeasonIndex = {2} AND online_episodes.EpisodeIndex = {3} ORDER BY online_episodes.SeasonIndex ASC, online_episodes.EpisodeIndex ASC", _
                                                         _SqlEpisodeConstructor, idSeries, SeriesNum, EpisodeNum)

                'Daten aus TvSeriesDB laden
                Dim _con As New ConnectDB(_SqlString)
                Dim _Result As SQLiteResultSet = _con.Execute
                _con.Dispose()

                Return Helper.GetEpisodes(_SqlString).Item(0)
            End Function

            ''' <summary>
            ''' Episode einer Serie laden (über idEpisode (CompositeID)) 
            ''' </summary>
            Public Shared Function Retrieve(ByVal idEpisode As String) As MyTvSeries.MyEpisode
                Dim _SqlString As String = String.Format("{0} WHERE online_episodes.CompositeID = '{1}' ORDER BY online_episodes.SeasonIndex ASC, online_episodes.EpisodeIndex ASC", _
                                                          _SqlEpisodeConstructor, idEpisode)
                'Daten aus TvSeriesDB laden
                Dim _con As New ConnectDB(_SqlString)
                Dim _Result As SQLiteResultSet = _con.Execute
                _con.Dispose()

                Return Helper.GetEpisodes(_SqlString).Item(0)
            End Function

            ''' <summary>
            ''' Episode einer Serie suchen (über idSeries, EpisodeName (levenshtein))
            ''' </summary>
            Public Shared Function Search(ByVal idSeries As Integer, ByVal EpisodeName As String) As MyTvSeries.MyEpisode
                Dim _SqlString As String = String.Format("{0} WHERE online_episodes.SeriesID = {1} AND online_episodes.EpisodeName LIKE '{2}' ORDER BY online_episodes.SeasonIndex ASC, online_episodes.EpisodeIndex ASC", _
                                                         _SqlEpisodeConstructor, idSeries, EpisodeName)
                Try
                    'Direkt gefunden, EpisodeName identisch
                    Helper.logLevenstein = String.Empty
                    Return Helper.GetEpisodes(_SqlString).Item(0)
                Catch ex As Exception
                    'Nicht gefunden, levenshtein Vergleich
                    Dim _EpisodeList As List(Of MyTvSeries.MyEpisode) = MyTvSeries.MyEpisode.ListAll(idSeries).ToList

                    _EpisodeList = _EpisodeList.FindAll(Function(x) Helper.levenshtein(x.EpisodeName, EpisodeName) <= 2)

                    If _EpisodeList.Count = 1 Then
                        If Not _EpisodeList(0).EpisodeName = EpisodeName Then
                            Helper.logLevenstein = String.Format(", levenshtein: {0}", _EpisodeList(0).EpisodeName)
                        Else
                            Helper.logLevenstein = String.Empty
                        End If

                        Return _EpisodeList(0)
                    Else
                        'Keine Übereinstimmung, Exception werfen
                        Helper.logLevenstein = String.Empty
                        Throw New Exception(String.Format("MyTvSeries.MyEpisode.Search: Episode '{0}' not found in TvSeries database!", EpisodeName))
                    End If

                End Try
            End Function
#End Region
        End Class
#End Region

        '----------------------------------------------------------------------------------------------------------------------
#Region "Class ConnectDB"
        Public Class ConnectDB
            Implements IDisposable
#Region "Members"
            Private _disposed As Boolean = False
            Private _SqlString As String = String.Empty
            Private m_db As SQLiteClient = Nothing
#End Region

#Region "Constructors"
            Public Sub New(ByVal SQLstring As String)
                _SqlString = SQLstring
                OpenTvSeriesDB()
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)> _
            Private Sub OpenTvSeriesDB()
                Try
                    ' Maybe called by an exception
                    If m_db IsNot Nothing Then
                        Try
                            m_db.Close()
                            m_db.Dispose()
                            MyLog.Debug("enrichEPG: [MyTvSeries]: [OpenTvSeriesDB]: Disposing current instance..")
                        Catch generatedExceptionName As Exception
                        End Try
                    End If

                    ' Open database
                    Dim layer As New TvBusinessLayer
                    If File.Exists(MySettings.MpDatabasePath & "\TVSeriesDatabase4.db3") = True Then

                        m_db = New SQLiteClient(MySettings.MpDatabasePath & "\TVSeriesDatabase4.db3")
                        ' Retry 10 times on busy (DB in use or system resources exhausted)
                        m_db.BusyRetries = 20
                        ' Wait 100 ms between each try (default 10)
                        m_db.BusyRetryDelay = 1000

                        DatabaseUtility.SetPragmas(m_db)
                        'MyLog.Debug("enrichEPG: [MyTvSeries]: [OpenTvSeriesDB]: Data readed from TvSeries database")
                    Else
                        MyLog.Error("enrichEPG: [MyTvSeries]: [OpenTvSeriesDB]: TvSeries Database not found: {0}", MySettings.MpDatabasePath & "\TVSeriesDatabase4.db3")
                    End If

                Catch ex As Exception
                    MyLog.Error("enrichEPG: [MyTvSeries]: [OpenTvSeriesDB]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    OpenTvSeriesDB()
                End Try
                'Mylog.Info("picture database opened")
            End Sub
#End Region

#Region "Functions"
            Public Function Execute() As SQLiteResultSet
                Try
                    Return m_db.Execute(_SqlString)
                Catch ex As Exception
                    MyLog.Error("[MyTvSeries]: [Execute]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    Return Nothing
                End Try
            End Function
#End Region

#Region " IDisposable Support "
            ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
            Public Sub Dispose() Implements IDisposable.Dispose
                If Not _disposed Then
                    _disposed = True
                    If m_db IsNot Nothing Then
                        Try
                            m_db.Close()
                            m_db.Dispose()
                        Catch generatedExceptionName As Exception
                        End Try
                        m_db = Nothing
                    End If
                End If
            End Sub
#End Region

        End Class
#End Region

        '----------------------------------------------------------------------------------------------------------------------
#Region "Class Helper"
        Public Class Helper
            Public Shared Function allowedSigns(ByVal expression As String) As String
                Return Replace(System.Text.RegularExpressions.Regex.Replace(expression, "[\:?,.!-*]", "_"), "'", "''")
            End Function
            ''' <summary>
            ''' Daten aus table online_series laden
            ''' </summary>
            Public Shared Function GetSeries(ByVal SQLstring As String) As IList(Of MyTvSeries)
                'Daten aus TvSeriesDB laden
                Dim _con As New ConnectDB(SQLstring)
                Dim _Result As SQLiteResultSet = _con.Execute
                _con.Dispose()

                Return ConvertToMyTvSeriesList(_Result)
            End Function
            ''' <summary>
            ''' Daten aus table online_episodes laden
            ''' </summary>
            Public Shared Function GetEpisodes(ByVal SQLstring As String) As IList(Of MyTvSeries.MyEpisode)
                'Daten aus TvSeriesDB laden
                Dim _con As New ConnectDB(SQLstring)
                Dim _Result As SQLiteResultSet = _con.Execute
                _con.Dispose()

                Return ConvertToEpisodeList(_Result)
            End Function

            Public Shared Function ReplaceSearchingString(ByVal expression As String) As String
                Return Replace(System.Text.RegularExpressions.Regex.Replace(expression, "[\:?,.!-*_]", ""), "'", "''")
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

            'Convert SQLiteResultSet -> IList(Of MyTvSeries)
            Private Shared Function ConvertToMyTvSeriesList(ByVal Result As SQLiteResultSet) As IList(Of MyTvSeries)
                Return Result.Rows.ConvertAll(Of MyTvSeries)(New Converter(Of SQLiteResultSet.Row, MyTvSeries)(Function(c As SQLiteResultSet.Row) New MyTvSeries() With { _
                            .idSeries = c.fields(0), _
                            .Title = c.fields(1), _
                            .SeriesorigName = c.fields(2), _
                            .SeriesPosterImage = If(String.IsNullOrEmpty(c.fields(8)), String.Empty, c.fields(3)), _
                            .Summary = c.fields(4), _
                            .Rating = CInt(Replace(c.fields(5), ".", ",")), _
                            .Network = c.fields(6), _
                            .Status = c.fields(7), _
                            .FanArt = If(String.IsNullOrEmpty(c.fields(8)), String.Empty, "Fan Art\" & c.fields(8)), _
                            .SeasonCount = CInt(c.fields(9)), _
                            .EpisodeCount = CInt(c.fields(10))}))
            End Function
            'Convert SQLiteResultSet -> IList(Of MyTvSeries.Episode)
            Private Shared Function ConvertToEpisodeList(ByVal Result As SQLiteResultSet) As IList(Of MyTvSeries.MyEpisode)
                Return Result.Rows.ConvertAll(Of MyTvSeries.MyEpisode)(New Converter(Of SQLiteResultSet.Row, MyTvSeries.MyEpisode)(Function(c As SQLiteResultSet.Row) New MyTvSeries.MyEpisode() With { _
                        .idEpisode = c.fields(0), _
                        .EpisodeName = c.fields(1), _
                        .SeriesNum = CInt(c.fields(2)), _
                        .EpisodeNum = CInt(c.fields(3)), _
                        .SeriesID = CInt(c.fields(4)), _
                        .Rating = CInt(Replace(c.fields(5), ".", ",")), _
                        .ThumbFilename = c.fields(6), _
                        .ExistLocal = CBool(If(String.IsNullOrEmpty(c.fields(7)), False, CBool(c.fields(7)))), _
                        .EpisodeFilename = c.fields(8)}))
                'Exception wenn ExistLocal Null, deshalb if abfangen
            End Function

            Private Shared _logLevenstein As String
            Public Shared Property logLevenstein() As String
                Get
                    Return _logLevenstein
                End Get
                Set(ByVal value As String)
                    _logLevenstein = value
                End Set
            End Property

        End Class
#End Region

    End Class
End Namespace