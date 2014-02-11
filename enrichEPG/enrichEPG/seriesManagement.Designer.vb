<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class seriesManagement
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.DGVepisodes = New System.Windows.Forms.DataGridView()
        Me.DGVseries = New System.Windows.Forms.DataGridView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TBSeriesName = New System.Windows.Forms.TextBox()
        Me.CheckDisable = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.L_idSeries = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tb_EPGTitle = New System.Windows.Forms.TextBox()
        Me.NumericminSeriesNum = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.BT_New = New System.Windows.Forms.Button()
        Me.BT_Save = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.Panel9 = New System.Windows.Forms.Panel()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.BT_NewEpisode = New System.Windows.Forms.Button()
        Me.BT_SaveEpisode = New System.Windows.Forms.Button()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.tb_epsidoeNameTvSeries = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.L_idepisode = New System.Windows.Forms.Label()
        Me.label20 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.tb_epsidoeNameEPG = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.Label8 = New System.Windows.Forms.Label()
        CType(Me.DGVepisodes, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVseries, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.NumericminSeriesNum, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel5.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.Panel9.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox5.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DGVepisodes
        '
        Me.DGVepisodes.AllowUserToAddRows = False
        Me.DGVepisodes.AllowUserToDeleteRows = False
        Me.DGVepisodes.AllowUserToResizeRows = False
        Me.DGVepisodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVepisodes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVepisodes.Location = New System.Drawing.Point(0, 0)
        Me.DGVepisodes.MultiSelect = False
        Me.DGVepisodes.Name = "DGVepisodes"
        Me.DGVepisodes.ReadOnly = True
        Me.DGVepisodes.RowHeadersVisible = False
        Me.DGVepisodes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVepisodes.Size = New System.Drawing.Size(662, 235)
        Me.DGVepisodes.TabIndex = 75
        '
        'DGVseries
        '
        Me.DGVseries.AllowUserToAddRows = False
        Me.DGVseries.AllowUserToDeleteRows = False
        Me.DGVseries.AllowUserToResizeRows = False
        Me.DGVseries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVseries.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVseries.Location = New System.Drawing.Point(0, 0)
        Me.DGVseries.MultiSelect = False
        Me.DGVseries.Name = "DGVseries"
        Me.DGVseries.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DGVseries.RowHeadersVisible = False
        Me.DGVseries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVseries.Size = New System.Drawing.Size(661, 235)
        Me.DGVseries.TabIndex = 74
        '
        'Panel1
        '
        Me.Panel1.AutoSize = True
        Me.Panel1.Controls.Add(Me.GroupBox1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Padding = New System.Windows.Forms.Padding(10)
        Me.Panel1.Size = New System.Drawing.Size(1161, 288)
        Me.Panel1.TabIndex = 76
        '
        'GroupBox1
        '
        Me.GroupBox1.AutoSize = True
        Me.GroupBox1.Controls.Add(Me.SplitContainer1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(10, 10)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(10)
        Me.GroupBox1.Size = New System.Drawing.Size(1141, 268)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Series Mapping"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Top
        Me.SplitContainer1.Location = New System.Drawing.Point(10, 23)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.DGVseries)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.GroupBox3)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Panel3)
        Me.SplitContainer1.Panel2.Padding = New System.Windows.Forms.Padding(10, 2, 10, 6)
        Me.SplitContainer1.Size = New System.Drawing.Size(1121, 235)
        Me.SplitContainer1.SplitterDistance = 661
        Me.SplitContainer1.TabIndex = 76
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.TableLayoutPanel1)
        Me.GroupBox3.Controls.Add(Me.Panel4)
        Me.GroupBox3.Controls.Add(Me.Panel5)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox3.Location = New System.Drawing.Point(10, 2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(436, 227)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.TBSeriesName, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.CheckDisable, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.L_idSeries, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.tb_EPGTitle, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.NumericminSeriesNum, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 5)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(430, 172)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'TBSeriesName
        '
        Me.TBSeriesName.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TBSeriesName.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBSeriesName.Location = New System.Drawing.Point(80, 16)
        Me.TBSeriesName.Name = "TBSeriesName"
        Me.TBSeriesName.ReadOnly = True
        Me.TBSeriesName.Size = New System.Drawing.Size(347, 21)
        Me.TBSeriesName.TabIndex = 2
        '
        'CheckDisable
        '
        Me.CheckDisable.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckDisable.AutoSize = True
        Me.CheckDisable.Location = New System.Drawing.Point(80, 43)
        Me.CheckDisable.Name = "CheckDisable"
        Me.CheckDisable.Size = New System.Drawing.Size(347, 17)
        Me.CheckDisable.TabIndex = 7
        Me.CheckDisable.Text = " (= not mark new Episodes)"
        Me.CheckDisable.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(3, 40)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(71, 23)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "Disable"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(71, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "idSeries:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'L_idSeries
        '
        Me.L_idSeries.AutoSize = True
        Me.L_idSeries.Dock = System.Windows.Forms.DockStyle.Fill
        Me.L_idSeries.Location = New System.Drawing.Point(80, 0)
        Me.L_idSeries.Name = "L_idSeries"
        Me.L_idSeries.Size = New System.Drawing.Size(347, 13)
        Me.L_idSeries.TabIndex = 1
        Me.L_idSeries.Text = "#idSeries"
        Me.L_idSeries.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(71, 27)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "TvSeries title:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 89)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(71, 26)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "EPG title:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tb_EPGTitle
        '
        Me.tb_EPGTitle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tb_EPGTitle.Location = New System.Drawing.Point(80, 92)
        Me.tb_EPGTitle.Name = "tb_EPGTitle"
        Me.tb_EPGTitle.Size = New System.Drawing.Size(347, 20)
        Me.tb_EPGTitle.TabIndex = 5
        '
        'NumericminSeriesNum
        '
        Me.NumericminSeriesNum.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.NumericminSeriesNum.Location = New System.Drawing.Point(80, 66)
        Me.NumericminSeriesNum.Name = "NumericminSeriesNum"
        Me.NumericminSeriesNum.Size = New System.Drawing.Size(61, 20)
        Me.NumericminSeriesNum.TabIndex = 9
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 63)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(71, 26)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "min season:"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label4, 2)
        Me.Label4.Location = New System.Drawing.Point(80, 115)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(347, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "use ""|"" as seperator ( without empty spaces between mapping names !!!)"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel4
        '
        Me.Panel4.AutoSize = True
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(3, 16)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(430, 0)
        Me.Panel4.TabIndex = 8
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.BT_New)
        Me.Panel5.Controls.Add(Me.BT_Save)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel5.Location = New System.Drawing.Point(3, 188)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Padding = New System.Windows.Forms.Padding(0, 6, 0, 0)
        Me.Panel5.Size = New System.Drawing.Size(430, 36)
        Me.Panel5.TabIndex = 9
        '
        'BT_New
        '
        Me.BT_New.AutoSize = True
        Me.BT_New.Dock = System.Windows.Forms.DockStyle.Left
        Me.BT_New.Location = New System.Drawing.Point(0, 6)
        Me.BT_New.Name = "BT_New"
        Me.BT_New.Size = New System.Drawing.Size(88, 30)
        Me.BT_New.TabIndex = 1
        Me.BT_New.Text = "New"
        Me.BT_New.UseVisualStyleBackColor = True
        '
        'BT_Save
        '
        Me.BT_Save.AutoSize = True
        Me.BT_Save.Dock = System.Windows.Forms.DockStyle.Right
        Me.BT_Save.Location = New System.Drawing.Point(342, 6)
        Me.BT_Save.Name = "BT_Save"
        Me.BT_Save.Size = New System.Drawing.Size(88, 30)
        Me.BT_Save.TabIndex = 0
        Me.BT_Save.Text = "Save"
        Me.BT_Save.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.AutoSize = True
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(10, 2)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(436, 0)
        Me.Panel3.TabIndex = 7
        '
        'GroupBox2
        '
        Me.GroupBox2.AutoSize = True
        Me.GroupBox2.Controls.Add(Me.SplitContainer2)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Location = New System.Drawing.Point(10, 10)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(10)
        Me.GroupBox2.Size = New System.Drawing.Size(1141, 268)
        Me.GroupBox2.TabIndex = 0
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Episode Mapping"
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Top
        Me.SplitContainer2.Location = New System.Drawing.Point(10, 23)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.DGVepisodes)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.GroupBox6)
        Me.SplitContainer2.Size = New System.Drawing.Size(1121, 235)
        Me.SplitContainer2.SplitterDistance = 662
        Me.SplitContainer2.TabIndex = 76
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.Panel9)
        Me.GroupBox6.Controls.Add(Me.TableLayoutPanel3)
        Me.GroupBox6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox6.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(455, 235)
        Me.GroupBox6.TabIndex = 1
        Me.GroupBox6.TabStop = False
        '
        'Panel9
        '
        Me.Panel9.Controls.Add(Me.Button3)
        Me.Panel9.Controls.Add(Me.BT_NewEpisode)
        Me.Panel9.Controls.Add(Me.BT_SaveEpisode)
        Me.Panel9.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel9.Location = New System.Drawing.Point(3, 196)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Padding = New System.Windows.Forms.Padding(0, 6, 0, 0)
        Me.Panel9.Size = New System.Drawing.Size(449, 36)
        Me.Panel9.TabIndex = 9
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(175, 6)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(82, 29)
        Me.Button3.TabIndex = 2
        Me.Button3.Text = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'BT_NewEpisode
        '
        Me.BT_NewEpisode.AutoSize = True
        Me.BT_NewEpisode.Dock = System.Windows.Forms.DockStyle.Left
        Me.BT_NewEpisode.Location = New System.Drawing.Point(0, 6)
        Me.BT_NewEpisode.Name = "BT_NewEpisode"
        Me.BT_NewEpisode.Size = New System.Drawing.Size(88, 30)
        Me.BT_NewEpisode.TabIndex = 1
        Me.BT_NewEpisode.Text = "New"
        Me.BT_NewEpisode.UseVisualStyleBackColor = True
        '
        'BT_SaveEpisode
        '
        Me.BT_SaveEpisode.AutoSize = True
        Me.BT_SaveEpisode.Dock = System.Windows.Forms.DockStyle.Right
        Me.BT_SaveEpisode.Location = New System.Drawing.Point(361, 6)
        Me.BT_SaveEpisode.Name = "BT_SaveEpisode"
        Me.BT_SaveEpisode.Size = New System.Drawing.Size(88, 30)
        Me.BT_SaveEpisode.TabIndex = 0
        Me.BT_SaveEpisode.Text = "Save"
        Me.BT_SaveEpisode.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.AutoSize = True
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel3.Controls.Add(Me.tb_epsidoeNameTvSeries, 1, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.Label11, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.L_idepisode, 1, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.label20, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.Label14, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.tb_epsidoeNameEPG, 1, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.Label9, 0, 3)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 4
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(449, 216)
        Me.TableLayoutPanel3.TabIndex = 0
        '
        'tb_epsidoeNameTvSeries
        '
        Me.tb_epsidoeNameTvSeries.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tb_epsidoeNameTvSeries.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tb_epsidoeNameTvSeries.Location = New System.Drawing.Point(85, 34)
        Me.tb_epsidoeNameTvSeries.Name = "tb_epsidoeNameTvSeries"
        Me.tb_epsidoeNameTvSeries.ReadOnly = True
        Me.tb_epsidoeNameTvSeries.Size = New System.Drawing.Size(361, 21)
        Me.tb_epsidoeNameTvSeries.TabIndex = 2
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label11.Location = New System.Drawing.Point(3, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(76, 30)
        Me.Label11.TabIndex = 0
        Me.Label11.Text = "idepisode:"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'L_idepisode
        '
        Me.L_idepisode.AutoSize = True
        Me.L_idepisode.Dock = System.Windows.Forms.DockStyle.Fill
        Me.L_idepisode.Location = New System.Drawing.Point(85, 0)
        Me.L_idepisode.Name = "L_idepisode"
        Me.L_idepisode.Size = New System.Drawing.Size(361, 30)
        Me.L_idepisode.TabIndex = 1
        Me.L_idepisode.Text = "#idEpisode"
        Me.L_idepisode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'label20
        '
        Me.label20.AutoSize = True
        Me.label20.Dock = System.Windows.Forms.DockStyle.Fill
        Me.label20.Location = New System.Drawing.Point(3, 30)
        Me.label20.Name = "label20"
        Me.label20.Size = New System.Drawing.Size(76, 30)
        Me.label20.TabIndex = 2
        Me.label20.Text = "EpisodeName:"
        Me.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label14.Location = New System.Drawing.Point(3, 60)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(76, 30)
        Me.Label14.TabIndex = 4
        Me.Label14.Text = "EPG title:"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tb_epsidoeNameEPG
        '
        Me.tb_epsidoeNameEPG.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tb_epsidoeNameEPG.Location = New System.Drawing.Point(85, 63)
        Me.tb_epsidoeNameEPG.Name = "tb_epsidoeNameEPG"
        Me.tb_epsidoeNameEPG.Size = New System.Drawing.Size(361, 20)
        Me.tb_epsidoeNameEPG.TabIndex = 5
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.TableLayoutPanel3.SetColumnSpan(Me.Label9, 2)
        Me.Label9.Location = New System.Drawing.Point(99, 90)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(347, 13)
        Me.Label9.TabIndex = 6
        Me.Label9.Text = "use ""|"" as seperator ( without empty spaces between mapping names !!!)"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel2
        '
        Me.Panel2.AutoSize = True
        Me.Panel2.Controls.Add(Me.GroupBox2)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 288)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Padding = New System.Windows.Forms.Padding(10)
        Me.Panel2.Size = New System.Drawing.Size(1161, 288)
        Me.Panel2.TabIndex = 77
        '
        'GroupBox4
        '
        Me.GroupBox4.AutoSize = True
        Me.GroupBox4.Location = New System.Drawing.Point(8, 8)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(200, 100)
        Me.GroupBox4.TabIndex = 0
        Me.GroupBox4.TabStop = False
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Top
        Me.SplitContainer3.Location = New System.Drawing.Point(10, 23)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.DataGridView1)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Padding = New System.Windows.Forms.Padding(10, 2, 10, 6)
        Me.SplitContainer3.Size = New System.Drawing.Size(150, 100)
        Me.SplitContainer3.TabIndex = 0
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.AllowUserToResizeRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.MultiSelect = False
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(50, 100)
        Me.DataGridView1.TabIndex = 74
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Panel6)
        Me.GroupBox5.Controls.Add(Me.Panel7)
        Me.GroupBox5.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(200, 100)
        Me.GroupBox5.TabIndex = 0
        Me.GroupBox5.TabStop = False
        '
        'Panel6
        '
        Me.Panel6.AutoSize = True
        Me.Panel6.Controls.Add(Me.Label7)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel6.Location = New System.Drawing.Point(3, 16)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(194, 14)
        Me.Panel6.TabIndex = 8
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(42, 1)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(347, 13)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "use ""|"" as seperator ( without empty spaces between mapping names !!!)"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Panel7
        '
        Me.Panel7.Controls.Add(Me.Button1)
        Me.Panel7.Controls.Add(Me.Button2)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel7.Location = New System.Drawing.Point(3, 68)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Padding = New System.Windows.Forms.Padding(0, 6, 0, 0)
        Me.Panel7.Size = New System.Drawing.Size(194, 29)
        Me.Panel7.TabIndex = 9
        '
        'Button1
        '
        Me.Button1.AutoSize = True
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Button1.Location = New System.Drawing.Point(0, 6)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(88, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "New"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.AutoSize = True
        Me.Button2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button2.Location = New System.Drawing.Point(106, 6)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(88, 23)
        Me.Button2.TabIndex = 0
        Me.Button2.Text = "Save"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.AutoSize = True
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.Controls.Add(Me.TextBox1, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.CheckBox1, 1, 2)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 3
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(200, 100)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.Location = New System.Drawing.Point(3, 23)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(435, 21)
        Me.TextBox1.TabIndex = 2
        '
        'CheckBox1
        '
        Me.CheckBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(3, 61)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(435, 17)
        Me.CheckBox1.TabIndex = 7
        Me.CheckBox1.Text = " (= not mark new Episodes)"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Location = New System.Drawing.Point(3, 60)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(71, 30)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "Disable"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'seriesManagement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(1161, 577)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "seriesManagement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "seriesManagement"
        CType(Me.DGVepisodes, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVseries, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.NumericminSeriesNum, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.Panel9.ResumeLayout(False)
        Me.Panel9.PerformLayout()
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DGVepisodes As System.Windows.Forms.DataGridView
    Friend WithEvents DGVseries As System.Windows.Forms.DataGridView
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents tb_EPGTitle As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CheckDisable As System.Windows.Forms.CheckBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents NumericminSeriesNum As System.Windows.Forms.NumericUpDown
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents BT_New As System.Windows.Forms.Button
    Friend WithEvents BT_Save As System.Windows.Forms.Button
    Friend WithEvents L_idSeries As System.Windows.Forms.Label
    Friend WithEvents TBSeriesName As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Panel9 As System.Windows.Forms.Panel
    Friend WithEvents BT_NewEpisode As System.Windows.Forms.Button
    Friend WithEvents BT_SaveEpisode As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tb_epsidoeNameTvSeries As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents L_idepisode As System.Windows.Forms.Label
    Friend WithEvents label20 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents tb_epsidoeNameEPG As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Button3 As System.Windows.Forms.Button
End Class
