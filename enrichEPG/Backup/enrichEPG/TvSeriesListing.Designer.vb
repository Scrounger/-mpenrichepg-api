<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TvSeriesListing
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
        Me.DGVSeriesList = New System.Windows.Forms.DataGridView
        Me.Button1 = New System.Windows.Forms.Button
        CType(Me.DGVSeriesList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DGVSeriesList
        '
        Me.DGVSeriesList.AllowUserToAddRows = False
        Me.DGVSeriesList.AllowUserToDeleteRows = False
        Me.DGVSeriesList.AllowUserToResizeRows = False
        Me.DGVSeriesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVSeriesList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVSeriesList.Location = New System.Drawing.Point(0, 0)
        Me.DGVSeriesList.Name = "DGVSeriesList"
        Me.DGVSeriesList.ReadOnly = True
        Me.DGVSeriesList.RowHeadersVisible = False
        Me.DGVSeriesList.Size = New System.Drawing.Size(305, 232)
        Me.DGVSeriesList.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button1.Location = New System.Drawing.Point(0, 232)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(305, 30)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TvSeriesListing
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(305, 262)
        Me.Controls.Add(Me.DGVSeriesList)
        Me.Controls.Add(Me.Button1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TvSeriesListing"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "TvSeriesListing"
        CType(Me.DGVSeriesList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DGVSeriesList As System.Windows.Forms.DataGridView
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
