Imports System.Collections.Generic
Imports System.Linq
Imports System.Xml.Linq
Imports System.Windows.Forms
Imports enrichEPG.TvDatabase
Imports TvDatabase
Imports enrichEPG.Database

Public Class seriesManagement

    Public Shared Label_idseries As String
    Public Shared Label_TvSeriesName As String

    Public Shared Label_idepisode As String
    Public Shared Label_TvSeriesEpisodeName As String

#Region "Form Events"

    Private Sub seriesManagement_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadSeriesMappings(0)
    End Sub

    Private Sub DGVseries_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVseries.CellClick
        Select Case (e.ColumnIndex)
            Case Is = 9
                Dim _SeriesMapping As TvMovieSeriesMapping = TvMovieSeriesMapping.Retrieve(DGVseries.Item(1, e.RowIndex).Value)

                _SeriesMapping.Remove()

                LoadSeriesMappings(e.RowIndex)

            Case Else
                LoadEpisodeMappings(CInt(DGVseries.Item(1, e.RowIndex).Value))

                L_idSeries.Text = DGVseries.Item(1, e.RowIndex).Value.ToString
                CheckDisable.Checked = CBool(DGVseries.Item(2, e.RowIndex).Value)
                TBSeriesName.Text = DGVseries.Item(3, e.RowIndex).Value.ToString
                tb_EPGTitle.Text = DGVseries.Item(4, e.RowIndex).Value.ToString
                NumericminSeriesNum.Value = CInt(DGVseries.Item(5, e.RowIndex).Value)

        End Select

    End Sub
    Private Sub DGVepisodes_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGVepisodes.CellClick

        L_idepisode.Text = String.Empty
        tb_epsidoeNameEPG.Text = String.Empty
        tb_epsidoeNameTvSeries.Text = String.Empty

        Select Case (e.ColumnIndex)
            Case Is = 9
                Dim _EpisodeMapping As TVMovieEpisodeMapping = TVMovieEpisodeMapping.Retrieve(DGVepisodes.Item(1, e.RowIndex).Value)

                _EpisodeMapping.Remove()

                LoadEpisodeMappings(DGVepisodes.Item(2, e.RowIndex).Value)

            Case Else
                L_idepisode.Text = DGVepisodes.Item(1, e.RowIndex).Value.ToString
                tb_epsidoeNameEPG.Text = DGVepisodes.Item(3, e.RowIndex).Value.ToString

                Dim _episode As MyTvSeries.MyEpisode = MyTvSeries.MyEpisode.Retrieve(DGVepisodes.Item(1, e.RowIndex).Value.ToString)
                tb_epsidoeNameTvSeries.Text = _episode.EpisodeName
        End Select
    End Sub

    'serien
    Private Sub BT_Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BT_Save.Click
        Try
            Dim _SeriesMapping As TvMovieSeriesMapping
            Try
                _SeriesMapping = TvMovieSeriesMapping.Retrieve(CInt(L_idSeries.Text))
            Catch ex As Exception
                _SeriesMapping = New TvMovieSeriesMapping(CInt(L_idSeries.Text))
            End Try

            _SeriesMapping.TvSeriesTitle = TBSeriesName.Text
            _SeriesMapping.EpgTitle = tb_EPGTitle.Text
            _SeriesMapping.minSeasonNum = NumericminSeriesNum.Value.ToString
            _SeriesMapping.disabled = CheckDisable.Checked

            _SeriesMapping.Persist()

            LoadSeriesMappings(DGVseries.CurrentCell.RowIndex)

        Catch ex As Exception
            MsgBox("Error: " & ex.Message & vbNewLine & vbNewLine & "stack: " & ex.StackTrace, MsgBoxStyle.Critical, "Error")
        End Try
    End Sub
    Private Sub BT_New_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BT_New.Click
        Dim _Window As New TvSeriesListing

        Dim _SeriesList As List(Of MyTvSeries) = MyTvSeries.ListAll

        _SeriesList = _SeriesList.FindAll(Function(x) FindidSeriesInDataGridView(x) = False)

        _Window.DGVSeriesList.AutoGenerateColumns = True
        _Window.DGVSeriesList.DataSource = _SeriesList

        _Window.DGVSeriesList.Columns(0).Visible = False
        _Window.DGVSeriesList.Columns(2).Visible = False
        _Window.DGVSeriesList.Columns(3).Visible = False
        _Window.DGVSeriesList.Columns(4).Visible = False
        _Window.DGVSeriesList.Columns(5).Visible = False
        _Window.DGVSeriesList.Columns(6).Visible = False
        _Window.DGVSeriesList.Columns(7).Visible = False
        _Window.DGVSeriesList.Columns(8).Visible = False
        _Window.DGVSeriesList.Columns(9).Visible = False
        _Window.DGVSeriesList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill



        _Window.Text = "Add new series mapping"

        TvSeriesListing.isNewEpisodeMapping = False

        _Window.ShowDialog()


        L_idSeries.Text = Label_idseries
        TBSeriesName.Text = Label_TvSeriesName
        CheckDisable.Checked = False
        tb_EPGTitle.Text = String.Empty
        NumericminSeriesNum.Value = 0
    End Sub

    'Episoden
    Private Sub BT_SaveEpisode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BT_SaveEpisode.Click

        If Not String.IsNullOrEmpty(L_idepisode.Text) Then

            Dim extractNums As String = Strings.Right(L_idepisode.Text, L_idepisode.Text.Length - InStr(L_idepisode.Text, "_"))

            Dim SeriesNum As Integer = CInt(Strings.Left(extractNums, InStr(extractNums, "x") - 1))
            Dim EpisodeNum As Integer = CInt(Strings.Right(extractNums, extractNums.Length - InStr(extractNums, "x")))

            Dim _Episode As TVMovieEpisodeMapping
            Dim _idSeries As Integer = CInt(Strings.Left(L_idepisode.Text, InStr(L_idepisode.Text, "_") - 1))
            Try
                _Episode = TVMovieEpisodeMapping.Retrieve(L_idepisode.Text)
            Catch ex As Exception
                _Episode = New TVMovieEpisodeMapping(L_idepisode.Text, _idSeries)
            End Try

            _Episode.EPGEpisodeName = tb_epsidoeNameEPG.Text
            _Episode.seriesNum = SeriesNum
            _Episode.episodeNum = EpisodeNum
            _Episode.Persist()

            LoadEpisodeMappings(_idSeries)

            L_idepisode.Text = String.Empty
            tb_epsidoeNameEPG.Text = String.Empty
            tb_epsidoeNameTvSeries.Text = String.Empty

        End If

    End Sub
    Private Sub BT_NewEpisode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BT_NewEpisode.Click

        Dim _Window As New TvSeriesListing
        Dim _idSeries As Integer
        Try
            _idSeries = CInt(Strings.Left(L_idepisode.Text, InStr(L_idepisode.Text, "_") - 1))
        Catch ex As Exception
            _idSeries = CInt(L_idSeries.Text)
        End Try

        Dim _EpisodeList As List(Of MyTvSeries.MyEpisode) = MyTvSeries.MyEpisode.ListAll(_idSeries)

        _EpisodeList = _EpisodeList.Where(Function(x) x.SeriesNum > 0).ToList
        _EpisodeList = _EpisodeList.OrderBy(Function(x) x.SeriesNum).ThenBy(Function(x) x.EpisodeNum).ToList()

        _EpisodeList = _EpisodeList.FindAll(Function(x) FindidEpisodeInDataGridView(x) = False)

        _Window.DGVSeriesList.AutoGenerateColumns = True
        _Window.DGVSeriesList.DataSource = _EpisodeList

        _Window.DGVSeriesList.Columns(0).Visible = False

        _Window.DGVSeriesList.Columns(4).Visible = False
        _Window.DGVSeriesList.Columns(5).Visible = False
        _Window.DGVSeriesList.Columns(6).Visible = False
        _Window.DGVSeriesList.Columns(7).Visible = False
        _Window.DGVSeriesList.Columns(8).Visible = False
        _Window.DGVSeriesList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _Window.DGVSeriesList.AutoResizeColumn(1, DataGridViewAutoSizeColumnMode.AllCells)
        '_Window.DGVSeriesList.Columns(2).Width
        ''_Window.DGVSeriesList.Columns(4).Width = 80


        _Window.Text = "Add new episode mapping"

        TvSeriesListing.isNewEpisodeMapping = True
        _Window.ShowDialog()

        L_idepisode.Text = Label_idepisode
        tb_epsidoeNameTvSeries.Text = Label_TvSeriesEpisodeName

        tb_epsidoeNameEPG.Text = String.Empty

    End Sub

#End Region

#Region "Functions"
    Private Sub LoadSeriesMappings(ByVal lastFoucsed As Integer)

        DGVseries.DataSource = Nothing
        DGVseries.Columns.Clear()

        'Alle Serien + idSeries in CB schreiben
        Dim _SeriesMappingList As IList(Of TvMovieSeriesMapping) = TvMovieSeriesMapping.ListAll
        _SeriesMappingList = _SeriesMappingList.OrderBy(Function(x As TvMovieSeriesMapping) x.TvSeriesTitle).ToList

        DGVseries.AutoGenerateColumns = True
        DGVseries.DataSource = _SeriesMappingList
        DGVseries.Columns("IsChanged").Visible = False
        DGVseries.Columns("CacheKey").Visible = False
        DGVseries.Columns("IsPersisted").Visible = False
        DGVseries.Columns("SessionBroker").Visible = False
        DGVseries.Columns("idSeries").Visible = False
        DGVseries.ReadOnly = True

        Dim _DeleteButton As New DataGridViewButtonColumn
        With _DeleteButton
            .DefaultCellStyle.Padding = New Padding(1, 1, 1, 1)
            .HeaderText = "Delete"
            .Text = "Delete"
            .Name = "Delete"
            '.FlatStyle = FlatStyle.Flat
            .UseColumnTextForButtonValue = True
            '_editbutton.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            .Width = 60
        End With
        Me.DGVseries.Columns.Add(_DeleteButton)

        DGVseries.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DGVseries.Columns(2).Width = 30
        DGVseries.AutoResizeColumn(3, DataGridViewAutoSizeColumnMode.DisplayedCells)
        DGVseries.Columns(5).Width = 30
        DGVseries.AutoResizeColumn(9, DataGridViewAutoSizeColumnMode.DisplayedCells)
        DGVseries.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

        L_idSeries.Text = DGVseries.Item(1, 0).Value.ToString
        CheckDisable.Checked = CBool(DGVseries.Item(2, 0).Value)
        TBSeriesName.Text = DGVseries.Item(3, 0).Value.ToString
        tb_EPGTitle.Text = DGVseries.Item(4, 0).Value.ToString
        NumericminSeriesNum.Value = CInt(DGVseries.Item(5, 0).Value)

        DGVseries.Rows(lastFoucsed).Cells(2).Selected = True

        L_idSeries.Text = DGVseries.Item(1, lastFoucsed).Value.ToString
        CheckDisable.Checked = CBool(DGVseries.Item(2, lastFoucsed).Value)
        TBSeriesName.Text = DGVseries.Item(3, lastFoucsed).Value.ToString
        tb_EPGTitle.Text = DGVseries.Item(4, lastFoucsed).Value.ToString
        NumericminSeriesNum.Value = CInt(DGVseries.Item(5, lastFoucsed).Value)

        LoadEpisodeMappings(_SeriesMappingList(0).idSeries)

    End Sub
    Private Sub LoadEpisodeMappings(ByVal idSeries As Integer)

        L_idepisode.Text = String.Empty
        tb_epsidoeNameEPG.Text = String.Empty
        tb_epsidoeNameTvSeries.Text = String.Empty

        DGVepisodes.DataSource = Nothing
        DGVepisodes.Columns.Clear()

        Dim _EpisodeMappingList As IList(Of TVMovieEpisodeMapping) = TVMovieEpisodeMapping.ListAll
        _EpisodeMappingList = _EpisodeMappingList.Where(Function(x As TVMovieEpisodeMapping) x.idSeries = idSeries).ToList
        _EpisodeMappingList = _EpisodeMappingList.OrderBy(Function(x As TVMovieEpisodeMapping) x.seriesNum).ThenBy(Function(x As TVMovieEpisodeMapping) x.episodeNum).ToList

        DGVepisodes.AutoGenerateColumns = True
        DGVepisodes.DataSource = _EpisodeMappingList
        DGVepisodes.Columns("IsChanged").Visible = False
        DGVepisodes.Columns("idSeries").Visible = False
        DGVepisodes.Columns("idepisode").Visible = False
        DGVepisodes.Columns("CacheKey").Visible = False
        DGVepisodes.Columns("IsPersisted").Visible = False
        DGVepisodes.Columns("SessionBroker").Visible = False

        Dim _DeleteButton As New DataGridViewButtonColumn
        With _DeleteButton
            .DefaultCellStyle.Padding = New Padding(1, 1, 1, 1)
            .HeaderText = "Delete"
            .Text = "Delete"
            .Name = "Delete"
            '.FlatStyle = FlatStyle.Flat
            .UseColumnTextForButtonValue = True
            '_editbutton.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            .Width = 60
        End With
        Me.DGVepisodes.Columns.Add(_DeleteButton)

        DGVepisodes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DGVepisodes.Columns(3).Width = 500
        DGVepisodes.Columns(7).Width = 80
        DGVepisodes.Columns(6).Width = 80
        DGVepisodes.AutoResizeColumn(9, DataGridViewAutoSizeColumnMode.DisplayedCells)
        DGVepisodes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

    End Sub
    Private Function FindidSeriesInDataGridView(ByVal series As MyTvSeries) As Boolean
        For i As Integer = 0 To DGVseries.Rows.Count - 1
            If series.idSeries = CInt(DGVseries.Item(1, i).Value) Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Function FindidEpisodeInDataGridView(ByVal episode As MyTvSeries.MyEpisode) As Boolean
        For i As Integer = 0 To DGVepisodes.Rows.Count - 1
            If episode.idEpisode = DGVepisodes.Item(1, i).Value Then
                Return True
            End If
        Next
        Return False
    End Function
#End Region


End Class
