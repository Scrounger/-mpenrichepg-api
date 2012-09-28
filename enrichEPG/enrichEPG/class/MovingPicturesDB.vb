﻿#Region "Copyright (C) 2005-2011 Team MediaPortal"

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

Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports TvLibrary.Log

Imports MediaPortal.Database
Imports SQLite.NET
Imports TvDatabase

Public Class MovingPicturesDB
    Implements IDisposable

#Region "Members"
    Private disposed As Boolean = False
    Private Shared m_db As SQLiteClient = Nothing
    Private Shared _MovingPicturesInfos As SQLiteResultSet
    Private _MovingPicturesID As Integer
    Private Shared _Index As Integer
#End Region

#Region "Constructors"
    Public Sub New()
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
                    MyLog.Debug("enrichEPG: [OpenMovingPicturesDB]: Disposing current instance..")
                Catch generatedExceptionName As Exception
                End Try
            End If


            ' Open database
            If File.Exists(EnrichEPG.MpDatabasePath & "\movingpictures.db3") = True Then

                m_db = New SQLiteClient(EnrichEPG.MpDatabasePath & "\movingpictures.db3")
                ' Retry 10 times on busy (DB in use or system resources exhausted)
                m_db.BusyRetries = 20
                ' Wait 100 ms between each try (default 10)
                m_db.BusyRetryDelay = 1000

                DatabaseUtility.SetPragmas(m_db)
            Else
                MyLog.[Error]("enrichEPG: [OpenMovingPicturesDB]: TvSeries Database not found: {0}", EnrichEPG.MpDatabasePath & "\movingpictures.db3")
            End If


        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [OpenMovingPicturesDB]: TvSeries Database exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenMovingPicturesDB()
        End Try
        'Mylog.Info("picture database opened")
    End Sub

    Public Sub LoadAllMovingPicturesFilms()

        Try
            _MovingPicturesInfos = m_db.Execute("SELECT * FROM movie_info ORDER BY title ASC")
            MyLog.Info("enrichEPG: [LoadAllMovingPicturesFilms]: success")
        Catch ex As Exception
            MyLog.[Error]("enrichEPG: [LoadAllMovingPicturesFilms]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenMovingPicturesDB()
        End Try

    End Sub
#End Region

#Region "Properties"
    Public ReadOnly Property Count() As Integer
        Get
            If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                Return _MovingPicturesInfos.Rows.Count
            Else
                Return 0
            End If
        End Get
    End Property

    'Get DBFields over Index
    Private _Item As New MovingPicturesItem
    Default Public ReadOnly Property MovingPictures(ByVal Index As Integer) As MovingPicturesItem
        Get
            _Index = Index
            Return _Item
        End Get
    End Property
    Public Class MovingPicturesItem
        Public ReadOnly Property MovingPicturesID() As Integer
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return CInt(DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "id"))
                Else
                    Return 0
                End If
            End Get
        End Property
        Public ReadOnly Property Title() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "title")
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property Cover() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then

                    Dim _Cover As String = DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "coverfullpath")
                    If _Cover.Length > 0 Then
                        Return Strings.Right(_Cover, _Cover.Length - Strings.InStr(_Cover, "\Thumbs\") - ("Thumbs\").Length)
                    Else
                        Return String.Empty
                    End If

                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property FanArt() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Dim _FanArt As String = DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "backdropfullpath")
                    If _FanArt.Length > 0 Then
                        Return Strings.Right(_FanArt, _FanArt.Length - Strings.InStr(_FanArt, "\Thumbs\") - ("Thumbs\").Length)
                    Else
                        Return String.Empty
                    End If
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property AlternateTitle() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return Replace(DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "alternate_titles"), "|", "")
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property TitlebyFilename() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Dim _idMovie_Info As Integer = CInt(DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "id"))
                    Dim _MovingPicturesTitleOverFilename As SQLiteResultSet


                    _MovingPicturesTitleOverFilename = m_db.Execute("SELECT * FROM local_media INNER JOIN local_media__movie_info ON local_media.id = local_media__movie_info.local_media_id WHERE movie_info_id = " & _idMovie_Info)
                    If _MovingPicturesTitleOverFilename IsNot Nothing AndAlso _MovingPicturesTitleOverFilename.Rows.Count > 0 Then
                        Return IO.Path.GetFileNameWithoutExtension(DatabaseUtility.[Get](_MovingPicturesTitleOverFilename, 0, "fullpath"))
                    Else
                        Return String.Empty
                    End If
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property Filename() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Dim _idMovie_Info As Integer = CInt(DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "id"))
                    Dim _MovingPicturesFilename As SQLiteResultSet

                    _MovingPicturesFilename = m_db.Execute("SELECT * FROM local_media INNER JOIN local_media__movie_info ON local_media.id = local_media__movie_info.local_media_id WHERE movie_info_id = " & _idMovie_Info)
                    If _MovingPicturesFilename IsNot Nothing AndAlso _MovingPicturesFilename.Rows.Count > 0 Then
                        Return DatabaseUtility.[Get](_MovingPicturesFilename, 0, "fullpath")
                    Else
                        Return String.Empty
                    End If
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property Rating() As Integer
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return CInt(Replace(DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "score"), ".", ","))
                Else
                    Return 0
                End If
            End Get
        End Property
    End Class

#End Region

#Region "IDisposable Members"

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not disposed Then
            disposed = True
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
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

#End Region

End Class
