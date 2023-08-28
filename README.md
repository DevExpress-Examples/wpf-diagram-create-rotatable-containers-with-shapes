<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/661660880/17.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1175899)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# WPF DiagramControl - Create Rotatable Containers with Shapes

This example demonstrates how to allow users to rotate containers with shapes.

![image](https://github.com/DevExpress-Examples/wpf-diagram-create-rotatable-containers-with-shapes/assets/65009440/d35f9b7c-720e-4944-8f2c-388f578b9f3f)

## Implementation Details

Default diagram containers do not support rotation operations. However, these operations are implemented in the base class. You can define a custom rotatable container in the following way:

1. Create a [DiagramContainer](https://docs.devexpress.com/WPF/DevExpress.Xpf.Diagram.DiagramContainer) class descendant.
2. Override the [CanRotate](https://docs.devexpress.com/WPF/DevExpress.Xpf.Diagram.DiagramItem.CanRotate) property.

   ```csharp
   public class CustomDiagramContainer : DiagramContainer {
       static CustomDiagramContainer() {
           CanRotateProperty.OverrideMetadata(typeof(CustomDiagramContainer), new FrameworkPropertyMetadata(true, null, (d, v) => v));
       }
   }
   ```

3. Handle the [DiagramControl.BeforeItemsRotating](https://docs.devexpress.com/WPF/DevExpress.Xpf.Diagram.DiagramControl.BeforeItemsRotating) event and pass container child items to the `e.Items` collection:

   ```csharp
   private void DiagramControl1_BeforeItemsRotating(object sender, DiagramBeforeItemsRotatingEventArgs e) {
       var containers = e.Items.OfType<CustomDiagramContainer>();
       foreach (var container in containers) {
           e.Items.Remove(container);
           foreach (var item in container.Items)
               e.Items.Add(item);
       }
   }
   ```

   In this case, the `DiagramControl` rotates these inner items instead of the parent container.
   
4. Handle the [DiagramControl.ItemsRotating](https://docs.devexpress.com/WPF/DevExpress.Xpf.Diagram.DiagramControl.ItemsRotating) event and correct the container position and size:

   ```csharp
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
   ``` 

## Files to Review

- [MainWindow.xaml.cs](./CS/WpfApp13/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/WpfApp13/MainWindow.xaml.vb))

## Documentation

- [Containers and Lists](https://docs.devexpress.com/WPF/117205/controls-and-libraries/diagram-control/diagram-items/containers)
- [DiagramControl.BeforeItemsRotating](https://docs.devexpress.com/WPF/DevExpress.Xpf.Diagram.DiagramControl.BeforeItemsRotating)
- [DiagramControl.ItemsRotating](https://docs.devexpress.com/WPF/DevExpress.Xpf.Diagram.DiagramControl.ItemsRotating)

## More Examples

- [WPF DiagramControl - Create Custom Shapes Based on Diagram Containers](https://github.com/DevExpress-Examples/wpf-diagram-create-custom-shapes-based-on-diagram-containers)
- [WPF DiagramControl - Proportionally Resize Shapes Within the Parent Container](https://github.com/DevExpress-Examples/wpf-diagram-proportionally-resize-shapes-within-container)
