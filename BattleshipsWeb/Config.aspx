<%@ Page Title="Configuration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Config.aspx.cs" Inherits="BattleshipsWeb.Config" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="Scripts/Dynamic.js"></script>

    <div class="configPanel">
        <h1>Select Board Size</h1>
        <input type="number" id="boardSize" name="boardSize" runat="server" min="8" max="20" value="10" onchange="selectionChanged()">
    </div>

    <div class="configPanel">
        <h1>Select Total Ships</h1>
        <input type="number" id="totalShips" name="totalShips" runat="server" min="3" max="10" value="3" onchange="selectionChanged()">
    </div>

    <div style="clear: both;"></div>

    <div class="configPanel">
        <h1>Preview</h1>
        <div id="previewPanel"></div>
    </div>

    <div class="configPanel">
        <h1>This means war</h1>
        <p>
            <a class="btn btn-primary btn-lg" runat="server" href="~/battle">Play &raquo;</a>
        </p>
    </div>
    <div style="clear: both;"></div>

    <script>
        function selectionChanged() {
            renderPreview($("#MainContent_boardSize").val(), $("#MainContent_totalShips").val(), "previewPanel");
        }

        selectionChanged();
    </script>
</asp:Content>
