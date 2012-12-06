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
Imports enrichEPG

Namespace Database
    Public Class MyMovingPictures

#Region "Member"
        Private Shared _movie_InfoCoulumns As String = "id, title, alternate_titles, (SELECT fullpath FROM local_media INNER JOIN local_media__movie_info ON local_media.id = local_media__movie_info.local_media_id WHERE movie_info_id = movie_Info.id) as FileName, year, score, certification, backdropfullpath, coverfullpath"
        Private Shared _SqlMovPicConstructor As String = String.Format("Select {0} FROM movie_Info", _movie_InfoCoulumns)
#End Region

#Region "Properties"

#Region "Values"
        Private m_ID As Integer
        Private m_Title As String
        Private m_AlternateTitles As String
        Private m_TitleByFileName As String
        Private m_Filename As String
        Private m_year As Date
        Private m_Rating As Integer
        Private m_Certification As Integer
        Private m_FanArt As String
        Private m_Cover As String

#End Region
        Public Property ID() As Integer
            Get
                Return m_ID
            End Get
            Set(ByVal value As Integer)
                m_ID = value
            End Set
        End Property
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal value As String)
                m_Title = value
            End Set
        End Property
        Public Property AlternateTitles() As String
            Get
                Return m_AlternateTitles
            End Get
            Set(ByVal value As String)
                m_AlternateTitles = value
            End Set
        End Property
        Public Property TitleByFileName()
            Get
                Return m_TitleByFileName
            End Get
            Set(ByVal value)
                m_TitleByFileName = value
            End Set
        End Property
        Public Property FileName() As String
            Get
                Return m_Filename
            End Get
            Set(ByVal value As String)
                m_Filename = value
            End Set
        End Property
        Public Property year() As Date
            Get
                Return m_year
            End Get
            Set(ByVal value As Date)
                m_year = value
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
        Public Property Certification() As Integer
            Get
                Return m_Certification
            End Get
            Set(ByVal value As Integer)
                m_Certification = value
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
        Public Property Cover() As String
            Get
                Return m_Cover
            End Get
            Set(ByVal value As String)
                m_Cover = value
            End Set
        End Property

#End Region

#Region "Retrieval"
        ''' <summary>
        ''' Alle Serien aus TvSeriesDB laden, ORDER BY Title ASC
        ''' </summary>
        Public Shared Function ListAll() As IList(Of MyMovingPictures)
            Dim _SqlString As String = String.Format("{0} ORDER BY title", _
                                                     _SqlMovPicConstructor)

            Return Helper.GetMovies(_SqlString)
        End Function
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
                OpenMovingPicturesDB()
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)> _
    Private Sub OpenMovingPicturesDB()
                Try
                    ' Maybe called by an exception
                    If m_db IsNot Nothing Then
                        Try
                            m_db.Close()
                            m_db.Dispose()
                            MyLog.Debug("enrichEPG: [MyMovingPictures]: Disposing current instance..")
                        Catch generatedExceptionName As Exception
                        End Try
                    End If


                    ' Open database
                    If File.Exists(MySettings.MpDatabasePath & "\movingpictures.db3") = True Then

                        m_db = New SQLiteClient(MySettings.MpDatabasePath & "\movingpictures.db3")
                        ' Retry 10 times on busy (DB in use or system resources exhausted)
                        m_db.BusyRetries = 20
                        ' Wait 100 ms between each try (default 10)
                        m_db.BusyRetryDelay = 1000

                        DatabaseUtility.SetPragmas(m_db)
                    Else
                        MyLog.[Error]("enrichEPG: [MyMovingPictures]: TvSeries Database not found: {0}", MySettings.MpDatabasePath & "\movingpictures.db3")
                    End If


                Catch ex As Exception
                    MyLog.[Error]("enrichEPG: [MyMovingPictures]: TvSeries Database exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    OpenMovingPicturesDB()
                End Try
                'Mylog.Info("picture database opened")
            End Sub
#End Region

#Region "Functions"
            Public Function Execute() As SQLiteResultSet
                Try
                    Return m_db.Execute(_SqlString)
                Catch ex As Exception
                    MyLog.Error("[MyMovingPictures]: [Execute]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
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
                Return Replace(Replace(System.Text.RegularExpressions.Regex.Replace(expression, "[\?]", "_"), "|", ""), "'", "''")
            End Function

            ''' <summary>
            ''' Daten aus table movie_Info laden
            ''' </summary>
            Public Shared Function GetMovies(ByVal SQLstring As String) As IList(Of MyMovingPictures)
                'Daten aus TvSeriesDB laden
                Dim _con As New ConnectDB(SQLstring)
                Dim _Result As SQLiteResultSet = _con.Execute
                _con.Dispose()


                Return ConvertToMyMovingPicturesList(_Result)
            End Function
            Private Shared Function ConvertToMyMovingPicturesList(ByVal Result As SQLiteResultSet) As IList(Of MyMovingPictures)
                Return Result.Rows.ConvertAll(Of MyMovingPictures)(New Converter(Of SQLiteResultSet.Row, MyMovingPictures)(Function(c As SQLiteResultSet.Row) New MyMovingPictures() With { _
                            .ID = c.fields(0), _
                            .Title = c.fields(1), _
                            .AlternateTitles = c.fields(2), _
                            .TitleByFileName = IO.Path.GetFileNameWithoutExtension(c.fields(3)), _
                            .FileName = c.fields(3), _
                            .year = CDate("01.01." & c.fields(4)), _
                            .Rating = CInt(Replace(c.fields(5), ".", ",")), _
                            .Certification = If(IsNumeric(c.fields(6)), CInt(c.fields(6)), 0), _
                            .FanArt = If(InStr(c.fields(7), "C:\ProgramData\Team MediaPortal\MediaPortal\Thumbs\") > 0, Replace(c.fields(7), "C:\ProgramData\Team MediaPortal\MediaPortal\Thumbs\", ""), String.Empty), _
                            .Cover = If(InStr(c.fields(8), "C:\ProgramData\Team MediaPortal\MediaPortal\Thumbs\") > 0, Replace(c.fields(8), "C:\ProgramData\Team MediaPortal\MediaPortal\Thumbs\", ""), String.Empty)}))

            End Function
        End Class
#End Region

    End Class
End Namespace
