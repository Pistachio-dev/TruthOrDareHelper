using ImGuiNET;
using System;
using System.Numerics;

namespace DalamudBasics.GUI.Configuration
{
    internal class ImGuiFormFactory<T>
    {
        private readonly Func<T> getData;
        private readonly Action<T> saveData;
        private static readonly Vector4 Red = new Vector4(1, 0, 0, 1);


        public ImGuiFormFactory(Func<T> getData, Action<T> saveData)
        {
            this.getData = getData;
            this.saveData = saveData;
        }

        public void DrawCheckbox(string label, string propertyName)
        {
            T data = getData();
            bool local = (bool)GetVarViaReflection(propertyName, data);
            if (ImGui.Checkbox(label, ref local))
            {
                SetVarViaReflection(propertyName, data, local);
                saveData(data);
            }
        }

        // Validable callbacks return a message if something happens, null otherwise.
        public void AddValidation(string? callWithValidation)
        {
            if (callWithValidation != null)
            {
                ImGui.TextColored(Red, callWithValidation);
            }
        }

        public string? DrawTextInput(string label, string propertyName, uint maxLength, Func<string, string?>? validation = null)
        {
            T data = getData();
            string local = (string)GetVarViaReflection(propertyName, data);
            if (ImGui.InputText(label, ref local, maxLength)
                && local != (string)GetVarViaReflection(propertyName, data))
            {
                string? validationMessage = validation != null ? validation(local) : null;
                if (validationMessage != null) { return validationMessage; }
                SetVarViaReflection(propertyName, data, local);
                saveData(data);
            }
            return null;
        }

        public string? DrawIntInput(string label, string propertyName, Func<int, string?>? validation = null)
        {
            T data = getData();
            int local = (int)GetVarViaReflection(propertyName, data);
            if (ImGui.InputInt(label, ref local)
                && local != (int)GetVarViaReflection(propertyName, data))
            {
                string? validationMessage = validation != null ? validation(local) : null;
                if (validationMessage != null) { return validationMessage; }
                SetVarViaReflection(propertyName, data, local);
                saveData(data);
            }
            return null;
        }

        public string? DrawDragInt(string label, string propertyName, int speed, int min, int max, Func<int, string?>? validation = null)
        {
            T data = getData();
            int local = (int)GetVarViaReflection(propertyName, data);
            if (ImGui.DragInt(label, ref local, speed, min, max)
                && local != (int)GetVarViaReflection(propertyName, data))
            {
                string? validationMessage = validation != null ? validation(local) : null;
                if (validationMessage != null) { return validationMessage; }
                SetVarViaReflection(propertyName, data, local);
                saveData(data);
            }

            return null;
        }

        public string? DrawComboBox(string label, string propertyName, string[] options)
        {
            T data = getData();
            int local = (int)GetVarViaReflection(propertyName, data);
            if (ImGui.Combo(label, ref local, options, options.Length)
                && local != (int)GetVarViaReflection(propertyName, data))
            {
                SetVarViaReflection(propertyName, data, local);
                saveData(data);
            }
            return null;
        }

        public object GetVarViaReflection(string propertyName, T instance)
        {
            return typeof(T).GetProperty(propertyName)?.GetValue(instance) ?? throw new Exception("Property was null. Please ensure a default value is set.");
        }

        public void SetVarViaReflection(string propertyName, T instance, object value)
        {
            typeof(T).GetProperty(propertyName)?.SetValue(instance, value);
        }
    }
}
