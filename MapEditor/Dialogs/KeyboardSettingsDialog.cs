using System;
using System.Collections.Generic;
using System.Linq;
using MapEditor.Extensions;
using TMPro;
using UI.Builder;
using UI.Common;
using UnityEngine;

namespace MapEditor.Dialogs
{
  public sealed class KeyboardSettingsDialog : DialogBase
  {

    public KeyboardSettingsDialog() : base("Keyboard bindings", 400, 500, Window.Position.Center)
    {
    }

    private readonly Dictionary<string, TMP_Text> _Labels = new Dictionary<string, TMP_Text>();
    private readonly Dictionary<string, KeyCode> _Bindings = new Dictionary<string, KeyCode>();
    private string _DuplicityWarning = string.Empty;

    private void BuildField(UIPanelBuilder builder, string label, KeyCode current, Action<KeyCode> onSelected)
    {
      _Bindings[label] = current;
      var field = builder.AddField(label, builder.AddEnumDropdown(current, value =>
      {
        onSelected(value);
        _Bindings[label] = value;
        OnKeyBindingChanged();
      }))!;
      var text = field.RectTransform!.Find("Label")!.GetComponent<TMP_Text>()!;
      _Labels.Add(label, text);
    }

    private void OnKeyBindingChanged()
    {
      var duplicateBindings = _Bindings
        .Where(o => o.Value != KeyCode.None)
        .GroupBy(o => o.Value)
        .Where(g => g.Count() > 1)
        .SelectMany(g => g, (g, o) => o.Key)
        .ToList();

      foreach (var label in _Labels)
      {
        label.Value!.color = duplicateBindings.Contains(label.Key) ? Color.yellow : Color.gray;
      }

      _DuplicityWarning = duplicateBindings.Count > 0 ? "Duplicate binding detected, changes would not be saved!" : string.Empty;
    }

    protected override void BuildWindow(UIPanelBuilder builder)
    {
      builder.AddLabel(() => _DuplicityWarning, UIPanelBuilder.Frequency.Periodic)!.GetComponent<TMP_Text>()!.color = Color.yellow;
      builder.Spacer();

      BuildField(builder, "Move /  Rotate", EditorContext.Settings.ToggleMode, value => EditorContext.Settings.ToggleMode = value);
      BuildField(builder, "Move Forward", EditorContext.Settings.MoveForward, value => EditorContext.Settings.MoveForward = value);
      BuildField(builder, "Move Backward", EditorContext.Settings.MoveBackward, value => EditorContext.Settings.MoveBackward = value);
      BuildField(builder, "Move Left", EditorContext.Settings.MoveLeft, value => EditorContext.Settings.MoveLeft = value);
      BuildField(builder, "Move Right", EditorContext.Settings.MoveRight, value => EditorContext.Settings.MoveRight = value);
      BuildField(builder, "Move Up", EditorContext.Settings.MoveUp, value => EditorContext.Settings.MoveUp = value);
      BuildField(builder, "Move Down", EditorContext.Settings.MoveDown, value => EditorContext.Settings.MoveDown = value);
      BuildField(builder, "Increment Scaling", EditorContext.Settings.IncrementScaling, value => EditorContext.Settings.IncrementScaling = value);
      BuildField(builder, "Decrement Scaling", EditorContext.Settings.DecrementScaling, value => EditorContext.Settings.DecrementScaling = value);
      BuildField(builder, "Multiply Scaling Delta", EditorContext.Settings.MultiplyScalingDelta, value => EditorContext.Settings.MultiplyScalingDelta = value);
      BuildField(builder, "Divide Scaling Delta", EditorContext.Settings.DivideScalingDelta, value => EditorContext.Settings.DivideScalingDelta = value);
      BuildField(builder, "Reset Scaling", EditorContext.Settings.ResetScaling, value => EditorContext.Settings.ResetScaling = value);
    }

    protected override void AfterWindowClosed()
    {
      EditorContext.SaveSettings();
    }

  }
}
