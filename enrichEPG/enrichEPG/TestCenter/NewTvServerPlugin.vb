Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Text
Imports System.Reflection
Imports System.Threading
Imports System.Windows.Forms

Imports SetupTv
Imports TvControl
Imports TvEngine
Imports TvEngine.Events

Namespace TvEngine

    <CLSCompliant(False)> _
    Public Class NewTvServerPluginSetup

        Implements ITvServerPlugin

#Region "Properties"

        ''' <summary>
        ''' returns the name of the plugin
        ''' </summary>
        Public ReadOnly Property Name() As String Implements ITvServerPlugin.Name
            Get
                Return "ClickfinderEPGImport"
            End Get
        End Property

        ''' <summary>
        ''' returns the version of the plugin
        ''' </summary>
        Public ReadOnly Property Version() As String Implements ITvServerPlugin.Version
            Get
                Return "0.0.1.0"
            End Get
        End Property

        ''' <summary>
        ''' returns the author of the plugin
        ''' </summary>
        Public ReadOnly Property Author() As String Implements ITvServerPlugin.Author
            Get
                Return "Scrounger"
            End Get
        End Property

        ''' <summary>
        ''' returns if the plugin should only run on the master server
        ''' or also on slave servers
        ''' </summary>
        Public ReadOnly Property MasterOnly() As Boolean Implements ITvServerPlugin.MasterOnly
            Get
                Return True
            End Get
        End Property
#End Region

#Region "Methods"
        ''' <summary>
        ''' Starts the plugin
        ''' </summary>
        Public Sub Start(ByVal controller As TvControl.IController) Implements ITvServerPlugin.Start
            MsgBox("hallo")
        End Sub

        ''' <summary>
        ''' Stops the plugin
        ''' </summary>
        Public Sub [Stop]() Implements ITvServerPlugin.Stop

        End Sub

        ''' <summary>
        ''' returns the setup sections for display in SetupTv
        ''' </summary>
        Public ReadOnly Property Setup() As SectionSettings Implements ITvServerPlugin.Setup
            Get
                Return New SetupTv.Sections.NewTvServerPluginConfig
            End Get
        End Property
#End Region

    End Class
End Namespace

