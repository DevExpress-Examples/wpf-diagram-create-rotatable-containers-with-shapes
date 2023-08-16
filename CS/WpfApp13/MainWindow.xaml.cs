using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Native;
using DevExpress.Xpf.Diagram;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WpfApp13 {
    public class CustomDiagramContainer : DiagramContainer {
        static CustomDiagramContainer() {
            CanRotateProperty.OverrideMetadata(typeof(CustomDiagramContainer), new FrameworkPropertyMetadata(true, null, (d, v) => v));
        }
    }
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            DiagramControl.ItemTypeRegistrator.Register(typeof(CustomDiagramContainer));

            diagramControl1.Items.Add(CreateContainerShape1());
            diagramControl1.BeforeItemsRotating += DiagramControl1_BeforeItemsRotating;
            diagramControl1.ItemsRotating += DiagramControl1_ItemsRotating;

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            diagramControl1.FitToItems(diagramControl1.Items);
        }
        private void DiagramControl1_BeforeItemsRotating(object sender, DiagramBeforeItemsRotatingEventArgs e) {
            var containers = e.Items.OfType<CustomDiagramContainer>();
            foreach (var container in containers) {
                e.Items.Remove(container);
                foreach (var item in container.Items)
                    e.Items.Add(item);
            }
        }

        private void DiagramControl1_ItemsRotating(object sender, DiagramItemsRotatingEventArgs e) {
            var groups = e.Items.GroupBy(x => x.Item.ParentItem);
            foreach (var group in groups) {
                if (group.Key is CustomDiagramContainer) {
                    var container = (CustomDiagramContainer)group.Key;
                    var containingRect = container.Items.Select(x => x.RotatedDiagramBounds().BoundedRect()).Aggregate(Rect.Empty, Rect.Union);
                    container.Position = new Point(containingRect.X, containingRect.Y);
                    container.Width = (float)containingRect.Width;
                    container.Height = (float)containingRect.Height;
                }
            }
        }

        public CustomDiagramContainer CreateContainerShape1() {
            var container = new CustomDiagramContainer() {
                Width = 200,
                Height = 200,
                Position = new Point(100, 100),
                CanAddItems = false,
                ItemsCanChangeParent = false,
                ItemsCanCopyWithoutParent = false,
                ItemsCanDeleteWithoutParent = false,
                ItemsCanAttachConnectorBeginPoint = false,
                ItemsCanAttachConnectorEndPoint = false
            };

            container.StrokeThickness = 0;
            container.Background = Brushes.Transparent;

            var innerShape1 = new DiagramShape() {
                CanSelect = true,
                CanChangeParent = false,
                CanEdit = true,
                CanResize = false,
                CanRotate = false,
                CanCopyWithoutParent = false,
                CanDeleteWithoutParent = false,
                CanMove = false,
                Shape = BasicShapes.Trapezoid,
                Height = 50,
                Width = 200,

                Content = "Custom text"
            };


            var innerShape2 = new DiagramShape() {
                CanSelect = false,
                CanChangeParent = false,
                CanEdit = false,
                CanCopyWithoutParent = false,
                CanDeleteWithoutParent = false,
                CanMove = false,
                Shape = BasicShapes.Rectangle,
                Height = 150,
                Width = 200,
                Position = new Point(0, 50),
            };

            container.Items.Add(innerShape1);
            container.Items.Add(innerShape2);

            return container;
        }
    }
}
