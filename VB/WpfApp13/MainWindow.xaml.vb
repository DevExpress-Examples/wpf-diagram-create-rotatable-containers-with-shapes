Imports DevExpress.Diagram.Core
Imports DevExpress.Diagram.Core.Native
Imports DevExpress.Xpf.Diagram
Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media

Namespace WpfApp13

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    ''' 
    Public Class CustomDiagramContainer
        Inherits DiagramContainer

        Shared Sub New()
            Call CanRotateProperty.OverrideMetadata(GetType(CustomDiagramContainer), New FrameworkPropertyMetadata(True, Nothing, Function(d, v) v))
        End Sub
    End Class

    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            Me.InitializeComponent()
            DiagramControl.ItemTypeRegistrator.Register(GetType(CustomDiagramContainer))
            Me.diagramControl1.Items.Add(CreateContainerShape1())
            AddHandler Me.diagramControl1.BeforeItemsRotating, AddressOf Me.DiagramControl1_BeforeItemsRotating
            AddHandler Me.diagramControl1.ItemsRotating, AddressOf Me.DiagramControl1_ItemsRotating
            AddHandler Loaded, AddressOf Me.MainWindow_Loaded
        End Sub

        Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.diagramControl1.FitToItems(Me.diagramControl1.Items)
        End Sub

        Private Sub DiagramControl1_BeforeItemsRotating(ByVal sender As Object, ByVal e As DiagramBeforeItemsRotatingEventArgs)
            Dim containers = e.Items.OfType(Of CustomDiagramContainer)()
            For Each container In containers
                e.Items.Remove(container)
                For Each item In container.Items
                    e.Items.Add(item)
                Next
            Next
        End Sub

        Private Sub DiagramControl1_ItemsRotating(ByVal sender As Object, ByVal e As DiagramItemsRotatingEventArgs)
            Dim groups = e.Items.GroupBy(Function(x) x.Item.ParentItem)
            Dim container As CustomDiagramContainer = Nothing
            For Each group In groups
                If CSharpImpl.__Assign(container, TryCast(group.Key, CustomDiagramContainer)) IsNot Nothing Then
                    Dim containingRect = container.Items.[Select](Function(x) x.RotatedDiagramBounds().BoundedRect()).Aggregate(Rect.Empty, New Func(Of Rect, Rect, Rect)(AddressOf Rect.Union))
                    container.Position = New Point(containingRect.X, containingRect.Y)
                    container.Width = CSng(containingRect.Width)
                    container.Height = CSng(containingRect.Height)
                End If
            Next
        End Sub

        Public Function CreateContainerShape1() As CustomDiagramContainer
            Dim container = New CustomDiagramContainer() With {.Width = 200, .Height = 200, .Position = New Point(100, 100), .CanAddItems = False, .ItemsCanChangeParent = False, .ItemsCanCopyWithoutParent = False, .ItemsCanDeleteWithoutParent = False, .ItemsCanAttachConnectorBeginPoint = False, .ItemsCanAttachConnectorEndPoint = False}
            container.StrokeThickness = 0
            container.Background = Brushes.Transparent
            Dim innerShape1 = New DiagramShape() With {.CanSelect = True, .CanChangeParent = False, .CanEdit = True, .CanResize = False, .CanCopyWithoutParent = False, .CanDeleteWithoutParent = False, .CanMove = False, .Shape = BasicShapes.Trapezoid, .Height = 50, .Width = 200, .Content = "Custom text"}
            Dim innerShape2 = New DiagramShape() With {.CanSelect = False, .CanChangeParent = False, .CanEdit = False, .CanCopyWithoutParent = False, .CanDeleteWithoutParent = False, .CanMove = False, .Shape = BasicShapes.Rectangle, .Height = 150, .Width = 200, .Position = New Point(0, 50)}
            container.Items.Add(innerShape1)
            container.Items.Add(innerShape2)
            Return container
        End Function

        Private Class CSharpImpl

            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
