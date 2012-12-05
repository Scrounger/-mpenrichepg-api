Imports System.Windows.Forms
Imports SetupTv

Namespace SetupTv.Sections

    Partial Class NewTvServerPluginConfig
        Inherits SectionSettings

        'Wird vom Windows Form-Designer benötigt.
        Private components As System.ComponentModel.IContainer

        'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

        'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
        'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
        'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.TabClickfinderEPGImporter = New System.Windows.Forms.TabControl
            Me.TabSettings = New System.Windows.Forms.TabPage
            Me.Button2 = New System.Windows.Forms.Button
            Me.Button1 = New System.Windows.Forms.Button
            Me.TabMapping = New System.Windows.Forms.TabPage
            Me.TvLogosList = New System.Windows.Forms.ImageList(Me.components)
            Me.Button3 = New System.Windows.Forms.Button
            Me.TabClickfinderEPGImporter.SuspendLayout()
            Me.TabSettings.SuspendLayout()
            Me.SuspendLayout()
            '
            'TabClickfinderEPGImporter
            '
            Me.TabClickfinderEPGImporter.Controls.Add(Me.TabSettings)
            Me.TabClickfinderEPGImporter.Controls.Add(Me.TabMapping)
            Me.TabClickfinderEPGImporter.Location = New System.Drawing.Point(3, 3)
            Me.TabClickfinderEPGImporter.Name = "TabClickfinderEPGImporter"
            Me.TabClickfinderEPGImporter.SelectedIndex = 0
            Me.TabClickfinderEPGImporter.Size = New System.Drawing.Size(470, 414)
            Me.TabClickfinderEPGImporter.TabIndex = 3
            '
            'TabSettings
            '
            Me.TabSettings.Controls.Add(Me.Button3)
            Me.TabSettings.Controls.Add(Me.Button2)
            Me.TabSettings.Controls.Add(Me.Button1)
            Me.TabSettings.Location = New System.Drawing.Point(4, 22)
            Me.TabSettings.Name = "TabSettings"
            Me.TabSettings.Padding = New System.Windows.Forms.Padding(3)
            Me.TabSettings.Size = New System.Drawing.Size(462, 388)
            Me.TabSettings.TabIndex = 0
            Me.TabSettings.Text = "Settings"
            Me.TabSettings.UseVisualStyleBackColor = True
            '
            'Button2
            '
            Me.Button2.Location = New System.Drawing.Point(193, 33)
            Me.Button2.Name = "Button2"
            Me.Button2.Size = New System.Drawing.Size(99, 52)
            Me.Button2.TabIndex = 1
            Me.Button2.Text = "Button2"
            Me.Button2.UseVisualStyleBackColor = True
            '
            'Button1
            '
            Me.Button1.Location = New System.Drawing.Point(241, 121)
            Me.Button1.Name = "Button1"
            Me.Button1.Size = New System.Drawing.Size(97, 38)
            Me.Button1.TabIndex = 0
            Me.Button1.Text = "Button1"
            Me.Button1.UseVisualStyleBackColor = True
            '
            'TabMapping
            '
            Me.TabMapping.Location = New System.Drawing.Point(4, 22)
            Me.TabMapping.Name = "TabMapping"
            Me.TabMapping.Padding = New System.Windows.Forms.Padding(3)
            Me.TabMapping.Size = New System.Drawing.Size(462, 388)
            Me.TabMapping.TabIndex = 1
            Me.TabMapping.Text = "Map Channels"
            Me.TabMapping.UseVisualStyleBackColor = True
            '
            'TvLogosList
            '
            Me.TvLogosList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
            Me.TvLogosList.ImageSize = New System.Drawing.Size(60, 40)
            Me.TvLogosList.TransparentColor = System.Drawing.Color.Transparent
            '
            'Button3
            '
            Me.Button3.Location = New System.Drawing.Point(241, 254)
            Me.Button3.Name = "Button3"
            Me.Button3.Size = New System.Drawing.Size(138, 73)
            Me.Button3.TabIndex = 2
            Me.Button3.Text = "Button3"
            Me.Button3.UseVisualStyleBackColor = True
            '
            'NewTvServerPluginConfig
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TabClickfinderEPGImporter)
            Me.Name = "NewTvServerPluginConfig"
            Me.Size = New System.Drawing.Size(476, 420)
            Me.TabClickfinderEPGImporter.ResumeLayout(False)
            Me.TabSettings.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TabClickfinderEPGImporter As System.Windows.Forms.TabControl
        Friend WithEvents TabSettings As System.Windows.Forms.TabPage
        Friend WithEvents TabMapping As System.Windows.Forms.TabPage
        Friend WithEvents TvLogosList As System.Windows.Forms.ImageList
        Friend WithEvents Button1 As System.Windows.Forms.Button
        Friend WithEvents Button2 As System.Windows.Forms.Button
        Friend WithEvents Button3 As System.Windows.Forms.Button
    End Class
End Namespace
