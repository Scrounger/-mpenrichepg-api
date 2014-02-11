Imports TvdbLib
Imports TvdbLib.Cache
Imports TvdbLib.Data
Imports System.Windows.Forms

Public Class clsTheTVdb



    Private Const TVapi As String = "ABDB3586A95665E0"

    Private mtvLanguage As String = "de"
    Public Property tvLanguage() As String
        Get
            Return mtvLanguage
        End Get
        Set(ByVal value As String)
            mtvLanguage = value
        End Set
    End Property

    Private mDBLanguage As TvdbLanguage
    Public Property DBLanguage() As TvdbLanguage
        Get
            Return mDBLanguage
        End Get
        Set(ByVal value As TvdbLanguage)
            mDBLanguage = value
        End Set
    End Property

    Private mCachePath As String = MySettings.TheTvDbCacheFolder
    Public Property CachePath() As String
        Get
            Return mCachePath
        End Get
        Set(ByVal value As String)
            mCachePath = value
        End Set
    End Property

    Private mTheTVdbHandler As TvdbHandler
    Public Property TheTVdbHandler() As TvdbHandler
        Get
            Return mTheTVdbHandler
        End Get
        Set(ByVal value As TvdbHandler)
            mTheTVdbHandler = value
        End Set
    End Property


    Public Sub New(ByVal lang As String)
        tvLanguage = lang
        Dim prov As New XmlCacheProvider(CachePath)
        TheTVdbHandler = New TvdbHandler(prov, TVapi)
        Dim m_languages As List(Of TvdbLanguage) = TheTVdbHandler.Languages


        TheTVdbHandler.InitCache()
        Dim myLanguages As TvdbLanguage = Nothing

        For l = 0 To m_languages.Count - 1
            Debug.WriteLine(m_languages(l).Name)
            If m_languages(l).Abbriviation = tvLanguage Then myLanguages = m_languages(l)
        Next
        If myLanguages = Nothing Then myLanguages = TvdbLanguage.DefaultLanguage
        DBLanguage = myLanguages


    End Sub

    'Friend Sub SearchSeries(ByVal SeriesTitel As String)
    '    listSeries = TheTVdbHandler.SearchSeries(SeriesTitel)
    'End Sub

    'Friend Sub SetAktSerie(ByVal SeriesID As Integer)
    '    AktSerie = TheTVdbHandler.GetSeries(SeriesID, DBLanguage, True, False, False)
    'End Sub




End Class
