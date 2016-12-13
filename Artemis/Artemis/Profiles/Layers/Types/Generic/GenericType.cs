﻿using System.Linq;
using System.Windows;
using System.Windows.Media;
using Artemis.Models.Interfaces;
using Artemis.Profiles.Layers.Abstract;
using Artemis.Profiles.Layers.Animations;
using Artemis.Profiles.Layers.Interfaces;
using Artemis.Profiles.Layers.Models;
using Artemis.Properties;
using Artemis.Utilities;
using Artemis.ViewModels.Profiles;

namespace Artemis.Profiles.Layers.Types.Generic
{
    public class GenericType : ILayerType
    {
        public string Name => "Generic (Logitech)";
        public bool ShowInEdtor { get; } = false;
        public DrawType DrawType { get; } = DrawType.Generic;

        public ImageSource DrawThumbnail(LayerModel layer)
        {
            var thumbnailRect = new Rect(0, 0, 18, 18);
            var visual = new DrawingVisual();
            using (var c = visual.RenderOpen())
            {
                c.DrawImage(ImageUtilities.BitmapToBitmapImage(Resources.generic), thumbnailRect);
            }

            var image = new DrawingImage(visual.Drawing);
            return image;
        }

        public void Draw(LayerModel layerModel, DrawingContext c)
        {
            // If an animation is present, let it handle the drawing
            if (layerModel.LayerAnimation != null && !(layerModel.LayerAnimation is NoneAnimation))
            {
                layerModel.LayerAnimation.Draw(layerModel, c);
                return;
            }

            // Otherwise draw the rectangle with its applied dimensions and brush
            var rect = layerModel.LayerRect();

            // Can't meddle with the original brush because it's frozen.
            var brush = layerModel.Brush.Clone();
            brush.Opacity = layerModel.Opacity;

            c.PushClip(new RectangleGeometry(rect));
            c.DrawRectangle(brush, null, rect);
            c.Pop();
        }

        public void Update(LayerModel layerModel, IDataModel dataModel, bool isPreview = false)
        {
            // Generic layers are always drawn 10*10 (which is 40*40 when scaled up)
            layerModel.Properties.Width = 10;
            layerModel.Properties.Height = 10;
            layerModel.Properties.X = 0;
            layerModel.Properties.Y = 0;
            layerModel.Properties.Contain = true;

            layerModel.ApplyProperties(false);

            if (isPreview || dataModel == null)
                return;

            // If not previewing, apply dynamic properties according to datamodel
            foreach (var dynamicProperty in layerModel.Properties.DynamicProperties)
                dynamicProperty.ApplyProperty(dataModel, layerModel);
        }

        public void SetupProperties(LayerModel layerModel)
        {
            if (layerModel.Properties is SimplePropertiesModel)
                return;

            layerModel.Properties = new SimplePropertiesModel(layerModel.Properties);

            // Remove height and width dynamic properties since they are not applicable
            layerModel.Properties.DynamicProperties.Remove(
                layerModel.Properties.DynamicProperties.FirstOrDefault(d => d.LayerProperty == "Height"));
            layerModel.Properties.DynamicProperties.Remove(
                layerModel.Properties.DynamicProperties.FirstOrDefault(d => d.LayerProperty == "Width"));
        }

        public LayerPropertiesViewModel SetupViewModel(LayerEditorViewModel layerEditorViewModel,
            LayerPropertiesViewModel layerPropertiesViewModel)
        {
            if (layerPropertiesViewModel is GenericPropertiesViewModel)
                return layerPropertiesViewModel;
            return new GenericPropertiesViewModel(layerEditorViewModel);
        }
    }
}