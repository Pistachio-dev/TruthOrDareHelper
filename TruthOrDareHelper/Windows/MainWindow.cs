using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Model;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration configuration;
    private TruthOrDareSession session;
    private static readonly Vector4 Green = new Vector4(0, 1, 0, 0.6f);
    private static readonly Vector4 Red = new Vector4(1, 0, 0, 0.6f);
    private static readonly Vector4 Gray = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    private const string RoundSymbol = "♦";

    public MainWindow(Plugin plugin)
        : base("Truth or Dare helper", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
        configuration = plugin.Configuration;
        session = Plugin.Session;
    }

    public void Dispose() { }

    public override void Draw()
    {
        DrawPlayerTable();
    }

    private void DrawPlayerTable()
    {
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;
        if (ImGui.BeginTable("##PlayerTable", 5, flags))
        {
            ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.8f);
            ImGui.TableSetupColumn("Wins", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("Losses", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("History", ImGuiTableColumnFlags.WidthStretch, 0.5f);
            ImGui.TableSetupColumn("Playing", ImGuiTableColumnFlags.WidthStretch, 0.2f);

            ImGui.TableHeadersRow();

            foreach (var player in session.PlayerInfo.Values)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(RemoveWorldFromName(player.FullName));

                ImGui.TableNextColumn();
                ImGui.TextUnformatted("5");

                ImGui.TableNextColumn();
                ImGui.TextUnformatted("3");

                ImGui.TableNextColumn();
                ImGui.BeginGroup();
                ImGui.TextUnformatted("♦♦♦♦♦♦♦♦♦");
                ImGui.EndGroup();

                ImGui.TableNextColumn();
                ImGui.TextUnformatted("✓"); // use "x" for "no".
            }

            ImGui.EndTable();
        }
    }

    private string RemoveWorldFromName(string name)
    {
        return name.Split("@").First();
    }
}
